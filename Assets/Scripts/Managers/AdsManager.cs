using System;
using System.Collections;
using Mycom.Target.Unity.Ads;
using UnityEngine;
#if (UNITY_WEBGL && !UNITY_EDITOR)
using System.Runtime.InteropServices;
#endif

public static class AdsManager
{
#if (UNITY_WEBGL && !UNITY_EDITOR)
     [DllImport("__Internal")]
     private static extern void ShowAdsExtern();
     [DllImport("__Internal")]
     private static extern void ShowAdvExtern();
#endif

    private static bool _canShowAds = true;
    private static InterstitialAd _interstitialAd;
    private static bool _adLoaded;

    private const uint ANDROID_SLOT_ID = 1428217;

    public static void Initialize()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer && !Application.isMobilePlatform) return;
        _canShowAds = true;

        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) return;
        _interstitialAd = CreateInterstitialAd();

        // Устанавливаем обработчики событий
        _interstitialAd.AdLoadCompleted += OnLoadCompleted;
        _interstitialAd.AdDismissed += OnAdDismissed;
        _interstitialAd.AdDisplayed += OnAdDisplayed;
        _interstitialAd.AdVideoCompleted += OnAdVideoCompleted;
        _interstitialAd.AdClicked += OnAdClicked;
        _interstitialAd.AdLoadFailed += OnAdLoadFailed;

        // Запускаем загрузку данных
        _interstitialAd.Load();
    }

    private static InterstitialAd CreateInterstitialAd()
    {
        uint slotId = ANDROID_SLOT_ID;

        // Включение режима отладки
        // InterstitialAd.IsDebugMode = true;
        // Создаем экземпляр InterstitialAd
        return new InterstitialAd(slotId);
    }

    private static void OnAdLoadFailed(object sender, ErrorEventArgs e)
    {

    }

    private static void OnAdClicked(object sender, EventArgs e)
    {

    }

    private static void OnAdVideoCompleted(object sender, EventArgs e)
    {

    }

    private static void OnAdDisplayed(object sender, EventArgs e)
    {

    }

    private static void OnAdDismissed(object sender, EventArgs e)
    {

    }

    private static void OnLoadCompleted(object sender, EventArgs e)
    {
        _adLoaded = true;
    }

    public static void ShowAds()
    {
        if (!_canShowAds) return;
#if (UNITY_WEBGL && !UNITY_EDITOR)
        ShowAdsExtern();
#elif (UNITY_IOS || UNITY_ANDROID)
        if (!_adLoaded) return;
        _interstitialAd.Show();
        _adLoaded = false;
#endif
        _canShowAds = false;
        Bootstrap.Instance.StartCoroutine(RefreshAdsIE());
    }

    /* public void ShowAdv(ProductCoin productCoin)
    {
        if (productCoin != null)
        {
            if (productCoin.CanPickup)
            {
                currentProductCoin = productCoin;
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    ShowAdvExtern();
                }
                else
                    AddCoins(1);
            }
            else
            {
                GameManager.Instance.HudManager.ConfirmationWindow.SetInfo(LocalizationManager.Instance.GetLocalizedText("You have already collected the daily reward come back later"), null, true);
            }
        }
    } */

    private static IEnumerator RefreshAdsIE()
    {
        yield return new WaitForSecondsRealtime(60);
        _canShowAds = true;
    }
}
