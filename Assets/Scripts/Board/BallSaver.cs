using System.Collections;
using UnityEngine;

public class BallSaver : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private float travelDuration = 10f;
    [SerializeField] private float travelSpeed = 5f;

    private bool isSaving;

    public bool IsActive { get; private set; }

    private void Start()
    {
        if (TryGetComponent<BoxColliderDrawer>(out var drawer))
        {
            drawer.SetIsDrawing(() => IsActive);
        }
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        Debug.Log("[ball saver] activating...");
        IsActive = true;
    }

    private IEnumerator SaveBall(GameObject ball)
    {
        isSaving = true;
        var ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.velocity = Vector2.zero;
        ballRb.isKinematic = true;

        while (ball != null && Vector2.Distance(ball.transform.position, destination.position) > 0.05f)
        {
            ball.transform.position = Vector2.MoveTowards(ball.transform.position, destination.position, travelSpeed * Time.deltaTime);
            yield return null;
        }

        ball.transform.position = destination.position;
        ballRb.velocity = Vector2.zero;
        ballRb.isKinematic = false;

        isSaving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision) || !IsActive || isSaving)
        {
            return;
        }

        Debug.Log("[ball saver] saving ball...");
        StartCoroutine(SaveBall(collision.gameObject));
        IsActive = false;
    }
}
