using Events;
using System;
using UnityEngine;
using GM = GameManager;

public class PlungerManager : MonoBehaviour
{
    [SerializeField] private Plunger defaultPlunger; // optional 

    private Plunger[] plungers;
    private int currentIndex;

    // For now it's important this runs before GameManager inits 
    private void Awake()
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

        // Hack to get around init order; really the plunger changed event should do everything 
        plungers[currentIndex].Activate();
    }

    private void Update()
    {
        // Should really track when the ball goes into a plunger vs. when it's in play but for now just check the rb 
        if (plungers.Length < 2 || GM.MinigameActive || GM.IsBallAlive && !GM.BallRb.isKinematic)
        {
            return;
        }

        // TODO: controller input (D pad?)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Array.ForEach(plungers, p => p.Deactivate());
            currentIndex = currentIndex < plungers.Length - 1 ? currentIndex + 1 : 0;
            SetActivePlunger(plungers[currentIndex]);
        }
    }

    private void SetActivePlunger(Plunger plunger)
    {
        Debug.Log("[plunger] switching active plunger to " + plunger.name);

        // Be careful with state if there are more subscribers later than just the plungers themselves 
        GM.EventService.Dispatch(new ActivePlungerChangedEvent(plungers[currentIndex]));
    }
}