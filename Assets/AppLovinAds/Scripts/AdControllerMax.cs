using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AdControllerMax : MonoBehaviour  //SingletonComponent<AdControllerMax>
{

    #region IOS_KEYS
    private string maxSdkKey_IOS = "QRogKrHW3wWksf63sF9cwlyolE8TuWadOGKXv-STG6WTXn-4kJLuui1yKpGvGSzttmf2Bh912skQw7949WWOKp";
    private string interstialKey_IOS = "2621fb73df977b6d";
    private string rewardedKey_IOS = "d662ea2089a8cb3e";
    private string reardeddBannerKey_IOS = "";
    private string bannerKey_IOS = "";
    #endregion

    #region Android_Keys
    private string maxSdkKey_Android = "QRogKrHW3wWksf63sF9cwlyolE8TuWadOGKXv-STG6WTXn-4kJLuui1yKpGvGSzttmf2Bh912skQw7949WWOKp";
    private string interstialKey_Android = "9adfe65e6669c145";
    private string rewardedKey_Android = "b13aeda399cbdea1";
    private string reardeddBannerKey_Android = "";
    private string bannerKey_Android = "";
    #endregion


    #region KEYS
    private string max_Sdk_Key = "";
    private string interstial_Ad_Id = "";
    private string rewarded_Ad_Id = "";
    private string rewarded_Interstial_Ad_Id = "";
    private string banner_Ad_Id = "";
    #endregion

    #region REWARD VALUE
    [Header("GIVE REWARDED COIN VALUE HERE")]
    [SerializeField] private int reward_Value;
    #endregion



    #region VARIABLES

    [Space]
    public bool ads_Initilized;
    public bool ads_Removed;
    [Space]
    public bool isRewardedAddLodded;



    private bool isBannerShowing = false;
    private int interstitialRetryAttempt;

    private bool m_runiing;
    private int rewardedRetryAttempt;
    #endregion


    #region CUSTOM EVENTS FOR ADS

    public AdEvent e_OnRewardAdClosed { get; set; }
    private RewardAdGrantedEvent e_onRewardGrantedCallback;


    public delegate void AdEvent();

    #endregion


    public static AdControllerMax Instance;
    public delegate void RewardAdGrantedEvent(double rewardAmount);


    //ACTUAL CODE STARTS FROM HERE

    #region UNITY METHODS
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            CheckForRemoveAds();
            SetupKeys();
            InitilizeAppLovinSDK();



            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        AdsEventManager.e_showbanner += ShowBanner;
        AdsEventManager.e_hidebaner += HideBanner;
       // AdsEventManager.e_show_interstial += ShowInterstitial;
        AdsEventManager.e_show_rewarded += ShowRewardedAd;
    }


    private void OnDisable()
    {
        AdsEventManager.e_showbanner -= ShowBanner;
        AdsEventManager.e_hidebaner -= HideBanner;
       // AdsEventManager.e_show_interstial -= ShowInterstitial;
        AdsEventManager.e_show_rewarded -= ShowRewardedAd;
    }


    #endregion


    #region SDK INITILIZATION


    void CheckForRemoveAds()
    {

    }


    /// <summary>
    /// SETUP KEYS
    /// </summary>
    void SetupKeys()
    {

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                max_Sdk_Key = maxSdkKey_Android;
                interstial_Ad_Id = interstialKey_Android;
                rewarded_Ad_Id = rewardedKey_Android;
                break;

            case RuntimePlatform.IPhonePlayer:
                max_Sdk_Key = maxSdkKey_IOS;
                interstial_Ad_Id = interstialKey_IOS;
                rewarded_Ad_Id = rewardedKey_IOS;
                break;
        }


#if UNITY_EDITOR
        max_Sdk_Key = maxSdkKey_Android;
        interstial_Ad_Id = interstialKey_Android;
        rewarded_Ad_Id = rewardedKey_Android;
#endif


    }

    void InitilizeAppLovinSDK()
    {


        //CHECK FOR REMOVE ADS
        if (ads_Removed) return;

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads
            Debug.Log("You Can Load Ads From Here");

            ads_Initilized = true;

            AdsEventManager.NotifyOnAdsInitilization();

            //  MaxSdk.ShowMediationDebugger();

            //SDK INITILIZED START INITILIZATION OF ADS
            Debug.Log("SDK INITILIZED START INITILIZATION OF ADS");
            InitializeInterstitialAds();
            InitializeRewardedAds();

        };

        MaxSdk.SetSdkKey(max_Sdk_Key);
        //    MaxSdk.SetUserId("USER_ID");

        MaxSdk.InitializeSdk();
    }
    #endregion


    #region ONGOIN CHECKS

    public bool HasRewardedAd()
    {
        return isRewardedAddLodded;
    }

    #endregion

    #region SHOW ADS METHODS
    public bool ShowInterstitial()
    {
        if (!ads_Initilized || ads_Removed)
        {
            Debug.Log("ADS NOT INITILIZED OR ADS HAS BEEN REMOVED");
            return false;
        }

        if (MaxSdk.IsInterstitialReady(interstial_Ad_Id))
        {
            Debug.Log("Showing Interstial");
            MaxSdk.ShowInterstitial(interstial_Ad_Id);
            return true;
        }
        else
        {
            Debug.Log("Interstial Ad not ready");
            LoadInterstitial();
            return false;
        }

        return false;
    }

    public void ShowRewardedAd()
    {

        if (!ads_Initilized || ads_Removed)
        {
            Debug.Log("ADS NOT INITILIZED OR ADS HAS BEEN REMOVED");
            return;
        }

        if (MaxSdk.IsRewardedAdReady(rewarded_Ad_Id))
        {
            isRewardedAddLodded = false;
            Debug.Log("Showin Rewarded");
            MaxSdk.ShowRewardedAd(rewarded_Ad_Id);
        }
        else
        {
            Debug.Log("Rewarded Ad not ready");
            LoadRewardedAd();
        }
    }

    public bool ShowRewardedAd(AdEvent onClosedCallback, RewardAdGrantedEvent onRewardGrantedCallback)
    {
        if (!ads_Initilized || ads_Removed)
        {
            return false;
        }

        if (onClosedCallback != null)
        {
            this.e_OnRewardAdClosed = onClosedCallback;
        }

        if(onRewardGrantedCallback != null)
        {
            this.e_onRewardGrantedCallback = onRewardGrantedCallback;
        }

        return false;
    }


    #endregion


    #region SHOW POPUP METHODS

    private void RewardCoins()
    {
        //Reward the coins here and call a popup.

        Debug.Log("Reward Recived");
        AdsEventManager.NotifyRewwrdDistriButed(reward_Value);
        //  GameController.Instance.GiveCoinsIAP(coinIncentReward);
        // PopupController.Instance.Show("reward_success");
        AdsEventManager.NotifyRewwrdDistriButed(reward_Value);

        if (e_onRewardGrantedCallback != null)
        {
            e_onRewardGrantedCallback(reward_Value);
        }

        LoadRewardedAd();
    }

    #endregion

    #region Banner Ad Methods

    private void InitializeBannerAds()
    {
        // Attach Callbacks
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(banner_Ad_Id, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(banner_Ad_Id, Color.black);
    }

    private void ShowBanner()
    {
        if (!isBannerShowing)
        {
            isBannerShowing = true;
            MaxSdk.ShowBanner(banner_Ad_Id);
        }

    }

    private void HideBanner()
    {
        MaxSdk.HideBanner(banner_Ad_Id);
        isBannerShowing = false;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad is ready to be shown.
        // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
        Debug.Log("Banner ad loaded");
    }

    private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad clicked");
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Banner ad revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        //TrackAdRevenue(adInfo);
    }

    #endregion


    #region Interstitial Ad Methods

    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        //MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    void LoadInterstitial()
    {
        Debug.Log("Loading Interstial");
        MaxSdk.LoadInterstitial(interstial_Ad_Id);
    }



    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        Debug.Log("Interstitial loaded");

        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).

        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

        // interstitialStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
        Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

        if (interstitialRetryAttempt < 5)
        {
            interstitialRetryAttempt++;
            Debug.Log("Failed So Wait Few Min And Load Again");
            WaitAndReLoadInterstial();
        }
    }

    private async void WaitAndReLoadInterstial()
    {

        if (m_runiing) return;
        m_runiing = true;

        //WAITS FOR % SECOND
        await CustomWaiting._Waiter(5000);
        LoadInterstitial();
        m_runiing = false;
    }




    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        Debug.Log("Interstitial dismissed");
        LoadInterstitial();
    }

    //private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    //{
    //    // Interstitial ad revenue paid. Use this callback to track user revenue.
    //    Debug.Log("Interstitial revenue paid");

    //    // Ad revenue
    //    double revenue = adInfo.Revenue;

    //    // Miscellaneous data
    //    string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
    //    string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
    //    string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
    //    string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

    //   // TrackAdRevenue(adInfo);
    //}

    #endregion


    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        //  MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        Debug.Log("Loading Rewarded");
        MaxSdk.LoadRewardedAd(rewarded_Ad_Id);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        Debug.Log("Rewarded ad loaded");

        isRewardedAddLodded = true;
        // Reset retry attempt
        rewardedRetryAttempt = 0;

        AdsEventManager.NotifyOnRewaerdedLodded();
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));


        Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);


        if (rewardedRetryAttempt < 5)
        {
            RewardedLoadAgain();
        }

        //Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private async void RewardedLoadAgain()
    {
        //WAITS FOR FIVE SECOND AND LOAD AGAIN
        await CustomWaiting._Waiter(5000);
        LoadRewardedAd();
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad displayed");
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Debug.Log("Rewarded ad dismissed");
        LoadRewardedAd();

        if (e_OnRewardAdClosed != null)
        {
            e_OnRewardAdClosed();
        }
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward
        Debug.Log("Rewarded ad received reward");
        RewardCoins();
    }

    //private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    //{
    //    // Rewarded ad revenue paid. Use this callback to track user revenue.
    //    Debug.Log("Rewarded ad revenue paid");

    //    // Ad revenue
    //    double revenue = adInfo.Revenue;

    //    // Miscellaneous data
    //    string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
    //    string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
    //    string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
    //    string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

    //    //TrackAdRevenue(adInfo);
    //}

    #endregion
}

public struct AdsCOde
{
    public const string reward_fail = "reward_fail";
    public const string reward_success = "reward_success";
}

[System.Serializable]
public class ApplovonAdsSetting
{
    #region IOS_KEYS
    public string maxSdkKey_IOS = "QRogKrHW3wWksf63sF9cwlyolE8TuWadOGKXv-STG6WTXn-4kJLuui1yKpGvGSzttmf2Bh912skQw7949WWOKp";
    public string interstialKey_IOS = "2621fb73df977b6d";
    public string rewardedKey_IOS = "d662ea2089a8cb3e";
    public string reardeddBannerKey_IOS = "";
    public string bannerKey_IOS = "";
    #endregion

    #region Android_Keys
    public string maxSdkKey_Android = "QRogKrHW3wWksf63sF9cwlyolE8TuWadOGKXv-STG6WTXn-4kJLuui1yKpGvGSzttmf2Bh912skQw7949WWOKp";
    public string interstialKey_Android = "9adfe65e6669c145";
    public string rewardedKey_Android = "b13aeda399cbdea1";
    public string reardeddBannerKey_Android = "";
    public string bannerKey_Android = "";
    #endregion
}
