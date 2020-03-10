    using System.Collections;
using UnityEngine;
#if ADSENABLED
using UnityEngine.Advertisements;
#endif
using PnCCasualGameKit;

//To avoid unncessary platform sepecific warnings
#pragma warning disable
/// <summary>
/// Handles Ad. 
/// Mandatory Requirements:
/// 1. Unitys Monetization/Ads sdk must be imported.
/// 2. The preprocessor symbol "ADSENABLED" must be added to the project for the script to work.
/// 
/// The preprocessor is added to 
/// 1. Make sure that there isn't a compile error if the Unity ad sdk is not imported.
/// 2. Enable/disable ads for the project.
/// The prepocessor can be enabled/disabled through the editor inspector of this script as well.(Enable Ads checkbox)
/// </summary>
public class AdsManager : LazySingleton<AdsManager>
{
    ///<summary>
    /// To enable/disable ads.(from inspector)
    /// set this to true only if Unity Ads sdk is imported.
    /// </summary>
    [HideInInspector]
    public bool isAdEnabled;

    /// <summary>
    /// If true, Interstial/banner ads won't show. This is mainly for "prenium/No ads" IAP. 
    /// </summary>
    [HideInInspector]
    public bool isAdRemoved;


    /// <summary>
    /// Game IDs from the Unity Ad portal. 
    /// </summary>
    [SerializeField]
    private string iosGameId = default;
    [SerializeField]
    private string AndroidGameId = default;
    private string gameId;

    ///<summary> Enable this if game not published yet</summary>
    public bool testMode = true;

    ///<summary>Required Ad types</summary>
    public Ad displayAd;
    public Ad bannerAd;
    public Ad rewardedAd;

    /// <summary>
    /// Shows a display/Interstitial ad.
    /// </summary>
    public void ShowDisplayAd()
    {
        if (isAdRemoved)
        {
            return;
        }

#if ADSENABLED
        if (displayAd.useFrequency)
        {
            displayAd.callCount++;
            if (displayAd.adFrequency != 0 && displayAd.callCount % displayAd.adFrequency == 0)
            {
                StartCoroutine(ShowDisplayAdWhenReady());
            }
        }
        else
        {
            StartCoroutine(ShowDisplayAdWhenReady());
        }
#endif
    }

    /// <summary>
    /// Shows a banner ad.
    /// </summary>
    public void ShowBannerAd()
    {
        if (isAdRemoved)
        {
            return;
        }

#if ADSENABLED
        if (bannerAd.useFrequency)
        {
            bannerAd.callCount++;
            if (bannerAd.adFrequency != 0 && bannerAd.callCount % bannerAd.adFrequency == 0)
            {
                StartCoroutine(ShowBannerAdWhenReady());
            }
        }
        else
        {
            StartCoroutine(ShowBannerAdWhenReady());
        }
#endif
    }

    /// <summary>
    /// Hides a banner ad.
    /// </summary>
    public void HideBannerAd()
    {
#if ADSENABLED
        Advertisement.Banner.Hide();
#endif
    }

    /// <summary>
    /// Shows the rewarded ad.
    /// </summary>
    public void ShowRewardedAd(System.Action success, System.Action failure)
    {
#if ADSENABLED
        rewardedAd.callCount++;
        if (rewardedAd.useFrequency && rewardedAd.adFrequency != 0 && rewardedAd.callCount % rewardedAd.adFrequency == 0)
        {
            StartCoroutine(ShowRewardedAdWhenReady(success, failure));
        }
        else
        {
            StartCoroutine(ShowRewardedAdWhenReady(success, failure));
        }
#endif
    }

    /// <summary>
    /// Get the targer game id according to platform and initialize ad.
    /// </summary>
#if ADSENABLED
    private void Awake()
    {

#if UNITY_ANDROID
        gameId = AndroidGameId;
#elif UNITY_IOS
        gameId = iosGameId;
#endif
        Advertisement.Initialize(gameId, testMode);
    }

    /// <summary>
    /// Couroutine to show display ad. Tries loading ad every 0.25 seconds.
    /// </summary>
    private IEnumerator ShowDisplayAdWhenReady()
    {
        while (!Advertisement.IsReady(displayAd.placementID))
        {
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(displayAd.delay);

        //In editor, the game doesn't pause when ad is shown. 
        //Workaround : set timescale to 0 when showing ad and to 1 when done showing.
#if UNITY_EDITOR
        Time.timeScale = 0;
#endif
        var options = new ShowOptions { resultCallback = HandleAdResult };

        Advertisement.Show(displayAd.placementID, options);
    }

    /// <summary>
    /// Couroutine to show Banner ad. Tries loading ad every 0.25 seconds.
    /// </summary>
    private IEnumerator ShowBannerAdWhenReady()
    {
        while (!Advertisement.IsReady(bannerAd.placementID))
        {
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(bannerAd.delay);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerAd.placementID);
    }

    /// <summary>
    /// Handles result for a display and banner ad.
    /// </summary>
    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
        //In editor, the game doesn't pause when ad is shown. 
        //Workaround : set timescale to 0 when showing ad and to 1 when done showing.
#if UNITY_EDITOR
        // StartCoroutine(ResumeGame());
        Time.timeScale = 1;
#endif
    }

    /// <summary>
    /// Couroutine to show Rewarded ad. Tries loading ad every 0.25 seconds.
    /// </summary>
    private IEnumerator ShowRewardedAdWhenReady(System.Action _success, System.Action _failure)
    {
        rewardedAd.success = _success;
        rewardedAd.failure = _failure;
        while (!Advertisement.IsReady(rewardedAd.placementID))
        {
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(rewardedAd.delay);

        //In editor, the game doesn't pause when ad is shown. 
        //Workaround : set timescale to 0 when showing ad and to 1 when done showing.
#if UNITY_EDITOR
        Time.timeScale = 0;
#endif
        var options = new ShowOptions { resultCallback = HandleRewardedAdResultwc };

        Advertisement.Show(rewardedAd.placementID, options);
    }

    /// <summary>
    /// Handles callback methods for a rewarded ad result.
    /// </summary>
    private void HandleRewardedAdResultwc(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Rewarded ad was successfully shown.");
                rewardedAd.success();
                break;
            case ShowResult.Skipped:
                Debug.Log("Rewarded ad was skipped before reaching the end.");
                rewardedAd.failure();
                break;
            case ShowResult.Failed:
                rewardedAd.failure();
                Debug.LogError("Rewarded ad failed to be shown.");
                break;
        }
        //In editor, the game doesn't pause when ad is shown. 
        //Workaround : set timescale to 0 when showing ad and to 1 when done showing.
#if UNITY_EDITOR
        Time.timeScale = 1;
#endif
    }
#endif

}


/// <summary>
/// Blueprint for an Ad
/// </summary>
[System.Serializable]
public class Ad
{
    /// <summary> The Ad placement id from Unity Ad portal</summary>
    public string placementID;

    /// <summary>should ad showing frequency be used</summary>
    public bool useFrequency = true;

    /// <summary> ad showing frequency</summary>
    public int adFrequency = 1;

    /// <summary> Ad shows after this delay</summary>
    public float delay;

    /// <summary> Current count of ad call. Used along with frequency to determine if ad should be shown or not</summary>
    [HideInInspector]
    public int callCount;

    /// <summary> Call back event for successful ad</summary>
    public System.Action success;

    /// <summary> Call back event for failed ad</summary>
    public System.Action failure;
}
