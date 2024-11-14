public class RingMinigame : Minigame
{
    protected override Type MinigameType => Type.Ring;

    private PlayerInnerRingTrigger playerInner;
    private PlayerOuterRingTrigger playerOuter;
    private TargetInnerRingTrigger targetInner;
    private TargetOuterRingTrigger targetOuter;

    private bool isPlayerInnerInsideTargetInner;
    private bool isPlayerOuterInsideTargetOuter;

    // Are both colliders inside the target
    private bool IsPlayerInsideTarget => isPlayerInnerInsideTargetInner && isPlayerOuterInsideTargetOuter;

    private void Awake()
    {
        playerInner = GetComponentInChildren<PlayerInnerRingTrigger>(true);
        playerOuter = GetComponentInChildren<PlayerOuterRingTrigger>(true);
        targetInner = GetComponentInChildren<TargetInnerRingTrigger>(true);
        targetOuter = GetComponentInChildren<TargetOuterRingTrigger>(true);

        playerInner.OnPlayerInnerTrigger += OnPlayerInnerTrigger;
        playerOuter.OnPlayerOuterTrigger += OnPlayerOuterTrigger;
        targetInner.OnTargetInnerTrigger += OnTargetInnerTrigger;
        targetOuter.OnTargetOuterTrigger += OnTargetOuterTrigger;
    }

    private void OnDestroy()
    {
        playerInner.OnPlayerInnerTrigger -= OnPlayerInnerTrigger;
        playerOuter.OnPlayerOuterTrigger -= OnPlayerOuterTrigger;
        targetInner.OnTargetInnerTrigger -= OnTargetInnerTrigger;
        targetOuter.OnTargetOuterTrigger -= OnTargetOuterTrigger;
    }

    private void OnPlayerInnerTrigger(bool isInside)
    {
        isPlayerInnerInsideTargetInner = isInside;
    }

    private void OnPlayerOuterTrigger(bool isInside)
    {
        isPlayerOuterInsideTargetOuter = isInside;
    }

    private void OnTargetInnerTrigger(bool isInside)
    {
        isPlayerInnerInsideTargetInner = isInside;
    }

    private void OnTargetOuterTrigger(bool isInside)
    {
        isPlayerOuterInsideTargetOuter = isInside;
    }
}
