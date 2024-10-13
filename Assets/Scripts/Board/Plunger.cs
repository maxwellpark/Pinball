using Events;
using UnityEngine;
using UnityEngine.UI;
using GM = GameManager;

public class Plunger : MonoBehaviour
{
    [SerializeField] private float maxForce = 1000f;
    [SerializeField] private float chargeSpeed = 100f;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private float inactiveTime = 2f;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform launchPosition;

    private Rigidbody2D ballRb;
    private float currentForce;
    private float lastChargeTime;

    private void Start()
    {
        chargeSlider.gameObject.SetActive(false);
        GM.EventService.Add<NewBallEvent>(OnNewBall);
    }

    private void Update()
    {
        UpdateChargeSlider();

        if (ballRb == null || GM.MinigameActive)
        {
            return;
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0)) && currentForce < maxForce)
        {
            lastChargeTime = Time.time;
            chargeSlider.gameObject.SetActive(true);
            currentForce += chargeSpeed * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0))
        {
            LaunchBall();
        }
    }

    private void OnNewBall()
    {
        var ball = GM.Instance.CreateBall(launchPosition.position);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        currentForce = 0;
        Debug.Log("[plunger] new ball added");
    }

    private void LaunchBall()
    {
        var launchDirection = (Vector2)transform.up;
        ballRb.isKinematic = false;
        ballRb.AddForce(launchDirection * currentForce, ForceMode2D.Impulse);
        ballRb = null;
        Debug.Log("[plunger] ball launched");
    }

    private void UpdateChargeSlider()
    {
        if (Time.time - lastChargeTime > inactiveTime)
        {
            chargeSlider.gameObject.SetActive(false);
        }

        chargeSlider.value = currentForce / maxForce;
    }
}
