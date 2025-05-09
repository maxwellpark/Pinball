using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: currently this only handles BumperGroups; we should port the UI stuff in the GameManager too.
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private RectTransform bumperGroupContainer;
    [SerializeField, Tooltip("Scroll view content")] private RectTransform bumperGroupContent;
    [SerializeField] private GameObject bumperGroupPrefab;
    [SerializeField] private Slider chargeSlider;

    private readonly Dictionary<BumperGroup, TMP_Text> textByBumperGroup = new();

    // TODO: hack to avoid plungers losing reference on scene change 
    public Slider ChargeSlider => chargeSlider;

    protected override void Awake()
    {
        base.Awake();
        bumperGroupContainer.gameObject.SetActive(false);
    }

    public void RegisterBumperGroup(BumperGroup bumperGroup)
    {
        if (textByBumperGroup.ContainsKey(bumperGroup))
        {
            return;
        }

        var textObj = Instantiate(bumperGroupPrefab, bumperGroupContent);
        var text = textObj.GetComponentInChildren<TMP_Text>();
        textByBumperGroup[bumperGroup] = text;
        UpdateBumperGroupText(bumperGroup);
        bumperGroupContainer.gameObject.SetActive(true);
    }

    public void UnregisterBumperGroup(BumperGroup bumperGroup)
    {
        if (textByBumperGroup.TryGetValue(bumperGroup, out var text))
        {
            if (text != null
                && text.gameObject != null
                && text.gameObject.transform.parent != null
                && text.gameObject.transform.parent.gameObject != null)
            {
                Destroy(text.gameObject.transform.parent.gameObject);
            }
            textByBumperGroup.Remove(bumperGroup);
        }

        bumperGroupContainer.gameObject.SetActive(textByBumperGroup.Any());
    }

    public void UpdateBumperGroupText(BumperGroup bumperGroup)
    {
        if (textByBumperGroup.TryGetValue(bumperGroup, out var text))
        {
            if (text == null)
            {
                textByBumperGroup.Remove(bumperGroup);
                return;
            }

            text.text = $"{bumperGroup.name} score: {bumperGroup.CurrentScore}/{bumperGroup.ScoreMinigameTrigger}";
        }
    }
}
