using UnityEngine;

[CreateAssetMenu(fileName = "ShooterAbility", menuName = "ScriptableObjects/ShooterAbility")]
public class ShooterAbility : Ability
{
    public override GameManager.Action ActionType => GameManager.Action.AddShooter;

    public override void Use()
    {
        if (GameManager.Instance.IsShooterActive)
        {
            return;
        }
        base.Use();
    }

    protected override void UseInternal()
    {
        GameManager.CreateShooter();
    }
}
