using System;
using System.Collections;
using System.Collections.Generic;
using StateMachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.Serialization;

[Serializable]
public enum ObjectAnimationType
{
    Clean,
    Mine,
    Cut,
    Walk,
    Idle,
    Throw,
    enumcount
}

[Serializable]
public enum ResourcePointType
{
    None,
    GoldPoint,
    WoodPoint,
}
[Serializable]
public enum ItemType
{
    None,
    Gold,
    Wood,
}
[Serializable]
public enum PlacedObjectType
{
    ResourcePoint,
    Item,
}
[Serializable]
public struct ItemType_Object
{
    [FormerlySerializedAs("itemType")] public PlacedObjectType placedObjectType;
    public GameObject itemGameObject;
}

[Serializable]
public struct ItemType_Sprite
{
    [FormerlySerializedAs("itemType")] public PlacedObjectType placedObjectType;
    public Sprite itemSprite;
}

[Serializable]
public struct ResourceType_Object
{
    [FormerlySerializedAs("ResourceType")] public ResourcePointType resourcePointType;
    public GameObject ResourceGameObject;
}
public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    public Sprite sprite;
    public GameObject player;
    public Sprite rifle;
    public Sprite WeaponSlot;
    public const int CharecterNumber = 1;
    
    public Sprite GoldPoint;
    public Sprite MiningShovel;
    public Sprite CutKnife;
    

    [SerializeField]
    public List<ItemType_Object> ItemTypeObjects;


    [SerializeField] 
    public List<ItemType_Sprite> ItemTypeSpries;

    [SerializeField] 
    public List<ResourceType_Object> ResourceTypeObjects;

    private Dictionary<PlacedObjectType, Sprite> itemTypeSpriteDictionary;

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
        itemTypeSpriteDictionary = new Dictionary<PlacedObjectType, Sprite>();
        if(ItemTypeSpries!=null)
        {
            for (int i = 0; i < ItemTypeSpries.Count; i++)
            {
                if (ItemTypeSpries[i].itemSprite != null)
                {
                    itemTypeSpriteDictionary[ItemTypeSpries[i].placedObjectType] = ItemTypeSpries[i].itemSprite;
                }
            }
        }

    }

    public GameObject createUnit(Transform  parent,Vector3 position)
    {
        GameObject Gb = Instantiate(player, position, Quaternion.identity);
        return Gb;
    }

    public GameObject createResource(Transform parent, Vector3 position,ResourcePointType _resourcePointType)
    {
        for (int i = 0; i < ResourceTypeObjects.Count; i++)
        {
            if (_resourcePointType == ResourceTypeObjects[i].resourcePointType)
            {
                return Instantiate(ResourceTypeObjects[i].ResourceGameObject, position, Quaternion.identity);
            }
        }
        return null;
    }
    public GameObject createItemSprite(Transform parent, Vector3 position,PlacedObjectType _placedObjectType)
    {
        GameObject result = null;
        result = MyClass.CreateWorldSprite(parent, _placedObjectType.ToString(), "Item", itemTypeSpriteDictionary[_placedObjectType],
            position, Vector3.one, 1, Color.white);
        return result;
    }

    public GameObject createAnimationGameObject(int id,ObjectAnimationType _objectAnimationType,
        FaceDirectionType faceDirectionType, Transform parent, Vector3 position)
    {
        string path = "animation/" + id + "_" + _objectAnimationType + "_" + faceDirectionType;
        GameObject animationObject = Resources.Load<GameObject>(path) as GameObject;
        if(animationObject != null)
            return Instantiate(animationObject, position, quaternion.identity,parent);
        return null; 
    }
}
    


