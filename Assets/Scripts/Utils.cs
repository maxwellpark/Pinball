using UnityEngine;

public static class Utils
{
    public static bool IsBall(this Collider2D collider)
    {
        return collider.CompareTag(Tags.Ball);
    }

    public static bool IsBall(this Collision2D collision)
    {
        return collision.gameObject.CompareTag(Tags.Ball);
    }

    public static bool IsBallOrGhostBall(this Collider2D collider)
    {
        return collider.gameObject.CompareTag(Tags.Ball) || collider.gameObject.CompareTag(Tags.GhostBall);
    }

    public static bool IsBallOrGhostBall(this Collision2D collision)
    {
        return collision.gameObject.CompareTag(Tags.Ball) || collision.gameObject.CompareTag(Tags.GhostBall);
    }
}