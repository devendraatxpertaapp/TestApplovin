using Google.Play.Review;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif


public class InAppReview : MonoBehaviour
{

    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;



    public static InAppReview Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    //private void Start()
    //{
    //    RequestInAppReview();
    //}

    /// <summary>
    /// REQUEST HERE
    /// </summary>
    public void RequestInAppReview()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                StartCoroutine(RequestReviewForAndroid());
                break;

            case RuntimePlatform.IPhonePlayer:
                RequestReviewForIPhone();
                break;
        }
    }

    private IEnumerator RequestReviewForAndroid()
    {

        //REQUESTING REVIEW
        Debug.Log("Requested Review");
        _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.Log("ERROR REQUESTING REVIEW  " + requestFlowOperation.Error.ToString());
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();


        //LAUNCHING REVIEW

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }

    private void RequestReviewForIPhone()
    {
        #if UNITY_IOS
        Device.RequestStoreReview();
        #endif
    }
}
