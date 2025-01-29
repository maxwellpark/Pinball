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
    private bool isActive;

    private void Awake()
    {
        chargeSlider.gameObject.SetActive(false);
        GM.EventService.Add<ActivePlungerChangedEvent>(OnActivePlungerChanged);
        GM.EventService.Add<NewBallEvent>(OnNewBall);
        GM.EventService.Add<BallStuckEvent>(OnBallStuck);
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

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
        if (!isActive)
        {
            return;
        }

        var ball = GM.Instance.CreateBall(launchPosition.position);

        // TODO: this probably shouldn't occur - it's when we're out of balls,
        // but really we shouldn't be raising a NewBallEvent in this scenario. 
        if (ball == null)
        {
            Debug.LogWarning("[plunger] CreateBall returned null. Cannot launch.");
            return;
        }

        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        currentForce = 0;
        Debug.Log("[plunger] new ball added");
    }

    private void OnBallStuck()
    {
        var ball = GM.Ball;
        if (ball == null)
        {
            Debug.LogWarning("[plunger] ball stuck listener fired but ball is not alive");
            return;
        }

        Debug.Log("[plunger] ball stuck - sending ball to plunger");
        ball.transform.position = launchPosition.position;
        // TODO: reuse the Freeze/Unfreeze methods on the Ball now, but make sure they're compatible with what we do here 
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        currentForce = 0;
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

    private void OnActivePlungerChanged(ActivePlungerChangedEvent evt)
    {
        isActive = evt.Plunger == this;

        if (isActive && GM.IsBallAlive)
        {
            ballRb = GM.BallRb;
            GM.Instance.SetBallPos(launchPosition.position);
        }
    }

    public void Activate()
    {
        Debug.Log($"[plunger] {name} becoming active");
        isActive = true;
    }

    public void Deactivate()
    {
        //Debug.Log($"[plunger] {name} becoming inactive");
        isActive = false;
    }
}
