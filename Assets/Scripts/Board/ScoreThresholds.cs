using System;
using UnityEngine;
using Action = GameManager.Action;

[CreateAssetMenu(fileName = "ScoreThresholds", menuName = "ScriptableObjects/ScoreThresholds", order = 1)]
public class ScoreThresholds : ScriptableObject
{
    public ScoreThreshold[] Thresholds;
}

[Serializable]
public class ScoreThreshold
{
    public int Score;
    public Action Action = Action.None;
}
