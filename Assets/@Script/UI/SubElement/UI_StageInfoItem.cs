using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_StageInfoItem : UI_SubItemElement
    {
        [SerializeField] private Image _stageImage;
        [SerializeField] private GameObject _lockObject;
        
        public void UpdateUI(bool isLock)
        {
            _lockObject.SetActive(isLock);
            _stageImage.enabled = !isLock;
            gameObject.SetActive(true);
        }
    }
}
