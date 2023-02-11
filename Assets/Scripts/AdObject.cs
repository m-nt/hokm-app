using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TapsellPlusSDK;

[Serializable]
public class AdObject
{
    public bool active = false;
    public delegate void callback(string result);

    private string ZONE_ID_INIT_VIDEO  = "61b11271e383cb23cb803362";
    private string ZONE_ID_REWRD_VIDEO = "61b2251863530e4c698af4ba";
    private string ZONE_ID_INTR_BANNER = "61b115e663530e4c698af3e2";
    private string ZONE_ID_STAN_BANNER = "61b2257877253b7a103eb742";
    private string TAPSELLPLUS_KEY     = "llpnqidhrdssindretbpmlfanioeqmnhlggnkpccsltkoqlhcphebdtsskmjpmdbthcdpo";

    public void init()
    {
        TapsellPlus.Initialize(TAPSELLPLUS_KEY,
            adNetworkName => Debug.Log(adNetworkName + " Initialized Successfully."),
            error => Debug.Log(error.ToString()));
        TapsellPlus.SetGdprConsent(true);
        
    }
    public void RewardVideo(callback open, callback reward, callback close, callback err)
    {
        RequestRewardedVideoAd(ZONE_ID_REWRD_VIDEO,open,reward,close,err);
    }
    private void RequestRewardedVideoAd(string ZONE_ID, callback open, callback reward, callback close, callback err)
    {
        TapsellPlus.RequestRewardedVideoAd(ZONE_ID,

                  tapsellPlusAdModel => {
                      Debug.Log("on response " + tapsellPlusAdModel.responseId);
                      ShowRewardedVideoAd(tapsellPlusAdModel.responseId, open, reward, close, err);
                  },
                  error => {
                      Debug.Log("Error " + error.message);
                      err(error.message);
                  }
              );
    }
    private void ShowRewardedVideoAd(string _responseId, callback open, callback reward, callback close, callback err)
    {
        TapsellPlus.ShowRewardedVideoAd(_responseId,

                  tapsellPlusAdModel => {
                      open("open");
                  },
                  tapsellPlusAdModel => {
                      reward("reward");
                  },
                  tapsellPlusAdModel => {
                      close("close");
                  },
                  error => {
                      err(error.errorMessage);
                  }
              );
    }

    public void InterVideo(callback open, callback close, callback err)
    {
        if (active)
        {
            RequestInterstitialVideoAd(ZONE_ID_INIT_VIDEO, open, close, err);
        }
        else
        {
            err("deactive");
        }
    }
    private void RequestInterstitialVideoAd(string ZONE_ID, callback open, callback close, callback err)
    {
        TapsellPlus.RequestInterstitialAd(ZONE_ID,

            tapsellPlusAdModel => {
                ShowInterstitialVideoAd(tapsellPlusAdModel.responseId,open,close,err);
            },
            error => {
                err(error.message);
            }
        );
    }
    private void ShowInterstitialVideoAd(string _responseId, callback open, callback close, callback err)
    {
        TapsellPlus.ShowInterstitialAd(_responseId,

            tapsellPlusAdModel => {
                open("open");
            },
            tapsellPlusAdModel => {
                close("close");
            },
            error => {
                err(error.errorMessage);
            }
        );
    
    }
    public void StanBannerAd(callback open, callback err)
    {
        RequestStanBannerAd(ZONE_ID_STAN_BANNER, open, err);
    }
    private void RequestStanBannerAd(string ZONE_ID,callback open,callback err)
    {
        TapsellPlus.RequestStandardBannerAd(ZONE_ID, BannerType.Banner320X50,

            tapsellPlusAdModel => {
                ShowStanBannerAd(tapsellPlusAdModel.responseId,open,err);
            },
            error => {
                err(error.message);
            }
        );
    }

    private void ShowStanBannerAd(string _responseId,callback open,callback err)
    {
        TapsellPlus.ShowStandardBannerAd(_responseId, Gravity.Bottom, Gravity.Center,

            tapsellPlusAdModel => {
                open("open");
            },
            error => {
                err(error.errorMessage);
            }
        );
    }
    public void InterBannerAd(callback open, callback close, callback err)
    {
        if (active)
        {
            RequestInterstitialAd(ZONE_ID_INTR_BANNER, open, close, err);
        }
        else
        {
            err("deactive");
        }
    }
    private void RequestInterstitialAd(string ZONE_ID, callback open, callback close, callback err)
    {
        TapsellPlus.RequestInterstitialAd(ZONE_ID,

            tapsellPlusAdModel => {
                ShowInterstitialAd(tapsellPlusAdModel.responseId, open, close, err);
            },
            error => {
                err(error.message);
            }
        );
    }
    private void ShowInterstitialAd(string _responseId, callback open, callback close, callback err)
    {
        TapsellPlus.ShowInterstitialAd(_responseId,

            tapsellPlusAdModel => {
                open("open");
            },
            tapsellPlusAdModel => {
                close("close");
            },
            error => {
                err(error.errorMessage);
            }
        );

    }
    public void HideStanBanner()
    {
        TapsellPlus.HideStandardBannerAd();
    }
    public void ShowStanBanner()
    {
        TapsellPlus.DisplayStandardBannerAd();
    }
    public void OnRandomAd(callback open,callback reward, callback close, callback err)
    {
        int number = UnityEngine.Random.Range(0, 3);
        switch (number)
        {
            case 0:
                InterBannerAd(open, close, err);
                break;
            case 1:
                InterVideo(open, close, err);
                break;
            case 2:
                RewardVideo(open, reward, close, err);
                break;
            default:
                break;
        }
    }
}
