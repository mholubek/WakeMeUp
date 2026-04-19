namespace WakeMeUp.Domain;

public sealed class AppSettings
{
    public ThemeMode ThemeMode { get; set; } = ThemeMode.Auto;
    public AppLanguage Language { get; set; } = AppLanguage.English;
    public bool LanguageInitialized { get; set; }
}
