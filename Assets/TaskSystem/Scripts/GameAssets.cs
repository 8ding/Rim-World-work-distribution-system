using System;
using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

[Serializable]
public enum ObjectAnimationType
{
    Clean,
    Mine,
    Cut,
    Walk,
    Idle,
    Throw
}

[Serializable]
public struct FaceType_Object
{
    public FaceDirectionType faceDirectionType;
    public GameObject animatiGameObject;
}

[Serializable]
public struct AnimationType_Struct
{
    public ObjectAnimationType objectAnimationType;
    public List<FaceType_Object> faceTypeObjects;
}


[Serializable]
public enum ItemType
{
    MinePoint,
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
    public List<AnimationType_Struct> AnimationObjects;
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

    public GameObject createAnimationGameObject(ObjectAnimationType _objectAnimationType,
        FaceDirectionType faceDirectionType, Transform parent, Vector3 position)
    {

        if (AnimationObjects != null)
        {
            for (int i = 0; i < AnimationObjects.Count; i++)
            {
                if (AnimationObjects[i].objectAnimationType == _objectAnimationType)
                {
                    for (int j = 0; j < AnimationObjects[i].faceTypeObjects.Count; j++)
                    {
                        if (faceDirectionType == AnimationObjects[i].faceTypeObjects[j].faceDirectionType)
                        {
                            return Instantiate(AnimationObjects[i].faceTypeObjects[j].animatiGameObject, position,
                                Quaternion.identity, parent);
                        }
                    }
                }
            }
        }
        return null; 
    }
}
    


