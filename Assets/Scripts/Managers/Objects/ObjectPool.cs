using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

public class ObjectPool : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> _objectPooling = new Dictionary<string, Queue<GameObject>>();
    public void MakePool(GameObject obj, [NotNull] string objectCode)
    {
        if(objectCode == null)
        {
            throw new System.Exception("Object Code is cannot be null");
        }
        if(!_objectPooling.ContainsKey(objectCode))
        {
            _objectPooling.Add(objectCode, new Queue<GameObject>());
        }
        var targetQueue = _objectPooling[objectCode];
        for (int i = 0; i < 10; i++)
        {
            var newObj = Instantiate(obj);
            newObj.SetActive(false);
            targetQueue.Enqueue(newObj);
        }
    }
    public GameObject CreateObject(GameObject obj,[NotNull] string objectCode)
    {
        if(objectCode == null)
        {
            throw new System.Exception("Object Code is cannot be null");
        }
        if(!_objectPooling.ContainsKey(objectCode))
        {
            _objectPooling.Add(objectCode, new Queue<GameObject>());
        }
        var targetQueue = _objectPooling[objectCode];
        if (targetQueue.Count != 0)
        {
            var targetObj = targetQueue.Dequeue();
            targetObj.SetActive(true);
            return targetObj;
        }
        var newObj = Instantiate(obj);
        newObj.SetActive(true);
        return newObj;
    }
    public void RemoveObject(GameObject obj,string objectCode)
    {
        obj.SetActive(false);
        if(_objectPooling[objectCode].Count < 10)
        {
            _objectPooling[objectCode].Enqueue(obj);
        }
    }
}
