using Cinemachine;
using Events;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GM = GameManager;

public class Plunger : MonoBehaviour
{
    [SerializeField] private float maxForce = 1000f;
    [SerializeField] private float chargeSpeed = 100f;
    [SerializeField] private float inactiveTime = 2f;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform launchPosition;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [Header("Sound")]
    [SerializeField] private AudioClip chargeSound;
    [SerializeField] private AudioClip launchSound;

    private Rigidbody2D ballRb;
    private AudioSource audioSource;
    private float currentForce;
    private float lastChargeTime;
    private bool isCharging;
    private bool isActive;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        GM.EventService.Add<ActivePlungerChangedEvent>(OnActivePlungerChanged);
        GM.EventService.Add<NewBallEvent>(OnNewBall);
        GM.EventService.Add<BallStuckEvent>(OnBallStuck);
        GM.EventService.Add<BoardChangedEvent>(OnBoardChanged);
    }

    private void Start()
    {
        var slider = UIManager.Instance.ChargeSlider;
        if (slider != null)
        {
            slider.gameObject.SetActive(false);
        }
    }

    private void OnBoardChanged(BoardChangedEvent evt)
    {
        if (audioSource != null)
        {
            if (evt.Config.PlungerChargeSound != null)
            {
                chargeSound = evt.Config.PlungerChargeSound;
            }

            if (evt.Config.PlungerLaunchSound != null)
            {
                launchSound = evt.Config.PlungerLaunchSound;
            }
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        var slider = UIManager.Instance.ChargeSlider;
        UpdateChargeSlider(slider);

        if (ballRb == null || GM.MinigameActive)
        {
            return;
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0)) && currentForce < maxForce)
        {
            lastChargeTime = Time.time;
            if (slider != null)
            {
                slider.gameObject.SetActive(true);
            }
            currentForce += chargeSpeed * Time.deltaTime;

            if (!isCharging)
            {
                StartCharging();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0))
        {
            LaunchBall();
            StopCharging();
        }

        if (isCharging)
        {
            // Vol proportional to charge progress 
            audioSource.volume = Mathf.Clamp01(currentForce / maxForce);
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

        if (audioSource != null && launchSound != null)
        {
            // TODO: separate audio source? 
            audioSource.PlayOneShot(launchSound);
        }

        Debug.Log("[plunger] ball launched");
    }

    private void UpdateChargeSlider(Slider slider)
    {
        if (slider == null)
        {
            return;
        }

        if (Time.time - lastChargeTime > inactiveTime)
        {
            slider.gameObject.SetActive(false);
        }

        slider.value = currentForce / maxForce;
    }

    private void OnActivePlungerChanged(ActivePlungerChangedEvent evt)
    {
        isActive = evt.Plunger == this;

        if (isActive && GM.IsBallAlive)
        {
            Debug.Log("[plunger] OnActivePlungerChanged to " + name);
            ballRb = GM.BallRb;
            GM.Instance.SetBallPos(launchPosition.position);

            // Keeping camera optional for now in case of boards where there's only 1 playfield
            if (vcam != null)
            {
                Debug.Log($"[plunger] vcam {vcam.name} following");
                vcam.Follow = ballRb.transform;
                CameraManager.SetPriority(vcam);
            }
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

    private void StartCharging()
    {
        if (isCharging)
        {
            return;
        }

        isCharging = true;
        Debug.Log("[plunger] start charging");

        if (audioSource != null)
        {
            audioSource.clip = chargeSound;
            audioSource.volume = Mathf.Clamp01(currentForce / maxForce);
            audioSource.Play();
        }
    }

    private void StopCharging()
    {
        if (!isCharging)
        {
            return;
        }

        isCharging = false;
        Debug.Log("[plunger] stop charging");

        if (audioSource != null)
        {
            StartCoroutine(FadeOutChargeSound());
        }
    }

    private IEnumerator FadeOutChargeSound()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * 2;
            yield return null;
        }
        audioSource.Stop();
    }

    private void OnDestroy()
    {
        GM.EventService.Remove<ActivePlungerChangedEvent>(OnActivePlungerChanged);
        GM.EventService.Remove<NewBallEvent>(OnNewBall);
        GM.EventService.Remove<BallStuckEvent>(OnBallStuck);
        GM.EventService.Remove<BoardChangedEvent>(OnBoardChanged);
    }
}
