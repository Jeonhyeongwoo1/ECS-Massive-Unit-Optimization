using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_ShopPurchaseSuccessItem : UI_SubItemElement
    {
        [SerializeField] private Image _bgImage;
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _amountText;

        public void UpdateUI(Sprite bgSprite, Sprite itemSprite, int rewardAmount, Vector2 itemImageSize)
        {
            if (bgSprite != null)
            {
                _bgImage.sprite = bgSprite;
            }
            
            if (itemSprite != null)
            {
                _itemImage.sprite = itemSprite;
            }
            else
            {
                _itemImage.sprite = null;
            }
            
            _amountText.text = $"x{rewardAmount}";
            _itemImage.rectTransform.sizeDelta = itemImageSize;
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }
    }
}