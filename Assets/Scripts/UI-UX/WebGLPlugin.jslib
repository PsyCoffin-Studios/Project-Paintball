mergeInto(LibraryManager.library, {
    DetectDevice: function () {
        var isMobile = /Mobi|Android/i.test(navigator.userAgent);
        if (isMobile) {
            console.log("Jugando en un smartphone");
            // Aquí puedes enviar un mensaje a Unity si es necesario
            SendMessage('MobileChecker', 'OnMobileDetected');
        } else {
            console.log("Jugando en una PC");
            // También puedes enviar un mensaje a Unity aquí si es necesario
            // SendMessage('GameObjectName', 'OnDesktopDetected');
        }
    }
});