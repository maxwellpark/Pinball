using UnityEngine;

[CreateAssetMenu(fileName = "BombAbility", menuName = "ScriptableObjects/BombAbility")]
public class BombAbility : Ability
{
    public override GameManager.Action ActionType => GameManager.Action.AddBomb;
    protected override void UseInternal()
    {
        GameManager.Instance.StartTriggerExplosion();
    }
}
