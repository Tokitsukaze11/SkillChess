using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_instance;
    public static T Instance
    {
        get
        {
            //if (s_instance == null)
            if(ReferenceEquals(s_instance,null))
            {
                s_instance = (T)FindObjectOfType(typeof(T));
                if (ReferenceEquals(s_instance,null))
                {
                    GameObject obj = new GameObject();
                    s_instance = obj.AddComponent(typeof(T)) as T;
                    obj.name = typeof(T).ToString();
                }
            }
            return s_instance;
        }
    }
}
