using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcherUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    private void Start()
    {
        var sceneCount = SceneManager.sceneCountInBuildSettings;

        for (var i = 0; i < sceneCount; i++)
        {
            var index = i;
            var path = SceneUtility.GetScenePathByBuildIndex(index);
            var sceneName = Path.GetFileNameWithoutExtension(path);

            var instance = Instantiate(buttonPrefab, buttonContainer);
            var button = instance.GetComponent<Button>();
            var label = instance.GetComponentInChildren<TMP_Text>();

            label.text = sceneName;
            button.onClick.AddListener(() => SceneManager.LoadScene(index));
        }

        container.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            container.gameObject.SetActive(!container.gameObject.activeSelf);
        }
    }
}
