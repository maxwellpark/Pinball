using Events;
using System.Collections.Generic;
using UnityEngine;

public class FallingFloorsMinigame : Minigame
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<GameObject> floors;
    [SerializeField] private float floorRotationSpeed = 100f;
    [SerializeField] private float fallSpeedMultiplier = 1f;

    protected override Type MinigameType => Type.FallingFloors;

    private readonly Dictionary<GameObject, Vector3> startRotByFloor = new();
    private GameObject ball;
    private Rigidbody2D ballRb;
    private int currentFloorIndex = 0;

    protected override void Start()
    {
        base.Start();

        foreach (var floor in floors)
        {
            startRotByFloor[floor] = floor.transform.eulerAngles;
        }
    }

    protected override void StartMinigame()
    {
        base.StartMinigame();

        foreach (var floor in floors)
        {
            floor.transform.eulerAngles = startRotByFloor[floor];
        }

        currentFloorIndex = 0;
        ball = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.gravityScale = fallSpeedMultiplier;
    }

    private void Update()
    {
        if (GameManager.CurrentMinigame != MinigameType || currentFloorIndex >= floors.Count)
        {
            return;
        }

        var currentFloor = floors[currentFloorIndex];

        var input = Input.GetAxis("Horizontal");
        if (input != 0)
        {
            var rotationAmount = floorRotationSpeed * input * Time.deltaTime;
            currentFloor.transform.Rotate(Vector3.forward, rotationAmount);
        }

        if (ball.transform.position.y < currentFloor.transform.position.y - 3f)
        {
            currentFloorIndex++;
            if (currentFloorIndex >= floors.Count)
            {
                won = true;
                GameManager.AddScore(winScore);
                StartCoroutine(EndAfterDelay());
            }
        }
    }

    protected override void EndMinigame()
    {
        Destroy(ball);
        base.EndMinigame();
    }
}
