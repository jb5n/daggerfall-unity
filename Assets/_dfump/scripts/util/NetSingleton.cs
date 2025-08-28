using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T instance;

    protected virtual void Awake()
    {
        if (instance == null && this is T)
        {
            instance = this as T;
        }
        else
        {
            Debug.Log("Deleting duplicate instance of " + GetType().ToString() + ", " + gameObject.name + ": class is marked as singleton.");
            Destroy(gameObject);
        }
    }
}
