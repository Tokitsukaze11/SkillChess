using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ObjectType
{
    GlobalObject,
    Pawn,
    Particle,
}

public class ObjectPool : MonoBehaviour
{
    private Dictionary<ObjectType, Queue<GameObject>> _objectPooling;
    public void CreateObject(GameObject obj,ObjectType eObjectType)
    {
        var targetQueue = _objectPooling[eObjectType];
        if(targetQueue.Count == 0)
        {
            var newObj = Instantiate(obj);
            newObj.SetActive(true);
            targetQueue.Enqueue(newObj);
        }
        else
        {
            var targetObj = targetQueue.Dequeue();
            targetObj.SetActive(true);
        }
    }
    public void RemoveObject(GameObject obj,ObjectType eObjectType)
    {
        obj.SetActive(false);
        _objectPooling[eObjectType].Enqueue(obj);
    }
}
