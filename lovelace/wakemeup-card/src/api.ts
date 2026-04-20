import type {
  Alarm,
  AlarmPayload,
  ApiChoice,
  ApiErrorDetails,
  HomeAssistant,
  MetaResponse,
  ValidationErrorMap,
} from "./types";

const BRIDGE_API_BASE = "wakemeup";

const buildApiPath = (basePath: string, endpoint: string): string =>
  `${basePath}${endpoint.startsWith("/") ? endpoint : `/${endpoint}`}`;

const normalizeValidation = (input: unknown): ValidationErrorMap | undefined => {
  if (!input || typeof input !== "object") {
    return undefined;
  }

  const entries = Object.entries(input as Record<string, unknown>)
    .map(([field, messages]) => {
      const normalizedField = field ? `${field.charAt(0).toLowerCase()}${field.slice(1)}` : field;
      if (Array.isArray(messages)) {
        return [normalizedField, messages.filter((message): message is string => typeof message === "string")];
      }

      if (typeof messages === "string") {
        return [normalizedField, [messages]];
      }

      return [normalizedField, []];
    })
    .filter(([, messages]) => messages.length > 0);

  if (entries.length === 0) {
    return undefined;
  }

  return Object.fromEntries(entries);
};

const buildCallApiError = (error: unknown): ApiErrorDetails => {
  if (!error || typeof error !== "object") {
    return { message: "Unexpected API error." };
  }

  const objectError = error as Record<string, unknown>;
  const body =
    (objectError.body && typeof objectError.body === "object" ? objectError.body : undefined) ??
    (objectError.error && typeof objectError.error === "object" ? objectError.error : undefined);

  if (body) {
    const bodyObject = body as Record<string, unknown>;
    const validationFromDetails = Array.isArray(bodyObject.details)
      ? Object.fromEntries(
          (bodyObject.details as Array<Record<string, unknown>>)
            .filter((detail) => typeof detail.field === "string" && typeof detail.message === "string")
            .map((detail) => [detail.field as string, [detail.message as string]]),
        )
      : undefined;
    const validation = normalizeValidation(bodyObject.errors) ??
      (validationFromDetails && Object.keys(validationFromDetails).length > 0 ? validationFromDetails : undefined);
    const message =
      (typeof bodyObject.error === "string" && bodyObject.error) ||
      (typeof bodyObject.detail === "string" && bodyObject.detail) ||
      (typeof bodyObject.title === "string" && bodyObject.title) ||
      (typeof objectError.message === "string" && objectError.message) ||
      "Unexpected API error.";

    return {
      status: typeof objectError.status_code === "number" ? objectError.status_code : undefined,
      message,
      validation,
    };
  }

  return {
    status: typeof objectError.status_code === "number" ? objectError.status_code : undefined,
    message:
      (typeof objectError.message === "string" && objectError.message) ||
      (typeof objectError.error === "string" && objectError.error) ||
      "Unexpected API error.",
  };
};

export class WakeMeUpApiClient {
  public constructor(private readonly hass: HomeAssistant) {}

  public async getMeta(): Promise<MetaResponse> {
    return this.request<MetaResponse>("GET", "/meta");
  }

  public async getAlarms(): Promise<Alarm[]> {
    return this.request<Alarm[]>("GET", "/alarms");
  }

  public async createAlarm(payload: AlarmPayload): Promise<Alarm> {
    return this.request<Alarm>("POST", "/alarms", payload);
  }

  public async updateAlarm(id: string, payload: AlarmPayload): Promise<Alarm> {
    return this.request<Alarm>("PUT", `/alarms/${encodeURIComponent(id)}`, payload);
  }

  public async setAllAlarmsEnabled(isEnabled: boolean): Promise<{ isEnabled: boolean; updatedCount: number }> {
    return this.request<{ isEnabled: boolean; updatedCount: number }>("PATCH", "/alarms/enabled", {
      isEnabled,
    });
  }

  public async toggleAlarm(id: string, isEnabled: boolean): Promise<void> {
    return this.request<void>("PATCH", `/alarms/${encodeURIComponent(id)}/enabled`, {
      isEnabled,
    });
  }

  public async deleteAlarm(id: string): Promise<void> {
    return this.request<void>("DELETE", `/alarms/${encodeURIComponent(id)}`);
  }

  private async request<T>(method: string, endpoint: string, body?: unknown): Promise<T> {
    const path = buildApiPath(BRIDGE_API_BASE, endpoint);

    try {
      return await this.hass.callApi<T>(method, path, body);
    } catch (error) {
      throw buildCallApiError(error);
    }
  }
}

export const normalizeChoices = (input?: MetaResponse["repeatModes"]): ApiChoice[] =>
  (input ?? [])
    .map((item) => {
      if (typeof item === "string") {
        return { value: item, label: item };
      }

      const value = item.value ?? item.key ?? item.id ?? item.code;
      const label = item.label ?? item.displayName ?? item.name ?? value;
      if (!value || !label) {
        return undefined;
      }

      return { value, label };
    })
    .filter((item): item is ApiChoice => Boolean(item));

export const normalizeWeekdays = (input?: MetaResponse["weekdays"]): ApiChoice[] =>
  normalizeChoices(input);

export const formatDateTime = (value?: string | null, language = "en"): string => {
  if (!value) {
    return "--";
  }

  const parsed = new Date(value);
  if (Number.isNaN(parsed.getTime())) {
    return value;
  }

  return new Intl.DateTimeFormat(language, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(parsed);
};

export const mapErrorMessage = (error: unknown): ApiErrorDetails => {
  if (error && typeof error === "object" && "message" in error) {
    return error as ApiErrorDetails;
  }

  if (error instanceof Error) {
    return { message: error.message };
  }

  return { message: "Unexpected API error." };
};
