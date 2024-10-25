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
    [SerializeField] private GameObject ghostBallPrefab;
    [SerializeField] private BallRescue ballRescue;
    [SerializeField] private BallSaver ballSaver;
    [SerializeField] private float sidewaysForce = 2f;
    [SerializeField] private float upwardsForce = 5f;
    [SerializeField] private int startingBalls = 3;
    [SerializeField] private int startingGhostBalls = 3;
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera ballCamera;
    [SerializeField] private Camera minigameCamera;
    // TODO: separate script eventually 
    [Header("UI")]
    [SerializeField] private GameObject scoreTextContainer;
    [SerializeField] private GameObject highScoreTextContainer;
    [SerializeField] private GameObject comboTextContainer;
    [SerializeField] private GameObject ballsTextContainer;
    [SerializeField] private GameObject ghostBallsTextContainer;
    [SerializeField] private GameObject velocityTextContainer;
    [Header("Explosion")]
    [SerializeField] private float explosionDamage = 100f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDuration = 2f;
    [SerializeField] private float explosionIntensity = 0.5f;
    [SerializeField] private float explosionSpeed = 50f;
    [Header("Combo")]
    [SerializeField] private float comboDecayTime = 5f;
    [SerializeField] private int baseCombo = 1000;
    [SerializeField] private float multiplierIncrement = 0.5f;
    [Header("SO's")]
    [SerializeField] private ScoreThresholds scoreThresholds;

    private readonly List<ScoreThreshold> unreachedThresholds = new();

    private TMP_Text scoreText;
    private TMP_Text highScoreText;
    private TMP_Text comboText;
    private TMP_Text ballsText;
    private TMP_Text ghostBallsText;
    private TMP_Text velocityText;

    private GameObject ball;
    private bool isBallProtected;
    private Vector3 explosionPos;
    private bool showExplosion;
    private bool showControls = true;

    public static bool IsBallAlive => Instance.ball != null;
    public static Minigame.Type CurrentMinigame { get; private set; }
    public static bool MinigameActive => CurrentMinigame != Minigame.Type.None;

    private float comboMultiplier = 1f;
    public float ComboMultiplier
    {
        get => comboMultiplier;
        private set
        {
            comboMultiplier = value;
            comboText.SetText("Combo *: " + comboMultiplier);
        }
    }
    private float comboDeltaTime;
    private int nextCombo;
    private int multiplierLevel;

    private int score;
    private int highScore;
    public int Score
    {
        get => score;
        private set
        {
            score = value;
            scoreText.SetText("Score: " + score);

            comboDeltaTime = 0;

            if (score > nextCombo)
            {
                multiplierLevel++;
                ComboMultiplier += multiplierIncrement;
                nextCombo = Mathf.RoundToInt(baseCombo * Mathf.Pow(2, multiplierLevel));
                Debug.Log($"Combo multiplier increased to {ComboMultiplier} | next combo at {nextCombo}");
            }

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
                    reachedThresholds.Add(threshold);

                    if (threshold.Action != Action.None)
                    {
                        Debug.Log("Triggering action " + threshold.Action);
                        TriggerAction(threshold.Action);
                    }
                }
            }

            foreach (var threshold in reachedThresholds)
            {
                unreachedThresholds.Remove(threshold);
            }
        }
    }

    public static void TriggerAction(Action action)
    {
        switch (action)
        {
            case Action.BallRescue:
                Instance.ballRescue.Activate();
                break;
            case Action.BallSaver:
                Instance.ballSaver.Activate();
                break;
            case Action.AddGhostBall:
                // TODO: probably want an abstraction for action notifications in general 
                NotificationManager.Notify("Ghost ball added!", 1);
                Instance.GhostBalls++;
                break;
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

    private int ghostBalls;
    public int GhostBalls
    {
        get => ghostBalls;
        private set
        {
            ghostBalls = value;
            ghostBallsText.SetText("Ghost balls: " + ghostBalls);
        }
    }

    public static void AddScore(int score)
    {
        Instance.Score += Mathf.RoundToInt(Mathf.Max(score * Instance.ComboMultiplier, 0));
    }

    public enum Action
    {
        None, BallRescue, BallSaver, AddGhostBall,
    }

    private void Start()
    {
        scoreText = scoreTextContainer.GetComponentInChildren<TMP_Text>();
        scoreTextContainer.SetActive(true);
        Score = 0;
        highScoreText = highScoreTextContainer.GetComponentInChildren<TMP_Text>();
        highScoreTextContainer.SetActive(highScore > 0);
        comboText = comboTextContainer.GetComponentInChildren<TMP_Text>();
        ballsText = ballsTextContainer.GetComponentInChildren<TMP_Text>();
        ghostBallsText = ghostBallsTextContainer.GetComponentInChildren<TMP_Text>();
        velocityText = velocityTextContainer.GetComponentInChildren<TMP_Text>();
        Balls = startingBalls;
        GhostBalls = startingGhostBalls;

        ball = GameObject.FindWithTag(Tags.Ball);
        minigameCamera.gameObject.SetActive(false);
        EventService.Add<MinigameEndedEvent>(EndMinigame);
        EventService.Add<BallSavedEvent>(OnBallSaved);

        unreachedThresholds.AddRange(scoreThresholds.Thresholds);
        EventService.Dispatch<NewBallEvent>();
    }

    private void Update()
    {
        // Hack for PS4/Xbox 
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton8))
        {
            DestroyBalls();
            NewBall();
        }

        if (Input.GetMouseButtonDown(0) && !MinigameActive)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ball = GameObject.FindWithTag(Tags.Ball);

            if (ball == null)
            {
                CreateBall(mousePos);
            }

            SetBallPos(mousePos);

            // In case it was in the plunger 
            ball.GetComponent<Rigidbody2D>().isKinematic = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showControls = !showControls;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ballRescue.Activate();
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            Balls++;
        }

        comboDeltaTime += Time.deltaTime;
        if (comboDeltaTime >= comboDecayTime)
        {
            // Reset combo if decayed
            ComboMultiplier = 1f;
            multiplierLevel = 0;
            nextCombo = baseCombo;
            comboDeltaTime = 0f;
            Debug.Log("Combo reset");
        }

        if (ball == null || MinigameActive)
        {
            velocityText.SetText(Vector2.zero.ToString());
            return;
        }

        if ((Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.JoystickButton2)) && ghostBalls > 0)
        {
            Instantiate(ghostBallPrefab, ball.transform.position, Quaternion.identity);
            GhostBalls--;
        }

        var ballRb = ball.GetComponent<Rigidbody2D>();

        // 4 = left bumper
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            ballRb.AddForce(Vector3.left * sidewaysForce, ForceMode2D.Impulse);
        }
        // 5 = right bumper
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            ballRb.AddForce(Vector3.right * sidewaysForce, ForceMode2D.Impulse);
        }
        // Just for testing 
        else if (Input.GetKey(KeyCode.W))
        {
            ballRb.AddForce(Vector2.up * upwardsForce, ForceMode2D.Impulse);
        }

        velocityText.SetText("Velocity: " + ballRb.velocity);

        // 3 = triangle 
        if (!showExplosion && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton3)))
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
                    db.StartDamageOverTime(explosionDamage, explosionDuration);
                }
            }
        }

        yield return new WaitForSeconds(explosionDuration);
        showExplosion = false;
        explosionPos = Vector3.zero;
    }

    public GameObject CreateBall(Vector3 pos)
    {
        if (ball != null || Balls < 1)
        {
            return null;
        }

        var instance = Instantiate(ballPrefab, pos, Quaternion.identity);
        ball = instance;
        ballCamera.Follow = instance.transform;
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

    public void StartMinigame(Minigame.Type type, UnityAction onEnd = null)
    {
        if (type == Minigame.Type.None)
        {
            Debug.LogWarning("No minigame to start (type was None)");
            return;
        }

        if (type == CurrentMinigame)
        {
            Debug.LogWarning("Minigame already active: " + type);
            return;
        }

        NotificationManager.Notify("Starting minigame...", 0.5f);
        minigameCamera.gameObject.SetActive(true);
        CurrentMinigame = type;
        scoreTextContainer.SetActive(false);
        highScoreTextContainer.SetActive(false);
        EventService.Dispatch(new MinigameStartedEvent(type, onEnd));
    }

    private void EndMinigame()
    {
        Debug.Log("Ending minigame...");
        minigameCamera.gameObject.SetActive(false);
        scoreTextContainer.SetActive(true);
        highScoreTextContainer.SetActive(highScore > 0);
        CurrentMinigame = Minigame.Type.None;
    }

    private void OnBallSaved()
    {
        isBallProtected = true;
    }

    // For now reference this from Floor in inspector 
    public void LoseBall()
    {
        if (isBallProtected)
        {
            NotificationManager.Notify("Ball saved!");
            isBallProtected = false;
            Destroy(ball);
            Invoke(nameof(NewBall), 0.1f);
            return;
        }

        Balls--;
        NotificationManager.Notify("Lost ball... Remaining: " + Balls);
        DestroyBalls();

        if (Balls == 0)
        {
            NotificationManager.Notify("Game over... Score: " + Score);
            Score = 0;
            Balls = startingBalls;
        }

        EventService.Dispatch<NewBallEvent>();
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

    private void DestroyBalls()
    {
        var balls = GameObject.FindGameObjectsWithTag(Tags.Ball);
        foreach (var ball in balls)
        {
            Destroy(ball);
        }
        ball = null;
    }

    // Just to call Invoke()
    private void NewBall()
    {
        EventService.Dispatch<NewBallEvent>();
    }
}

public static class Tags
{
    public static readonly string Ball = "Ball";
    public static readonly string GhostBall = "GhostBall";
    public static readonly string Catcher = "Catcher";
    public static readonly string Ground = "Ground";
    public static readonly string Flipper = "Flipper";
    public static readonly string Gap = "Gap";
    public static readonly string Finish = "Finish";
}
