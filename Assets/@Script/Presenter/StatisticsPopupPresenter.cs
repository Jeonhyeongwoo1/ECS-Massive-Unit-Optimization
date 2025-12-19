using System.Collections.Generic;
using MewVivor.Popup;

namespace MewVivor.Presenter
{
    public class StatisticsPopupPresenter : BasePresenter
    {
        private UI_StatisticsPopup _popup;

        public void OpenStatisticsPopup()
        {
            _popup = Manager.I.UI.OpenPopup<UI_StatisticsPopup>();
            _popup.AddEvent(OnPopupClose);   
            List<TotalDamageInfoData> list = Manager.I.Object.Player.GetTotalDamageInfoData();
            _popup.UpdateUI(list);
        }

        private void OnPopupClose()
        {
            Manager.I.UI.ClosePopup();
        }
    }
}