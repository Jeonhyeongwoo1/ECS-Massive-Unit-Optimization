using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Key;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using MewVivor.Util;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class HuntPassPopupPresenter : BasePresenter
    {
        private HuntPassModel HuntPassModel => ModelFactory.CreateOrGetModel<HuntPassModel>();
        private UI_HuntPassPopup _popup;
        
        public async void OpenPopup()
        {
            await RequestHuntPassData();
            
            _popup = Manager.I.UI.OpenPopup<UI_HuntPassPopup>();
            _popup.AddEvent(OnClosePopup, OnPurchasePremiumPass);
            
            Refresh();
        }

        private async void OnPurchasePremiumPass()
        {
            var shopPopupPresenter = PresenterFactory.CreateOrGet<ShopPopupPresenter>();
            await shopPopupPresenter.OnPurchaseItemAsync(ShopIdType.Shop_HuntPass, false);

            var shopModel = ModelFactory.CreateOrGetModel<ShopModel>();
            bool isPurchasable = shopModel.IsPurchasable(ShopIdType.Shop_HuntPass);
            if (isPurchasable)
            {
                return;
            }

            await RequestHuntPassData();
            Refresh();
        }

        private void Refresh()
        {
            _popup.Release();
            int normalPassLevel = HuntPassModel.HuntPassData.normalClaimableLevel ?? 0;
            int premiumPassLevel = HuntPassModel.HuntPassData.premiumClaimableLevel ?? 0;
            int maxClaimableLevel = Mathf.Max(normalPassLevel, premiumPassLevel);
            List<PassRewardData> rewardList = Manager.I.Data.PassRewardDataDict.Values.OrderBy(r => r.Level).ToList();
            UI_HuntPassSubElement targetSubElement = null;
            for (int i = 0; i < rewardList.Count; i++)
            {
                PassRewardData reward = rewardList[i];
                var element = Manager.I.UI.AddSubElementItem<UI_HuntPassSubElement>(_popup.ScrollRectContentTransform);
                element.AddEvent(OnGetRewardHuntPass, reward.Level);
                element.UpdateUI(reward, HuntPassModel.HuntPassData);

                if (reward.Level == maxClaimableLevel)
                {
                    targetSubElement = element;
                }
            }

            int currentPoint = HuntPassModel.HuntPassData.point;
            int nextLevel = (normalPassLevel == 0) ? 1 : normalPassLevel + 1;
            int goalPoint = 0;
            var list = Manager.I.Data.PassRewardDataDict.Values.ToList();
            foreach (PassRewardData passRewardData in list)
            {
                if (passRewardData.Level > nextLevel)
                {
                    break;
                }
                
                goalPoint += passRewardData.NeedPoint;
            }
            
            float ratio = (float)currentPoint / goalPoint;
            if (targetSubElement != null)
            {
                _popup.LevelLineRectTransform.SetParent(targetSubElement.transform, false); // local 좌표 유지
                _popup.LevelLineRectTransform.anchoredPosition = new Vector2(0, -220);
            }
            
            _popup.UpdateUI(
                (normalPassLevel == 0 ? "1" : (maxClaimableLevel + 1).ToString()),
                ratio,
                HuntPassModel.HuntPassData.isPremium
            );
        }

        private async void OnGetRewardHuntPass(bool isPremium, int level)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }

            var requestData = new HuntPassClaimRequestData
            {
                isPremium = isPremium
            };

            TaskHelper.InitTcs();
            PassType passType = PassType.HuntPass;
            var response = await Manager.I.Web.SendRequest<HuntPassClaimResponseData>($"/pass/{passType}/claim/{level}",
                requestData,
                MethodType.POST.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"Failed error {response.message}");
                return;
            }

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int gold = response.data.inventory.gold - userModel.Gold.Value;
            int jewel = response.data.inventory.jewel - userModel.Jewel.Value;
            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold);
            }
            userModel.SetInventory(response.data.inventory);
            var huntPassModel = ModelFactory.CreateOrGetModel<HuntPassModel>();
            HuntPassData huntPassData = response.data.userPassStatus;
            huntPassModel.SetHuntPassData(huntPassData);

            var presenter = PresenterFactory.CreateOrGet<QuestRewardPopupPresenter>();
            presenter.OpenPopup(gold, jewel, 0, 0, 0, 0, 0, 0, 0);
            Refresh();

            // var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            // if (jewel > 0)
            // {
            //     Vector3 position = lobbyPresenter.JewelImagePosition;
            //     CurrencyCollectAction.I.ShowAnimation(CurrencyType.Jewel, position);
            // }
            //
            // if (gold > 0)
            // {
            //     Vector3 position = lobbyPresenter.GoldImagePosition;
            //     CurrencyCollectAction.I.ShowAnimation(CurrencyType.Gold, position);
            // }
        }

        public async UniTask RequestHuntPassData()
        {
            PassType passType = PassType.HuntPass;
            var response = await Manager.I.Web.SendRequest<HuntPassResponseData>($"/pass/{passType}/status", null,
                MethodType.GET.ToString());

            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"Failed error {response.message}");
                return;
            }

            var huntPassModel = ModelFactory.CreateOrGetModel<HuntPassModel>();            
            HuntPassData huntPassData = response.data;
            huntPassModel.SetHuntPassData(huntPassData);
        }

        public async UniTask RequestHuntPassBoostAsync()
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            PassType passType = PassType.HuntPass;
            var response =
                await Manager.I.Web.SendRequest<HuntPassResponseData>($"/pass/{passType}/boost", null,
                    MethodType.PATCH.ToString());

            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"Failed error {response.message}");
                return;
            }

            var huntPassModel = ModelFactory.CreateOrGetModel<HuntPassModel>();            
            HuntPassData huntPassData = response.data;
            huntPassModel.SetHuntPassData(huntPassData);
        }

        private void OnClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }
        
    }
}