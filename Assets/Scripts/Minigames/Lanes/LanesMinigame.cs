using System.Collections.Generic;
using UnityEngine;

public class LanesMinigame : Minigame
{
    protected override Type MinigameType => Type.Lanes;

    [SerializeField] private LanePlayer player;
    [SerializeField] private int cellCount = 8;
    [SerializeField, Range(0f, 1f)] private float groundChance = 0.6f;

    private const int MAX_GEN_ATTEMPTS = 10;

    // 0 = gap, 1 = ground
    private int[,] grid;
    private Lane[] lanes;

    private void OnEnable()
    {
        player.OnHitGap += OnHitGap;
        player.OnFinished += OnFinished;
        lanes = GetComponentsInChildren<Lane>();

        var success = false;
        var attempts = 0;

        while (!success && attempts < MAX_GEN_ATTEMPTS)
        {
            attempts++;
            GenerateGrid();
            success = PathExists();
        }

        if (!success)
        {
            Debug.LogError($"[lanes] failed to generate valid path after {MAX_GEN_ATTEMPTS} attempts.");
            return;
        }

        SpawnPath();

        foreach (var lane in lanes)
        {
            //lane.IsMoving = true;
        }
    }

    private void OnDisable()
    {
        player.OnHitGap -= OnHitGap;
        player.OnFinished -= OnFinished;

        foreach (Lane lane in lanes)
        {
            lane.IsMoving = false;
            lane.Clear();
        }
    }

    private void GenerateGrid()
    {
        int laneCount = lanes.Length;
        grid = new int[laneCount, cellCount];

        for (int row = 0; row < cellCount; row++)
        {
            bool hasGround = false;
            for (int l = 0; l < laneCount; l++)
            {
                if (Random.value < groundChance)
                {
                    grid[l, row] = 1; // ground
                    hasGround = true;
                }
                else
                {
                    grid[l, row] = 0; // gap
                }
            }

            // Ensure at least one ground in this row
            if (!hasGround)
            {
                int forcedLane = Random.Range(0, laneCount);
                grid[forcedLane, row] = 1;
            }
        }
    }

    private bool PathExists()
    {
        var laneCount = lanes.Length;
        var visited = new bool[laneCount, cellCount];

        var queue = new Queue<(int lane, int row)>();
        for (int i = 0; i < laneCount; i++)
        {
            if (grid[i, 0] == 1)
            {
                queue.Enqueue((i, 0));
                visited[i, 0] = true;
            }
        }

        // BFS
        while (queue.Count > 0)
        {
            var (lane, row) = queue.Dequeue();

            if (row == cellCount - 1)
            {
                return true; // Success
            }

            // Horizontal 
            for (int i = lane - 1; i <= lane + 1; i += 2)
            {
                if (i >= 0 && i < laneCount)
                {
                    if (grid[i, row] == 1 && !visited[i, row])
                    {
                        visited[i, row] = true;
                        queue.Enqueue((i, row));
                    }
                }
            }

            // Vertical 
            int nextRow = row + 1;
            if (nextRow < cellCount)
            {
                if (grid[lane, nextRow] == 1 && !visited[lane, nextRow])
                {
                    visited[lane, nextRow] = true;
                    queue.Enqueue((lane, nextRow));
                }
            }
        }

        return false;
    }

    private void SpawnPath()
    {
        var laneCount = lanes.Length;
        for (int i = 0; i < cellCount; i++)
        {
            for (int j = 0; j < laneCount; j++)
            {
                if (grid[j, i] == 1)
                {
                    lanes[j].SpawnGround(i);
                }
                else
                {
                    lanes[j].SpawnGap(i);
                }
            }
        }

        int finishLane = Random.Range(0, laneCount);
        lanes[finishLane].SpawnFinish(cellCount);
    }

    private void OnHitGap()
    {
        won = false;
        EndImmediate();
    }

    private void OnFinished()
    {
        won = true;
        EndImmediate();
    }
}
