using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public static class AdsManager
{
    [DllImport("__Internal")]
    private static extern void ShowAdsExtern();
    [DllImport("__Internal")]
    private static extern void ShowAdvExtern();

    private static bool _canShowAds = true;

    public static void Initialize()
    {
        _canShowAds = true;
    }

    public static void ShowAds()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (_canShowAds)
            {
                ShowAdsExtern();
                _canShowAds = false;
                Bootstrap.Instance.StartCoroutine(RefreshAdsIE());
            }
        }
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
