using MewVivor.Data;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class QuestRewardPopupPresenter : BasePresenter
    {
        private UI_QuestRewardPopup _popup;

        public void OpenPopup(int gold, int jewel, int weaponScroll, int glovesScroll, int ringScroll, int armorScroll,
            int beltScroll, int bootsScroll, int questStagePoint)
        {
            _popup = Manager.I.UI.OpenPopup<UI_QuestRewardPopup>();

            _popup.ReleaseChild();

            Sprite sprite = null;
            int amount = 0;
            if (gold > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.BundleGoldSpriteName);
                amount = gold;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }

            if (jewel > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.JewelSpriteName);
                amount = jewel;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }

            if (weaponScroll > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.Scroll_Weapon_Icon_SpriteName);
                amount = weaponScroll;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }
            
            if (glovesScroll > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.Scroll_Gloves_Icon_SpriteName);
                amount = glovesScroll;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }
            
            if (armorScroll > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.Scroll_Armor_Icon_SpriteName);
                amount = armorScroll;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }
            
            if (beltScroll > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.Scroll_Belt_Icon_SpriteName);
                amount = beltScroll;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }
            
            if (bootsScroll > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.Scroll_Boots_Icon_SpriteName);
                amount = bootsScroll;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }
            
            if (ringScroll > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.Scroll_Ring_Icon_SpriteName);
                amount = ringScroll;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }

            if (questStagePoint > 0)
            {
                sprite = Manager.I.Resource.Load<Sprite>(Const.QuestExp_Icon_SpriteName);
                amount = questStagePoint;
                var uiRewardItem = Manager.I.UI.AddSubElementItem<UI_QuestStageRewardItem>(_popup.ContentTransform);
                uiRewardItem.UpdateUI(sprite, amount);
            }
        }
    }
}