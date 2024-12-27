using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject gapPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private Transform start;
    [SerializeField] private float speed = 5f;

    private readonly List<GameObject> instances = new();
    private Vector3 startPos;

    public bool IsMoving { get; set; }

    private void Awake()
    {
        startPos = transform.localPosition;
    }

    public void SpawnGround(int row)
    {
        SpawnObject(groundPrefab, row);
    }

    public void SpawnGap(int row)
    {
        SpawnObject(gapPrefab, row);
    }

    public void SpawnFinish(int row)
    {
        SpawnObject(finishPrefab, row);
    }

    private void SpawnObject(GameObject prefab, int row)
    {
        var instance = Instantiate(prefab, transform);
        instances.Add(instance);

        // 1 unit spacing 
        var yPos = start.position.y + row;
        var xPos = transform.position.x;
        instance.transform.position = new Vector2(xPos, yPos);
    }

    public void Clear()
    {
        foreach (var instance in instances)
        {
            if (instance != null)
            {
                Destroy(instance);
            }
        }
        instances.Clear();
        transform.localPosition = startPos;
    }

    private void Update()
    {
        if (!IsMoving)
        {
            return;
        }

        // For now need world space as we've rotated 90 degrees out of laziness 
        transform.Translate(speed * Time.deltaTime * Vector3.down, Space.World);

        // TODO: if making longer, make the lane respawn i.e. wrap around 
        // if (transform.position.y <= endPoint.position.y)
        // {
        //     transform.position = new Vector3(transform.position.x, respawnPoint.position.y, transform.position.z);
        // }
    }
}
