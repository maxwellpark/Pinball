using Cinemachine;
using Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// For prototyping this is a massive class but will be split up later 
public class GameManager : Singleton<GameManager>
{
    private static readonly EventService eventService = new();
    public static EventService EventService => eventService;

    [Header("Ball")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject ghostBallPrefab;
    [SerializeField] private GameObject shooterPrefab;
    [SerializeField] private float sidewaysForce = 2f;
    [SerializeField] private float upwardsForce = 5f;
    [SerializeField] private int startingBalls = 3;
    [SerializeField] private int startingGhostBalls = 3;
    [SerializeField] private int startingBombs = 3;
    [SerializeField] private int startingShooters = 3;
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera ballCamera;
    // TODO: separate script eventually 
    [Header("UI")]
    [SerializeField] private GameObject scoreTextContainer;
    [SerializeField] private GameObject highScoreTextContainer;
    [SerializeField] private GameObject comboTextContainer;
    [SerializeField] private GameObject ballsTextContainer;
    [SerializeField] private GameObject ghostBallsTextContainer;
    [SerializeField] private GameObject bombsTextContainer;
    [SerializeField] private GameObject shootersTextContainer;
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
    [SerializeField] private BoardConfigs boardConfigs;
    [SerializeField] private ScoreThresholds scoreThresholds;

    private readonly List<ScoreThreshold> unreachedThresholds = new();

    private TMP_Text scoreText;
    private TMP_Text highScoreText;
    private TMP_Text comboText;
    private TMP_Text ballsText;
    private TMP_Text ghostBallsText;
    private TMP_Text bombsText;
    private TMP_Text shootersText;
    private TMP_Text velocityText;

    private GameObject ball;
    private BallRescue ballRescue;
    private BallSaver ballSaver;
    private Camera minigameCamera;
    private bool isBallProtected;
    private Vector3 explosionPos;
    private bool showExplosion;
    private bool isShooterActive;
    private bool showControls = true;

    public static bool IsBallAlive => Instance.ball != null;
    public static Vector3 BallPos => IsBallAlive ? Instance.ball.transform.localPosition : Vector3.zero;

    // Probably shouldn't be exposing these and just listen to events but too lazy for now 
    public static Rigidbody2D BallRb => IsBallAlive ? Instance.ball.GetComponent<Rigidbody2D>() : null;
    public static Ball Ball => IsBallAlive ? Instance.ball.GetComponent<Ball>() : null;

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
            case Action.AddBomb:
                NotificationManager.Notify("Bomb added!", 1);
                Instance.Bombs++;
                break;
            case Action.AddShooter:
                NotificationManager.Notify("Shooter added!", 1);
                Instance.Shooters++;
                break;
        }
    }

    // TODO: some abstraction for resouce-driven entities & UI  
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

    private int bombs;
    public int Bombs
    {
        get => bombs;
        private set
        {
            bombs = value;
            bombsText.SetText("Bombs: " + bombs);
        }
    }

    private int shooters;
    public int Shooters
    {
        get => shooters;
        private set
        {
            shooters = value;
            shootersText.SetText("Shooters: " + shooters);
        }
    }

    public static void AddScore(int score)
    {
        Instance.Score += Mathf.RoundToInt(Mathf.Max(score * Instance.ComboMultiplier, 0));
    }

    public enum Action
    {
        None, BallRescue, BallSaver, AddGhostBall, AddBomb, AddShooter,
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
        bombsText = bombsTextContainer.GetComponentInChildren<TMP_Text>();
        shootersText = shootersTextContainer.GetComponentInChildren<TMP_Text>();
        velocityText = velocityTextContainer.GetComponentInChildren<TMP_Text>();
        Balls = startingBalls;
        GhostBalls = startingGhostBalls;
        Bombs = startingBombs;
        Shooters = startingShooters;
        ballRescue = FindObjectOfType<BallRescue>();
        ballSaver = FindObjectOfType<BallSaver>();
        ball = GameObject.FindWithTag(Tags.Ball);
        UpdateMinigameCamera();

        EventService.Add<MinigameEndedEvent>(EndMinigame);
        EventService.Add<BallSavedEvent>(OnBallSaved);
        EventService.Add<BallChargedEvent>(OnBallCharged);
        EventService.Add<BallDischargedEvent>(OnBallDischarged);
        EventService.Add<ShooterCreatedEvent>(() => isShooterActive = true);
        EventService.Add<ShooterDestroyedEvent>(() => isShooterActive = false);
        EventService.Add<CamerasUpdatedEvent>(OnCamerasUpdated);

        unreachedThresholds.AddRange(scoreThresholds.Thresholds);
        Debug.Log("[game] first new ball event");
        EventService.Dispatch<NewBallEvent>();

        Debug.Log("[game] board change on init");
        ChangeBoard(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene prev, Scene next)
    {
        Debug.Log($"[game] scene changed from {prev.name} to {next.name}");
        // TODO: might want to decouple scene names from board names later 
        ChangeBoard(next.name);

        ballRescue = FindObjectOfType<BallRescue>();
        ballSaver = FindObjectOfType<BallSaver>();
        UpdateMinigameCamera();
        ResetBall();
    }

    private void UpdateMinigameCamera()
    {
        var minigameCam = Utils.FindWithTagIncludingInactive(Tags.MinigameCamera);
        if (minigameCam == null)
        {
            Debug.LogWarning("[game] no minigame camera found");
        }
        else
        {
            minigameCamera = minigameCam.GetComponent<Camera>();
            minigameCamera.gameObject.SetActive(false);
        }
    }

    private void OnCamerasUpdated()
    {
        ballCamera = CameraManager.GetPriorityCamera();
    }

    private void ChangeBoard(string name)
    {
        Debug.Log("[game] changing board to " + name);
        if (boardConfigs.TryGetBoard(name, out var config))
        {
            EventService.Dispatch(new BoardChangedEvent(config));
        }
        else
        {
            Debug.LogWarning("[game] no board config found for board with name " + name);
        }
    }

    private void ResetBall()
    {
        DestroyBalls();
        NewBall();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton8))
        {
            ResetBall();
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
            //Debug.Log("Combo reset");
        }

        if (ball == null)
        {
            velocityText.SetText(Vector2.zero.ToString());
            return;
        }

        // Just for testing 
        if (MinigameActive && Input.GetKeyDown(KeyCode.Escape))
        {
            EventService.Dispatch<MinigameCancelledEvent>();
        }

        // TODO: use the latest helper functions in InputManager for controller buttons 
        if ((Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.JoystickButton2)) && ghostBalls > 0)
        {
            CreateGhostBall();
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
        if (!showExplosion && Bombs > 0 && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton3)))
        {
            StartCoroutine(TriggerExplosion());
        }

        if (!isShooterActive && Shooters > 0 && Input.GetKeyDown(KeyCode.B))
        {
            ball.GetComponent<Ball>().Freeze();
            Instantiate(shooterPrefab, ball.transform.position, Quaternion.identity);
        }
    }

    public static GameObject CreateGhostBall()
    {
        // Shouldn't happen but leave for now 
        if (Instance.ball == null)
        {
            return null;
        }
        return Instantiate(Instance.ghostBallPrefab, Instance.ball.transform.position, Quaternion.identity);
    }

    private IEnumerator TriggerExplosion()
    {
        if (Bombs < 1)
        {
            Debug.LogWarning("[game] tried to trigger explosion with 0 bombs");
            yield break;
        }

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

        Bombs--;
        if (Ball != null)
        {
            Ball.PlayBombSound();
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

        // TODO: something in the order is messed up with CreateBall vs. updating cam refs 
        if (ballCamera == null)
        {
            ballCamera = CameraManager.GetPriorityCamera();
        }
        ballCamera.Follow = instance.transform;
        return instance;
    }

    // Probably shouldn't be exposing this and just listen to events but too lazy for now 
    public void SetBallPos(Vector3 pos)
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
        ballRb.gravityScale = 1f;

        var trail = ball.GetComponent<TrailRenderer>();
        trail.Clear();
    }

    private void OnGUI()
    {
        if (!showControls)
        {
            return;
        }

        var controlRect = new Rect(10, 10, 220, 240);
        var boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.5f));

        GUI.Box(controlRect, GUIContent.none, boxStyle);

        GUI.Label(new Rect(20, 20, 200, 20), "A/D: Left/Right flipper");
        GUI.Label(new Rect(20, 40, 200, 20), "S: Both flippers");
        GUI.Label(new Rect(20, 60, 200, 20), "Q/E: Nudge ball left/right");
        GUI.Label(new Rect(20, 80, 200, 20), "F: AOE explosion");
        GUI.Label(new Rect(20, 100, 200, 20), "B: Shooter");
        GUI.Label(new Rect(20, 120, 200, 20), "Space (hold): Plunger");
        GUI.Label(new Rect(20, 140, 200, 20), "R: Reset balls");
        GUI.Label(new Rect(20, 160, 300, 20), "G: Spawn ghost ball");
        GUI.Label(new Rect(20, 180, 300, 20), "Left click: Spawn ball at mouse");
        GUI.Label(new Rect(20, 200, 300, 20), "ESC: Toggle controls");
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

    public void StartMinigame(Minigame.Type type, UnityAction onStart = null, UnityAction onEnd = null)
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
        velocityTextContainer.SetActive(false);
        EventService.Dispatch(new MinigameStartedEvent(type, onEnd));
        onStart?.Invoke();
    }

    private void EndMinigame()
    {
        Debug.Log("Ending minigame...");
        minigameCamera.gameObject.SetActive(false);
        scoreTextContainer.SetActive(true);
        velocityTextContainer.SetActive(true);
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

    private void OnBallCharged()
    {
        if (ball == null)
        {
            return;
        }
        ball.GetComponent<Ball>().Charge();
    }

    private void OnBallDischarged()
    {
        if (ball == null)
        {
            return;
        }
        ball.GetComponent<Ball>().Discharge();
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
    public static readonly string MinigameCamera = "MinigameCamera";
    public static readonly string TargetRingInner = "TargetRingInner";
    public static readonly string TargetRingOuter = "TargetRingOuter";
    public static readonly string PlayerRingInner = "PlayerRingInner";
    public static readonly string PlayerRingOuter = "PlayerRingOuter";
}
