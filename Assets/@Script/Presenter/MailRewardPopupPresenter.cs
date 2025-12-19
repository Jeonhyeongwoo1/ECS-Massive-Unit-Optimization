using System.Collections.Generic;
using MewVivor.Popup;

namespace MewVivor.Presenter
{
    public class MailRewardPopupPresenter : BasePresenter
    {
        private UI_MailRewardPopup _popup;

        public void OpenPopup(Dictionary<string, int> rewardDict)
        {
            _popup = Manager.I.UI.OpenPopup<UI_MailRewardPopup>();
            
        }
    }
}