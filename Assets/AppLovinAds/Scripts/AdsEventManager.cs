using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsEventManager : MonoBehaviour
{
    public delegate void AdsDelegateEvents();
    public static event AdsDelegateEvents e_ads_initilized, e_showbanner, e_hidebaner, e_load_interstial, e_interstial_close, e_rewarded_lodded, e_show_rewarded, e_rewarded_canceled, e_ads_removed;

    public delegate void RewardedEvents(int m_reward_amount);
    public static event RewardedEvents e_recived_reward;


    public delegate void AdFailEvent(string m_error);
    public static event AdFailEvent e_bannder_failed, e_interstal_failed, e_rewarder_failed;

    #region BANNER EVENTS
    public static void NotifyOnShowinngBaanner()
    {
        if (e_showbanner != null)
        {
            e_showbanner();
        }
    }

    public static void NotifyHideBanner()
    {
        if (e_hidebaner != null)
        {
            e_hidebaner();
        }
    }

    public static void NotifyOnBannerFailed(string error)
    {
        if (e_bannder_failed != null)
        {
            e_bannder_failed(error);
        }
    }

    #endregion


    #region INTERSTIAL EVENTS

    public static void NotifyLoadInterstial()
    {
        if (e_load_interstial != null)
        {
            e_load_interstial();
        }
    }

    //public static void NotifyShowInterstial()
    //{
    //    if (e_show_interstial != null)
    //    {
    //        e_show_interstial();
    //    }
    //}

    public static void NotifyCloseInterstial()
    {
        if (e_interstial_close != null)
        {
            e_interstial_close();
        }
    }

    #endregion


    #region REWARDED EVENTS

    public static void NotifyOnRewaerdedLodded()
    {
        if (e_rewarded_lodded != null)
        {
            e_rewarded_lodded();
        }
    }

    public static void NotifyShowRewarded()
    {
        if (e_show_rewarded != null)
        {
            e_show_rewarded();
        }
    }

    public static void NotifyRewardedCanceled()
    {
        if (e_rewarded_canceled != null)
        {
            e_rewarded_canceled();
        }
    }

    public static void NotifyRewwrdDistriButed(int rewarde_amount)
    {
        if (e_recived_reward != null)
        {
            e_recived_reward(rewarde_amount);
        }
    }

    public static void NotifyOnRewardedFailed(string error)
    {
        if (e_rewarder_failed != null)
        {
            e_rewarder_failed(error);
        }
    }

    #endregion

    #region REMOVE ADS

    public static void NotifyOnRemoveAds()
    {
        if (e_ads_removed != null)
        {
            e_ads_removed();
        }
    }
    #endregion

    #region ADS INITILIZATION

    public static void NotifyOnAdsInitilization()
    {
        if (e_ads_initilized != null)
        {
            e_ads_initilized();
        }
    }
    #endregion
}
