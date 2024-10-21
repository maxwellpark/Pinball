using UnityEngine;
using GM = GameManager;

public class TargetRing : MonoBehaviour
{
    [SerializeField] private float minRadius = 0.5f;
    [SerializeField] private float maxRadius = 5.0f;
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float changeInterval = 3.0f;

    private Vector3 targetScale;
    private float timer;

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
        if (timer >= changeInterval)
        {
            SetRandomTargetScale();
            timer = 0;
        }
    }

    private void SetRandomTargetScale()
    {
        var randomSize = Random.Range(minRadius, maxRadius);
        targetScale = new Vector3(randomSize, randomSize, 1);
    }
}
