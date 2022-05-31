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
    },
    SendGetBattleDeckRequest: function (objectName, callback, fallback) {
        requestGetBattleDeck(
            UTF8ToString(objectName),
            UTF8ToString(callback),
            UTF8ToString(fallback),
        );
    },
    SendLoadBattleDeckRequest: function (objectName, callback, fallback){
        requestLoadBattleDeck(
            UTF8ToString(objectName),
            UTF8ToString(callback),
            UTF8ToString(fallback),
        )
    }
});