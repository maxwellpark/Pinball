using UnityEngine;

[CreateAssetMenu(fileName = "GhostBallAbility", menuName = "ScriptableObjects/GhostBallAbility")]
public class GhostBallAbility : Ability
{
    public override GameManager.Action ActionType => GameManager.Action.AddGhostBall;
    protected override void UseInternal()
    {
        GameManager.CreateGhostBall();
    }
}
