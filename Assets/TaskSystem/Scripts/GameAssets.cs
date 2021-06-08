using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

[Serializable]
public enum ObjectAnimationType
{
    CleanUP,
    Mine,
}
[Serializable]
public enum ItemType
{
    MinePoint,
}
[Serializable]
public struct Type_Object
{
    public ObjectAnimationType objectAnimationType;
    public GameObject animationGameObject;
}

[Serializable]
public struct ItemType_Object
{
    public ItemType itemType;
    public GameObject itemGameObject;
}
[Serializable]
public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    public Sprite sprite;
    public GameObject player;
    public Sprite rifle;
    public Sprite WeaponSlot;
    public Sprite gold;
    public Sprite GoldPoint;
    
    [SerializeField]
    public List<Type_Object> AnimationObjects;
    [SerializeField]
    public List<ItemType_Object> ItemTypeObjects;

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

    public GameObject createItem(Transform parent, Vector3 position,ItemType itemType)
    {
        for (int i = 0; i < ItemTypeObjects.Count; i++)
        {
            if (itemType == ItemTypeObjects[i].itemType)
            {
                return Instantiate(ItemTypeObjects[i].itemGameObject, position, Quaternion.identity);
            }
        }
        return null;
    }

    public GameObject createAnimationGameObject(ObjectAnimationType _objectAnimationType,Transform parent, Vector3 position)
    {
        
        if(AnimationObjects != null)
        {
            for (int i = 0; i < AnimationObjects.Count; i++)
            {
                if(AnimationObjects[i].objectAnimationType == _objectAnimationType)
                {
                    return Instantiate(AnimationObjects[i].animationGameObject, position, Quaternion.identity);
                }
            }
        }
        return null;
    }

}
