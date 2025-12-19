using System.Collections.Generic;
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
    public class ShopPurchaseSuccessPopupPresenter : BasePresenter
    {
        private UI_ShopPurchaseSuccessPopup _popup;

        public void OpenPopup(GrantedItem grantedItem)
        {
            _popup = Manager.I.UI.OpenPopup<UI_ShopPurchaseSuccessPopup>();
            _popup.AddEvent(OnConfirm);
            _popup.ReleaseItem();

            ResourcesManager resourcesManager = Manager.I.Resource;

            if (grantedItem != null)
            {
                int amountValue = 0;
                Sprite bgSprite = null;
                Sprite itemSprite = null;
                if (grantedItem.gold > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Gold_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.GoldSpriteName);
                    amountValue = grantedItem.gold;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new Vector2(100, 100));
                }

                if (grantedItem.jewel > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Jewel_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.JewelSpriteName);
                    amountValue = grantedItem.jewel;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new(100, 100));
                }

                if (grantedItem.weaponScroll > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Scroll_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.Scroll_Weapon_Icon_SpriteName);
                    amountValue = grantedItem.weaponScroll;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new Vector2(120, 120));
                }

                if (grantedItem.armorScroll > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Scroll_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.Scroll_Armor_Icon_SpriteName);
                    amountValue = grantedItem.armorScroll;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new Vector2(120, 120));
                }

                if (grantedItem.glovesScroll > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Scroll_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.Scroll_Gloves_Icon_SpriteName);
                    amountValue = grantedItem.glovesScroll;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new Vector2(120, 120));
                }

                if (grantedItem.beltScroll > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Scroll_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.Scroll_Belt_Icon_SpriteName);
                    amountValue = grantedItem.beltScroll;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new Vector2(120, 120));
                }

                if (grantedItem.bootsScroll > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Scroll_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.Scroll_Boots_Icon_SpriteName);
                    amountValue = grantedItem.bootsScroll;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new Vector2(120, 120));
                }

                if (grantedItem.ringScroll > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Scroll_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.Scroll_Ring_Icon_SpriteName);
                    amountValue = grantedItem.ringScroll;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new Vector2(120, 120));
                }

                if (grantedItem.infiniteTicket > 0)
                {
                    bgSprite = resourcesManager.Load<Sprite>(Const.Shop_RewardBox_Scroll_SpriteName);
                    itemSprite = resourcesManager.Load<Sprite>(Const.INFINITE_Icon_SpriteName);
                    amountValue = grantedItem.infiniteTicket;
                    var shopPurchaseSuccessItem =
                        Manager.I.UI.AddSubElementItem<UI_ShopPurchaseSuccessItem>(_popup.ContentTransform);
                    shopPurchaseSuccessItem.UpdateUI(bgSprite, itemSprite, amountValue, new Vector2(100, 100));
                }
            }
        }

        private void OnConfirm()
        {
            OnClosePopup();
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