
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static T GetOrAdd<T>(this MonoBehaviour monoBehaviour) where T : Component
    {
        T component = monoBehaviour.gameObject.GetComponent<T>();
        if (!component) component = monoBehaviour.gameObject.AddComponent<T>();
        return component;
    }

    public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

    public static void DestroyChildren(this MonoBehaviour monoBehaviour)
    {
        foreach (Transform child in monoBehaviour.gameObject.transform)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static GameObject Clone(this MonoBehaviour original, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject clone = Object.Instantiate(original.gameObject, position, rotation, parent);
        return clone;
    }
}

