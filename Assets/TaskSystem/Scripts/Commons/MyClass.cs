
using UnityEngine;

public static class MyClass
{
    public static Vector3 GetMouseWorldPosition(float ScreenZ,Camera camera)
    {
        return camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            ScreenZ - camera.transform.position.z));
    }
    
    public static GameObject CreateWorldSprite(Transform parent, string name, string LayerName,Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color) {
        GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = localScale;
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerName = LayerName;
        spriteRenderer.color = color;
        return gameObject;
    }

}
