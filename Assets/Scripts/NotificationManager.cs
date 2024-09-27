using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : Singleton<NotificationManager>
{
    [SerializeField] private GameObject textContainer;
    [SerializeField] private float duration = 2f;

    private TMP_Text text;
    private static readonly Queue<string> messageQueue = new();
    private static bool isDisplaying;

    private void Start()
    {
        text = textContainer.GetComponentInChildren<TMP_Text>();
        textContainer.SetActive(false);
    }

    public static void Notify(string message, float? duration = null)
    {
        messageQueue.Enqueue(message);

        if (!isDisplaying)
        {
            Instance.StartCoroutine(DisplayNextMessage(duration ?? Instance.duration));
        }
    }

    private static IEnumerator DisplayNextMessage(float duration)
    {
        isDisplaying = true;

        while (messageQueue.Count > 0)
        {
            var message = messageQueue.Dequeue();
            Instance.text.text = message;
            Instance.textContainer.SetActive(true);

            yield return new WaitForSeconds(duration);

            Instance.textContainer.SetActive(false);

            yield return new WaitForSeconds(0.1f);
        }

        isDisplaying = false;
    }
}
