mergeInto(LibraryManager.library, {
    SendBattleRequest: function (objectName, callback, fallback) {
        requestEntranceToBattle(
            UTF8ToString(objectName),
            UTF8ToString(callback),
            UTF8ToString(fallback),
        );
    },
    SendAuthenticateRequest: function (objectName, callback, fallback) {
        requestAuthentication(
            UTF8ToString(objectName),
            UTF8ToString(callback),
            UTF8ToString(fallback),
        );
    }
});