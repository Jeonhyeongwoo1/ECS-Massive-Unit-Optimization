using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Managers;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class MergeSuccessPopupPresenter : BasePresenter
    {
        private UI_MergeSuccessPopup _popup;

        public void OpenPopup(List<Equipment> mergedEquipmentList, List<UserEquipmentData> userAddedEquipmentDataList)
        {
            _popup = Manager.I.UI.OpenPopup<UI_MergeSuccessPopup>();
            _popup.AddEvent(ClosePopup);
            _popup.UpdateUI(mergedEquipmentList, userAddedEquipmentDataList);
        }

        public void ClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }
    }
}