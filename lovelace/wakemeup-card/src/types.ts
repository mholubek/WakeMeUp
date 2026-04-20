export interface HomeAssistant {
  callApi<T>(method: string, path: string, body?: unknown): Promise<T>;
  localize?: (key: string, ...args: unknown[]) => string;
}

export interface WakeMeUpCardConfig {
  type: string;
  title?: string;
}

export interface Alarm {
  id: string;
  name: string;
  description?: string | null;
  time: string;
  isEnabled: boolean;
  repeatMode: string;
  days: string[];
  nextTriggerLocal?: string | null;
  createdUtc?: string | null;
  lastTriggeredUtc?: string | null;
  lastResultMessage?: string | null;
}

export interface ApiChoice {
  value: string;
  label: string;
}

export interface MetaResponse {
  timeZoneId?: string;
  timeZoneDisplayName?: string;
  currentLanguage?: string;
  currentTheme?: string;
  currentLocalTime?: string;
  supportedLanguages?: string[];
  repeatModes?: Array<string | { value?: string; key?: string; id?: string; code?: string; label?: string; name?: string; displayName?: string }>;
  weekdays?: Array<string | { value?: string; key?: string; id?: string; code?: string; label?: string; name?: string; displayName?: string }>;
}

export interface AlarmPayload {
  name: string;
  description?: string;
  time: string;
  isEnabled: boolean;
  repeatMode: string;
  days: string[];
}

export interface ValidationErrorMap {
  [field: string]: string[];
}

export interface ApiErrorDetails {
  status?: number;
  message: string;
  validation?: ValidationErrorMap;
}

export interface TranslationSet {
  cardTitle: string;
  addAlarm: string;
  enableAllAlarms: string;
  disableAllAlarms: string;
  editAlarm: string;
  noAlarms: string;
  loading: string;
  refresh: string;
  deleteAlarm: string;
  deleteConfirm: string;
  cancel: string;
  save: string;
  create: string;
  enabled: string;
  disabled: string;
  name: string;
  description: string;
  time: string;
  repeatMode: string;
  days: string;
  timezone: string;
  nextTrigger: string;
  createdAt: string;
  lastTriggered: string;
  lastResult: string;
  emptyDescription: string;
  unnamedAlarm: string;
  errorTitle: string;
  validationTitle: string;
  retry: string;
  close: string;
  requiredField: string;
  formHelp: string;
  unknownError: string;
  saveSuccess: string;
  deleteSuccess: string;
  toggleSuccess: string;
  bulkEnableSuccess: string;
  bulkDisableSuccess: string;
}
