mergeInto(LibraryManager.library, {

    CheckSDKExtern: function () {
        if (!ysdkInitialized) return 0;
        else return 1;
    },

    GameReadyExtern: function(){
        if (!ysdkInitialized) return;
        ysdk.features.LoadingAPI?.ready(); // Показываем SDK, что игра загрузилась и можно начинать играть
    },

    GameStartedExtern: function(){
        if (!ysdkInitialized) return;
        ysdk.features.GameplayAPI?.start();
    },

    GameStoppedExtern: function(){
        if (!ysdkInitialized) return;
        ysdk.features.GameplayAPI?.stop();
    },

    SetToLeaderboardExtern: function (frags) {
        if (!ysdkInitialized) return;

        ysdk.isAvailableMethod('leaderboards.setLeaderboardScore').then(available => {
            if (available) {
                console.log('Leaderboard is available!');
                initLeaderboards().then(_lb => {
                    _lb.getLeaderboardPlayerEntry('frags')
                        .then(res => {
                            //console.log('Trying add ' + frags + ' frags to leaderboards');
                            if (res.score < frags) {
                                _lb.setLeaderboardScore('frags', frags);
                                //console.log('Frags' + frags + ' added to leaderboard!');
                            }
                        })
                        .catch(err => {
                            if (err.code === 'LEADERBOARD_PLAYER_NOT_PRESENT') {
                                // Срабатывает, если у игрока нет записи в лидерборде
                                //console.log('Leaderboard: Player not present');
                                _lb.setLeaderboardScore('frags', frags);
                            }
                            else {
                                //console.log('Leaderboard error: ' + err.code);
                            }
                        });
                });
            }
            else {
                console.log('Leaderboard is not available!');
            }
        })
    },

    SaveGameExtern: function (data, flush) {
        if (!ysdkInitialized || player == null) return;

        var dataString = UTF8ToString(data);
        var myObj = JSON.parse(dataString);
        player.setData(myObj, flush);
    },

    LoadGameExtern: function () {
        if (!ysdkInitialized || player == null) return;

        player.getData().then(data => {
            const myJson = JSON.stringify(data);
            myGameInstance.SendMessage("WorldManager", "LoadGame", myJson);
        });
    },

    ShowAdsExtern: function () {
        if (!ysdkInitialized) return;

        ysdk.adv.showFullscreenAdv({
            callbacks: {
                onClose: function (wasShown) {
                    // some action after close
                    console.log("Adv Closed");
                },
                onError: function (error) {
                    // some action on error
                }
            }
        })
    },

    ShowAdvExtern: function () {
        if (!ysdkInitialized) return;

        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    console.log('Video ad open.');
                },
                onRewarded: () => {
                    console.log('Rewarded!');
                    // myGameInstance.SendMessage('YandexAds', 'AddCoins', 1);
                },
                onClose: () => {
                    console.log('Video ad closed.');
                },
                onError: (e) => {
                    console.log('Error while open video ad:', e);
                }
            }
        })
    },

    RateGameExtern: function () {
        if (!ysdkInitialized) return;

        ysdk.feedback.canReview()
            .then(({ value, reason }) => {
                if (value) {
                    ysdk.feedback.requestReview()
                        .then(({ feedbackSent }) => {
                            console.log(feedbackSent);
                        })
                } else {
                    myGameInstance.SendMessage("Yandex", "IsGameRated", 1);
                    console.log(reason);
                }
            })
    },

    CanRateGameExtern: function () {
        if (!ysdkInitialized) return;

        ysdk.feedback.canReview()
            .then(({ value, reason }) => {
                if (value) {
                    myGameInstance.SendMessage("Yandex", "IsGameRated", 0);
                } else {
                    myGameInstance.SendMessage("Yandex", "IsGameRated", 1);
                    console.log(reason);
                }
            })
    },

    GetAuthExtern: function () {
        if (!ysdkInitialized) return;

        initPlayer().then(_player => {
            if (_player.getMode() === 'lite') {
                myGameInstance.SendMessage("Yandex", "SetAuth", 0);
                return;
            }

            myGameInstance.SendMessage("Yandex", "SetAuth", 1);
            if (lb == null) initLeaderboards();

        }).catch(err => {
            myGameInstance.SendMessage("Yandex", "SetAuth", 0);
        });
    },

    AuthExtern: function () {
        if (!ysdkInitialized) return;

        initPlayer().then(_player => {
            if (_player.getMode() === 'lite') {
                ysdk.auth.openAuthDialog().then(() => {
                    myGameInstance.SendMessage("Yandex", "SetAuth", 1);

                    if (lb == null) initLeaderboards();
                }).catch(() => {
                    myGameInstance.SendMessage("Yandex", "SetAuth", 0);
                });
            }
        }).catch(err => {
            myGameInstance.SendMessage("Yandex", "SetAuth", 0);
        });
    },

    GetLangExtern: function () {
        if (!ysdkInitialized) {
            var lang = "en";
            var bufSize = lengthBytesUTF8(lang) + 1;
            var buf = _malloc(bufSize);
            stringToUTF8(lang, buf, bufSize);
            return buf;
        }

        var returnStr = ysdk.environment.i18n.lang;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
});