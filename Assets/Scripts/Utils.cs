using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static bool IsBall(this GameObject gameObject)
    {
        return gameObject.CompareTag(Tags.Ball);
    }

    public static bool IsBall(this Collider2D collider)
    {
        return collider.CompareTag(Tags.Ball);
    }

    public static bool IsBall(this Collision2D collision)
    {
        return collision.gameObject.CompareTag(Tags.Ball);
    }

    public static bool IsGhostBall(this Collider2D collider)
    {
        return collider.gameObject.CompareTag(Tags.GhostBall);
    }

    public static bool IsBallOrGhostBall(this Collider2D collider)
    {
        return collider.gameObject.CompareTag(Tags.Ball) || collider.gameObject.CompareTag(Tags.GhostBall);
    }

    public static bool IsBallOrGhostBall(this Collision2D collision)
    {
        return collision.gameObject.CompareTag(Tags.Ball) || collision.gameObject.CompareTag(Tags.GhostBall);
    }

    public static bool AnyKeysUp(IEnumerable<KeyCode> keys)
    {
        return keys.Any(k => Input.GetKeyUp(k));
    }

    public static bool AnyKeysDown(IEnumerable<KeyCode> keys)
    {
        return keys.Any(k => Input.GetKeyDown(k));
    }

    public static bool AnyKeys(IEnumerable<KeyCode> keys)
    {
        return keys.Any(k => Input.GetKey(k));
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> self)
    {
        return self == null || !self.Any();
    }

    // TODO: maybe just hard requirement for these objects to start active in the scene... 
    public static List<GameObject> FindAllWithTagIncludingInactive(string tag)
    {
        var results = new List<GameObject>();
        var all = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var go in all)
        {
            if (go.CompareTag(tag) && go.hideFlags == HideFlags.None)
            {
                results.Add(go);
            }
        }
        return results;
    }

    public static GameObject FindWithTagIncludingInactive(string tag)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var go in all)
        {
            if (go.CompareTag(tag) && go.hideFlags == HideFlags.None)
            {
                return go;
            }
        }
        return null;
    }
}