using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject _normalAttackHitParticle;
    public GameObject _attackSkillHitParticle;
    public GameObject _magicMineParticle;
    public GameObject _magicOtherParticle;
    Dictionary<string,GameObject> _particleDic = new Dictionary<string, GameObject>();
    private void Awake()
    {
        _particleDic.Add(StringKeys.NORMAL_ATTACK_HIT, _normalAttackHitParticle);
        _particleDic.Add(StringKeys.ATTACK_SKILL_HIT, _attackSkillHitParticle);
        _particleDic.Add(StringKeys.MAGIC_MINE, _magicMineParticle);
        _particleDic.Add(StringKeys.MAGIC_OTHER, _magicOtherParticle);
    }
    private void Start()
    {
        ObjectManager.Instance.MakePool(_normalAttackHitParticle, StringKeys.NORMAL_ATTACK_HIT);
    }
    public GameObject SpawnParticle(GameObject particle, string objectCode = null, bool isPooling = false)
    {
        var particleObj = ObjectManager.Instance.SpawnObject(particle, objectCode, isPooling);
        return particleObj;
    }
    public GameObject SpawnParticleViaCode(string objectCode)
    {
        _particleDic.TryGetValue(objectCode, out var particle);
        var particleObj = ObjectManager.Instance.SpawnObject(particle, objectCode, true);
        return particleObj;
    }
}
