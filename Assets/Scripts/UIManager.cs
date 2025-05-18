using Events;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: currently this only handles BumperGroups; we should port the UI stuff in the GameManager too.
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TMP_Text abilityText;
    [SerializeField] private Image abilityImage;
    [SerializeField] private Slider chargeSlider;

    [Header("Bumper groups")]
    [SerializeField] private RectTransform bumperGroupContainer;
    [SerializeField, Tooltip("Scroll view content")] private RectTransform bumperGroupContent;
    [SerializeField] private GameObject bumperGroupPrefab;

    private readonly Dictionary<BumperGroup, TMP_Text> textByBumperGroup = new();

    // TODO: hack to avoid plungers losing reference on scene change 
    public Slider ChargeSlider => chargeSlider;

    protected override void Awake()
    {
        base.Awake();
        bumperGroupContainer.gameObject.SetActive(value: false);
    }

    private void OnEnable()
    {
        //SceneManager.activeSceneChanged += OnSceneChanged;
        GameManager.EventService.Add<AbilityChangedEvent>(OnAbilityChanged);
        GameManager.EventService.Add<AbilityUsedEvent>(OnAbilityUsed);
    }

    private void OnDisable()
    {
        //SceneManager.activeSceneChanged -= OnSceneChanged;
        GameManager.EventService.Remove<AbilityChangedEvent>(OnAbilityChanged);
        GameManager.EventService.Remove<AbilityUsedEvent>(OnAbilityUsed);
    }

    //private void OnSceneChanged(Scene prev, Scene next)
    //{
    //}
    private void OnAbilityChanged(AbilityChangedEvent evt)
    {
        UpdateAbilityUI(evt.Ability, evt.Ability.Uses);
    }

    private void OnAbilityUsed(AbilityUsedEvent evt)
    {
        UpdateAbilityUI(evt.Ability, evt.Uses);
    }

    private void UpdateAbilityUI(Ability ability, int uses)
    {
        abilityText.SetText($"{ability.Name}s: {uses}");
        abilityImage.sprite = ability.Icon;
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

        if (bumperGroupContainer != null)
        {
            bumperGroupContainer.gameObject.SetActive(textByBumperGroup.Any());
        }
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

            text.text = $"{bumperGroup.name} score: {bumperGroup.CurrentScore}/{bumperGroup.ScoreReceiverTrigger}";
        }
    }
}
