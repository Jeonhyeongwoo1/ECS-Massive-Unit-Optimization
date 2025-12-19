using System;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Key;
using MewVivor.Managers;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class GemController : DropItemController
    {
        public GemType GemType => _gemType;
        private GemType _gemType;
        private float _elapsed = 0;
        private int _lifeTime = 40;

        public override void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            IsRelease = false;
            transform.position = spawnPosition;
            gameObject.SetActive(true);
            _dropableItemType = DropableItemType.Gem;
            _spriteRenderer.sortingOrder = Const.SortinglayerOrder_Gem;
            _elapsed = 0;
        }

        public void SetGemInfo(GemType gemType, Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            _gemType = gemType;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed > _lifeTime)
            {
                if (!Manager.I.Object.IsInCameraView(transform.position))
                {
                    _elapsed = 0;
                    Release();
                }
            }
        }

        protected override void CompletedGetItem()
        {
            base.CompletedGetItem();

            switch (_gemType)
            {
                case GemType.PurpleGem:
                    Manager.I.Audio.Play(Sound.SFX, SoundKey.GetPurpleGem, 0.2f, 0.1f);
                    break;
                case GemType.GreenGem:
                    Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGreenGem, 0.2f, 0.1f);
                    break;
                case GemType.BlueGem:
                    Manager.I.Audio.Play(Sound.SFX, SoundKey.GetBlueGem, 0.2f, 0.1f);
                    break;
                case GemType.RedGem:
                    Manager.I.Audio.Play(Sound.SFX, SoundKey.GetRedGem, 0.2f, 0.1f);
                    break;
            }
        }

        public int GetExp()
        {
            if (_gemType == GemType.None)
            {
                Debug.LogError("gemType is none");
                return 0;
            }

            DataManager data = Manager.I.Data;
            return _gemType switch
            {
                GemType.PurpleGem => (int)data.GlobalConfigDataDict[GlobalConfigName.PurpleGemExpAmount].Value,
                GemType.GreenGem => (int)data.GlobalConfigDataDict[GlobalConfigName.GreenGemExpAmount].Value,
                GemType.BlueGem => (int)data.GlobalConfigDataDict[GlobalConfigName.BlueGemExpAmount].Value,
                GemType.RedGem => (int)data.GlobalConfigDataDict[GlobalConfigName.RedGemExpAmount].Value
            };
        }
    }
}