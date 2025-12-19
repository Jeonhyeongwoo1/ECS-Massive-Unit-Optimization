using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.InGame.Entity;
using UnityEngine;

namespace MewVivor.InGame
{
    public enum BarrierType
    {
        Circle,
    }
    
    public class BossBarrierController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bossBarrierAlarmSpriteRenderer;
        
        private BarrierType _barrierType;
        private List<BossBarrier> _spawnedBossBarrierList = new();
        
        public void Initialize(BarrierType barrierType)
        {
            _barrierType = barrierType;
            Vector3 worldPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
            worldPoint.z = 0;
            transform.position = worldPoint;
            MakeSquareBarrier();
            gameObject.SetActive(true);
            StartCoroutine(SpawnAnimationCor());
        }

        private IEnumerator SpawnAnimationCor()
        {
            _bossBarrierAlarmSpriteRenderer.gameObject.SetActive(true);

            yield return new WaitForSeconds((3f));
            
            _bossBarrierAlarmSpriteRenderer.gameObject.SetActive(false);
            foreach (BossBarrier bossBarrier in _spawnedBossBarrierList)
            {
                bossBarrier.SpawnAnimation();
            }
        }

        public void Release()
        {
            Manager.I.Pool.ReleaseObject(nameof(BossBarrierController), gameObject);
            foreach (BossBarrier bossBarrier in _spawnedBossBarrierList)
            {
                bossBarrier.Release();
            }
            
            _spawnedBossBarrierList.Clear();
        }

        private void MakeBarrierAnimation()
        {
            int radius = 20;
            int count = 60;
            float angle = 360 / count;
            for (int i = 0; i < count; i++)
            {
                float spawnAngle = angle * i;
                Vector3 spawnPosition = transform.position + Utils.GetCirclePosition(spawnAngle, radius);
                SpawnBarrier(spawnPosition);
            }
        }

        private void SpawnBarrier(Vector3 spawnPosition)
        {
            var prefab = Manager.I.Resource.Instantiate(nameof(BossBarrier));
            prefab.TryGetComponent<BossBarrier>(out var bossBarrier);
            bossBarrier.Initialize(transform, spawnPosition);
            _spawnedBossBarrierList.Add(bossBarrier);
        }
        
        private void MakeSquareBarrier()
        {
            int countHorizontalPerSide = 30; // 한 변당 배리어 수
            int countVerticalPerSide = 50;
            float length = 40f;    // 한 변의 길이
            float half = length / 2f;
            Vector3 center = transform.position;

            _bossBarrierAlarmSpriteRenderer.size = new Vector2(length, length);
            _bossBarrierAlarmSpriteRenderer.transform.localPosition = Vector3.zero;
                
            // 아래쪽 (왼 → 오)
            for (int i = 0; i < countHorizontalPerSide; i++)
            {
                float t = i / (float)(countHorizontalPerSide - 1);
                Vector3 pos = new Vector3(Mathf.Lerp(-half, half, t), -half, 0);
                SpawnBarrier(center + pos);
            }

            // 오른쪽 (아래 → 위)
            for (int i = 0; i < countVerticalPerSide; i++)
            {
                float t = i / (float)(countHorizontalPerSide - 1);
                Vector3 pos = new Vector3(half, Mathf.Lerp(-half, half, t), 0);
                SpawnBarrier(center + pos);
            }

            // 위쪽 (오 → 왼)
            for (int i = 0; i < countHorizontalPerSide; i++)
            {
                float t = i / (float)(countHorizontalPerSide - 1);
                Vector3 pos = new Vector3(Mathf.Lerp(half, -half, t), half, 0);
                SpawnBarrier(center + pos);
            }

            // 왼쪽 (위 → 아래)
            for (int i = 0; i < countVerticalPerSide; i++)
            {
                float t = i / (float)(countHorizontalPerSide - 1);
                Vector3 pos = new Vector3(-half, Mathf.Lerp(half, -half, t), 0);
                SpawnBarrier(center + pos);
            }
        }
    }
}