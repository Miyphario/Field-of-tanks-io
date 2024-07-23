using System.Collections;
using UnityEngine;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public static class AdsManager
{
#if UNITY_WEBGL
     [DllImport("__Internal")]
     private static extern void ShowAdsExtern();
     [DllImport("__Internal")]
     private static extern void ShowAdvExtern();
#endif

    private static bool _canShowAds = true;

    private static WaitForSecondsRealtime _refreshAdsWait = new(60f);

    public static void Initialize()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer && !Application.isMobilePlatform) return;
        _canShowAds = true;

        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) return;
    }

    public static void ShowAds()
    {
        if (!_canShowAds) return;
#if (UNITY_WEBGL && !UNITY_EDITOR)
        ShowAdsExtern();
#elif (UNITY_IOS || UNITY_ANDROID)
        if (!_adLoaded) return;
        // Show ads
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
        yield return _refreshAdsWait;
        _canShowAds = true;
    }
}
