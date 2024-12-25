using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject gapPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private Transform start;
    [SerializeField] private int cellCount = 8;
    [SerializeField] private float speed = 5f;

    private Vector3 startPos;
    private readonly List<GameObject> instances = new();

    private void Awake()
    {
        startPos = transform.localPosition;
    }

    private void OnEnable()
    {
        // TODO: random path algorithm
        for (int i = 0; i < cellCount; i++)
        {
            if (i % 2 == 0)
            {
                SpawnGround();
            }
            else
            {
                SpawnGap();
            }
        }
    }

    private void OnDisable()
    {
        transform.localPosition = startPos;

        foreach (var instance in instances)
        {
            Destroy(instance);
        }
        instances.Clear();
    }

    private void Update()
    {
        // For now need world space as we've rotated 90 degrees out of laziness 
        transform.Translate(speed * Time.deltaTime * Vector3.down, Space.World);

        // TODO: if making longer, make the lane respawn i.e. wrap around 
        // if (transform.position.y <= endPoint.position.y)
        // {
        //     transform.position = new Vector3(transform.position.x, respawnPoint.position.y, transform.position.z);
        // }
    }

    private void SpawnObject(GameObject prefab, Transform parent = null)
    {
        var instance = Instantiate(prefab);

        if (parent != null)
        {
            instance.transform.SetParent(parent.transform, false);
        }

        instances.Add(instance);
        var yPos = start.transform.position.y + instances.Count - 1;
        instance.transform.position = new Vector2(instance.transform.position.x, yPos);
    }

    public void SpawnGround()
    {
        SpawnObject(groundPrefab, transform);
    }

    public void SpawnGap()
    {
        SpawnObject(gapPrefab, transform);
    }

    public void SpawnFinish()
    {
        SpawnObject(finishPrefab, transform);
    }
}
