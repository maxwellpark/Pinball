using UnityEngine;
using UnityEngine.Events;

public class ButtonSwitch : MonoBehaviour
{
    [SerializeField] private float returnSpeed = 1f;
    [SerializeField] private Transform switchTop;
    [SerializeField] private UnityEvent onPressed;
    [SerializeField] private GameManager.Action action;

    private SpringJoint2D springJoint;
    private Rigidbody2D rb;
    private bool isPressed;
    private Vector3 startPos;
    //private Coroutine returnRoutine;

    private void Start()
    {
        springJoint = switchTop.GetComponent<SpringJoint2D>();
        startPos = switchTop.localPosition;

        springJoint.connectedAnchor = startPos;
        springJoint.autoConfigureConnectedAnchor = false;
        rb = switchTop.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPressed || !Utils.IsBall(collision))
        {
            return;
        }

        isPressed = true;
        onPressed?.Invoke();
        rb.isKinematic = false;
        GameManager.TriggerAction(action);

        //if (returnRoutine != null)
        //{
        //    StopCoroutine(returnRoutine);
        //    returnRoutine = null;
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isPressed || !Utils.IsBall(collision))
        {
            return;
        }

        //rb.isKinematic = false;
        //returnRoutine = StartCoroutine(ReturnToStartPosition());
        isPressed = false;
    }

    //private IEnumerator ReturnToStartPosition()
    //{
    //    rb.isKinematic = true;

    //    var currentPos = switchTop.localPosition;
    //    var elapsedTime = 0f;

    //    while (elapsedTime < 1f)
    //    {
    //        elapsedTime += Time.deltaTime * returnSpeed;
    //        switchTop.localPosition = Vector3.Lerp(currentPos, startPos, elapsedTime);
    //        yield return null;
    //    }

    //    switchTop.localPosition = startPos;
    //    returnRoutine = null;
    //}
}
