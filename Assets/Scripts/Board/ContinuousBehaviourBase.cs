using UnityEngine;

public abstract class ContinuousBehaviour : MonoBehaviour
{
    [SerializeField] protected bool isActive;

    private void Update()
    {
        if (isActive)
        {
            Behave();
        }
    }

    protected abstract void Behave();

    // Only referenced in inspector for now 
    public void SetActive(bool active)
    {
        isActive = active;
    }

    // Only referenced in inspector for now 
    public void ToggleActive()
    {
        isActive = !isActive;
    }
}
