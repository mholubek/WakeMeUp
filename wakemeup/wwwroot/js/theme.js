(function () {
    function detectHomeAssistantTheme() {
        try {
            var hostDocument = window.top && window.top.document ? window.top.document : document;
            var root = hostDocument.documentElement;
            var markers = [
                root.getAttribute("data-theme"),
                root.getAttribute("theme"),
                root.className,
                hostDocument.body ? hostDocument.body.className : ""
            ]
                .filter(Boolean)
                .join(" ")
                .toLowerCase();

            if (markers.includes("dark")) {
                return "dark";
            }

            if (markers.includes("light")) {
                return "light";
            }
        } catch (error) {
            console.debug("WakeMeUp theme host detection unavailable.", error);
        }

        return null;
    }

    function detectHomeAssistantLanguage() {
        try {
            var hostDocument = window.top && window.top.document ? window.top.document : document;
            var candidates = [
                hostDocument.documentElement ? hostDocument.documentElement.lang : null,
                document.documentElement ? document.documentElement.lang : null,
                hostDocument.body ? hostDocument.body.getAttribute("lang") : null
            ].filter(Boolean);

            if (candidates.length > 0) {
                return candidates[0];
            }
        } catch (error) {
            console.debug("WakeMeUp language host detection unavailable.", error);
        }

        return null;
    }

    function detectBrowserLanguage() {
        if (navigator.languages && navigator.languages.length > 0) {
            return navigator.languages[0];
        }

        return navigator.language || navigator.userLanguage || "en";
    }

    function detectDeviceTheme() {
        return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
    }

    function resolveTheme(mode) {
        if (mode === "Light") {
            return "light";
        }

        if (mode === "Dark") {
            return "dark";
        }

        var hostTheme = detectHomeAssistantTheme();
        if (hostTheme) {
            return hostTheme;
        }

        return detectDeviceTheme();
    }

    function apply(mode) {
        var theme = resolveTheme(mode || localStorage.getItem("wakeMeUp.themeMode") || "Auto");
        document.documentElement.setAttribute("data-bs-theme", theme);
        localStorage.setItem("wakeMeUp.themeMode", mode || "Auto");
    }

    window.wakeMeUpTheme = {
        apply: apply,
        getPreferredLanguage: function () {
            return detectHomeAssistantLanguage() || detectBrowserLanguage() || "en";
        }
    };

    window.wakeMeUpUi = {
        focusElement: function (element) {
            if (element && typeof element.focus === "function") {
                element.focus();
            }
        }
    };

    apply(localStorage.getItem("wakeMeUp.themeMode") || "Auto");

    window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", function () {
        if ((localStorage.getItem("wakeMeUp.themeMode") || "Auto") === "Auto") {
            apply("Auto");
        }
    });
})();
