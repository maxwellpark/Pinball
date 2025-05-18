using Events;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public int Uses;
    public int StartingUses = 3;

    public abstract GameManager.Action ActionType { get; }
    protected abstract void UseInternal();

    private void Awake()
    {
        Uses = StartingUses;
    }

    public virtual void Use()
    {
        if (Uses <= 0)
        {
            Debug.Log($"[ability] {GetType()} can't use due to no uses");
            return;
        }

        Debug.Log($"[ability] using {GetType()}");
        UseInternal();
        Uses--;
        GameManager.EventService.Dispatch(new AbilityUsedEvent(this, Uses));
    }

    public void AddUses(int count)
    {
        Debug.Log($"[ability] adding {GetType()} {count} uses");
        Uses += Mathf.Max(0, count);
    }

    private void OnValidate()
    {
        Uses = Mathf.Max(0, Uses);
    }
}
