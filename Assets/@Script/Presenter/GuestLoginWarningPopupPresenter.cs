using System;
using MewVivor.Popup;

namespace MewVivor.Presenter
{
    public class GuestLoginWarningPopupPresenter : BasePresenter
    {
        private UI_GuestLoginWarningPopup _popup;

        public void OpenPopup(Action onConfirmAction)
        {
            _popup = Manager.I.UI.OpenPopup<UI_GuestLoginWarningPopup>();
            _popup.AddEvent(OnClosePopup, onConfirmAction);
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