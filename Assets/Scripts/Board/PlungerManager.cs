using Events;
using System;
using UnityEngine;

public class PlungerManager : MonoBehaviour
{
    [SerializeField] private Plunger defaultPlunger; // optional 

    private Plunger[] plungers;
    private int currentIndex;

    private void Start()
    {
        plungers = FindObjectsOfType<Plunger>();

        if (plungers.IsNullOrEmpty())
        {
            Debug.LogError("[plunger] no plungers found");
            return;
        }

        Array.ForEach(plungers, p => p.Deactivate());

        if (defaultPlunger != null)
        {
            currentIndex = Array.IndexOf(plungers, defaultPlunger);
        }
        else
        {
            currentIndex = 0;
        }

        plungers[currentIndex].Activate();
    }

    private void Update()
    {
        if (plungers.Length < 2 || GameManager.MinigameActive)
        {
            return;
        }

        // TODO: controller input (D pad?)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Array.ForEach(plungers, p => p.Deactivate());
            currentIndex = currentIndex < plungers.Length - 1 ? currentIndex + 1 : 0;
            var plunger = plungers[currentIndex];

            Debug.Log("[plunger] switching active plunger to " + plunger.name);

            // Be careful with state if there are more subscribers later than just the plungers themselves 
            GameManager.EventService.Dispatch(new ActivePlungerChangedEvent(plungers[currentIndex]));
        }
    }
}