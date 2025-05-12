using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardConfigs", menuName = "ScriptableObjects/BoardConfigs", order = 1)]
public class BoardConfigs : ScriptableObject
{
    [SerializeField] private BoardConfig[] boards;
    private Dictionary<string, BoardConfig> boardByName;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Debug.Log("[board] init lookup");
        boardByName = boards.ToDictionary(k => k.UniqueName, v => v);
    }

    public bool TryGetBoard(string name, out BoardConfig board)
    {
        if (boardByName == null)
        {
            Init();
        }
        return boardByName.TryGetValue(name, out board);
    }
}

[Serializable]
public class BoardConfig
{
    [Tooltip("Needs to match the scene name of the board")]
    public string UniqueName;

    [Header("Sound")]
    public AudioClip BumperSound;
    public AudioClip KickerSound;
    public AudioClip FlipperSound;
    public AudioClip PistonSound;
    public AudioClip PlungerChargeSound;
    public AudioClip PlungerLaunchSound;
    public AudioClip CollectibleSound;
}
