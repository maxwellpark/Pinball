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

    public void SetActive(bool active)
    {
        isActive = active;
    }
}
