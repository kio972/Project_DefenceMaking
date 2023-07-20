using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singleton = new GameObject();
                instance = singleton.AddComponent<T>();
                singleton.name = "(singleton)" + typeof(T).Name;
                singleton.hideFlags = HideFlags.HideAndDontSave;
            }

            return instance;
        }
    }
}