using UnityEngine;
using GM = GameManager;

public class PlayerRing : MonoBehaviour
{
    [SerializeField] private TargetRing targetRing;
    [SerializeField] private float maxRadius = 5.0f;
    [SerializeField] private float expansionSpeed = 2.0f;
    [SerializeField] private float contractionSpeed = 2.0f;

    private Vector3 initialScale;
    private bool isExpanding;

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

        if (Input.GetKey(KeyCode.Space))
        {
            isExpanding = true;
        }
        else
        {
            isExpanding = false;
        }

        if (isExpanding)
        {
            Expand();
        }
        else
        {
            Contract();
        }

        //var randomRingScale = targetRing.transform.localScale.x;
        //var playerRingScale = transform.localScale.x;

        //if (playerRingScale > randomRingScale)
        //{
        //    Debug.Log("[ring] out of bounds!");
        //    // TODO: win/lose logic 
        //}
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
