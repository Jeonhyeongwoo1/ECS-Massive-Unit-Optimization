using System;
using MewVivor.Data;
using MewVivor.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UI
{
    public class LocalizedTextSetter : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private bool _isChangeFontMaterial = true;
        
        private TextMeshProUGUI _textMeshProUGUI;
        private Text _text;
        
        private void Awake()
        {
            TryGetComponent(out _text);
            TryGetComponent(out _textMeshProUGUI);
            Manager.I.Event?.AddEvent(GameEventType.ChangeLanguage, OnChangedLocalizationText);
        }

        private void Start()
        {
            OnChangedLocalizationText(null);
        }

        private void OnDestroy()
        {
            Manager.I.Event?.RemoveEvent(GameEventType.ChangeLanguage, OnChangedLocalizationText);
        }

        private void OnChangedLocalizationText(object v)
        {
            if (_textMeshProUGUI != null)
            {
                OnChangeTextMeshLocalization();
            }

            if (_text != null)
            {
                OnChangeTextLocalization();
            }
        }

        private void OnChangeTextLocalization()
        {
            if (_text == null)
            {
                Debug.LogWarning($"localization text is null name : " + transform.name);
                return;
            }
            
            if (Manager.I.Data.LocalizationDataDict.TryGetValue(_key, out LocalizationData data))
            {
                _text.text = data.GetValueByLanguage();
            }
        }

        private void OnChangeTextMeshLocalization()
        {
            if (_textMeshProUGUI == null)
            {
                Debug.LogWarning($"localization text is null name : " + transform.name);
                return;
            }
            
            string key = Manager.I.LanguageType == LanguageType.Eng ? "ENG" : "KOR";
            var font = Manager.I.Resource.Load<TMP_FontAsset>(key);
            _textMeshProUGUI.font = font;

            if (_isChangeFontMaterial)
            {
                switch (Manager.I.LanguageType)
                {
                    case LanguageType.Eng:
                        _textMeshProUGUI.fontSharedMaterial = Manager.I.Resource.Load<Material>("ENG_Material");
                        break;
                    case LanguageType.Kor:
                        // _textMeshProUGUI.fontSharedMaterial = Manager.I.Resource.Load<Material>("KOR_Material");
                        break;
                }
            
                _textMeshProUGUI.UpdateMeshPadding();
                _textMeshProUGUI.SetMaterialDirty();
            }

            if (Manager.I.Data.LocalizationDataDict.TryGetValue(_key, out LocalizationData data))
            {
                _textMeshProUGUI.text = data.GetValueByLanguage();
            }
        }
    }
}