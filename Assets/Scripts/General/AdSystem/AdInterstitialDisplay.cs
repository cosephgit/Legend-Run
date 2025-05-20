using TMPro;
//using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI;

public class AdInterstitialDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textStatus;
    [SerializeField] private Button buttonLoad;
    [SerializeField] private Button buttonShow;
    [SerializeField] private TextMeshProUGUI textButtonLoad;
    [SerializeField] private TextMeshProUGUI textButtonShow;

    public static string INTERSTITIAL_INSTANCE_ID = "0";

    //LevelPlayInterstitialAd interstitialAd;

    // Use this for initialization
    void Start()
    {
        Debug.Log("unity-script: ShowInterstitialScript Start called");
    }

    /************* Interstitial API *************/
    /*
    public void LoadInterstitialButtonClicked()
    {
        // Create interstitial Ad
        interstitialAd = new LevelPlayInterstitialAd(GlobalVars.interstitialAdUnitId);

        // Register to events
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;

        Debug.Log("unity-script: LoadInterstitialButtonClicked");
        interstitialAd.LoadAd();
    }

    public void ShowInterstitialButtonClicked()
    {
        Debug.Log("unity-script: ShowInterstitialButtonClicked");
        if (interstitialAd.IsAdReady())
            interstitialAd.ShowAd();
        else
            Debug.Log("unity-script: interstitialAd.IsAdReady - False");
    }

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdLoadedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log("unity-script: I got InterstitialOnAdLoadFailedEvent With Error " + error);
    }

    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdDisplayedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError)
    {
        Debug.Log("unity-script: I got InterstitialOnAdDisplayFailedEvent With InfoError " + infoError);
    }

    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdInfoChangedEvent With AdInfo " + adInfo);
    }

    void OnDestroy()
    {
        interstitialAd?.DestroyAd();
    }
    */
}