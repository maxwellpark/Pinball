using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DestructibleBumperGroup : MonoBehaviour
{
    [SerializeField] private UnityEvent onGroupDestroyed;
    private List<DestructibleBumper> bumpers;

    private void Start()
    {
        bumpers = GetComponentsInChildren<DestructibleBumper>().ToList();

        if (!bumpers.Any())
        {
            Debug.LogWarning($"[bumper group] {name} has no bumpers as children");
            Destroy(gameObject);
        }
        else
        {
            bumpers.ForEach(b => b.OnDestroyed += OnBumperDestroyed);
        }
    }

    private void OnBumperDestroyed(DestructibleBumper bumper)
    {
        Debug.Log($"[bumper group] bumper {bumper.name} destroyed");
        bumper.OnDestroyed -= OnBumperDestroyed;
        bumpers.Remove(bumper);

        if (!bumpers.Any())
        {
            Debug.Log("[bumper group] all bumpers destroyed");
            onGroupDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
