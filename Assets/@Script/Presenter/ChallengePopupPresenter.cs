using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Popup;

namespace MewVivor.Presenter
{
    public class ChallengePopupPresenter : BasePresenter
    {
        private UI_ChallengePopup _popup;
        
        public void OpenPopup()
        {
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            _popup = Manager.I.UI.OpenPopup<UI_ChallengePopup>();
            _popup.AddEvent(OnShowChallengePopup);
            _popup.UpdateUI(userModel.Inventory.infiniteTicket);
        }

        public void ClosePopup() 
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }

        private void OnShowChallengePopup(ChallengeType challengeType)
        {
            if (challengeType == ChallengeType.Infinity)
            {
                InfinityChallengePopupPresenter presenter =
                    PresenterFactory.CreateOrGet<InfinityChallengePopupPresenter>();
                presenter.OpenPopup();
            }
        }
    }
}