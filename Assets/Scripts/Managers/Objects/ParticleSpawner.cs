using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject SpawnParticle(GameObject particle, string objectCode = null, bool isPooling = false)
    {
        var particleObj = ObjectManager.Instance.SpawnObject(particle, objectCode, isPooling);
        return particleObj;
    }
}
