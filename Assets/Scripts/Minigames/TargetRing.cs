using UnityEngine;
using GM = GameManager;

public class TargetRing : MonoBehaviour
{
    [SerializeField] private float minRadius = 1f;
    [SerializeField] private float maxRadius = 5f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float minChangeInterval = 1f;
    [SerializeField] private float maxChangeInterval = 3f;

    private Vector3 targetScale;
    private float timer;
    private float currentChangeInterval;

    private void Start()
    {
        SetRandomTargetScale();
    }

    private void Update()
    {
        if (GM.CurrentMinigame != Minigame.Type.Ring)
        {
            return;
        }

        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= currentChangeInterval)
        {
            SetRandomTargetScale();
            timer = 0;
        }
    }

    private void SetRandomTargetScale()
    {
        var randomSize = Random.Range(minRadius, maxRadius);
        targetScale = new Vector3(randomSize, randomSize, 1);
        currentChangeInterval = Random.Range(minChangeInterval, maxChangeInterval);
    }
}
