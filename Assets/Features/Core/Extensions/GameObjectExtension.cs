using System;
using UnityEngine;
using Object = UnityEngine.Object;


public static class GameObjectExtension
{
    public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (!component) component = gameObject.AddComponent<T>();
        return component;
    }
    
    public static T GetOrAdd<T>(this Component component) where T : Component
    {
        T _component = component.gameObject.GetComponent<T>();
        if (!_component) _component = component.gameObject.AddComponent<T>();
        return _component;
    }

    public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

    public static void DestroyChildren(this GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static GameObject Clone(this GameObject original, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject clone = Object.Instantiate(original, position, rotation, parent);
        return clone;
    }
}
