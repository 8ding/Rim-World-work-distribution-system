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
public enum ResourceType
{
    Gold,
    Wood,
}
[Serializable]
public enum ItemType
{
    Gold,
    Wood,
}
[Serializable]
public struct ItemType_Object
{
    public ItemType itemType;
    public GameObject itemGameObject;
}

[Serializable]
public struct ItemType_Sprite
{
    public ItemType itemType;
    public Sprite itemSprite;
}

[Serializable]
public struct ResourceType_Object
{
    public ResourceType ResourceType;
    public GameObject ResourceGameObject;
}
public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    public Sprite sprite;
    public GameObject player;
    public Sprite rifle;
    public Sprite WeaponSlot;
    
    
    public Sprite GoldPoint;
    public Sprite MiningShovel;
    public Sprite CutKnife;
    
    [SerializeField]
    public List<AnimationType_Struct> AnimationObjects;
    [SerializeField]
    public List<ItemType_Object> ItemTypeObjects;

    [SerializeField] 
    public List<ItemType_Sprite> ItemTypeSpries;

    [SerializeField] 
    public List<ResourceType_Object> ResourceTypeObjects;

    private Dictionary<ItemType, Sprite> itemTypeSpriteDictionary;

    public static GameAssets Instance
    {
        get
        {
            if (instance == null) instance = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return instance;
        }
    }

    private void Awake()
    {
        itemTypeSpriteDictionary = new Dictionary<ItemType, Sprite>();
        if(ItemTypeSpries!=null)
        {
            for (int i = 0; i < ItemTypeSpries.Count; i++)
            {
                if (ItemTypeSpries[i].itemSprite != null)
                {
                    itemTypeSpriteDictionary[ItemTypeSpries[i].itemType] = ItemTypeSpries[i].itemSprite;
                }
            }
        }
    }

    public GameObject createUnit(Transform  parent,Vector3 position)
    {
        GameObject Gb = Instantiate(player, position, Quaternion.identity);
        return Gb;
    }

    public GameObject createResource(Transform parent, Vector3 position,ResourceType resourceType)
    {
        for (int i = 0; i < ResourceTypeObjects.Count; i++)
        {
            if (resourceType == ResourceTypeObjects[i].ResourceType)
            {
                return Instantiate(ResourceTypeObjects[i].ResourceGameObject, position, Quaternion.identity);
            }
        }
        return null;
    }
    public GameObject createItemSprite(Transform parent, Vector3 position,ItemType itemType)
    {
        GameObject result = null;
        result = MyClass.CreateWorldSprite(parent, itemType.ToString(), "Item", itemTypeSpriteDictionary[itemType],
            position, Vector3.one, 1, Color.white);
        return result;
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
    


