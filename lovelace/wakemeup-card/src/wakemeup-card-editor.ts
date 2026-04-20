import { LitElement, css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import type { HomeAssistant, WakeMeUpCardConfig } from "./types";

@customElement("wakemeup-card-editor")
export class WakeMeUpCardEditor extends LitElement {
  @property({ attribute: false }) public hass?: HomeAssistant;

  @state() private _config?: WakeMeUpCardConfig;

  public setConfig(config: WakeMeUpCardConfig): void {
    this._config = config;
  }

  protected render() {
    if (!this._config) {
      return html``;
    }

    return html`
      <div class="editor">
        <label>
          <span>Title</span>
          <input
            .value=${this._config.title ?? ""}
            @input=${this._onInput}
            data-field="title"
            placeholder="WakeMeUp"
          />
        </label>
      </div>
    `;
  }

  private _onInput(event: Event): void {
    const target = event.currentTarget as HTMLInputElement;
    const field = target.dataset.field as keyof WakeMeUpCardConfig;
    const nextConfig = {
      ...this._config,
      type: "custom:wakemeup-card",
      [field]: target.value.trim() || undefined,
    };

    this._config = nextConfig;
    this.dispatchEvent(
      new CustomEvent("config-changed", {
        detail: { config: nextConfig },
      }),
    );
  }

  static styles = css`
    .editor {
      display: grid;
      gap: 16px;
    }

    label {
      display: grid;
      gap: 8px;
      color: var(--primary-text-color);
      font-size: 14px;
    }

    input {
      border: 1px solid var(--divider-color);
      border-radius: 16px;
      padding: 12px 14px;
      background: var(--card-background-color);
      color: var(--primary-text-color);
      font: inherit;
    }
  `;
}

declare global {
  interface HTMLElementTagNameMap {
    "wakemeup-card-editor": WakeMeUpCardEditor;
  }
}
