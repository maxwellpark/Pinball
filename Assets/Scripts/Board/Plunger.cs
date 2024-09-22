using UnityEngine;
using UnityEngine.UI;

public class Plunger : MonoBehaviour
{
    [SerializeField] private float maxForce = 1000f;
    [SerializeField] private float chargeSpeed = 100f;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private float inactiveTime = 2f;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform launchPosition;

    private float currentForce;
    private float lastChargeTime;

    private void Start()
    {
        chargeSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateChargeSlider();

        if (GameManager.MinigameActive)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Space) && currentForce < maxForce)
        {
            lastChargeTime = Time.time;
            chargeSlider.gameObject.SetActive(true);
            currentForce += chargeSpeed * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            LaunchBall();
        }
    }

    private void LaunchBall()
    {
        var ball = Instantiate(ballPrefab, launchPosition.position, Quaternion.identity);
        var ballRb = ball.GetComponent<Rigidbody2D>();

        var launchDirection = (Vector2)transform.up;

        if (ballRb != null)
        {
            ballRb.AddForce(launchDirection * currentForce, ForceMode2D.Impulse);
        }

        currentForce = 0;
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
