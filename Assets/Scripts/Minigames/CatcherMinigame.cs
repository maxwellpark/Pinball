using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatcherMinigame : Minigame
{
    [SerializeField] private GameObject catcherPlatform;
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private BoxCollider2D spawnCollider;
    [SerializeField] private float objectDropInterval = 1f;
    [SerializeField] private int objectCount = 3;
    [SerializeField] private float catcherSpeed = 5f;

    private Vector3 startPos;
    private int objectsCaught;
    private int objectsMissed;
    private readonly List<GameObject> objects = new();

    protected override Type MinigameType => Type.Catcher;

    protected override void Start()
    {
        base.Start();
        startPos = catcherPlatform.transform.localPosition;
        GameManager.EventService.Add<ObjectCaughtEvent>(ObjectCaught);
        GameManager.EventService.Add<ObjectMissedEvent>(ObjectMissed);
    }

    public override void OnMinigameStarted(MinigameStartedEvent evt)
    {
        base.OnMinigameStarted(evt);

        if (evt.Type == MinigameType)
        {
            catcherPlatform.transform.localPosition = startPos;
            objectsCaught = 0;
            objectsMissed = 0;
            StartCoroutine(SpawnObjects());
        }
    }

    protected override void EndMinigame()
    {
        base.EndMinigame();
        objects.ForEach(o => Destroy(o));
        objects.Clear();
    }

    private void Update()
    {
        if (!GameManager.MinigameActive)
        {
            return;
        }

        var input = Input.GetAxis("Horizontal");
        catcherPlatform.transform.Translate(catcherSpeed * input * Time.deltaTime * Vector2.right);
    }

    private IEnumerator SpawnObjects()
    {
        for (int i = 0; i < objectCount; i++)
        {
            var bounds = spawnCollider.bounds;
            var size = objectPrefab.GetComponent<SpriteRenderer>().bounds.size;
            var minX = bounds.min.x + size.x / 2;
            var maxX = bounds.max.x - size.x / 2;

            var randomX = Random.Range(minX, maxX);
            var spawnPosition = new Vector3(randomX, bounds.max.y, 0f);

            var instance = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
            objects.Add(instance);
            yield return new WaitForSeconds(objectDropInterval);
        }
    }

    public void ObjectLanded(bool caught)
    {
        if (caught)
        {
            objectsCaught++;
        }
        else
        {
            objectsMissed++;
        }

        if (objectsCaught >= objectCount)
        {
            won = true;
            GameManager.AddScore(winScore);
            StartCoroutine(EndAfterDelay());
        }
        else if (objectsMissed + objectsCaught >= objectCount)
        {
            won = false;
            StartCoroutine(EndAfterDelay());
        }
    }

    public void ObjectCaught()
    {
        ObjectLanded(true);
    }

    public void ObjectMissed()
    {
        ObjectLanded(false);
    }
}
