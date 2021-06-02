
using UnityEngine;

public static class MyClass
{
    public static Vector3 GetMouseWorldPosition(float ScreenZ,Camera camera)
    {
        return camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            ScreenZ - camera.transform.position.z));
    }
}
