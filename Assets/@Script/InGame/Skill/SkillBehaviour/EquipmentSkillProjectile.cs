using System;
using System.Collections;
using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.InGame.Enum;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class EquipmentSkillProjectile : MonoBehaviour
    {
        public Action<Transform, EquipmentSkillProjectile> OnHit { get; set; }
        public Action<Transform, EquipmentSkillProjectile> OnExit { get; set; }
        private List<Transform> _onTriggerEnterTransformList = new List<Transform>();
        private float _interval;
        private float _elapsed = 0;
        
        public void Generate(CreatureController owner, EquipmentSkillData equipmentSkillData)
        {
            transform.SetParent(owner.transform);
            Vector3 scale = Vector3.one * equipmentSkillData.Type_Value3;
            _interval = equipmentSkillData.Type_Value2;

            Transform[] childs = transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in childs)
            {
                child.localScale = scale;
            }

            gameObject.SetActive(true);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if ((other.CompareTag(Tag.Monster))
                && !_onTriggerEnterTransformList.Contains(other.transform))
            {
                _onTriggerEnterTransformList.Add(other.transform);
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if ((other.CompareTag(Tag.Monster))
                && _onTriggerEnterTransformList.Contains(other.transform))
            {
                _onTriggerEnterTransformList.Remove(other.transform);
            }
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed > _interval)
            {
                _elapsed = 0;
                foreach (Transform tr in _onTriggerEnterTransformList)
                {
                    OnHit?.Invoke(tr, this);
                }
            }
        }
    }
}