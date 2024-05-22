using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectManager : Singleton<ObjectManager>
{
    [SerializeField] private ObjectPool _objectPool;
    [SerializeField] private ParticleSpawner _particleSpawner;
    public Transform globalObjectParent;
    private void Awake()
    {
        if(_objectPool == null)
        {
            _objectPool = GetComponent<ObjectPool>();
        }
    }
    public void MakePool(GameObject obj, string objectCode)
    {
        _objectPool.MakePool(obj, objectCode);
    }
    /// <summary>
    /// Spawn object
    /// </summary>
    /// <param name="obj">Target of spawn object</param>
    /// <param name="objectCode">Target object code. If object is not pooling object, can use null</param>
    /// <param name="isPooling">Is pooling object?</param>
    /// <returns>Spawned object</returns>
    /// <exception cref="Exception">If pooling object, but object code is null, throw exception</exception>
    public GameObject SpawnObject(GameObject obj, string objectCode, bool isPooling = true)
    {
        return isPooling ? _objectPool.CreateObject(obj, objectCode) : Instantiate(obj);
    }
    public void RemoveObject(GameObject obj, string objectCode, bool isPooling = true)
    {
        if(isPooling)
        {
            _objectPool.RemoveObject(obj, objectCode);
        }
        else
        {
            Destroy(obj);
        }
    }
    /// <summary>
    /// Spawn Particle
    /// </summary>
    /// <param name="particle">Set particle object</param>
    /// <param name="objectCode">Set object code. If not pooling object, can use null</param>
    /// <param name="isPooling">Is pooling object?</param>
    /// <returns>Spawned particle object</returns>
    public GameObject SpawnParticle(GameObject particle, string objectCode = null, bool isPooling = false)
    {
        return _particleSpawner.SpawnParticle(particle, objectCode, isPooling);
    }
}
