
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_GachaProbabilityTableElement : UI_SubItemElement
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _probabilityText;
        [SerializeField] private Image _bgImage;

        public void UpdateUI(string name, string probability, Color color)
        {
            _nameText.text = name;
            _probabilityText.text = probability;
            _bgImage.color = color;
            gameObject.SetActive(true);
        }
    }
}