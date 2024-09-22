using Events;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Camera minigameCamera;
    [SerializeField] private TMP_Text scoreText;

    private static readonly EventService eventService = new();
    public static EventService EventService => eventService;

    private GameObject ball;
    private bool showControls = true;

    public static bool MinigameActive { get; private set; }

    private int score;
    public int Score
    {
        get => score;
        private set
        {
            score = value;
            scoreText.SetText("Score: " + score);
        }
    }

    public static void AddScore(int score)
    {
        Instance.Score += Mathf.Max(score, 0);
    }

    private void Start()
    {
        score = 0;
        ball = GameObject.FindWithTag(Tags.Ball);
        minigameCamera.gameObject.SetActive(false);
        EventService.Add<MinigameEndedEvent>(EndMinigame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            var balls = GameObject.FindGameObjectsWithTag(Tags.Ball);
            foreach (var ball in balls)
            {
                Destroy(ball);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ball = GameObject.FindWithTag(Tags.Ball);

            if (ball == null)
            {
                Instantiate(ballPrefab, mousePos, Quaternion.identity);
            }

            SetBallPos(mousePos);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showControls = !showControls;
        }

        if (!MinigameActive && Input.GetKeyDown(KeyCode.M))
        {
            StartMinigame();
        }
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

        var controlRect = new Rect(10, 10, 220, 140);
        var boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.5f));

        GUI.Box(controlRect, GUIContent.none, boxStyle);

        GUI.Label(new Rect(20, 20, 200, 20), "A/D: Left/Right flipper");
        GUI.Label(new Rect(20, 40, 200, 20), "S: Both flippers");
        GUI.Label(new Rect(20, 60, 200, 20), "Space (hold): Plunger");
        GUI.Label(new Rect(20, 80, 200, 20), "R: Reset balls");
        GUI.Label(new Rect(20, 100, 300, 20), "Left click: Spawn ball at mouse");
        GUI.Label(new Rect(20, 120, 300, 20), "ESC: Toggle controls");
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

    public void StartMinigame()
    {
        Debug.Log("Starting minigame...");
        minigameCamera.gameObject.SetActive(true);
        MinigameActive = true;
        EventService.Dispatch<MinigameStartedEvent>();
    }

    private void EndMinigame()
    {
        Debug.Log("Ending minigame...");
        MinigameActive = false;
        minigameCamera.gameObject.SetActive(false);
    }
}

public static class Tags
{
    public static readonly string Ball = "Ball";
    public static readonly string Catcher = "Catcher";
    public static readonly string Ground = "Ground";
}
