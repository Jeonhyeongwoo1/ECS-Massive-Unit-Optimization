using System;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Popup;

namespace MewVivor.Presenter
{
    public class InfinityChallengePopupPresenter : BasePresenter
    {
        private UI_InfinityChallengePopup _popup;

        public void OpenPopup()
        {
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            _popup = Manager.I.UI.OpenPopup<UI_InfinityChallengePopup>();
            _popup.OpenPopup();
            _popup.AddEvent(OnGameStart, ClosePopup);

            int remainTicketCount = userModel.Inventory.infiniteTicket;
            int monsterKillCount = userModel.InfinityHistory.maxMonsterKillCount;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(userModel.InfinityHistory.survivalTime);
            string formatted = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            string aliveTime = formatted;
            _popup.UpdateUI(remainTicketCount.ToString(), monsterKillCount.ToString(), aliveTime);
        }

        public void ClosePopup()
        {
            if(_popup == null)
            {
                return;
            }

            Manager.I.UI.ClosePopup();
        }

        private async void OnGameStart()
        {
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            if (userModel.Stamina.Value < Const.InfinityTickCountForStartGame)
            {
                string message = Manager.I.Data.LocalizationDataDict["Not_enough_stamina"].GetValueByLanguage();
                Manager.I.UI.OpenSystemPopup(message);
                return;
            }

            await Manager.I.StartGame(GameType.INFINITY, 0);
        }
    }
}