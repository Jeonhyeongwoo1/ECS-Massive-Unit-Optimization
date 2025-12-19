using MewVivor.Equipmenets;
using MewVivor.Popup;

namespace MewVivor.Presenter
{
    public class EquipmentLevelDownSuccessPopupPresenter : BasePresenter
    {
        private UI_EquipmentLevelDownSuccessPopup _popup;

        public void OpenEquipmentLevelDownSuccessPopup(Equipment equipment, int scrollAmount)
        {
            _popup = Manager.I.UI.OpenPopup<UI_EquipmentLevelDownSuccessPopup>();
            _popup.UpdateUI(equipment, scrollAmount);
        }
    }
}