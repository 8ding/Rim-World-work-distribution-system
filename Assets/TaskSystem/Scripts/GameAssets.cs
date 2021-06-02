using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    
    public static GameAssets Instance
    {
        get
        {
            if (instance == null) instance = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return instance;
        }
    }

    public GameObject createUnit(Transform  parent,Vector3 position)
    {
        GameObject Gb = Instantiate(player, position, Quaternion.identity);
        return Gb;
    }
    public Sprite sprite;
    public GameObject player;
}
