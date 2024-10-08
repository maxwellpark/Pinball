using System.Collections;
using UnityEngine;

public class Valve : MonoBehaviour
{
    [SerializeField] private float closeDelay = 1f;
    [SerializeField] private string animationName;
    [SerializeField] private Collider2D exitCollider;

    [SerializeField] private Color openColor = new(1, 1, 1, 0.25f);
    [SerializeField] private Color closedColor = new(1, 1, 1, 0.5f);

    //private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isOpen;

    private void Start()
    {
        //animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = openColor;
        exitCollider.isTrigger = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision) || isOpen)
        {
            return;
        }

        Debug.Log("[valve] opening");
        // TODO: animation 
        //animator.SetTrigger(animationName);
        isOpen = true;
        spriteRenderer.color = openColor;
        exitCollider.isTrigger = true;

        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        exitCollider.isTrigger = false;
        isOpen = false;
        spriteRenderer.color = closedColor;
        Debug.Log("[valve] closed");
    }
}
