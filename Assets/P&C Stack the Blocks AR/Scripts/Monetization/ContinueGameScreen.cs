using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Handles for the Continue game screen shown after game-over
/// </summary>
public class ContinueGameScreen : MonoBehaviour {

    [Tooltip("Will time out after these many seconds")]
    public float timeInSeconds;

    /// <summary>The Fill type loading Image for timer</summary>
    public Image loadingImg;

    /// <summary>
    /// Callbacks for the ad is successfully viewed or not.
    /// Put the game resuming and game over code here.
    /// </summary>
    public UnityEvent successCallback, failureCallback;

    private void OnEnable () {
        StartCoroutine("TimerCoroutine");
	}

    private void OnDisable()
    {
        StopCoroutine("TimerCoroutine");
    }

    IEnumerator TimerCoroutine(){
        float timeleft = timeInSeconds;
        while (timeleft > 0)
        {
            yield return new WaitForEndOfFrame();
            timeleft -= Time.deltaTime;
            loadingImg.fillAmount = timeleft / timeInSeconds;
        }
        CloseScreen();
        OnAdfailed();
    }

    public void ShowAd(){
        CloseScreen();
        AdsManager.Instance.ShowRewardedAd(OnAdSuccess, OnAdfailed);
    }

    public void CloseScreen(){
        gameObject.SetActive(false);
    }

    void OnAdSuccess(){
        successCallback.Invoke();
    }

    void OnAdfailed(){
        failureCallback.Invoke();
    }
}
