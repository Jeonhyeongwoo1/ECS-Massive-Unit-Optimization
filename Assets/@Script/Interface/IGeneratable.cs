using System;
using MewVivor.Data;
using MewVivor.InGame.Skill;
using UnityEngine;

public interface IGeneratable
{
    Action<Transform, Projectile> OnHit { get; set; }
    Action<Transform, Projectile> OnExit { get; set; }
    void Generate(Transform targetTransform, Vector3 direction, AttackSkillData attackSkillData, CreatureController owner, int currentLevel);
    void Generate(Vector3 spawnPosition, Vector3 direction, AttackSkillData attackSkillData, CreatureController owner);
    int Level { get; set; }
    Projectile ProjectileMono { get; }
    void Release();
    void OnChangedSkillData(AttackSkillData attackSkillData);
    bool IsRelease { get; }
}