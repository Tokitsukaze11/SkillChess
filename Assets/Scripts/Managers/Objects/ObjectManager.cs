using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectManager : Singleton<ObjectManager>
{
    [SerializeField] private ObjectPool _objectPool;
    private void Awake()
    {
        if(_objectPool == null)
        {
            _objectPool = GetComponent<ObjectPool>();
        }
    }
    public void SpawnObject(GameObject obj, ObjectType eObjectType, bool isPooling = true)
    {
        if(isPooling)
        {
            _objectPool.CreateObject(obj, eObjectType);
        }
        else
        {
            Instantiate(obj);
        }
    }
    public void RemoveObject(GameObject obj, ObjectType eObjectType, bool isPooling = true)
    {
        if(isPooling)
        {
            _objectPool.RemoveObject(obj, eObjectType);
        }
        else
        {
            Destroy(obj);
        }
    }
}
