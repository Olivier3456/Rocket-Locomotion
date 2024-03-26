using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneManager : MonoBehaviour
{
    public const int MAIN_MENU_SCENE_BUILD_INDEX = 0;
    public const int LOADING_SCENE_BUILD_INDEX = 1;
    public const int NEW_YORK_1_FREE_FLIGHT = 2;
    public const int NEW_YORK_2_FREE_FLIGHT = 3;
    public const int RACE_1 = 4;
    public const int RACE_2 = 5;
    public const int RACE_3 = 6;
    public const int PROTECT_THE_SKYLORD_1 = 7;

    private bool isLoading;

    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex == LOADING_SCENE_BUILD_INDEX)
        {
            Debug.LogError($"Build index {LOADING_SCENE_BUILD_INDEX} is the index of the loading scene itself!");
            return;
        }

        if (!isLoading)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex));
        }
        else
        {
            Debug.Log("MySceneManager is already loading a scene.");
        }
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        isLoading = true;

        yield return SceneManager.LoadSceneAsync(LOADING_SCENE_BUILD_INDEX);

        Image loadProgressImage = GameObject.Find("Loading Bar").GetComponent<Image>();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadProgressImage.fillAmount = progress;
        }

        isLoading = false;
    }
}
