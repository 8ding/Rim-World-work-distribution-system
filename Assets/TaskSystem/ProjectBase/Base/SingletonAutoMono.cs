using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
 
    private static T instance;

    public static T Instance()
    {
        if(instance == null)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = typeof(T).ToString();
            instance = gameObject.AddComponent<T>();
        }
        return instance;
    }

    
}
