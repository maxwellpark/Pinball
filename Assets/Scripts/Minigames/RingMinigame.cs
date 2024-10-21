using UnityEngine;

public class RingMinigame : Minigame
{
    protected override Type MinigameType => Type.Ring;

    [SerializeField] private float maxRadius = 5.0f;
    [SerializeField] private float expansionSpeed = 2.0f;
    [SerializeField] private float contractionSpeed = 2.0f;

    private Vector3 initialScale;
    private bool isExpanding;

    protected override void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        isExpanding = Input.GetKey(KeyCode.Space);

        if (isExpanding)
        {
            Expand();
        }
        else
        {
            Contract();
        }
    }

    private void Expand()
    {
        if (transform.localScale.x < maxRadius)
        {
            transform.localScale += expansionSpeed * Time.deltaTime * Vector3.one;
        }
    }

    private void Contract()
    {
        if (transform.localScale.x > initialScale.x)
        {
            transform.localScale -= contractionSpeed * Time.deltaTime * Vector3.one;
        }
    }
}
