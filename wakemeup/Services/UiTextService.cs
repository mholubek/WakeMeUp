using System.Globalization;
using WakeMeUp.Domain;

namespace WakeMeUp.Services;

public sealed class UiTextService
{
    private static readonly IReadOnlyList<LanguageOption> SupportedLanguages =
    [
        new(AppLanguage.English, "English"),
        new(AppLanguage.German, "Deutsch"),
        new(AppLanguage.French, "Français"),
        new(AppLanguage.Spanish, "Español"),
        new(AppLanguage.Portuguese, "Português"),
        new(AppLanguage.Italian, "Italiano"),
        new(AppLanguage.Slovak, "Slovenčina"),
        new(AppLanguage.Czech, "Čeština"),
        new(AppLanguage.Polish, "Polski"),
        new(AppLanguage.Ukrainian, "Українська"),
        new(AppLanguage.Greek, "Ελληνικά"),
        new(AppLanguage.Esperanto, "Esperanto"),
        new(AppLanguage.Klingon, "tlhIngan Hol")
    ];

    private static readonly IReadOnlyDictionary<AppLanguage, IReadOnlyDictionary<string, string>> Translations =
        new Dictionary<AppLanguage, IReadOnlyDictionary<string, string>>
        {
            [AppLanguage.English] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp",
                ["BrandSection"] = "Alarms",
                ["NavAlarmOverview"] = "Alarm overview",
                ["NavSettings"] = "Settings",
                ["OpenMenu"] = "Open menu",
                ["UnexpectedError"] = "An unexpected error has occurred.",
                ["Reload"] = "Reload",
                ["Settings"] = "Settings",
                ["Theme"] = "Theme",
                ["ThemeDescription"] = "Auto mode follows the Home Assistant theme when available and falls back to the device theme.",
                ["Automatic"] = "Automatic",
                ["Light"] = "Light",
                ["Dark"] = "Dark",
                ["Language"] = "Language",
                ["SaveSettings"] = "Save settings",
                ["SettingsSaved"] = "Settings saved.",
                ["TimeZone"] = "Time zone",
                ["PageTitleSettings"] = "Settings",
                ["PageTitleHome"] = "WakeMeUp",
                ["Alarms"] = "Alarms",
                ["AlarmOverview"] = "Alarm overview",
                ["NewAlarm"] = "New alarm",
                ["LoadingSavedAlarms"] = "Loading saved alarms...",
                ["NoAlarmsYet"] = "No alarms yet",
                ["NoAlarmsYetText"] = "Create your first alarm and set a time and repeat rule.",
                ["Enabled"] = "Enabled",
                ["Disabled"] = "Disabled",
                ["NextTrigger"] = "Next trigger",
                ["NotScheduled"] = "Not scheduled",
                ["Edit"] = "Edit",
                ["Delete"] = "Delete",
                ["Editor"] = "Editor",
                ["EditAlarm"] = "Edit alarm",
                ["Close"] = "Close",
                ["Name"] = "Name",
                ["Time"] = "Time",
                ["Repeat"] = "Repeat",
                ["ChooseDays"] = "Choose days",
                ["Note"] = "Note",
                ["AlarmIsEnabled"] = "Alarm is enabled",
                ["SaveAlarm"] = "Save alarm",
                ["ClearForm"] = "Clear form",
                ["AlarmEnabled"] = "Alarm enabled.",
                ["AlarmDisabled"] = "Alarm disabled.",
                ["AlarmDeleted"] = "Alarm deleted.",
                ["AlarmCreated"] = "Alarm created.",
                ["AlarmUpdated"] = "Alarm updated.",
                ["UnableUpdateAlarm"] = "Unable to update the alarm.",
                ["UnableDeleteAlarm"] = "Unable to delete the alarm.",
                ["UnableSaveAlarm"] = "Unable to save the alarm.",
                ["RepeatNever"] = "Never",
                ["RepeatDaily"] = "Daily",
                ["RepeatWeekdays"] = "Weekdays",
                ["RepeatWeekends"] = "Weekends",
                ["RepeatCustomDays"] = "Custom days",
                ["Mon"] = "Mon",
                ["Tue"] = "Tue",
                ["Wed"] = "Wed",
                ["Thu"] = "Thu",
                ["Fri"] = "Fri",
                ["Sat"] = "Sat",
                ["Sun"] = "Sun",
                ["ValidationNameRequired"] = "Name is required.",
                ["ValidationNameTooLong"] = "Name must be 80 characters or fewer.",
                ["ValidationTimeFormat"] = "Time must use the HH:mm format.",
                ["ValidationDescriptionTooLong"] = "Note must be 160 characters or fewer.",
                ["ValidationSelectDay"] = "Select at least one day for a custom repeat rule.",
                ["PageTitleError"] = "Application error",
                ["Error"] = "Error",
                ["ErrorHeadline"] = "An error occurred while processing your request",
                ["RequestId"] = "Request ID",
                ["ErrorHelp"] = "If the problem persists, check the container logs and the Home Assistant connection setup.",
                ["PageTitleNotFound"] = "Page not found",
                ["NotFoundHeadline"] = "Page not found",
                ["NotFoundText"] = "Check the address or return to the alarm overview.",
                ["BackToDashboard"] = "Back to dashboard",
                ["RejoiningServer"] = "Rejoining the server...",
                ["RejoinFailedRetrying"] = "Rejoin failed... trying again in {0} seconds.",
                ["FailedToRejoin"] = "Failed to rejoin.",
                ["PleaseRetryOrReload"] = "Please retry or reload the page.",
                ["Retry"] = "Retry",
                ["SessionPaused"] = "The session has been paused by the server.",
                ["FailedToResume"] = "Failed to resume the session.",
                ["Resume"] = "Resume"
            },
            [AppLanguage.German] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Alarme", ["NavAlarmOverview"] = "Alarmübersicht", ["NavSettings"] = "Einstellungen",
                ["OpenMenu"] = "Menü öffnen", ["UnexpectedError"] = "Ein unerwarteter Fehler ist aufgetreten.", ["Reload"] = "Neu laden",
                ["Settings"] = "Einstellungen", ["Theme"] = "Design", ["ThemeDescription"] = "Der automatische Modus übernimmt das Home Assistant-Design, falls verfügbar, und sonst das Geräte-Design.",
                ["Automatic"] = "Automatisch", ["Light"] = "Hell", ["Dark"] = "Dunkel", ["Language"] = "Sprache", ["SaveSettings"] = "Einstellungen speichern",
                ["SettingsSaved"] = "Einstellungen gespeichert.", ["TimeZone"] = "Zeitzone", ["PageTitleSettings"] = "Einstellungen", ["PageTitleHome"] = "WakeMeUp",
                ["Alarms"] = "Alarme", ["AlarmOverview"] = "Alarmübersicht", ["NewAlarm"] = "Neuer Alarm", ["LoadingSavedAlarms"] = "Gespeicherte Alarme werden geladen...",
                ["NoAlarmsYet"] = "Noch keine Alarme", ["NoAlarmsYetText"] = "Erstelle deinen ersten Alarm und lege Uhrzeit sowie Wiederholung fest.", ["Enabled"] = "Aktiv",
                ["Disabled"] = "Deaktiviert", ["NextTrigger"] = "Nächster Alarm", ["NotScheduled"] = "Nicht geplant", ["Edit"] = "Bearbeiten",
                ["Delete"] = "Löschen", ["Editor"] = "Editor", ["EditAlarm"] = "Alarm bearbeiten", ["Close"] = "Schließen", ["Name"] = "Name",
                ["Time"] = "Uhrzeit", ["Repeat"] = "Wiederholung", ["ChooseDays"] = "Tage auswählen", ["Note"] = "Notiz", ["AlarmIsEnabled"] = "Alarm ist aktiv",
                ["SaveAlarm"] = "Alarm speichern", ["ClearForm"] = "Formular leeren", ["AlarmEnabled"] = "Alarm aktiviert.", ["AlarmDisabled"] = "Alarm deaktiviert.",
                ["AlarmDeleted"] = "Alarm gelöscht.", ["AlarmCreated"] = "Alarm erstellt.", ["AlarmUpdated"] = "Alarm aktualisiert.", ["UnableUpdateAlarm"] = "Alarm konnte nicht aktualisiert werden.",
                ["UnableDeleteAlarm"] = "Alarm konnte nicht gelöscht werden.", ["UnableSaveAlarm"] = "Alarm konnte nicht gespeichert werden.", ["RepeatNever"] = "Nie",
                ["RepeatDaily"] = "Täglich", ["RepeatWeekdays"] = "Werktage", ["RepeatWeekends"] = "Wochenende", ["RepeatCustomDays"] = "Benutzerdefinierte Tage",
                ["Mon"] = "Mo", ["Tue"] = "Di", ["Wed"] = "Mi", ["Thu"] = "Do", ["Fri"] = "Fr", ["Sat"] = "Sa", ["Sun"] = "So",
                ["ValidationNameRequired"] = "Name ist erforderlich.", ["ValidationNameTooLong"] = "Der Name darf höchstens 80 Zeichen haben.",
                ["ValidationTimeFormat"] = "Die Uhrzeit muss im Format HH:mm sein.", ["ValidationDescriptionTooLong"] = "Die Notiz darf höchstens 160 Zeichen haben.",
                ["ValidationSelectDay"] = "Wähle mindestens einen Tag für die benutzerdefinierte Wiederholung.", ["PageTitleError"] = "Anwendungsfehler", ["Error"] = "Fehler",
                ["ErrorHeadline"] = "Beim Verarbeiten deiner Anfrage ist ein Fehler aufgetreten", ["RequestId"] = "Anfrage-ID", ["ErrorHelp"] = "Wenn das Problem bleibt, prüfe die Container-Logs und die Home Assistant-Verbindung.",
                ["PageTitleNotFound"] = "Seite nicht gefunden", ["NotFoundHeadline"] = "Seite nicht gefunden", ["NotFoundText"] = "Prüfe die Adresse oder kehre zur Alarmübersicht zurück.",
                ["BackToDashboard"] = "Zurück zum Dashboard", ["RejoiningServer"] = "Verbindung zum Server wird wiederhergestellt...", ["RejoinFailedRetrying"] = "Verbindung fehlgeschlagen... neuer Versuch in {0} Sekunden.",
                ["FailedToRejoin"] = "Wiederverbinden fehlgeschlagen.", ["PleaseRetryOrReload"] = "Bitte erneut versuchen oder Seite neu laden.", ["Retry"] = "Erneut versuchen",
                ["SessionPaused"] = "Die Sitzung wurde vom Server pausiert.", ["FailedToResume"] = "Die Sitzung konnte nicht fortgesetzt werden.", ["Resume"] = "Fortsetzen"
            },
            [AppLanguage.French] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Alarmes", ["NavAlarmOverview"] = "Vue des alarmes", ["NavSettings"] = "Paramètres",
                ["OpenMenu"] = "Ouvrir le menu", ["UnexpectedError"] = "Une erreur inattendue s'est produite.", ["Reload"] = "Recharger",
                ["Settings"] = "Paramètres", ["Theme"] = "Thème", ["ThemeDescription"] = "Le mode automatique suit le thème Home Assistant si disponible, sinon le thème de l'appareil.",
                ["Automatic"] = "Automatique", ["Light"] = "Clair", ["Dark"] = "Sombre", ["Language"] = "Langue", ["SaveSettings"] = "Enregistrer les paramètres",
                ["SettingsSaved"] = "Paramètres enregistrés.", ["TimeZone"] = "Fuseau horaire", ["PageTitleSettings"] = "Paramètres", ["PageTitleHome"] = "WakeMeUp",
                ["Alarms"] = "Alarmes", ["AlarmOverview"] = "Vue des alarmes", ["NewAlarm"] = "Nouvelle alarme", ["LoadingSavedAlarms"] = "Chargement des alarmes enregistrées...",
                ["NoAlarmsYet"] = "Aucune alarme", ["NoAlarmsYetText"] = "Créez votre première alarme et définissez l'heure et la répétition.", ["Enabled"] = "Activée",
                ["Disabled"] = "Désactivée", ["NextTrigger"] = "Prochain déclenchement", ["NotScheduled"] = "Non planifiée", ["Edit"] = "Modifier", ["Delete"] = "Supprimer",
                ["Editor"] = "Éditeur", ["EditAlarm"] = "Modifier l'alarme", ["Close"] = "Fermer", ["Name"] = "Nom", ["Time"] = "Heure", ["Repeat"] = "Répétition",
                ["ChooseDays"] = "Choisir les jours", ["Note"] = "Note", ["AlarmIsEnabled"] = "L'alarme est activée", ["SaveAlarm"] = "Enregistrer l'alarme", ["ClearForm"] = "Effacer le formulaire",
                ["AlarmEnabled"] = "Alarme activée.", ["AlarmDisabled"] = "Alarme désactivée.", ["AlarmDeleted"] = "Alarme supprimée.", ["AlarmCreated"] = "Alarme créée.",
                ["AlarmUpdated"] = "Alarme mise à jour.", ["UnableUpdateAlarm"] = "Impossible de mettre à jour l'alarme.", ["UnableDeleteAlarm"] = "Impossible de supprimer l'alarme.",
                ["UnableSaveAlarm"] = "Impossible d'enregistrer l'alarme.", ["RepeatNever"] = "Jamais", ["RepeatDaily"] = "Tous les jours", ["RepeatWeekdays"] = "Jours ouvrés",
                ["RepeatWeekends"] = "Week-ends", ["RepeatCustomDays"] = "Jours personnalisés", ["Mon"] = "Lun", ["Tue"] = "Mar", ["Wed"] = "Mer", ["Thu"] = "Jeu",
                ["Fri"] = "Ven", ["Sat"] = "Sam", ["Sun"] = "Dim", ["ValidationNameRequired"] = "Le nom est requis.", ["ValidationNameTooLong"] = "Le nom doit contenir 80 caractères maximum.",
                ["ValidationTimeFormat"] = "L'heure doit être au format HH:mm.", ["ValidationDescriptionTooLong"] = "La note doit contenir 160 caractères maximum.", ["ValidationSelectDay"] = "Sélectionnez au moins un jour pour la répétition personnalisée.",
                ["PageTitleError"] = "Erreur de l'application", ["Error"] = "Erreur", ["ErrorHeadline"] = "Une erreur s'est produite lors du traitement de votre demande", ["RequestId"] = "ID de requête",
                ["ErrorHelp"] = "Si le problème persiste, vérifiez les journaux du conteneur et la configuration Home Assistant.", ["PageTitleNotFound"] = "Page introuvable",
                ["NotFoundHeadline"] = "Page introuvable", ["NotFoundText"] = "Vérifiez l'adresse ou revenez à la vue des alarmes.", ["BackToDashboard"] = "Retour au tableau de bord",
                ["RejoiningServer"] = "Reconnexion au serveur...", ["RejoinFailedRetrying"] = "Échec de la reconnexion... nouvelle tentative dans {0} secondes.", ["FailedToRejoin"] = "Échec de la reconnexion.",
                ["PleaseRetryOrReload"] = "Veuillez réessayer ou recharger la page.", ["Retry"] = "Réessayer", ["SessionPaused"] = "La session a été mise en pause par le serveur.",
                ["FailedToResume"] = "Échec de la reprise de la session.", ["Resume"] = "Reprendre"
            },
            [AppLanguage.Spanish] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Alarmas", ["NavAlarmOverview"] = "Resumen de alarmas", ["NavSettings"] = "Configuración", ["OpenMenu"] = "Abrir menú",
                ["UnexpectedError"] = "Ha ocurrido un error inesperado.", ["Reload"] = "Recargar", ["Settings"] = "Configuración", ["Theme"] = "Tema",
                ["ThemeDescription"] = "El modo automático sigue el tema de Home Assistant cuando está disponible y, si no, el tema del dispositivo.", ["Automatic"] = "Automático",
                ["Light"] = "Claro", ["Dark"] = "Oscuro", ["Language"] = "Idioma", ["SaveSettings"] = "Guardar configuración", ["SettingsSaved"] = "Configuración guardada.",
                ["TimeZone"] = "Zona horaria", ["PageTitleSettings"] = "Configuración", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Alarmas", ["AlarmOverview"] = "Resumen de alarmas",
                ["NewAlarm"] = "Nueva alarma", ["LoadingSavedAlarms"] = "Cargando alarmas guardadas...", ["NoAlarmsYet"] = "Aún no hay alarmas", ["NoAlarmsYetText"] = "Crea tu primera alarma y establece una hora y una repetición.",
                ["Enabled"] = "Activa", ["Disabled"] = "Desactivada", ["NextTrigger"] = "Próximo disparo", ["NotScheduled"] = "No programada", ["Edit"] = "Editar", ["Delete"] = "Eliminar",
                ["Editor"] = "Editor", ["EditAlarm"] = "Editar alarma", ["Close"] = "Cerrar", ["Name"] = "Nombre", ["Time"] = "Hora", ["Repeat"] = "Repetición",
                ["ChooseDays"] = "Elegir días", ["Note"] = "Nota", ["AlarmIsEnabled"] = "La alarma está activa", ["SaveAlarm"] = "Guardar alarma", ["ClearForm"] = "Limpiar formulario",
                ["AlarmEnabled"] = "Alarma activada.", ["AlarmDisabled"] = "Alarma desactivada.", ["AlarmDeleted"] = "Alarma eliminada.", ["AlarmCreated"] = "Alarma creada.", ["AlarmUpdated"] = "Alarma actualizada.",
                ["UnableUpdateAlarm"] = "No se pudo actualizar la alarma.", ["UnableDeleteAlarm"] = "No se pudo eliminar la alarma.", ["UnableSaveAlarm"] = "No se pudo guardar la alarma.",
                ["RepeatNever"] = "Nunca", ["RepeatDaily"] = "Diariamente", ["RepeatWeekdays"] = "Días laborables", ["RepeatWeekends"] = "Fines de semana", ["RepeatCustomDays"] = "Días personalizados",
                ["Mon"] = "Lun", ["Tue"] = "Mar", ["Wed"] = "Mié", ["Thu"] = "Jue", ["Fri"] = "Vie", ["Sat"] = "Sáb", ["Sun"] = "Dom",
                ["ValidationNameRequired"] = "El nombre es obligatorio.", ["ValidationNameTooLong"] = "El nombre debe tener 80 caracteres o menos.", ["ValidationTimeFormat"] = "La hora debe usar el formato HH:mm.",
                ["ValidationDescriptionTooLong"] = "La nota debe tener 160 caracteres o menos.", ["ValidationSelectDay"] = "Selecciona al menos un día para una repetición personalizada.", ["PageTitleError"] = "Error de la aplicación",
                ["Error"] = "Error", ["ErrorHeadline"] = "Se produjo un error al procesar tu solicitud", ["RequestId"] = "ID de solicitud", ["ErrorHelp"] = "Si el problema persiste, revisa los registros del contenedor y la conexión con Home Assistant.",
                ["PageTitleNotFound"] = "Página no encontrada", ["NotFoundHeadline"] = "Página no encontrada", ["NotFoundText"] = "Comprueba la dirección o vuelve al resumen de alarmas.", ["BackToDashboard"] = "Volver al panel",
                ["RejoiningServer"] = "Reconectando al servidor...", ["RejoinFailedRetrying"] = "La reconexión falló... reintentando en {0} segundos.", ["FailedToRejoin"] = "No se pudo reconectar.",
                ["PleaseRetryOrReload"] = "Vuelve a intentarlo o recarga la página.", ["Retry"] = "Reintentar", ["SessionPaused"] = "La sesión ha sido pausada por el servidor.", ["FailedToResume"] = "No se pudo reanudar la sesión.", ["Resume"] = "Reanudar"
            },
            [AppLanguage.Portuguese] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Alarmes", ["NavAlarmOverview"] = "Visão geral dos alarmes", ["NavSettings"] = "Definições", ["OpenMenu"] = "Abrir menu",
                ["UnexpectedError"] = "Ocorreu um erro inesperado.", ["Reload"] = "Recarregar", ["Settings"] = "Definições", ["Theme"] = "Tema",
                ["ThemeDescription"] = "O modo automático segue o tema do Home Assistant quando disponível e, caso contrário, o tema do dispositivo.", ["Automatic"] = "Automático",
                ["Light"] = "Claro", ["Dark"] = "Escuro", ["Language"] = "Idioma", ["SaveSettings"] = "Guardar definições", ["SettingsSaved"] = "Definições guardadas.",
                ["TimeZone"] = "Fuso horário", ["PageTitleSettings"] = "Definições", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Alarmes", ["AlarmOverview"] = "Visão geral dos alarmes",
                ["NewAlarm"] = "Novo alarme", ["LoadingSavedAlarms"] = "A carregar alarmes guardados...", ["NoAlarmsYet"] = "Ainda não existem alarmes", ["NoAlarmsYetText"] = "Crie o seu primeiro alarme e defina uma hora e uma repetição.",
                ["Enabled"] = "Ativado", ["Disabled"] = "Desativado", ["NextTrigger"] = "Próximo disparo", ["NotScheduled"] = "Não agendado", ["Edit"] = "Editar", ["Delete"] = "Eliminar",
                ["Editor"] = "Editor", ["EditAlarm"] = "Editar alarme", ["Close"] = "Fechar", ["Name"] = "Nome", ["Time"] = "Hora", ["Repeat"] = "Repetição",
                ["ChooseDays"] = "Escolher dias", ["Note"] = "Nota", ["AlarmIsEnabled"] = "O alarme está ativado", ["SaveAlarm"] = "Guardar alarme", ["ClearForm"] = "Limpar formulário",
                ["AlarmEnabled"] = "Alarme ativado.", ["AlarmDisabled"] = "Alarme desativado.", ["AlarmDeleted"] = "Alarme eliminado.", ["AlarmCreated"] = "Alarme criado.", ["AlarmUpdated"] = "Alarme atualizado.",
                ["UnableUpdateAlarm"] = "Não foi possível atualizar o alarme.", ["UnableDeleteAlarm"] = "Não foi possível eliminar o alarme.", ["UnableSaveAlarm"] = "Não foi possível guardar o alarme.",
                ["RepeatNever"] = "Nunca", ["RepeatDaily"] = "Diariamente", ["RepeatWeekdays"] = "Dias úteis", ["RepeatWeekends"] = "Fins de semana", ["RepeatCustomDays"] = "Dias personalizados",
                ["Mon"] = "Seg", ["Tue"] = "Ter", ["Wed"] = "Qua", ["Thu"] = "Qui", ["Fri"] = "Sex", ["Sat"] = "Sáb", ["Sun"] = "Dom",
                ["ValidationNameRequired"] = "O nome é obrigatório.", ["ValidationNameTooLong"] = "O nome deve ter no máximo 80 caracteres.", ["ValidationTimeFormat"] = "A hora deve usar o formato HH:mm.",
                ["ValidationDescriptionTooLong"] = "A nota deve ter no máximo 160 caracteres.", ["ValidationSelectDay"] = "Selecione pelo menos um dia para uma repetição personalizada.", ["PageTitleError"] = "Erro da aplicação",
                ["Error"] = "Erro", ["ErrorHeadline"] = "Ocorreu um erro ao processar o seu pedido", ["RequestId"] = "ID do pedido", ["ErrorHelp"] = "Se o problema persistir, verifique os logs do contentor e a ligação ao Home Assistant.",
                ["PageTitleNotFound"] = "Página não encontrada", ["NotFoundHeadline"] = "Página não encontrada", ["NotFoundText"] = "Verifique o endereço ou volte à visão geral dos alarmes.", ["BackToDashboard"] = "Voltar ao painel",
                ["RejoiningServer"] = "A restabelecer ligação ao servidor...", ["RejoinFailedRetrying"] = "Falha ao restabelecer... a tentar novamente em {0} segundos.", ["FailedToRejoin"] = "Falha ao restabelecer ligação.",
                ["PleaseRetryOrReload"] = "Tente novamente ou recarregue a página.", ["Retry"] = "Tentar novamente", ["SessionPaused"] = "A sessão foi pausada pelo servidor.", ["FailedToResume"] = "Falha ao retomar a sessão.", ["Resume"] = "Retomar"
            },
            [AppLanguage.Italian] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Allarmi", ["NavAlarmOverview"] = "Panoramica allarmi", ["NavSettings"] = "Impostazioni", ["OpenMenu"] = "Apri menu",
                ["UnexpectedError"] = "Si è verificato un errore imprevisto.", ["Reload"] = "Ricarica", ["Settings"] = "Impostazioni", ["Theme"] = "Tema",
                ["ThemeDescription"] = "La modalità automatica segue il tema di Home Assistant quando disponibile, altrimenti il tema del dispositivo.", ["Automatic"] = "Automatico",
                ["Light"] = "Chiaro", ["Dark"] = "Scuro", ["Language"] = "Lingua", ["SaveSettings"] = "Salva impostazioni", ["SettingsSaved"] = "Impostazioni salvate.",
                ["TimeZone"] = "Fuso orario", ["PageTitleSettings"] = "Impostazioni", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Allarmi", ["AlarmOverview"] = "Panoramica allarmi",
                ["NewAlarm"] = "Nuova sveglia", ["LoadingSavedAlarms"] = "Caricamento delle sveglie salvate...", ["NoAlarmsYet"] = "Nessuna sveglia", ["NoAlarmsYetText"] = "Crea la tua prima sveglia e imposta ora e ripetizione.",
                ["Enabled"] = "Attiva", ["Disabled"] = "Disattivata", ["NextTrigger"] = "Prossima attivazione", ["NotScheduled"] = "Non pianificata", ["Edit"] = "Modifica", ["Delete"] = "Elimina",
                ["Editor"] = "Editor", ["EditAlarm"] = "Modifica sveglia", ["Close"] = "Chiudi", ["Name"] = "Nome", ["Time"] = "Ora", ["Repeat"] = "Ripetizione",
                ["ChooseDays"] = "Scegli i giorni", ["Note"] = "Nota", ["AlarmIsEnabled"] = "La sveglia è attiva", ["SaveAlarm"] = "Salva sveglia", ["ClearForm"] = "Pulisci modulo",
                ["AlarmEnabled"] = "Sveglia attivata.", ["AlarmDisabled"] = "Sveglia disattivata.", ["AlarmDeleted"] = "Sveglia eliminata.", ["AlarmCreated"] = "Sveglia creata.", ["AlarmUpdated"] = "Sveglia aggiornata.",
                ["UnableUpdateAlarm"] = "Impossibile aggiornare la sveglia.", ["UnableDeleteAlarm"] = "Impossibile eliminare la sveglia.", ["UnableSaveAlarm"] = "Impossibile salvare la sveglia.",
                ["RepeatNever"] = "Mai", ["RepeatDaily"] = "Ogni giorno", ["RepeatWeekdays"] = "Giorni feriali", ["RepeatWeekends"] = "Weekend", ["RepeatCustomDays"] = "Giorni personalizzati",
                ["Mon"] = "Lun", ["Tue"] = "Mar", ["Wed"] = "Mer", ["Thu"] = "Gio", ["Fri"] = "Ven", ["Sat"] = "Sab", ["Sun"] = "Dom",
                ["ValidationNameRequired"] = "Il nome è obbligatorio.", ["ValidationNameTooLong"] = "Il nome deve contenere al massimo 80 caratteri.", ["ValidationTimeFormat"] = "L'ora deve usare il formato HH:mm.",
                ["ValidationDescriptionTooLong"] = "La nota deve contenere al massimo 160 caratteri.", ["ValidationSelectDay"] = "Seleziona almeno un giorno per una ripetizione personalizzata.", ["PageTitleError"] = "Errore applicazione",
                ["Error"] = "Errore", ["ErrorHeadline"] = "Si è verificato un errore durante l'elaborazione della richiesta", ["RequestId"] = "ID richiesta", ["ErrorHelp"] = "Se il problema persiste, controlla i log del container e la connessione a Home Assistant.",
                ["PageTitleNotFound"] = "Pagina non trovata", ["NotFoundHeadline"] = "Pagina non trovata", ["NotFoundText"] = "Controlla l'indirizzo o torna alla panoramica allarmi.", ["BackToDashboard"] = "Torna alla dashboard",
                ["RejoiningServer"] = "Riconnessione al server...", ["RejoinFailedRetrying"] = "Riconnessione fallita... nuovo tentativo tra {0} secondi.", ["FailedToRejoin"] = "Riconnessione non riuscita.",
                ["PleaseRetryOrReload"] = "Riprova o ricarica la pagina.", ["Retry"] = "Riprova", ["SessionPaused"] = "La sessione è stata sospesa dal server.", ["FailedToResume"] = "Impossibile riprendere la sessione.", ["Resume"] = "Riprendi"
            },
            [AppLanguage.Slovak] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Budíky", ["NavAlarmOverview"] = "Prehľad budíkov", ["NavSettings"] = "Nastavenia", ["OpenMenu"] = "Otvoriť menu",
                ["UnexpectedError"] = "Nastala neočakávaná chyba.", ["Reload"] = "Načítať znova", ["Settings"] = "Nastavenia", ["Theme"] = "Téma",
                ["ThemeDescription"] = "Automatický režim rešpektuje tému Home Assistanta, ak je dostupná, inak použije tému zariadenia.", ["Automatic"] = "Automatická",
                ["Light"] = "Svetlá", ["Dark"] = "Tmavá", ["Language"] = "Jazyk", ["SaveSettings"] = "Uložiť nastavenia", ["SettingsSaved"] = "Nastavenia boli uložené.",
                ["TimeZone"] = "Časové pásmo", ["PageTitleSettings"] = "Nastavenia", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Budíky", ["AlarmOverview"] = "Prehľad budíkov",
                ["NewAlarm"] = "Nový budík", ["LoadingSavedAlarms"] = "Načítavam uložené budíky...", ["NoAlarmsYet"] = "Zatiaľ žiadne budíky", ["NoAlarmsYetText"] = "Vytvor svoj prvý budík a nastav čas aj opakovanie.",
                ["Enabled"] = "Zapnutý", ["Disabled"] = "Vypnutý", ["NextTrigger"] = "Ďalšie spustenie", ["NotScheduled"] = "Nenaplánované", ["Edit"] = "Upraviť", ["Delete"] = "Zmazať",
                ["Editor"] = "Editor", ["EditAlarm"] = "Upraviť budík", ["Close"] = "Zavrieť", ["Name"] = "Názov", ["Time"] = "Čas", ["Repeat"] = "Opakovanie",
                ["ChooseDays"] = "Vybrať dni", ["Note"] = "Poznámka", ["AlarmIsEnabled"] = "Budík je zapnutý", ["SaveAlarm"] = "Uložiť budík", ["ClearForm"] = "Vyčistiť formulár",
                ["AlarmEnabled"] = "Budík bol zapnutý.", ["AlarmDisabled"] = "Budík bol vypnutý.", ["AlarmDeleted"] = "Budík bol zmazaný.", ["AlarmCreated"] = "Budík bol vytvorený.", ["AlarmUpdated"] = "Budík bol upravený.",
                ["UnableUpdateAlarm"] = "Budík sa nepodarilo upraviť.", ["UnableDeleteAlarm"] = "Budík sa nepodarilo zmazať.", ["UnableSaveAlarm"] = "Budík sa nepodarilo uložiť.",
                ["RepeatNever"] = "Nikdy", ["RepeatDaily"] = "Denne", ["RepeatWeekdays"] = "Pracovné dni", ["RepeatWeekends"] = "Víkendy", ["RepeatCustomDays"] = "Vlastné dni",
                ["Mon"] = "Po", ["Tue"] = "Ut", ["Wed"] = "St", ["Thu"] = "Št", ["Fri"] = "Pi", ["Sat"] = "So", ["Sun"] = "Ne",
                ["ValidationNameRequired"] = "Názov je povinný.", ["ValidationNameTooLong"] = "Názov môže mať najviac 80 znakov.", ["ValidationTimeFormat"] = "Čas musí byť vo formáte HH:mm.",
                ["ValidationDescriptionTooLong"] = "Poznámka môže mať najviac 160 znakov.", ["ValidationSelectDay"] = "Pre vlastné opakovanie vyber aspoň jeden deň.", ["PageTitleError"] = "Chyba aplikácie",
                ["Error"] = "Chyba", ["ErrorHeadline"] = "Pri spracovaní požiadavky nastala chyba", ["RequestId"] = "ID požiadavky", ["ErrorHelp"] = "Ak problém pretrváva, skontroluj logy kontajnera a nastavenie spojenia s Home Assistantom.",
                ["PageTitleNotFound"] = "Stránka sa nenašla", ["NotFoundHeadline"] = "Stránka sa nenašla", ["NotFoundText"] = "Skontroluj adresu alebo sa vráť na prehľad budíkov.", ["BackToDashboard"] = "Späť na prehľad",
                ["RejoiningServer"] = "Obnovujem spojenie so serverom...", ["RejoinFailedRetrying"] = "Obnovenie zlyhalo... ďalší pokus o {0} sekúnd.", ["FailedToRejoin"] = "Nepodarilo sa obnoviť spojenie.",
                ["PleaseRetryOrReload"] = "Skús to znova alebo obnov stránku.", ["Retry"] = "Skúsiť znova", ["SessionPaused"] = "Relácia bola serverom pozastavená.", ["FailedToResume"] = "Reláciu sa nepodarilo obnoviť.", ["Resume"] = "Obnoviť"
            },
            [AppLanguage.Czech] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Budíky", ["NavAlarmOverview"] = "Přehled budíků", ["NavSettings"] = "Nastavení", ["OpenMenu"] = "Otevřít menu",
                ["UnexpectedError"] = "Došlo k neočekávané chybě.", ["Reload"] = "Načíst znovu", ["Settings"] = "Nastavení", ["Theme"] = "Motiv",
                ["ThemeDescription"] = "Automatický režim respektuje motiv Home Assistanta, pokud je dostupný, jinak použije motiv zařízení.", ["Automatic"] = "Automatický",
                ["Light"] = "Světlý", ["Dark"] = "Tmavý", ["Language"] = "Jazyk", ["SaveSettings"] = "Uložit nastavení", ["SettingsSaved"] = "Nastavení bylo uloženo.",
                ["TimeZone"] = "Časové pásmo", ["PageTitleSettings"] = "Nastavení", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Budíky", ["AlarmOverview"] = "Přehled budíků",
                ["NewAlarm"] = "Nový budík", ["LoadingSavedAlarms"] = "Načítám uložené budíky...", ["NoAlarmsYet"] = "Zatím žádné budíky", ["NoAlarmsYetText"] = "Vytvoř svůj první budík a nastav čas i opakování.",
                ["Enabled"] = "Zapnutý", ["Disabled"] = "Vypnutý", ["NextTrigger"] = "Další spuštění", ["NotScheduled"] = "Nenaplánováno", ["Edit"] = "Upravit", ["Delete"] = "Smazat",
                ["Editor"] = "Editor", ["EditAlarm"] = "Upravit budík", ["Close"] = "Zavřít", ["Name"] = "Název", ["Time"] = "Čas", ["Repeat"] = "Opakování",
                ["ChooseDays"] = "Vybrat dny", ["Note"] = "Poznámka", ["AlarmIsEnabled"] = "Budík je zapnutý", ["SaveAlarm"] = "Uložit budík", ["ClearForm"] = "Vyčistit formulář",
                ["AlarmEnabled"] = "Budík byl zapnut.", ["AlarmDisabled"] = "Budík byl vypnut.", ["AlarmDeleted"] = "Budík byl smazán.", ["AlarmCreated"] = "Budík byl vytvořen.", ["AlarmUpdated"] = "Budík byl upraven.",
                ["UnableUpdateAlarm"] = "Budík se nepodařilo upravit.", ["UnableDeleteAlarm"] = "Budík se nepodařilo smazat.", ["UnableSaveAlarm"] = "Budík se nepodařilo uložit.",
                ["RepeatNever"] = "Nikdy", ["RepeatDaily"] = "Denně", ["RepeatWeekdays"] = "Pracovní dny", ["RepeatWeekends"] = "Víkendy", ["RepeatCustomDays"] = "Vlastní dny",
                ["Mon"] = "Po", ["Tue"] = "Út", ["Wed"] = "St", ["Thu"] = "Čt", ["Fri"] = "Pá", ["Sat"] = "So", ["Sun"] = "Ne",
                ["ValidationNameRequired"] = "Název je povinný.", ["ValidationNameTooLong"] = "Název může mít maximálně 80 znaků.", ["ValidationTimeFormat"] = "Čas musí být ve formátu HH:mm.",
                ["ValidationDescriptionTooLong"] = "Poznámka může mít maximálně 160 znaků.", ["ValidationSelectDay"] = "Pro vlastní opakování vyber alespoň jeden den.", ["PageTitleError"] = "Chyba aplikace",
                ["Error"] = "Chyba", ["ErrorHeadline"] = "Při zpracování požadavku došlo k chybě", ["RequestId"] = "ID požadavku", ["ErrorHelp"] = "Pokud problém přetrvává, zkontroluj logy kontejneru a nastavení připojení k Home Assistantu.",
                ["PageTitleNotFound"] = "Stránka nenalezena", ["NotFoundHeadline"] = "Stránka nenalezena", ["NotFoundText"] = "Zkontroluj adresu nebo se vrať na přehled budíků.", ["BackToDashboard"] = "Zpět na přehled",
                ["RejoiningServer"] = "Obnovuji spojení se serverem...", ["RejoinFailedRetrying"] = "Obnovení selhalo... další pokus za {0} sekund.", ["FailedToRejoin"] = "Nepodařilo se obnovit spojení.",
                ["PleaseRetryOrReload"] = "Zkus to znovu nebo obnov stránku.", ["Retry"] = "Zkusit znovu", ["SessionPaused"] = "Relace byla serverem pozastavena.", ["FailedToResume"] = "Relaci se nepodařilo obnovit.", ["Resume"] = "Obnovit"
            },
            [AppLanguage.Polish] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Alarmy", ["NavAlarmOverview"] = "Przegląd alarmów", ["NavSettings"] = "Ustawienia", ["OpenMenu"] = "Otwórz menu",
                ["UnexpectedError"] = "Wystąpił nieoczekiwany błąd.", ["Reload"] = "Przeładuj", ["Settings"] = "Ustawienia", ["Theme"] = "Motyw",
                ["ThemeDescription"] = "Tryb automatyczny używa motywu Home Assistanta, jeśli jest dostępny, a w przeciwnym razie motywu urządzenia.", ["Automatic"] = "Automatyczny",
                ["Light"] = "Jasny", ["Dark"] = "Ciemny", ["Language"] = "Język", ["SaveSettings"] = "Zapisz ustawienia", ["SettingsSaved"] = "Ustawienia zostały zapisane.",
                ["TimeZone"] = "Strefa czasowa", ["PageTitleSettings"] = "Ustawienia", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Alarmy", ["AlarmOverview"] = "Przegląd alarmów",
                ["NewAlarm"] = "Nowy alarm", ["LoadingSavedAlarms"] = "Ładowanie zapisanych alarmów...", ["NoAlarmsYet"] = "Brak alarmów", ["NoAlarmsYetText"] = "Utwórz pierwszy alarm i ustaw godzinę oraz powtarzanie.",
                ["Enabled"] = "Włączony", ["Disabled"] = "Wyłączony", ["NextTrigger"] = "Następne uruchomienie", ["NotScheduled"] = "Nie zaplanowano", ["Edit"] = "Edytuj", ["Delete"] = "Usuń",
                ["Editor"] = "Edytor", ["EditAlarm"] = "Edytuj alarm", ["Close"] = "Zamknij", ["Name"] = "Nazwa", ["Time"] = "Czas", ["Repeat"] = "Powtarzanie",
                ["ChooseDays"] = "Wybierz dni", ["Note"] = "Notatka", ["AlarmIsEnabled"] = "Alarm jest włączony", ["SaveAlarm"] = "Zapisz alarm", ["ClearForm"] = "Wyczyść formularz",
                ["AlarmEnabled"] = "Alarm został włączony.", ["AlarmDisabled"] = "Alarm został wyłączony.", ["AlarmDeleted"] = "Alarm został usunięty.", ["AlarmCreated"] = "Alarm został utworzony.", ["AlarmUpdated"] = "Alarm został zaktualizowany.",
                ["UnableUpdateAlarm"] = "Nie udało się zaktualizować alarmu.", ["UnableDeleteAlarm"] = "Nie udało się usunąć alarmu.", ["UnableSaveAlarm"] = "Nie udało się zapisać alarmu.",
                ["RepeatNever"] = "Nigdy", ["RepeatDaily"] = "Codziennie", ["RepeatWeekdays"] = "Dni robocze", ["RepeatWeekends"] = "Weekendy", ["RepeatCustomDays"] = "Wybrane dni",
                ["Mon"] = "Pon", ["Tue"] = "Wt", ["Wed"] = "Śr", ["Thu"] = "Czw", ["Fri"] = "Pt", ["Sat"] = "Sob", ["Sun"] = "Ndz",
                ["ValidationNameRequired"] = "Nazwa jest wymagana.", ["ValidationNameTooLong"] = "Nazwa może mieć maksymalnie 80 znaków.", ["ValidationTimeFormat"] = "Czas musi mieć format HH:mm.",
                ["ValidationDescriptionTooLong"] = "Notatka może mieć maksymalnie 160 znaków.", ["ValidationSelectDay"] = "Wybierz co najmniej jeden dzień dla niestandardowego powtarzania.", ["PageTitleError"] = "Błąd aplikacji",
                ["Error"] = "Błąd", ["ErrorHeadline"] = "Wystąpił błąd podczas przetwarzania żądania", ["RequestId"] = "ID żądania", ["ErrorHelp"] = "Jeśli problem nadal występuje, sprawdź logi kontenera i połączenie z Home Assistantem.",
                ["PageTitleNotFound"] = "Nie znaleziono strony", ["NotFoundHeadline"] = "Nie znaleziono strony", ["NotFoundText"] = "Sprawdź adres lub wróć do przeglądu alarmów.", ["BackToDashboard"] = "Wróć do panelu",
                ["RejoiningServer"] = "Ponowne łączenie z serwerem...", ["RejoinFailedRetrying"] = "Ponowne połączenie nie powiodło się... kolejna próba za {0} sekund.", ["FailedToRejoin"] = "Nie udało się ponownie połączyć.",
                ["PleaseRetryOrReload"] = "Spróbuj ponownie lub odśwież stronę.", ["Retry"] = "Spróbuj ponownie", ["SessionPaused"] = "Sesja została wstrzymana przez serwer.", ["FailedToResume"] = "Nie udało się wznowić sesji.", ["Resume"] = "Wznów"
            },
            [AppLanguage.Ukrainian] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Будильники", ["NavAlarmOverview"] = "Огляд будильників", ["NavSettings"] = "Налаштування", ["OpenMenu"] = "Відкрити меню",
                ["UnexpectedError"] = "Сталася неочікувана помилка.", ["Reload"] = "Перезавантажити", ["Settings"] = "Налаштування", ["Theme"] = "Тема",
                ["ThemeDescription"] = "Автоматичний режим використовує тему Home Assistant, якщо вона доступна, інакше тему пристрою.", ["Automatic"] = "Автоматично",
                ["Light"] = "Світла", ["Dark"] = "Темна", ["Language"] = "Мова", ["SaveSettings"] = "Зберегти налаштування", ["SettingsSaved"] = "Налаштування збережено.",
                ["TimeZone"] = "Часовий пояс", ["PageTitleSettings"] = "Налаштування", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Будильники", ["AlarmOverview"] = "Огляд будильників",
                ["NewAlarm"] = "Новий будильник", ["LoadingSavedAlarms"] = "Завантаження збережених будильників...", ["NoAlarmsYet"] = "Ще немає будильників", ["NoAlarmsYetText"] = "Створіть свій перший будильник і встановіть час та повторення.",
                ["Enabled"] = "Увімкнено", ["Disabled"] = "Вимкнено", ["NextTrigger"] = "Наступний запуск", ["NotScheduled"] = "Не заплановано", ["Edit"] = "Редагувати", ["Delete"] = "Видалити",
                ["Editor"] = "Редактор", ["EditAlarm"] = "Редагувати будильник", ["Close"] = "Закрити", ["Name"] = "Назва", ["Time"] = "Час", ["Repeat"] = "Повторення",
                ["ChooseDays"] = "Вибрати дні", ["Note"] = "Примітка", ["AlarmIsEnabled"] = "Будильник увімкнений", ["SaveAlarm"] = "Зберегти будильник", ["ClearForm"] = "Очистити форму",
                ["AlarmEnabled"] = "Будильник увімкнено.", ["AlarmDisabled"] = "Будильник вимкнено.", ["AlarmDeleted"] = "Будильник видалено.", ["AlarmCreated"] = "Будильник створено.", ["AlarmUpdated"] = "Будильник оновлено.",
                ["UnableUpdateAlarm"] = "Не вдалося оновити будильник.", ["UnableDeleteAlarm"] = "Не вдалося видалити будильник.", ["UnableSaveAlarm"] = "Не вдалося зберегти будильник.",
                ["RepeatNever"] = "Ніколи", ["RepeatDaily"] = "Щодня", ["RepeatWeekdays"] = "Будні", ["RepeatWeekends"] = "Вихідні", ["RepeatCustomDays"] = "Вибрані дні",
                ["Mon"] = "Пн", ["Tue"] = "Вт", ["Wed"] = "Ср", ["Thu"] = "Чт", ["Fri"] = "Пт", ["Sat"] = "Сб", ["Sun"] = "Нд",
                ["ValidationNameRequired"] = "Назва обов'язкова.", ["ValidationNameTooLong"] = "Назва має містити не більше 80 символів.", ["ValidationTimeFormat"] = "Час має бути у форматі HH:mm.",
                ["ValidationDescriptionTooLong"] = "Примітка має містити не більше 160 символів.", ["ValidationSelectDay"] = "Виберіть принаймні один день для власного повторення.", ["PageTitleError"] = "Помилка застосунку",
                ["Error"] = "Помилка", ["ErrorHeadline"] = "Під час обробки запиту сталася помилка", ["RequestId"] = "ID запиту", ["ErrorHelp"] = "Якщо проблема не зникає, перевірте логи контейнера та підключення до Home Assistant.",
                ["PageTitleNotFound"] = "Сторінку не знайдено", ["NotFoundHeadline"] = "Сторінку не знайдено", ["NotFoundText"] = "Перевірте адресу або поверніться до огляду будильників.", ["BackToDashboard"] = "Повернутися до панелі",
                ["RejoiningServer"] = "Повторне підключення до сервера...", ["RejoinFailedRetrying"] = "Не вдалося підключитися... повторна спроба через {0} секунд.", ["FailedToRejoin"] = "Не вдалося повторно підключитися.",
                ["PleaseRetryOrReload"] = "Спробуйте ще раз або перезавантажте сторінку.", ["Retry"] = "Спробувати ще раз", ["SessionPaused"] = "Сесію призупинено сервером.", ["FailedToResume"] = "Не вдалося відновити сесію.", ["Resume"] = "Відновити"
            },
            [AppLanguage.Greek] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Ξυπνητήρια", ["NavAlarmOverview"] = "Επισκόπηση ξυπνητηριών", ["NavSettings"] = "Ρυθμίσεις", ["OpenMenu"] = "Άνοιγμα μενού",
                ["UnexpectedError"] = "Παρουσιάστηκε ένα μη αναμενόμενο σφάλμα.", ["Reload"] = "Επαναφόρτωση", ["Settings"] = "Ρυθμίσεις", ["Theme"] = "Θέμα",
                ["ThemeDescription"] = "Η αυτόματη λειτουργία ακολουθεί το θέμα του Home Assistant όταν είναι διαθέσιμο, διαφορετικά το θέμα της συσκευής.", ["Automatic"] = "Αυτόματο",
                ["Light"] = "Φωτεινό", ["Dark"] = "Σκούρο", ["Language"] = "Γλώσσα", ["SaveSettings"] = "Αποθήκευση ρυθμίσεων", ["SettingsSaved"] = "Οι ρυθμίσεις αποθηκεύτηκαν.",
                ["TimeZone"] = "Ζώνη ώρας", ["PageTitleSettings"] = "Ρυθμίσεις", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Ξυπνητήρια", ["AlarmOverview"] = "Επισκόπηση ξυπνητηριών",
                ["NewAlarm"] = "Νέο ξυπνητήρι", ["LoadingSavedAlarms"] = "Φόρτωση αποθηκευμένων ξυπνητηριών...", ["NoAlarmsYet"] = "Δεν υπάρχουν ακόμη ξυπνητήρια", ["NoAlarmsYetText"] = "Δημιουργήστε το πρώτο σας ξυπνητήρι και ορίστε ώρα και επανάληψη.",
                ["Enabled"] = "Ενεργό", ["Disabled"] = "Ανενεργό", ["NextTrigger"] = "Επόμενη ενεργοποίηση", ["NotScheduled"] = "Δεν έχει προγραμματιστεί", ["Edit"] = "Επεξεργασία", ["Delete"] = "Διαγραφή",
                ["Editor"] = "Επεξεργαστής", ["EditAlarm"] = "Επεξεργασία ξυπνητηριού", ["Close"] = "Κλείσιμο", ["Name"] = "Όνομα", ["Time"] = "Ώρα", ["Repeat"] = "Επανάληψη",
                ["ChooseDays"] = "Επιλογή ημερών", ["Note"] = "Σημείωση", ["AlarmIsEnabled"] = "Το ξυπνητήρι είναι ενεργό", ["SaveAlarm"] = "Αποθήκευση ξυπνητηριού", ["ClearForm"] = "Καθαρισμός φόρμας",
                ["AlarmEnabled"] = "Το ξυπνητήρι ενεργοποιήθηκε.", ["AlarmDisabled"] = "Το ξυπνητήρι απενεργοποιήθηκε.", ["AlarmDeleted"] = "Το ξυπνητήρι διαγράφηκε.", ["AlarmCreated"] = "Το ξυπνητήρι δημιουργήθηκε.", ["AlarmUpdated"] = "Το ξυπνητήρι ενημερώθηκε.",
                ["UnableUpdateAlarm"] = "Δεν ήταν δυνατή η ενημέρωση του ξυπνητηριού.", ["UnableDeleteAlarm"] = "Δεν ήταν δυνατή η διαγραφή του ξυπνητηριού.", ["UnableSaveAlarm"] = "Δεν ήταν δυνατή η αποθήκευση του ξυπνητηριού.",
                ["RepeatNever"] = "Ποτέ", ["RepeatDaily"] = "Καθημερινά", ["RepeatWeekdays"] = "Καθημερινές", ["RepeatWeekends"] = "Σαββατοκύριακα", ["RepeatCustomDays"] = "Προσαρμοσμένες ημέρες",
                ["Mon"] = "Δευ", ["Tue"] = "Τρι", ["Wed"] = "Τετ", ["Thu"] = "Πεμ", ["Fri"] = "Παρ", ["Sat"] = "Σαβ", ["Sun"] = "Κυρ",
                ["ValidationNameRequired"] = "Το όνομα είναι υποχρεωτικό.", ["ValidationNameTooLong"] = "Το όνομα πρέπει να έχει έως 80 χαρακτήρες.", ["ValidationTimeFormat"] = "Η ώρα πρέπει να είναι στη μορφή HH:mm.",
                ["ValidationDescriptionTooLong"] = "Η σημείωση πρέπει να έχει έως 160 χαρακτήρες.", ["ValidationSelectDay"] = "Επιλέξτε τουλάχιστον μία ημέρα για προσαρμοσμένη επανάληψη.", ["PageTitleError"] = "Σφάλμα εφαρμογής",
                ["Error"] = "Σφάλμα", ["ErrorHeadline"] = "Παρουσιάστηκε σφάλμα κατά την επεξεργασία του αιτήματός σας", ["RequestId"] = "Αναγνωριστικό αιτήματος", ["ErrorHelp"] = "Αν το πρόβλημα παραμένει, ελέγξτε τα logs του container και τη σύνδεση με το Home Assistant.",
                ["PageTitleNotFound"] = "Η σελίδα δεν βρέθηκε", ["NotFoundHeadline"] = "Η σελίδα δεν βρέθηκε", ["NotFoundText"] = "Ελέγξτε τη διεύθυνση ή επιστρέψτε στην επισκόπηση ξυπνητηριών.", ["BackToDashboard"] = "Επιστροφή στον πίνακα",
                ["RejoiningServer"] = "Επανασύνδεση με τον διακομιστή...", ["RejoinFailedRetrying"] = "Η επανασύνδεση απέτυχε... νέα προσπάθεια σε {0} δευτερόλεπτα.", ["FailedToRejoin"] = "Αποτυχία επανασύνδεσης.",
                ["PleaseRetryOrReload"] = "Δοκιμάστε ξανά ή επαναφορτώστε τη σελίδα.", ["Retry"] = "Προσπάθεια ξανά", ["SessionPaused"] = "Η συνεδρία έχει τεθεί σε παύση από τον διακομιστή.", ["FailedToResume"] = "Αποτυχία επαναφοράς της συνεδρίας.", ["Resume"] = "Συνέχεια"
            },
            [AppLanguage.Esperanto] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "Alarmoj", ["NavAlarmOverview"] = "Superrigardo de alarmoj", ["NavSettings"] = "Agordoj", ["OpenMenu"] = "Malfermi menuon",
                ["UnexpectedError"] = "Okazis neatendita eraro.", ["Reload"] = "Reŝargi", ["Settings"] = "Agordoj", ["Theme"] = "Temo",
                ["ThemeDescription"] = "Aŭtomata reĝimo sekvas la etoson de Home Assistant kiam disponebla, alie la etoson de la aparato.", ["Automatic"] = "Aŭtomata",
                ["Light"] = "Luma", ["Dark"] = "Malhela", ["Language"] = "Lingvo", ["SaveSettings"] = "Konservi agordojn", ["SettingsSaved"] = "Agordoj konservitaj.",
                ["TimeZone"] = "Horzono", ["PageTitleSettings"] = "Agordoj", ["PageTitleHome"] = "WakeMeUp", ["Alarms"] = "Alarmoj", ["AlarmOverview"] = "Superrigardo de alarmoj",
                ["NewAlarm"] = "Nova alarmo", ["LoadingSavedAlarms"] = "Ŝargante konservitajn alarmojn...", ["NoAlarmsYet"] = "Ankoraŭ neniuj alarmoj", ["NoAlarmsYetText"] = "Kreu vian unuan alarmon kaj elektu horon kaj ripeton.",
                ["Enabled"] = "Ŝaltita", ["Disabled"] = "Malŝaltita", ["NextTrigger"] = "Sekva ekigo", ["NotScheduled"] = "Ne planita", ["Edit"] = "Redakti", ["Delete"] = "Forigi",
                ["Editor"] = "Redaktilo", ["EditAlarm"] = "Redakti alarmon", ["Close"] = "Fermi", ["Name"] = "Nomo", ["Time"] = "Horo", ["Repeat"] = "Ripeto",
                ["ChooseDays"] = "Elekti tagojn", ["Note"] = "Noto", ["AlarmIsEnabled"] = "Alarmo estas ŝaltita", ["SaveAlarm"] = "Konservi alarmon", ["ClearForm"] = "Purigi formularon",
                ["AlarmEnabled"] = "Alarmo ŝaltita.", ["AlarmDisabled"] = "Alarmo malŝaltita.", ["AlarmDeleted"] = "Alarmo forigita.", ["AlarmCreated"] = "Alarmo kreita.", ["AlarmUpdated"] = "Alarmo ĝisdatigita.",
                ["UnableUpdateAlarm"] = "Ne eblis ĝisdatigi la alarmon.", ["UnableDeleteAlarm"] = "Ne eblis forigi la alarmon.", ["UnableSaveAlarm"] = "Ne eblis konservi la alarmon.",
                ["RepeatNever"] = "Neniam", ["RepeatDaily"] = "Ĉiutage", ["RepeatWeekdays"] = "Labortagoj", ["RepeatWeekends"] = "Semajnfinoj", ["RepeatCustomDays"] = "Propraj tagoj",
                ["Mon"] = "Lu", ["Tue"] = "Ma", ["Wed"] = "Me", ["Thu"] = "Ĵa", ["Fri"] = "Ve", ["Sat"] = "Sa", ["Sun"] = "Di",
                ["ValidationNameRequired"] = "Nomo estas bezonata.", ["ValidationNameTooLong"] = "Nomo devas havi maksimume 80 signojn.", ["ValidationTimeFormat"] = "Horo devas uzi la formaton HH:mm.",
                ["ValidationDescriptionTooLong"] = "Noto devas havi maksimume 160 signojn.", ["ValidationSelectDay"] = "Elektu almenaŭ unu tagon por propra ripeto.", ["PageTitleError"] = "Eraro de aplikaĵo",
                ["Error"] = "Eraro", ["ErrorHeadline"] = "Okazis eraro dum prilaborado de via peto", ["RequestId"] = "Peta ID", ["ErrorHelp"] = "Se la problemo plu okazas, kontrolu la ujo-protokolojn kaj la Home Assistant-konekton.",
                ["PageTitleNotFound"] = "Paĝo ne trovita", ["NotFoundHeadline"] = "Paĝo ne trovita", ["NotFoundText"] = "Kontrolu la adreson aŭ revenu al la superrigardo de alarmoj.", ["BackToDashboard"] = "Reiri al panelo",
                ["RejoiningServer"] = "Rekonektante al la servilo...", ["RejoinFailedRetrying"] = "Rekonekto malsukcesis... nova provo post {0} sekundoj.", ["FailedToRejoin"] = "Malsukcesis rekonekti.",
                ["PleaseRetryOrReload"] = "Bonvolu reprovi aŭ reŝargi la paĝon.", ["Retry"] = "Reprovi", ["SessionPaused"] = "La seanco estis paŭzita de la servilo.", ["FailedToResume"] = "Malsukcesis rekomenci la seancon.", ["Resume"] = "Rekomenci"
            },
            [AppLanguage.Klingon] = new Dictionary<string, string>
            {
                ["AppName"] = "WakeMeUp", ["BrandSection"] = "QInmey", ["NavAlarmOverview"] = "QInmey cha'ang", ["NavSettings"] = "SeHlaw", ["OpenMenu"] = "menu' yIpoS",
                ["UnexpectedError"] = "Qagh 'unexpected' qaS.", ["Reload"] = "yIcherqa'", ["Settings"] = "SeHlaw", ["Theme"] = "mI' poj",
                ["ThemeDescription"] = "Automatic Dotlh Home Assistant mI' poj buS. tu'be'lu'chugh, jan mI' poj buS.", ["Automatic"] = "Automatic", ["Light"] = "wov", ["Dark"] = "Hurgh",
                ["Language"] = "Hol", ["SaveSettings"] = "SeHlaw yIpol", ["SettingsSaved"] = "SeHlaw polta'.", ["TimeZone"] = "poH botlh", ["PageTitleSettings"] = "SeHlaw", ["PageTitleHome"] = "WakeMeUp",
                ["Alarms"] = "QInmey", ["AlarmOverview"] = "QInmey cha'ang", ["NewAlarm"] = "QIn chu'", ["LoadingSavedAlarms"] = "QInmey polpu'bogh vIlaD...", ["NoAlarmsYet"] = "QInmey pagh",
                ["NoAlarmsYetText"] = "QIn wa'DIch yIchenmoH. poH je ghunmeH, latlh poH yISeH.", ["Enabled"] = "chu'", ["Disabled"] = "chu'Ha'", ["NextTrigger"] = "latlh QIn", ["NotScheduled"] = "ghunlu'be'",
                ["Edit"] = "choH", ["Delete"] = "Qaw'", ["Editor"] = "choHmeH jan", ["EditAlarm"] = "QIn choH", ["Close"] = "SoQmoH", ["Name"] = "pong", ["Time"] = "poH", ["Repeat"] = "qaSqa'",
                ["ChooseDays"] = "jajmey yIwIv", ["Note"] = "ghItlh", ["AlarmIsEnabled"] = "QIn chu'taH", ["SaveAlarm"] = "QIn yIpol", ["ClearForm"] = "pat yIteq", ["AlarmEnabled"] = "QIn chu'ta'.",
                ["AlarmDisabled"] = "QIn chu'Ha'ta'.", ["AlarmDeleted"] = "QIn Qaw'ta'.", ["AlarmCreated"] = "QIn chenmoHta'.", ["AlarmUpdated"] = "QIn choHta'.", ["UnableUpdateAlarm"] = "QIn choHlaHbe'.",
                ["UnableDeleteAlarm"] = "QIn Qaw'laHbe'.", ["UnableSaveAlarm"] = "QIn pollaHbe'.", ["RepeatNever"] = "not", ["RepeatDaily"] = "Hoch jaj", ["RepeatWeekdays"] = "jajvam ghom",
                ["RepeatWeekends"] = "jaj ghom Qav", ["RepeatCustomDays"] = "jajmey chovlu'bogh", ["Mon"] = "wa'", ["Tue"] = "cha'", ["Wed"] = "wej", ["Thu"] = "loS", ["Fri"] = "vagh", ["Sat"] = "jav", ["Sun"] = "Soch",
                ["ValidationNameRequired"] = "pong potrzeb. pong yIghItlh.", ["ValidationNameTooLong"] = "pong tIq law' Hoch Daq 80 signmey pIm.", ["ValidationTimeFormat"] = "poH HH:mm pat yIlo'.",
                ["ValidationDescriptionTooLong"] = "ghItlh tIq law' Hoch Daq 160 signmey pIm.", ["ValidationSelectDay"] = "qaSqa' chovmeH, jaj wa' yIwIv.", ["PageTitleError"] = "jan Qagh", ["Error"] = "Qagh",
                ["ErrorHeadline"] = "ghotvam ra' ghajtaHvIS Qagh qaS", ["RequestId"] = "ra' ID", ["ErrorHelp"] = "Qagh taHchugh, container logmey je Home Assistant rargh yIchu'Ha'.", ["PageTitleNotFound"] = "Daq tu'lu'be'",
                ["NotFoundHeadline"] = "Daq tu'lu'be'", ["NotFoundText"] = "Daq yIchovbe' pagh QInmey cha'angDaq yIchegh.", ["BackToDashboard"] = "panelDaq yIchegh", ["RejoiningServer"] = "server rarqa'taH...",
                ["RejoinFailedRetrying"] = "rarqa'lu'be'... {0} lupmey pIq qa'angqa'.", ["FailedToRejoin"] = "rarqa'lu'be'.", ["PleaseRetryOrReload"] = "yIqa'angqa' pagh yIcherqa'.", ["Retry"] = "qa'angqa'", ["SessionPaused"] = "server pa' Dotlh mevmoHta'.",
                ["FailedToResume"] = "Dotlh taghqa'laHbe'.", ["Resume"] = "taghqa'"
            }
        };

    public event Action? Changed;

    public AppLanguage CurrentLanguage { get; private set; } = AppLanguage.English;

    public UiTextService()
    {
        ApplyCulture(CurrentLanguage);
    }

    public string this[string key] => Get(key);

    public IReadOnlyList<LanguageOption> GetSupportedLanguages()
    {
        return SupportedLanguages;
    }

    public string Get(string key)
    {
        if (Translations.TryGetValue(CurrentLanguage, out var languageMap) &&
            languageMap.TryGetValue(key, out var value))
        {
            return value;
        }

        if (Translations[AppLanguage.English].TryGetValue(key, out var fallback))
        {
            return fallback;
        }

        return key;
    }

    public string Format(string key, params object[] args)
    {
        return string.Format(Get(key), args);
    }

    public string GetHtmlLanguage()
    {
        return CurrentLanguage switch
        {
            AppLanguage.English => "en",
            AppLanguage.German => "de",
            AppLanguage.French => "fr",
            AppLanguage.Spanish => "es",
            AppLanguage.Portuguese => "pt",
            AppLanguage.Italian => "it",
            AppLanguage.Slovak => "sk",
            AppLanguage.Czech => "cs",
            AppLanguage.Polish => "pl",
            AppLanguage.Ukrainian => "uk",
            AppLanguage.Greek => "el",
            AppLanguage.Esperanto => "eo",
            AppLanguage.Klingon => "tlh",
            _ => "en"
        };
    }

    public void SetLanguage(AppLanguage language)
    {
        if (CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;
        ApplyCulture(language);
        Changed?.Invoke();
    }

    private static void ApplyCulture(AppLanguage language)
    {
        var cultureName = language switch
        {
            AppLanguage.English => "en-US",
            AppLanguage.German => "de-DE",
            AppLanguage.French => "fr-FR",
            AppLanguage.Spanish => "es-ES",
            AppLanguage.Portuguese => "pt-PT",
            AppLanguage.Italian => "it-IT",
            AppLanguage.Slovak => "sk-SK",
            AppLanguage.Czech => "cs-CZ",
            AppLanguage.Polish => "pl-PL",
            AppLanguage.Ukrainian => "uk-UA",
            AppLanguage.Greek => "el-GR",
            AppLanguage.Esperanto => "eo",
            AppLanguage.Klingon => "en-US",
            _ => "en-US"
        };

        var culture = CultureInfo.GetCultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    public sealed record LanguageOption(AppLanguage Value, string DisplayName);
}
