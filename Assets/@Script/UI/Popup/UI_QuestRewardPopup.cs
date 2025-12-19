using MewVivor.Common;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Popup
{
    public class UI_QuestRewardPopup : BasePopup
    {
        public Transform ContentTransform => _contentTransform;

        [SerializeField] private Transform _contentTransform;

        public void ReleaseChild()
        {
            var childs = Utils.GetChildComponent<UI_QuestStageRewardItem>(_contentTransform);
            if (childs != null)
            {
                foreach (UI_QuestStageRewardItem uiRewardItem in childs)
                {
                    Manager.I.Pool.ReleaseObject(nameof(UI_QuestStageRewardItem), uiRewardItem.gameObject);
                }
            }
        }
    }
}