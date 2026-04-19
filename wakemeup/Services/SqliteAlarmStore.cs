using Microsoft.Data.Sqlite;
using System.Data.Common;
using WakeMeUp.Domain;

namespace WakeMeUp.Services;

public sealed class SqliteAlarmStore(IWebHostEnvironment environment, ILogger<SqliteAlarmStore> logger) : IAlarmStore
{
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private readonly string _databasePath = ResolveDatabasePath(environment);
    private bool _initialized;

    public async Task<AppState> GetStateAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        return new AppState
        {
            Alarms = (await GetAlarmsAsync(cancellationToken)).ToList(),
            Settings = await GetSettingsAsync(cancellationToken)
        };
    }

    public async Task<IReadOnlyList<AlarmDefinition>> GetAlarmsAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);

        try
        {
            await using var connection = OpenConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, name, description, time, is_enabled, repeat_mode, days, created_utc,
                       last_processed_occurrence_utc, last_triggered_utc, last_result_message
                FROM alarms
                ORDER BY time, name;
                """;

            var result = new List<AlarmDefinition>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new AlarmDefinition
                {
                    Id = Guid.Parse(reader.GetString(0)),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Time = TimeOnly.Parse(reader.GetString(3)),
                    IsEnabled = reader.GetInt64(4) == 1,
                    RepeatMode = (RepeatMode)reader.GetInt32(5),
                    Days = (WeekdaySelection)reader.GetInt32(6),
                    CreatedUtc = DateTimeOffset.Parse(reader.GetString(7)),
                    LastProcessedOccurrenceUtc = reader.IsDBNull(8) ? null : DateTimeOffset.Parse(reader.GetString(8)),
                    LastTriggeredUtc = reader.IsDBNull(9) ? null : DateTimeOffset.Parse(reader.GetString(9)),
                    LastResultMessage = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                });
            }

            return result;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<AppSettings> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);

        try
        {
            await using var connection = OpenConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText = "SELECT theme_mode, language, language_initialized FROM app_settings WHERE id = 1;";

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return new AppSettings();
            }

            return new AppSettings
            {
                ThemeMode = Enum.TryParse<ThemeMode>(Convert.ToString(reader.GetValue(0)), out var themeMode)
                    ? themeMode
                    : ThemeMode.Auto,
                Language = Enum.TryParse<AppLanguage>(Convert.ToString(reader.GetValue(1)), out var language)
                    ? language
                    : AppLanguage.English,
                LanguageInitialized = !reader.IsDBNull(2) && reader.GetInt64(2) == 1
            };
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<AlarmDefinition> SaveAlarmAsync(AlarmDefinition alarm, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);

        try
        {
            await using var connection = OpenConnection();
            await connection.OpenAsync(cancellationToken);
            await SaveAlarmInternalAsync(connection, alarm, transaction: null, cancellationToken);

            return alarm;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task SaveAlarmsAsync(IEnumerable<AlarmDefinition> alarms, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);

        try
        {
            await using var connection = OpenConnection();
            await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            foreach (var alarm in alarms)
            {
                await SaveAlarmInternalAsync(connection, alarm, transaction, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task DeleteAlarmAsync(Guid alarmId, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);

        try
        {
            await using var connection = OpenConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM alarms WHERE id = $id;";
            command.Parameters.AddWithValue("$id", alarmId.ToString());
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);

        try
        {
            await using var connection = OpenConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO app_settings (id, theme_mode, language, language_initialized)
                VALUES (1, $theme_mode, $language, $language_initialized)
                ON CONFLICT(id) DO UPDATE SET
                    theme_mode = excluded.theme_mode,
                    language = excluded.language,
                    language_initialized = excluded.language_initialized;
                """;

            command.Parameters.AddWithValue("$theme_mode", settings.ThemeMode.ToString());
            command.Parameters.AddWithValue("$language", settings.Language.ToString());
            command.Parameters.AddWithValue("$language_initialized", settings.LanguageInitialized ? 1 : 0);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        await _mutex.WaitAsync(cancellationToken);

        try
        {
            if (_initialized)
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_databasePath)!);

            await using var connection = OpenConnection();
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText =
                """
                PRAGMA journal_mode = WAL;

                CREATE TABLE IF NOT EXISTS alarms (
                    id TEXT PRIMARY KEY,
                    name TEXT NOT NULL,
                    description TEXT NOT NULL,
                    time TEXT NOT NULL,
                    is_enabled INTEGER NOT NULL,
                    repeat_mode INTEGER NOT NULL,
                    days INTEGER NOT NULL,
                    created_utc TEXT NOT NULL,
                    last_processed_occurrence_utc TEXT NULL,
                    last_triggered_utc TEXT NULL,
                    last_result_message TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS app_settings (
                    id INTEGER PRIMARY KEY CHECK (id = 1),
                    theme_mode TEXT NOT NULL,
                    language TEXT NOT NULL DEFAULT 'English',
                    language_initialized INTEGER NOT NULL DEFAULT 0
                );
                """;

            await command.ExecuteNonQueryAsync(cancellationToken);
            await EnsureColumnExistsAsync(connection, "app_settings", "language", "TEXT NOT NULL DEFAULT 'English'", cancellationToken);
            await EnsureColumnExistsAsync(connection, "app_settings", "language_initialized", "INTEGER NOT NULL DEFAULT 1", cancellationToken);
            await SeedDefaultsAsync(connection, cancellationToken);

            _initialized = true;
            logger.LogInformation("WakeMeUp SQLite store initialized at {DatabasePath}", _databasePath);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private async Task SeedDefaultsAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        var settingsCountCommand = connection.CreateCommand();
        settingsCountCommand.CommandText = "SELECT COUNT(*) FROM app_settings;";
        var settingsCount = Convert.ToInt32(await settingsCountCommand.ExecuteScalarAsync(cancellationToken));

        if (settingsCount == 0)
        {
            var insertSettings = connection.CreateCommand();
            insertSettings.CommandText = "INSERT INTO app_settings (id, theme_mode, language, language_initialized) VALUES (1, $theme_mode, $language, $language_initialized);";
            insertSettings.Parameters.AddWithValue("$theme_mode", ThemeMode.Auto.ToString());
            insertSettings.Parameters.AddWithValue("$language", AppLanguage.English.ToString());
            insertSettings.Parameters.AddWithValue("$language_initialized", 0);
            await insertSettings.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private static async Task EnsureColumnExistsAsync(
        SqliteConnection connection,
        string tableName,
        string columnName,
        string columnDefinition,
        CancellationToken cancellationToken)
    {
        var pragmaCommand = connection.CreateCommand();
        pragmaCommand.CommandText = $"PRAGMA table_info({tableName});";

        await using var reader = await pragmaCommand.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        await reader.DisposeAsync();

        var alterCommand = connection.CreateCommand();
        alterCommand.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};";
        await alterCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    private SqliteConnection OpenConnection()
    {
        return new SqliteConnection($"Data Source={_databasePath}");
    }

    private static async Task SaveAlarmInternalAsync(
        SqliteConnection connection,
        AlarmDefinition alarm,
        DbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction as SqliteTransaction;
        command.CommandText =
            """
            INSERT INTO alarms (
                id, name, description, time, is_enabled, repeat_mode, days, created_utc,
                last_processed_occurrence_utc, last_triggered_utc, last_result_message
            )
            VALUES (
                $id, $name, $description, $time, $is_enabled, $repeat_mode, $days, $created_utc,
                $last_processed_occurrence_utc, $last_triggered_utc, $last_result_message
            )
            ON CONFLICT(id) DO UPDATE SET
                name = excluded.name,
                description = excluded.description,
                time = excluded.time,
                is_enabled = excluded.is_enabled,
                repeat_mode = excluded.repeat_mode,
                days = excluded.days,
                created_utc = excluded.created_utc,
                last_processed_occurrence_utc = excluded.last_processed_occurrence_utc,
                last_triggered_utc = excluded.last_triggered_utc,
                last_result_message = excluded.last_result_message;
            """;

        AddAlarmParameters(command, alarm);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddAlarmParameters(SqliteCommand command, AlarmDefinition alarm)
    {
        command.Parameters.AddWithValue("$id", alarm.Id.ToString());
        command.Parameters.AddWithValue("$name", alarm.Name);
        command.Parameters.AddWithValue("$description", alarm.Description);
        command.Parameters.AddWithValue("$time", alarm.Time.ToString("HH\\:mm"));
        command.Parameters.AddWithValue("$is_enabled", alarm.IsEnabled ? 1 : 0);
        command.Parameters.AddWithValue("$repeat_mode", (int)alarm.RepeatMode);
        command.Parameters.AddWithValue("$days", (int)alarm.Days);
        command.Parameters.AddWithValue("$created_utc", alarm.CreatedUtc.ToString("O"));
        command.Parameters.AddWithValue("$last_processed_occurrence_utc", alarm.LastProcessedOccurrenceUtc?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$last_triggered_utc", alarm.LastTriggeredUtc?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$last_result_message", alarm.LastResultMessage);
    }

    private static string ResolveDatabasePath(IWebHostEnvironment environment)
    {
        var overridePath = Environment.GetEnvironmentVariable("WAKEMEUP_DB_PATH");
        if (!string.IsNullOrWhiteSpace(overridePath))
        {
            return overridePath;
        }

        const string persistentAddonDirectory = "/data";
        if (Directory.Exists(persistentAddonDirectory))
        {
            return Path.Combine(persistentAddonDirectory, "wakemeup.db");
        }

        return Path.Combine(environment.ContentRootPath, "App_Data", "wakemeup.db");
    }
}
