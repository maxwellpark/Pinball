using UnityEngine;
using UnityEngine.UI;

public class RingMinigame : Minigame
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private float fillRate = 0.1f;
    [SerializeField] private float depletionRate = 0.05f;
    [SerializeField] private float winThreshold = 1f;

    private PlayerRing playerRing;
    private float progress;

    protected override Type MinigameType => Type.Ring;

    protected override void Start()
    {
        base.Start();
        playerRing = GetComponentInChildren<PlayerRing>(true);

        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = progress;
        }
    }

    private void OnEnable()
    {
        progressBar.value = 0f;
    }

    private void Update()
    {
        if (GameManager.CurrentMinigame != MinigameType || won)
        {
            return;
        }

        if (playerRing.IsInsideTarget)
        {
            progress += fillRate * Time.deltaTime;
        }
        else
        {
            progress -= depletionRate * Time.deltaTime;
        }

        progress = Mathf.Clamp(progress, 0f, winThreshold);

        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progress == winThreshold)
        {
            won = true;
            StartCoroutine(EndAfterDelay());
        }
    }
}
