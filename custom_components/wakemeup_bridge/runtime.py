from __future__ import annotations

from dataclasses import dataclass
from datetime import datetime
from typing import Any

from homeassistant.core import Event, HomeAssistant, callback
from homeassistant.helpers.dispatcher import async_dispatcher_send
from homeassistant.helpers.event import async_call_later
from homeassistant.util import dt as dt_util

from .const import EVENT_ALARM_TRIGGERED, SIGNAL_STATE_UPDATED, TRIGGER_RESET_SECONDS


@dataclass(slots=True)
class WakeMeUpTriggerState:
    is_triggered: bool = False
    triggered_at: datetime | None = None
    last_alarm_name: str = ""
    last_alarm_time: str = ""
    last_alarm_description: str = ""


class WakeMeUpRuntime:
    def __init__(self, hass: HomeAssistant) -> None:
        self._hass = hass
        self._state = WakeMeUpTriggerState()
        self._unsubscribe_event = None
        self._unsubscribe_reset = None

    @property
    def state(self) -> WakeMeUpTriggerState:
        return self._state

    async def async_setup(self) -> None:
        if self._unsubscribe_event is None:
            self._unsubscribe_event = self._hass.bus.async_listen(
                EVENT_ALARM_TRIGGERED,
                self._handle_alarm_triggered,
            )

    async def async_unload(self) -> None:
        if self._unsubscribe_event is not None:
            self._unsubscribe_event()
            self._unsubscribe_event = None

        self._cancel_reset()

    @callback
    def _handle_alarm_triggered(self, event: Event) -> None:
        data = event.data or {}
        self._state.is_triggered = True
        self._state.triggered_at = dt_util.utcnow()
        self._state.last_alarm_name = str(data.get("name", "") or "")
        self._state.last_alarm_time = str(data.get("time", "") or "")
        self._state.last_alarm_description = str(data.get("description", "") or "")

        self._cancel_reset()
        self._unsubscribe_reset = async_call_later(
            self._hass,
            TRIGGER_RESET_SECONDS,
            self._async_reset_trigger,
        )
        self._notify_state_changed()

    @callback
    def _async_reset_trigger(self, _now: datetime) -> None:
        self._unsubscribe_reset = None
        self._state.is_triggered = False
        self._notify_state_changed()

    @callback
    def _cancel_reset(self) -> None:
        if self._unsubscribe_reset is not None:
            self._unsubscribe_reset()
            self._unsubscribe_reset = None

    @callback
    def _notify_state_changed(self) -> None:
        async_dispatcher_send(self._hass, SIGNAL_STATE_UPDATED)
