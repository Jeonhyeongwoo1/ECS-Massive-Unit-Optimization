using System.Collections.Generic;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Interface;
using UniRx;

namespace MewVivor.Model
{
    public class ShopModel : IModel
    {
        public ReactiveProperty<bool> IsPossiblePurchaseVip = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsPossiblePurchaseVVip = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsPossiblePurchaseVipAndVVip = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsExistFreeReward = new ReactiveProperty<bool>();
        
        public List<ShopItemInfoData> ShopItemInfoDataList => _shopItemInfoDataList;
        
        private List<ShopItemInfoData> _shopItemInfoDataList;

        public void SetShopItemInfoData(List<ShopItemInfoData> shopItemInfoDataList)
        {
            shopItemInfoDataList ??= new List<ShopItemInfoData>();
            _shopItemInfoDataList = shopItemInfoDataList;

            var vipShopItemData = shopItemInfoDataList.Find(v => v.itemId == ShopIdType.Shop_VIP.ToString());
            var vvipShopItemData = shopItemInfoDataList.Find(v => v.itemId == ShopIdType.Shop_VVIP.ToString());
            IsPossiblePurchaseVip.Value = vipShopItemData.isPurchasable;
            IsPossiblePurchaseVVip.Value = vvipShopItemData.isPurchasable;
            IsPossiblePurchaseVipAndVVip.Value = vipShopItemData.isPurchasable && vvipShopItemData.isPurchasable;

            var shopGold1Data = shopItemInfoDataList.Find(v => v.itemId == ShopIdType.Shop_Gold1.ToString());
            var shopGold2Data = shopItemInfoDataList.Find(v => v.itemId == ShopIdType.Shop_Gold2.ToString());
            bool isExistFreeGold = !shopGold1Data.isPurchasable && !shopGold2Data.isPurchasable;

            var shopJewel1Data = shopItemInfoDataList.Find(v => v.itemId == ShopIdType.Shop_Jewel1.ToString());
            var shopJewel2Data = shopItemInfoDataList.Find(v => v.itemId == ShopIdType.Shop_Jewel2.ToString());
            bool isExistFreeJewel = !shopJewel1Data.isPurchasable && !shopJewel2Data.isPurchasable;

            IsExistFreeReward.Value = !isExistFreeGold && !isExistFreeJewel;
        }

        public bool IsPurchasable(ShopIdType shopIdType)
        {
            ShopItemInfoData shopItemData = _shopItemInfoDataList.Find(v => v.itemId == shopIdType.ToString());
            if (shopItemData == null)
            {
                return false;
            }
            
            return shopItemData.isPurchasable;
        }
        
        public void Reset()
        {
            _shopItemInfoDataList = null;
        }
    }
}