using WakeMeUp.Domain;
using WakeMeUp.Services;

namespace WakeMeUp.Api;

public static class AlarmApiEndpoints
{
    public static IEndpointRouteBuilder MapAlarmApi(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api/v1");

        api.MapGet("/alarms", async (
            IAlarmStore store,
            AlarmOccurrenceService occurrenceService,
            CancellationToken cancellationToken) =>
        {
            var alarms = await store.GetAlarmsAsync(cancellationToken);
            var now = DateTimeOffset.Now;
            return Results.Ok(alarms.Select(alarm => ToResponse(alarm, occurrenceService, now)));
        });

        api.MapGet("/alarms/{alarmId:guid}", async (
            Guid alarmId,
            IAlarmStore store,
            AlarmOccurrenceService occurrenceService,
            CancellationToken cancellationToken) =>
        {
            var alarm = (await store.GetAlarmsAsync(cancellationToken)).FirstOrDefault(x => x.Id == alarmId);
            if (alarm is null)
            {
                return Results.NotFound(NotFoundResponse());
            }

            return Results.Ok(ToResponse(alarm, occurrenceService, DateTimeOffset.Now));
        });

        api.MapPost("/alarms", async (
            AlarmUpsertRequest request,
            IAlarmStore store,
            AlarmMutationService alarmMutations,
            AlarmOccurrenceService occurrenceService,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            var logger = loggerFactory.CreateLogger("WakeMeUp.Api.Alarms");
            if (!TryCreateInput(Guid.NewGuid(), request, out var input, out var parseError))
            {
                return Results.BadRequest(ValidationResponse(parseError!));
            }

            if (!alarmMutations.TryBuild(input!, existingAlarm: null, out var alarm, out var validationError))
            {
                return Results.BadRequest(ValidationResponse(validationError!));
            }

            await store.SaveAlarmAsync(alarm, cancellationToken);
            logger.LogInformation(
                "[{LoggedAt}] Alarm set via API: Name='{AlarmName}', Time='{AlarmTime}', Description='{AlarmDescription}'",
                GetLogTimestamp(),
                alarm.Name,
                alarm.Time.ToString("HH\\:mm"),
                GetLogDescription(alarm.Description));

            return Results.Created($"/api/v1/alarms/{alarm.Id}", ToResponse(alarm, occurrenceService, DateTimeOffset.Now));
        });

        api.MapPut("/alarms/{alarmId:guid}", async (
            Guid alarmId,
            AlarmUpsertRequest request,
            IAlarmStore store,
            AlarmMutationService alarmMutations,
            AlarmOccurrenceService occurrenceService,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            var logger = loggerFactory.CreateLogger("WakeMeUp.Api.Alarms");
            var existingAlarm = (await store.GetAlarmsAsync(cancellationToken)).FirstOrDefault(x => x.Id == alarmId);
            if (existingAlarm is null)
            {
                return Results.NotFound(NotFoundResponse());
            }

            if (!TryCreateInput(alarmId, request, out var input, out var parseError))
            {
                return Results.BadRequest(ValidationResponse(parseError!));
            }

            if (!alarmMutations.TryBuild(input!, existingAlarm, out var alarm, out var validationError))
            {
                return Results.BadRequest(ValidationResponse(validationError!));
            }

            await store.SaveAlarmAsync(alarm, cancellationToken);
            logger.LogInformation(
                "[{LoggedAt}] Alarm updated via API: Name='{AlarmName}', Time='{AlarmTime}', Description='{AlarmDescription}'",
                GetLogTimestamp(),
                alarm.Name,
                alarm.Time.ToString("HH\\:mm"),
                GetLogDescription(alarm.Description));

            return Results.Ok(ToResponse(alarm, occurrenceService, DateTimeOffset.Now));
        });

        api.MapPatch("/alarms/{alarmId:guid}/enabled", async (
            Guid alarmId,
            AlarmToggleRequest request,
            IAlarmStore store,
            AlarmMutationService alarmMutations,
            AlarmOccurrenceService occurrenceService,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            var logger = loggerFactory.CreateLogger("WakeMeUp.Api.Alarms");
            var existingAlarm = (await store.GetAlarmsAsync(cancellationToken)).FirstOrDefault(x => x.Id == alarmId);
            if (existingAlarm is null)
            {
                return Results.NotFound(NotFoundResponse());
            }

            var updatedAlarm = alarmMutations.ApplyEnabledState(existingAlarm, request.IsEnabled);
            await store.SaveAlarmAsync(updatedAlarm, cancellationToken);

            logger.LogInformation(
                "[{LoggedAt}] Alarm updated via API: Name='{AlarmName}', Time='{AlarmTime}', Description='{AlarmDescription}', Enabled={IsEnabled}",
                GetLogTimestamp(),
                updatedAlarm.Name,
                updatedAlarm.Time.ToString("HH\\:mm"),
                GetLogDescription(updatedAlarm.Description),
                updatedAlarm.IsEnabled);

            return Results.Ok(ToResponse(updatedAlarm, occurrenceService, DateTimeOffset.Now));
        });

        api.MapDelete("/alarms/{alarmId:guid}", async (
            Guid alarmId,
            IAlarmStore store,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            var logger = loggerFactory.CreateLogger("WakeMeUp.Api.Alarms");
            var existingAlarm = (await store.GetAlarmsAsync(cancellationToken)).FirstOrDefault(x => x.Id == alarmId);
            if (existingAlarm is null)
            {
                return Results.NotFound(NotFoundResponse());
            }

            await store.DeleteAlarmAsync(alarmId, cancellationToken);
            logger.LogInformation(
                "[{LoggedAt}] Alarm deleted via API: Name='{AlarmName}', Time='{AlarmTime}', Description='{AlarmDescription}'",
                GetLogTimestamp(),
                existingAlarm.Name,
                existingAlarm.Time.ToString("HH\\:mm"),
                GetLogDescription(existingAlarm.Description));

            return Results.NoContent();
        });

        api.MapGet("/meta", async (IAlarmStore store, UiTextService texts, CancellationToken cancellationToken) =>
        {
            var settings = await store.GetSettingsAsync(cancellationToken);
            var supportedLanguages = texts.GetSupportedLanguages()
                .Select(language => new ApiOptionResponse(ToLanguageValue(language.Value), language.DisplayName))
                .ToArray();

            var repeatModes = new[]
            {
                new ApiOptionResponse("never", "Never"),
                new ApiOptionResponse("daily", "Daily"),
                new ApiOptionResponse("weekdays", "Weekdays"),
                new ApiOptionResponse("weekends", "Weekends"),
                new ApiOptionResponse("custom_days", "Custom days")
            };

            var weekdays = new[]
            {
                new ApiOptionResponse("monday", "Monday"),
                new ApiOptionResponse("tuesday", "Tuesday"),
                new ApiOptionResponse("wednesday", "Wednesday"),
                new ApiOptionResponse("thursday", "Thursday"),
                new ApiOptionResponse("friday", "Friday"),
                new ApiOptionResponse("saturday", "Saturday"),
                new ApiOptionResponse("sunday", "Sunday")
            };

            return Results.Ok(new AlarmApiMetadataResponse(
                TimeZoneInfo.Local.Id,
                TimeZoneInfo.Local.DisplayName,
                ToLanguageValue(settings.Language),
                settings.ThemeMode.ToString().ToLowerInvariant(),
                DateTimeOffset.Now.ToString("O"),
                supportedLanguages,
                repeatModes,
                weekdays));
        });

        return endpoints;
    }

    private static AlarmResponse ToResponse(
        AlarmDefinition alarm,
        AlarmOccurrenceService occurrenceService,
        DateTimeOffset now)
    {
        var nextTrigger = occurrenceService.GetNextOccurrence(alarm, now);
        return new AlarmResponse(
            alarm.Id,
            alarm.Name,
            alarm.Description,
            alarm.Time.ToString("HH\\:mm"),
            alarm.IsEnabled,
            ToRepeatModeValue(alarm.RepeatMode),
            ToDayValues(alarm.Days),
            nextTrigger?.ToString("O"),
            alarm.CreatedUtc.ToString("O"),
            alarm.LastTriggeredUtc?.ToString("O"),
            alarm.LastResultMessage);
    }

    private static bool TryCreateInput(
        Guid alarmId,
        AlarmUpsertRequest request,
        out AlarmMutationInput? input,
        out AlarmValidationError? validationError)
    {
        input = null;
        validationError = null;

        if (!TryParseRepeatMode(request.RepeatMode, out var repeatMode))
        {
            validationError = new AlarmValidationError("repeatMode", "repeat_mode_invalid", "Repeat mode is invalid.");
            return false;
        }

        if (!TryParseDays(request.Days, out var days))
        {
            validationError = new AlarmValidationError("days", "day_invalid", "One or more selected days are invalid.");
            return false;
        }

        input = new AlarmMutationInput(
            alarmId,
            request.Name,
            request.Description,
            request.Time,
            request.IsEnabled,
            repeatMode,
            days);
        return true;
    }

    private static bool TryParseRepeatMode(string? value, out RepeatMode repeatMode)
    {
        var normalizedValue = value?.Trim().ToLowerInvariant();
        repeatMode = normalizedValue switch
        {
            "never" => RepeatMode.Never,
            "daily" => RepeatMode.Daily,
            "weekdays" => RepeatMode.Weekdays,
            "weekends" => RepeatMode.Weekends,
            "custom_days" => RepeatMode.CustomDays,
            "customdays" => RepeatMode.CustomDays,
            _ => default
        };

        if (normalizedValue is null)
        {
            return false;
        }

        if (Enum.TryParse<RepeatMode>(value, ignoreCase: true, out repeatMode))
        {
            return true;
        }

        return normalizedValue is "never" or "daily" or "weekdays" or "weekends" or "custom_days" or "customdays";
    }

    private static bool TryParseDays(IReadOnlyList<string>? values, out WeekdaySelection days)
    {
        days = WeekdaySelection.None;

        if (values is null)
        {
            return true;
        }

        foreach (var value in values)
        {
            var normalizedValue = value.Trim().ToLowerInvariant();
            var day = normalizedValue switch
            {
                "monday" => WeekdaySelection.Monday,
                "tuesday" => WeekdaySelection.Tuesday,
                "wednesday" => WeekdaySelection.Wednesday,
                "thursday" => WeekdaySelection.Thursday,
                "friday" => WeekdaySelection.Friday,
                "saturday" => WeekdaySelection.Saturday,
                "sunday" => WeekdaySelection.Sunday,
                _ => WeekdaySelection.None
            };

            if (day == WeekdaySelection.None)
            {
                return false;
            }

            days |= day;
        }

        return true;
    }

    private static IReadOnlyList<string> ToDayValues(WeekdaySelection days)
    {
        var result = new List<string>();

        if (days.HasFlag(WeekdaySelection.Monday))
        {
            result.Add("monday");
        }

        if (days.HasFlag(WeekdaySelection.Tuesday))
        {
            result.Add("tuesday");
        }

        if (days.HasFlag(WeekdaySelection.Wednesday))
        {
            result.Add("wednesday");
        }

        if (days.HasFlag(WeekdaySelection.Thursday))
        {
            result.Add("thursday");
        }

        if (days.HasFlag(WeekdaySelection.Friday))
        {
            result.Add("friday");
        }

        if (days.HasFlag(WeekdaySelection.Saturday))
        {
            result.Add("saturday");
        }

        if (days.HasFlag(WeekdaySelection.Sunday))
        {
            result.Add("sunday");
        }

        return result;
    }

    private static string ToRepeatModeValue(RepeatMode repeatMode)
    {
        return repeatMode switch
        {
            RepeatMode.Never => "never",
            RepeatMode.Daily => "daily",
            RepeatMode.Weekdays => "weekdays",
            RepeatMode.Weekends => "weekends",
            RepeatMode.CustomDays => "custom_days",
            _ => "never"
        };
    }

    private static string ToLanguageValue(AppLanguage language)
    {
        return language switch
        {
            AppLanguage.English => "english",
            AppLanguage.German => "german",
            AppLanguage.French => "french",
            AppLanguage.Spanish => "spanish",
            AppLanguage.Portuguese => "portuguese",
            AppLanguage.Italian => "italian",
            AppLanguage.Slovak => "slovak",
            AppLanguage.Czech => "czech",
            AppLanguage.Polish => "polish",
            AppLanguage.Ukrainian => "ukrainian",
            AppLanguage.Greek => "greek",
            AppLanguage.Esperanto => "esperanto",
            AppLanguage.Klingon => "klingon",
            _ => "english"
        };
    }

    private static ApiErrorResponse ValidationResponse(AlarmValidationError validationError)
    {
        return new ApiErrorResponse(
            "Validation failed.",
            [new ApiValidationErrorResponse(validationError.Field, validationError.Code, validationError.Message)]);
    }

    private static ApiErrorResponse NotFoundResponse()
    {
        return new ApiErrorResponse("Alarm not found.", []);
    }

    private static string GetLogDescription(string? description)
    {
        return description?.Trim() ?? string.Empty;
    }

    private static string GetLogTimestamp()
    {
        return DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
    }
}
