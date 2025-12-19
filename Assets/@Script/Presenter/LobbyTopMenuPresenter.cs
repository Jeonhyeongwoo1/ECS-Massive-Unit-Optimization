using MewVivor.Factory;

namespace MewVivor.Presenter
{
    public class LobbyTopMenuPresenter : BasePresenter
    {
        
        public void OpenPopup(bool isSettingPopup)
        {
            if (isSettingPopup)
            {
                var settingPopupPresenter = PresenterFactory.CreateOrGet<SettingPopupPresenter>();
                settingPopupPresenter.OpenPopup();
            }
            else
            {
                var mailPopupPresenter = PresenterFactory.CreateOrGet<MailPopupPresenter>();
                mailPopupPresenter.OpenPopup();
            }
        }
    }
}