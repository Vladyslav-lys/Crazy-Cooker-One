using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager<T> : MonoBehaviour where T : class
{
    public static T instance;
    
    protected virtual void Awake()
    {
        InitializeManager();
    }
    
    protected virtual void InitializeManager() {}
}
