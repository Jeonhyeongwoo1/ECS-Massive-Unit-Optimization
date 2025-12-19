using MewVivor.InGame.Popup;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class ExitGamePresenter : BasePresenter
    {
        private UI_ExitGamePopup _popup;
        
        public void OpenExitGamePopup()
        {
            _popup = Manager.I.UI.OpenPopup<UI_ExitGamePopup>();
            _popup.AddEvent(OnQuickGame, OnResumeGame, OnCloseAction);
        }

        private void OnCloseAction()
        {
            Manager.I.UI.ClosePopup();
        }

        private void OnResumeGame()
        {
            Manager.I.UI.ClosePopup();
        }

        private void OnQuickGame()
        {
            Manager.I.Game.GameEnd();
        }
    }
}