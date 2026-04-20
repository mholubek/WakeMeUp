import { LitElement, css, html, nothing } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import {
  WakeMeUpApiClient,
  formatDateTime,
  mapErrorMessage,
  normalizeChoices,
  normalizeWeekdays,
} from "./api";
import { getTranslations } from "./translations";
import type {
  Alarm,
  AlarmPayload,
  ApiChoice,
  HomeAssistant,
  MetaResponse,
  TranslationSet,
  ValidationErrorMap,
  WakeMeUpCardConfig,
} from "./types";

interface AlarmFormState {
  id?: string;
  name: string;
  description: string;
  time: string;
  isEnabled: boolean;
  repeatMode: string;
  days: string[];
}

const buildDefaultForm = (repeatModes: ApiChoice[]): AlarmFormState => ({
  name: "",
  description: "",
  time: "07:00",
  isEnabled: true,
  repeatMode: repeatModes[0]?.value ?? "",
  days: [],
});

const fromAlarm = (alarm: Alarm): AlarmFormState => ({
  id: alarm.id,
  name: alarm.name ?? "",
  description: alarm.description ?? "",
  time: alarm.time ?? "07:00",
  isEnabled: alarm.isEnabled ?? true,
  repeatMode: alarm.repeatMode ?? "",
  days: [...(alarm.days ?? [])],
});

const sortAlarms = (alarms: Alarm[]): Alarm[] =>
  [...alarms].sort((left, right) => left.time.localeCompare(right.time) || left.name.localeCompare(right.name));

const daysRelevant = (repeatMode: string): boolean =>
  /(week|day|custom|selected|repeat)/i.test(repeatMode);

@customElement("wakemeup-card")
export class WakeMeUpCard extends LitElement {
  @property({ attribute: false }) public hass?: HomeAssistant;

  @state() private _config?: WakeMeUpCardConfig;
  @state() private _alarms: Alarm[] = [];
  @state() private _meta?: MetaResponse;
  @state() private _translations: TranslationSet = getTranslations();
  @state() private _repeatModes: ApiChoice[] = [];
  @state() private _weekdays: ApiChoice[] = [];
  @state() private _loading = true;
  @state() private _busy = false;
  @state() private _dialogOpen = false;
  @state() private _form: AlarmFormState = buildDefaultForm([]);
  @state() private _error?: string;
  @state() private _validation?: ValidationErrorMap;
  @state() private _statusMessage?: string;

  private _client?: WakeMeUpApiClient;

  public static async getConfigElement(): Promise<HTMLElement> {
    return document.createElement("wakemeup-card-editor");
  }

  public static getStubConfig(): WakeMeUpCardConfig {
    return {
      type: "custom:wakemeup-card",
      title: "WakeMeUp",
    };
  }

  public setConfig(config: WakeMeUpCardConfig): void {
    if (!config.type) {
      throw new Error("Invalid configuration");
    }

    this._config = config;
    this._client = undefined;
  }

  public getCardSize(): number {
    return Math.max(3, this._alarms.length + 1);
  }

  protected willUpdate(changed: Map<string, unknown>): void {
    if ((changed.has("hass") || changed.has("_config")) && this.hass && this._config) {
      this._client = new WakeMeUpApiClient(this.hass);
    }
  }

  protected updated(changed: Map<string, unknown>): void {
    if ((changed.has("hass") || changed.has("_config")) && this._client) {
      void this._loadData();
    }
  }

  protected render() {
    if (!this._config) {
      return html``;
    }

    const title = this._config.title || this._translations.cardTitle;
    const locale = this._meta?.currentLanguage ?? "en";

    return html`
      <ha-card>
        <div class="shell">
          <div class="header">
            <div>
              <div class="eyebrow">${this._translations.timezone}</div>
              <h1>${title}</h1>
              <div class="subtle">${this._meta?.timeZoneDisplayName ?? "--"}</div>
            </div>
            <div class="actions">
              <button class="soft" @click=${() => this._setAllAlarmsEnabled(true)} ?disabled=${this._loading || this._busy}>
                ${this._translations.enableAllAlarms}
              </button>
              <button class="soft" @click=${() => this._setAllAlarmsEnabled(false)} ?disabled=${this._loading || this._busy}>
                ${this._translations.disableAllAlarms}
              </button>
              <button class="soft" @click=${this._loadData} ?disabled=${this._loading || this._busy}>
                ${this._translations.refresh}
              </button>
              <button class="primary" @click=${this._openCreateDialog} ?disabled=${this._loading || this._busy}>
                ${this._translations.addAlarm}
              </button>
            </div>
          </div>

          ${this._statusMessage
            ? html`<div class="banner success">${this._statusMessage}</div>`
            : nothing}
          ${this._error ? html`<div class="banner error">${this._error}</div>` : nothing}

          ${this._loading
            ? html`<div class="state">${this._translations.loading}</div>`
            : this._alarms.length === 0
              ? html`<div class="state">${this._translations.noAlarms}</div>`
              : html`
                  <div class="list">
                    ${this._alarms.map(
                      (alarm) => html`
                        <article class="alarm">
                          <div class="alarm-main">
                            <div class="pill-row">
                              <span class="time-pill">${alarm.time}</span>
                              <label class="toggle">
                                <input
                                  type="checkbox"
                                  .checked=${alarm.isEnabled}
                                  @change=${(event: Event) => this._toggleAlarm(alarm, event)}
                                  ?disabled=${this._busy}
                                />
                                <span>${alarm.isEnabled ? this._translations.enabled : this._translations.disabled}</span>
                              </label>
                            </div>

                            <div class="alarm-title">${alarm.name || this._translations.unnamedAlarm}</div>
                            <div class="alarm-description">
                              ${alarm.description || this._translations.emptyDescription}
                            </div>

                            <div class="meta-grid">
                              <div>
                                <span>${this._translations.repeatMode}</span>
                                <strong>${alarm.repeatMode}</strong>
                              </div>
                              <div>
                                <span>${this._translations.days}</span>
                                <strong>${alarm.days?.join(", ") || "--"}</strong>
                              </div>
                              <div>
                                <span>${this._translations.nextTrigger}</span>
                                <strong>${formatDateTime(alarm.nextTriggerLocal, locale)}</strong>
                              </div>
                              <div>
                                <span>${this._translations.lastTriggered}</span>
                                <strong>${formatDateTime(alarm.lastTriggeredUtc, locale)}</strong>
                              </div>
                            </div>

                            ${alarm.lastResultMessage
                              ? html`
                                  <div class="result">
                                    <span>${this._translations.lastResult}</span>
                                    <strong>${alarm.lastResultMessage}</strong>
                                  </div>
                                `
                              : nothing}
                          </div>

                          <div class="alarm-actions">
                            <button class="soft" @click=${() => this._openEditDialog(alarm)} ?disabled=${this._busy}>
                              ${this._translations.editAlarm}
                            </button>
                            <button class="danger" @click=${() => this._deleteAlarm(alarm)} ?disabled=${this._busy}>
                              ${this._translations.deleteAlarm}
                            </button>
                          </div>
                        </article>
                      `,
                    )}
                  </div>
                `}

          ${this._dialogOpen ? this._renderDialog() : nothing}
        </div>
      </ha-card>
    `;
  }

  private _renderDialog() {
    const submitLabel = this._form.id ? this._translations.save : this._translations.create;
    const showDays = daysRelevant(this._form.repeatMode) && this._weekdays.length > 0;

    return html`
      <div class="overlay" @click=${this._closeDialog}>
        <div class="dialog" @click=${(event: Event) => event.stopPropagation()}>
          <div class="dialog-head">
            <div>
              <div class="eyebrow">${this._translations.formHelp}</div>
              <h2>${this._form.id ? this._translations.editAlarm : this._translations.addAlarm}</h2>
            </div>
            <button class="icon" @click=${this._closeDialog}>${this._translations.close}</button>
          </div>

          ${this._validation ? html`<div class="banner error">${this._translations.validationTitle}</div>` : nothing}

          <form @submit=${this._submitForm}>
            ${this._renderField("name", this._translations.name, html`
              <input
                name="name"
                .value=${this._form.name}
                @input=${this._handleInput}
                required
              />
            `)}

            ${this._renderField("description", this._translations.description, html`
              <textarea
                name="description"
                .value=${this._form.description}
                @input=${this._handleInput}
                rows="3"
              ></textarea>
            `)}

            <div class="form-grid">
              ${this._renderField("time", this._translations.time, html`
                <input
                  type="time"
                  name="time"
                  .value=${this._form.time}
                  @input=${this._handleInput}
                  required
                />
              `)}

              ${this._renderField("repeatMode", this._translations.repeatMode, html`
                <select name="repeatMode" .value=${this._form.repeatMode} @change=${this._handleInput}>
                  ${this._repeatModes.map(
                    (option) => html`<option value=${option.value}>${option.label}</option>`,
                  )}
                </select>
              `)}
            </div>

            <label class="checkbox-row">
              <input
                type="checkbox"
                name="isEnabled"
                .checked=${this._form.isEnabled}
                @change=${this._handleInput}
              />
              <span>${this._translations.enabled}</span>
            </label>

            ${showDays
              ? html`
                  <div class="field">
                    <span class="label">${this._translations.days}</span>
                    <div class="chips">
                      ${this._weekdays.map(
                        (day) => html`
                          <label class="chip ${this._form.days.includes(day.value) ? "selected" : ""}">
                            <input
                              type="checkbox"
                              .checked=${this._form.days.includes(day.value)}
                              @change=${(event: Event) => this._toggleDay(day.value, event)}
                            />
                            <span>${day.label}</span>
                          </label>
                        `,
                      )}
                    </div>
                    ${this._renderErrors("days")}
                  </div>
                `
              : nothing}

            <div class="dialog-actions">
              <button type="button" class="soft" @click=${this._closeDialog}>${this._translations.cancel}</button>
              <button type="submit" class="primary" ?disabled=${this._busy}>${submitLabel}</button>
            </div>
          </form>
        </div>
      </div>
    `;
  }

  private _renderField(field: string, label: string, control: unknown) {
    return html`
      <label class="field">
        <span class="label">${label}</span>
        ${control}
        ${this._renderErrors(field)}
      </label>
    `;
  }

  private _renderErrors(field: string) {
    const errors = this._validation?.[field];
    if (!errors?.length) {
      return nothing;
    }

    return html`<div class="field-error">${errors.join(" ")}</div>`;
  }

  private async _loadData(): Promise<void> {
    if (!this._client) {
      return;
    }

    this._loading = true;
    this._error = undefined;
    this._statusMessage = undefined;

    try {
      const [meta, alarms] = await Promise.all([this._client.getMeta(), this._client.getAlarms()]);
      this._meta = meta;
      this._translations = getTranslations(meta.currentLanguage);
      this._repeatModes = normalizeChoices(meta.repeatModes);
      this._weekdays = normalizeWeekdays(meta.weekdays);
      this._alarms = sortAlarms(alarms);
      if (!this._form.id) {
        this._form = buildDefaultForm(this._repeatModes);
      }
    } catch (error) {
      this._error = mapErrorMessage(error).message;
    } finally {
      this._loading = false;
    }
  }

  private _openCreateDialog = (): void => {
    this._statusMessage = undefined;
    this._error = undefined;
    this._validation = undefined;
    this._form = buildDefaultForm(this._repeatModes);
    this._dialogOpen = true;
  };

  private _openEditDialog(alarm: Alarm): void {
    this._statusMessage = undefined;
    this._error = undefined;
    this._validation = undefined;
    this._form = fromAlarm(alarm);
    this._dialogOpen = true;
  }

  private _closeDialog = (): void => {
    this._dialogOpen = false;
    this._validation = undefined;
    this._error = undefined;
  };

  private _handleInput = (event: Event): void => {
    const target = event.currentTarget as HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement;
    const { name } = target;
    const value = target instanceof HTMLInputElement && target.type === "checkbox" ? target.checked : target.value;

    this._form = {
      ...this._form,
      [name]: value,
    };

    if (name === "repeatMode" && !daysRelevant(String(value))) {
      this._form = {
        ...this._form,
        days: [],
      };
    }

    if (this._validation?.[name]) {
      const nextValidation = { ...this._validation };
      delete nextValidation[name];
      this._validation = Object.keys(nextValidation).length ? nextValidation : undefined;
    }
  };

  private _toggleDay(day: string, event: Event): void {
    const target = event.currentTarget as HTMLInputElement;
    const nextDays = target.checked
      ? [...this._form.days, day]
      : this._form.days.filter((item) => item !== day);

    this._form = {
      ...this._form,
      days: nextDays,
    };
  }

  private async _submitForm(event: SubmitEvent): Promise<void> {
    event.preventDefault();
    if (!this._client) {
      return;
    }

    this._busy = true;
    this._error = undefined;
    this._validation = undefined;

    const payload: AlarmPayload = {
      name: this._form.name.trim(),
      description: this._form.description.trim(),
      time: this._form.time,
      isEnabled: this._form.isEnabled,
      repeatMode: this._form.repeatMode,
      days: this._form.days,
    };

    try {
      if (this._form.id) {
        await this._client.updateAlarm(this._form.id, payload);
      } else {
        await this._client.createAlarm(payload);
      }

      this._dialogOpen = false;
      this._statusMessage = this._translations.saveSuccess;
      await this._loadData();
    } catch (error) {
      const details = mapErrorMessage(error);
      this._error = details.message;
      this._validation = details.validation;
    } finally {
      this._busy = false;
    }
  }

  private async _toggleAlarm(alarm: Alarm, event: Event): Promise<void> {
    if (!this._client) {
      return;
    }

    const target = event.currentTarget as HTMLInputElement;
    this._busy = true;
    this._error = undefined;
    this._statusMessage = undefined;

    try {
      await this._client.toggleAlarm(alarm.id, target.checked);
      this._alarms = this._alarms.map((item) =>
        item.id === alarm.id ? { ...item, isEnabled: target.checked } : item,
      );
      this._statusMessage = this._translations.toggleSuccess;
    } catch (error) {
      target.checked = !target.checked;
      this._error = mapErrorMessage(error).message;
    } finally {
      this._busy = false;
    }
  }

  private async _setAllAlarmsEnabled(isEnabled: boolean): Promise<void> {
    if (!this._client) {
      return;
    }

    this._busy = true;
    this._error = undefined;
    this._statusMessage = undefined;

    try {
      await this._client.setAllAlarmsEnabled(isEnabled);
      await this._loadData();
      this._statusMessage = isEnabled
        ? this._translations.bulkEnableSuccess
        : this._translations.bulkDisableSuccess;
    } catch (error) {
      this._error = mapErrorMessage(error).message;
    } finally {
      this._busy = false;
    }
  }

  private async _deleteAlarm(alarm: Alarm): Promise<void> {
    if (!this._client || !window.confirm(this._translations.deleteConfirm)) {
      return;
    }

    this._busy = true;
    this._error = undefined;
    this._statusMessage = undefined;

    try {
      await this._client.deleteAlarm(alarm.id);
      this._alarms = this._alarms.filter((item) => item.id !== alarm.id);
      this._statusMessage = this._translations.deleteSuccess;
    } catch (error) {
      this._error = mapErrorMessage(error).message;
    } finally {
      this._busy = false;
    }
  }

  static styles = css`
    :host {
      display: block;
    }

    ha-card {
      border-radius: 28px;
      overflow: hidden;
      background:
        radial-gradient(circle at top left, color-mix(in srgb, var(--primary-color) 14%, transparent), transparent 45%),
        linear-gradient(
          160deg,
          color-mix(in srgb, var(--card-background-color) 92%, var(--primary-background-color)),
          color-mix(in srgb, var(--ha-card-background, var(--card-background-color)) 98%, black 2%)
        );
      box-shadow:
        0 18px 40px rgba(0, 0, 0, 0.12),
        inset 0 1px 0 rgba(255, 255, 255, 0.12);
    }

    .shell {
      padding: 20px;
      color: var(--primary-text-color);
    }

    .header,
    .dialog-head,
    .alarm-actions,
    .pill-row,
    .actions,
    .dialog-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 12px;
    }

    .header {
      align-items: flex-start;
      margin-bottom: 18px;
    }

    .eyebrow,
    .subtle,
    .meta-grid span,
    .result span,
    .label {
      color: var(--secondary-text-color);
    }

    h1,
    h2 {
      margin: 4px 0;
      font-size: 1.4rem;
      line-height: 1.1;
    }

    button,
    input,
    select,
    textarea {
      font: inherit;
    }

    button {
      border: none;
      border-radius: 999px;
      padding: 10px 16px;
      cursor: pointer;
      transition: transform 120ms ease, opacity 120ms ease, background 120ms ease;
    }

    button:hover {
      transform: translateY(-1px);
    }

    button:disabled {
      opacity: 0.55;
      cursor: default;
      transform: none;
    }

    .primary {
      background: var(--primary-color);
      color: var(--text-primary-color, white);
    }

    .soft,
    .icon {
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 82%, transparent);
      color: var(--primary-text-color);
    }

    .danger {
      background: color-mix(in srgb, var(--error-color) 18%, transparent);
      color: var(--error-color);
    }

    .state,
    .banner {
      border-radius: 22px;
      padding: 14px 16px;
      margin-bottom: 16px;
    }

    .state {
      text-align: center;
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 80%, transparent);
    }

    .banner.success {
      background: color-mix(in srgb, var(--success-color, #2e7d32) 18%, transparent);
      color: var(--success-color, #2e7d32);
    }

    .banner.error {
      background: color-mix(in srgb, var(--error-color) 14%, transparent);
      color: var(--primary-text-color);
    }

    .list {
      display: grid;
      gap: 14px;
    }

    .alarm {
      display: grid;
      gap: 16px;
      padding: 16px;
      border-radius: 26px;
      background:
        linear-gradient(
          180deg,
          color-mix(in srgb, var(--card-background-color) 88%, white 12%),
          color-mix(in srgb, var(--card-background-color) 98%, black 2%)
        );
      box-shadow:
        inset 0 1px 0 rgba(255, 255, 255, 0.14),
        0 10px 24px rgba(0, 0, 0, 0.08);
    }

    .alarm-main {
      display: grid;
      gap: 12px;
    }

    .time-pill,
    .toggle {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      border-radius: 999px;
      padding: 8px 12px;
      background: color-mix(in srgb, var(--primary-color) 14%, transparent);
    }

    .time-pill {
      font-size: 1.15rem;
      font-weight: 700;
      letter-spacing: 0.04em;
    }

    .toggle input {
      accent-color: var(--primary-color);
    }

    .alarm-title {
      font-size: 1.1rem;
      font-weight: 600;
    }

    .alarm-description {
      color: var(--secondary-text-color);
    }

    .meta-grid {
      display: grid;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 12px;
    }

    .meta-grid div,
    .result {
      display: grid;
      gap: 4px;
      padding: 12px 14px;
      border-radius: 18px;
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 74%, transparent);
    }

    .overlay {
      position: fixed;
      inset: 0;
      display: grid;
      place-items: center;
      padding: 16px;
      background: rgba(0, 0, 0, 0.4);
      z-index: 10;
    }

    .dialog {
      width: min(560px, 100%);
      max-height: min(92vh, 880px);
      overflow: auto;
      border-radius: 30px;
      padding: 20px;
      background: var(--card-background-color);
      box-shadow: 0 24px 60px rgba(0, 0, 0, 0.28);
    }

    form,
    .field,
    .chips {
      display: grid;
      gap: 10px;
    }

    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 12px;
    }

    input:not([type="checkbox"]),
    select,
    textarea {
      width: 100%;
      box-sizing: border-box;
      border: 1px solid color-mix(in srgb, var(--divider-color) 72%, transparent);
      border-radius: 18px;
      padding: 12px 14px;
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 80%, transparent);
      color: var(--primary-text-color);
    }

    textarea {
      resize: vertical;
    }

    .checkbox-row,
    .chip {
      display: inline-flex;
      align-items: center;
      gap: 8px;
    }

    .chips {
      grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    }

    .chip {
      justify-content: center;
      border-radius: 999px;
      padding: 10px 12px;
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 82%, transparent);
    }

    .chip.selected {
      background: color-mix(in srgb, var(--primary-color) 22%, transparent);
    }

    .chip input {
      accent-color: var(--primary-color);
    }

    .field-error {
      color: var(--error-color);
      font-size: 0.9rem;
    }

    @media (max-width: 640px) {
      .shell,
      .dialog {
        padding: 16px;
      }

      .header,
      .alarm-actions,
      .actions,
      .dialog-head,
      .dialog-actions,
      .pill-row {
        flex-direction: column;
        align-items: stretch;
      }

      .meta-grid,
      .form-grid {
        grid-template-columns: 1fr;
      }

      button {
        width: 100%;
      }
    }
  `;
}

declare global {
  interface HTMLElementTagNameMap {
    "wakemeup-card": WakeMeUpCard;
  }
}
