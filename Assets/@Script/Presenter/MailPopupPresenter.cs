using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Key;
using MewVivor.Model;
using MewVivor.OutGame.UI;
using MewVivor.Popup;
using MewVivor.Util;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class MailPopupPresenter : BasePresenter
    {
        private UI_MailPopup _popup;
        
        public void OpenPopup()
        {
            OpenMail();
        }

        private async void OpenMail()
        {
            var response =
                await Manager.I.Web.SendRequest<GetMailResponseData>("/mails", null, MethodType.GET.ToString());

            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"Failed error {response.statusCode}");
                return;
            }

            _popup = Manager.I.UI.OpenPopup<UI_MailPopup>();
            _popup.AddEvent(OnAllClaim, OnClosePopup);

            UserModel model = ModelFactory.CreateOrGetModel<UserModel>();
            List<MailData> list = response.data.list;
            model.SetMailData(list);
            RefreshUI(list);
        }

        private void RefreshUI(List<MailData> list)
        {
            _popup.ReleaseMailItem();
            foreach (MailData mailData in list)
            {
                string description = mailData.text;
                string title = mailData.title;
                DateTime createdAt = mailData.createdAt;
                DateTime expiredAt = mailData.expiredAt;
                TimeSpan remainAt = expiredAt - createdAt;
                if (remainAt.TotalSeconds < 0)
                {
                    continue;
                }

                var mailItem = Manager.I.UI.AddSubElementItem<UI_MailItem>(_popup.MailGroupTransform);
                //우선 하나만 나온다고 하자
                Dictionary<string, int> rewardDict = mailData.rewards;
                KeyValuePair<string, int> reward = rewardDict.FirstOrDefault();
                int rewardId = int.Parse(reward.Key);
                ItemData itemData = Manager.I.Data.ItemDataDict[rewardId];
                Sprite sprite = Manager.I.Resource.Load<Sprite>(itemData.SpriteName);
                Dictionary<string, LocalizationData> dict = Manager.I.Data.LocalizationDataDict;
                if (mailData.type == MailType.Vip_Mail.ToString())
                {
                    if (description.Contains("Subscription"))
                    {
                        description = dict["Vip_Active"].GetValueByLanguage();
                        title = dict["Vip_Active"].GetValueByLanguage();
                    }
                    else
                    {
                        description = dict["Vip_DailyReward"].GetValueByLanguage();
                        
                        int num = GetNumData(title);
                        title = dict["Vip_DailyReward_day"].GetValueByLanguage();
                        title = string.Format(title, num);
                    }
                }
                else if (mailData.type == MailType.Vvip_Mail.ToString())
                {
                    if (description.Contains("Subscription"))
                    {
                        description = dict["VVip_Active"].GetValueByLanguage();
                        title = dict["VVip_Active"].GetValueByLanguage();
                    }
                    else
                    {
                        description = dict["VVip_DailyReward"].GetValueByLanguage();
                        
                        int num = GetNumData(title);
                        title = dict["VVip_DailyReward_day"].GetValueByLanguage();
                        title = string.Format(title, num);
                    }
                } 
                else if (mailData.type == MailType.LevelUp_Mail.ToString())
                {
                    int num = GetNumData(description);
                    string levelUp = dict["Level_Up"].GetValueByLanguage();
                    title = dict["Level_Up_!"].GetValueByLanguage();
                    description = string.Format(levelUp, num);
                }
                
                mailItem.AddEvent(() => OnClaim(mailData.id));
                mailItem.UpdateUI(sprite, title, description, reward.Value, remainAt);
                mailItem.transform.localScale = Vector3.one;
            }

            bool isEmpty = list.Count == 0;
            _popup.UpdateUI(isEmpty);
        }

        private async void OnClaim(string mailId)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<ClaimMailResponseData>($"/mails/{mailId}/claim", null,
                    MethodType.POST.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                switch (response.message)
                {
                    case ServerMessageType.MAIL_NOT_FOUND_EXPIRED:
                        string message = Manager.I.Data.LocalizationDataDict["MAIL_NOT_FOUND_EXPIRED"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                    case ServerMessageType.MAIL_ALREADY_CLAIMED:
                        message = Manager.I.Data.LocalizationDataDict["MAIL_ALREADY_CLAIMED"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                    case ServerMessageType.MAIL_NOT_FOUND:
                        message = Manager.I.Data.LocalizationDataDict["MAIL_NOT_FOUND"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                }
                
                return;
            }

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int gold = response.data.inventory.gold - userModel.Gold.Value;
            int jewel = response.data.inventory.jewel - userModel.Jewel.Value;
            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold);
            }
            userModel.SetUserData(response.data.userData);
            userModel.SetInventory(response.data.inventory);
            List<MailData> list = response.data.mails;
            userModel.SetMailData(list);
            RefreshUI(list);
            
            var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            if (jewel > 0)
            {
                Vector3 position = lobbyPresenter.JewelImagePosition;
                CurrencyCollectAction.I.ShowAnimation(CurrencyType.Jewel, position);
            }
            
            if (gold > 0)
            {
                Vector3 position = lobbyPresenter.GoldImagePosition;
                CurrencyCollectAction.I.ShowAnimation(CurrencyType.Gold, position);
            }

            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold);
            }
        }

        private async void OnAllClaim()
        {  
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<AllClaimMailResponseData>($"/mails/claim-all", null,
                    MethodType.POST.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                return;
            }
            
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int gold = response.data.inventory.gold - userModel.Gold.Value;
            int jewel = response.data.inventory.jewel - userModel.Jewel.Value;
            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold);
            }
            userModel.SetUserData(response.data.userData);
            userModel.SetInventory(response.data.inventory);
            userModel.SetMailData(response.data.mails);
            var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            if (jewel > 0)
            {
                Vector3 position = lobbyPresenter.JewelImagePosition;
                CurrencyCollectAction.I.ShowAnimation(CurrencyType.Jewel, position);
            }
            
            if (gold > 0)
            {
                Vector3 position = lobbyPresenter.GoldImagePosition;
                CurrencyCollectAction.I.ShowAnimation(CurrencyType.Gold, position);
            }

            OnClosePopup();
        }

        private void OnClosePopup()
        {
            if (_popup == null)
            {
                return;
            }

            var battlePopupPresenter = PresenterFactory.CreateOrGet<BattlePopupPresenter>();
            battlePopupPresenter.CloseBGPanel();
            Manager.I.UI.ClosePopup();
        }
        
        private readonly Regex NumRx = new(@"\d+", RegexOptions.Compiled);

        public int GetNumData(string input)
        {
            var m = NumRx.Match(input);
            if (!m.Success)
            {
                return 0;
            }
            int value = int.Parse(m.Value);
            return value;
        }
    }
}