using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.InGame.View
{
    public class UI_SkillList : MonoBehaviour
    {
        [SerializeField] private List<Image> _skillImageList;

        public void UpdateSkillInfo(Sprite sprite)
        {
            var image = _skillImageList.Find(v => !v.enabled);
            if (image)
            {
                image.enabled = true;
                image.sprite = sprite;
            }
        }
    }
}