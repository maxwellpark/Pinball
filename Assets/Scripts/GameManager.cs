using Cinemachine;
using Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private static readonly EventService eventService = new();
    public static EventService EventService => eventService;

    [Header("Ball")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private BallSaver ballSaver;
    [SerializeField] private float nudgeForce = 2f;
    [SerializeField] private int startingBalls = 3;
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera ballCamera;
    [SerializeField] private Camera minigameCamera;
    [Header("UI")]
    [SerializeField] private GameObject scoreTextContainer;
    [SerializeField] private GameObject highScoreTextContainer;
    [SerializeField] private GameObject ballsTextContainer;
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDuration = 2f;
    [SerializeField] private float explosionIntensity = 0.5f;
    [SerializeField] private float explosionSpeed = 50f;
    [Header("SO's")]
    [SerializeField] private ScoreThresholds scoreThresholds;

    private readonly List<ScoreThreshold> unreachedThresholds = new();

    private TMP_Text scoreText;
    private TMP_Text highScoreText;
    private TMP_Text ballsText;

    private GameObject ball;
    private Vector3 explosionPos;
    private bool showExplosion;
    private bool showControls = true;

    public static bool MinigameActive { get; private set; }

    private int score;
    private int highScore;
    public int Score
    {
        get => score;
        private set
        {
            score = value;
            scoreText.SetText("Score: " + score);

            if (score > highScore)
            {
                highScore = score;
                highScoreText.SetText("High score: " + highScore);
                highScoreTextContainer.SetActive(true);
            }

            var reachedThresholds = new List<ScoreThreshold>();

            foreach (var threshold in unreachedThresholds)
            {
                if (score >= threshold.Score)
                {
                    Debug.Log($"Threshold {threshold.Action} reached at {threshold.Score}");

                    switch (threshold.Action)
                    {
                        case Action.None:
                            break;
                        case Action.BallSaver:
                            ballSaver.Activate();
                            reachedThresholds.Add(threshold);
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (var threshold in reachedThresholds)
            {
                unreachedThresholds.Remove(threshold);
            }
        }
    }

    private int balls;
    public int Balls
    {
        get => balls;
        private set
        {
            balls = value;
            ballsText.SetText("Balls: " + balls);
        }
    }

    public static void AddScore(int score)
    {
        Instance.Score += Mathf.Max(score, 0);
    }

    public enum Action
    {
        None, BallSaver,
    }

    private void Start()
    {
        scoreText = scoreTextContainer.GetComponentInChildren<TMP_Text>();
        scoreTextContainer.SetActive(true);
        Score = 0;
        highScoreText = highScoreTextContainer.GetComponentInChildren<TMP_Text>();
        highScoreTextContainer.SetActive(highScore > 0);
        ballsText = ballsTextContainer.GetComponentInChildren<TMP_Text>();
        Balls = startingBalls;

        ball = GameObject.FindWithTag(Tags.Ball);
        minigameCamera.gameObject.SetActive(false);
        EventService.Add<MinigameEndedEvent>(EndMinigame);

        unreachedThresholds.AddRange(scoreThresholds.Thresholds);
    }

    private void DestroyBalls()
    {
        var balls = GameObject.FindGameObjectsWithTag(Tags.Ball);
        foreach (var ball in balls)
        {
            Destroy(ball);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DestroyBalls();
        }

        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ball = GameObject.FindWithTag(Tags.Ball);

            if (ball == null)
            {
                CreateBall(mousePos);
            }

            SetBallPos(mousePos);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showControls = !showControls;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ballSaver.Activate();
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            Balls++;
        }

        if (ball == null || MinigameActive)
        {
            return;
        }

        var ballRb = ball.GetComponent<Rigidbody2D>();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ballRb.AddForce(Vector3.left * nudgeForce, ForceMode2D.Impulse);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ballRb.AddForce(Vector3.right * nudgeForce, ForceMode2D.Impulse);
        }

        if (!showExplosion && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(TriggerExplosion());
        }
    }

    private IEnumerator TriggerExplosion()
    {
        explosionPos = ball.transform.position;
        showExplosion = true;

        var colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Bumper>(out var bumper))
            {
                bumper.StartVibrate(explosionDuration, explosionIntensity, explosionSpeed);

                if (bumper is DestructibleBumper db)
                {
                    db.StartDamageOverTime(explosionDuration);
                }
            }
        }

        yield return new WaitForSeconds(explosionDuration);
        showExplosion = false;
        explosionPos = Vector3.zero;
    }

    public GameObject CreateBall(Vector3 pos)
    {
        if (Balls < 1)
        {
            return null;
        }

        var instance = Instantiate(ballPrefab, pos, Quaternion.identity);

        if (ball == null)
        {
            ball = instance;
        }
        else
        {
            // Tag balls after the main ball differently
            // TODO: separate prefab 
            instance.tag = Tags.GhostBall;
            instance.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }

        if (ballCamera.Follow == null)
        {
            ballCamera.Follow = instance.transform;
        }

        Balls--;
        return instance;
    }

    private void SetBallPos(Vector3 pos)
    {
        if (ball == null)
        {
            ball = GameObject.FindWithTag(Tags.Ball);
        }

        if (ball == null)
        {
            return;
        }

        ball.transform.position = new Vector3(pos.x, pos.y, 0f);
        var ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        var trail = ball.GetComponent<TrailRenderer>();
        trail.Clear();
    }

    private void OnGUI()
    {
        if (!showControls)
        {
            return;
        }

        var controlRect = new Rect(10, 10, 220, 180);
        var boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.5f));

        GUI.Box(controlRect, GUIContent.none, boxStyle);

        GUI.Label(new Rect(20, 20, 200, 20), "A/D: Left/Right flipper");
        GUI.Label(new Rect(20, 40, 200, 20), "S: Both flippers");
        GUI.Label(new Rect(20, 60, 200, 20), "Q/E: Nudge ball left/right");
        GUI.Label(new Rect(20, 80, 200, 20), "F: AOE explosion");
        GUI.Label(new Rect(20, 100, 200, 20), "Space (hold): Plunger");
        GUI.Label(new Rect(20, 120, 200, 20), "R: Reset balls");
        GUI.Label(new Rect(20, 140, 300, 20), "Left click: Spawn ball at mouse");
        GUI.Label(new Rect(20, 160, 300, 20), "ESC: Toggle controls");
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        var pix = new Color[width * height];
        for (var i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        var result = new Texture2D(width, height);
#pragma warning disable UNT0017 // SetPixels invocation is slow
        result.SetPixels(pix);
#pragma warning restore UNT0017 // SetPixels invocation is slow
        result.Apply();
        return result;
    }

    public void StartMinigame(UnityAction onEnd = null)
    {
        Debug.Log("Starting minigame...");
        minigameCamera.gameObject.SetActive(true);
        MinigameActive = true;
        scoreTextContainer.SetActive(false);
        highScoreTextContainer.SetActive(false);
        EventService.Dispatch(new MinigameStartedEvent(onEnd));
    }

    private void EndMinigame()
    {
        Debug.Log("Ending minigame...");
        MinigameActive = false;
        minigameCamera.gameObject.SetActive(false);
        scoreTextContainer.SetActive(true);
        highScoreTextContainer.SetActive(highScore > 0);
    }

    // For now reference this from Floor in inspector 
    public void LoseBall()
    {
        Debug.Log("Lost ball... Remaining: " + Balls);
        DestroyBalls();

        if (Balls == 0)
        {
            Debug.Log("Game over... Score: " + Score);
            Score = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showExplosion)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(explosionPos, explosionRadius);
    }
}

public static class Tags
{
    public static readonly string Ball = "Ball";
    public static readonly string GhostBall = "GhostBall";
    public static readonly string Catcher = "Catcher";
    public static readonly string Ground = "Ground";
}
