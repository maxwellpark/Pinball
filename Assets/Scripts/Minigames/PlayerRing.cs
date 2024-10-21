using UnityEngine;
using GM = GameManager;

public class PlayerRing : MonoBehaviour
{
    [SerializeField] private TargetRing targetRing;
    [SerializeField] private float maxRadius = 5.0f;
    [SerializeField] private bool use2KeyInput;

    [Header("1 key input")]
    [SerializeField] private float expansionSpeed = 2f;
    [SerializeField] private float contractionSpeed = 2f;

    [Header("2 key input")]
    [SerializeField] private float baseSpeed = 2.0f;

    private Vector3 initialScale;

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (GM.CurrentMinigame != Minigame.Type.Ring)
        {
            return;
        }

        if (use2KeyInput)
        {
            var expansionInput = GetExpansionInput();

            if (expansionInput != 0)
            {
                AdjustSize(expansionInput);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Expand();
            }
            else
            {
                Contract();
            }
        }
    }

    // Used by 1 key input 
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

    // Used by 2 key input 
    private float GetExpansionInput()
    {
        // Expansion 
        if (Input.GetKey(KeyCode.Space))
        {
            return 1f;
        }

        // Contraction 
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return -1f;
        }
        return 0f;
    }

    private void AdjustSize(float expansionInput)
    {
        var expansionSpeed = baseSpeed * expansionInput * Time.deltaTime;
        var newScale = transform.localScale + expansionSpeed * Vector3.one;

        newScale.x = Mathf.Clamp(newScale.x, initialScale.x, maxRadius);
        newScale.y = Mathf.Clamp(newScale.y, initialScale.y, maxRadius);

        transform.localScale = newScale;
    }
}
