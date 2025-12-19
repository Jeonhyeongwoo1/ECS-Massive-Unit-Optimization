using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using MewVivor.Data.Server;
using MewVivor.Factory;
using MewVivor.Model;
using UnityEngine;
using Reward = GoogleMobileAds.Api.Reward;

namespace MewVivor.Managers
{
    public class AdsManager
    {

        public void Initialize()
        {
            LoadRewardAdAsync().Forget();
#if UNITY_IOS
            MobileAds.SetiOSAppPauseOnBackground(true);
#endif

        }

        #region Rewarded Ads

        public GameObject rewardAdLoadedStatus;

        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private const string _sampleRewardAdUnitId = "ca-app-pub-3940256099942544/5224354917";
        [SerializeField] private string _rewardAdUnitId = "";
#elif UNITY_IPHONE
        private const string _sampleRewardAdUnitId = "ca-app-pub-3940256099942544/1712485313";
        [SerializeField] private string _rewardAdUnitId = "";
#else
        private const string _sampleRewardAdUnitId = "";
        [SerializeField] private string _rewardAdUnitId = "";
#endif

        private RewardedAd _rewardedAd;
        private Action _onSuccessShowAdsAction;
        private Action _onFailedShowAdsAction;

        public async UniTaskVoid LoadRewardAdAsync()
        {
            while (true)
            {
                if (_rewardedAd == null || !_rewardedAd.CanShowAd())
                {
                    await UniTask.SwitchToMainThread();
                    LoadRewardAd();
                }

                await UniTask.WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadRewardAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                DestroyRewardAd();
            }

            Debug.Log("Loading rewarded ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            string id = _sampleRewardAdUnitId;
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            
            // Send the request to load the ad.
            RewardedAd.Load(id, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                    return;
                }

                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                _rewardedAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);

                // Inform the UI that the ad is ready.
                // rewardAdLoadedStatus?.SetActive(true);
            });
        }

        /// <summary>
        /// Shows the ad.
        /// 보상형 광고
        /// </summary>
        public void ShowRewardAd(Action successCallback, Action failCallback)
        {
            _onSuccessShowAdsAction = successCallback;
            _onFailedShowAdsAction = failCallback;

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            ShopSubscriptionInfoData vipData = userModel.SubscriptionInfo["Shop_VVIP"];
            if (vipData.isSubscribed)
            {
                _onSuccessShowAdsAction?.Invoke();
                return;
            }

            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                Debug.Log("Showing rewarded ad.");
        
                bool hasReward = false;

                _rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log($"Rewarded ad granted a reward: {reward.Amount} {reward.Type}");
                    hasReward = true;
                });

                _rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("OnAdFullScreenContentClosed : " + hasReward);
                    if (hasReward)
                    {
                        Debug.Log("OnSuccessCallback");
                        _onSuccessShowAdsAction?.Invoke(); // 광고 종료 + 보상 수령 후 실행
                    }
                    else
                    {
                        _onFailedShowAdsAction?.Invoke(); // 광고 닫힘은 있지만 보상은 없음
                    }

                    DestroyRewardAd(); // 광고는 반드시 재로드 필요
                Debug.Log("_Reswardsssss");
                };
                
                Debug.Log("_Resward");
            }
            else
            {
                Debug.LogError("Rewarded ad is not ready yet.");
                DestroyRewardAd();
                _onFailedShowAdsAction?.Invoke();
            }
        }


        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyRewardAd()
        {
            if (_rewardedAd != null)
            {
                Debug.Log("Destroying rewarded ad.");
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            // Inform the UI that the ad is not ready.
            // rewardAdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogRewardAdResponseInfo()
        {
            if (_rewardedAd != null)
            {
                var responseInfo = _rewardedAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () => { Debug.Log("Rewarded ad recorded an impression."); };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () => { Debug.Log("Rewarded ad was clicked."); };
            // Raised when the ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () => { Debug.Log("Rewarded ad full screen content opened."); };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () => { Debug.Log("Rewarded ad full screen content closed."); };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content with error : "
                               + error);
            };
        }

        #endregion
    }
}