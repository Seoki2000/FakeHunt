using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSingleTon<T> : MonoBehaviour where T : TSingleTon<T>
{
    static volatile T _uniqueInstance = null;
    static volatile GameObject _uniqueObeject = null;

    protected TSingleTon()
    {

    }

    public static T _instnace
    {
        get
        {
            if(_uniqueInstance == null)
            {
                lock (typeof(T))
                {
                    if(_uniqueInstance == null && _uniqueObeject == null)
                    {
                        _uniqueObeject = new GameObject(typeof(T).Name, typeof(T));
                        _uniqueInstance = _uniqueObeject.GetComponent<T>();
                        _uniqueInstance.Init();
                    }
                }
            }
            return _uniqueInstance;
        }
    }

    protected virtual void Init()
    {
        DontDestroyOnLoad(gameObject);
    }





}
