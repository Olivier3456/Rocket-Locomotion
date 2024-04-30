using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneManager : MonoBehaviour
{
    public static int MAIN_MENU_SCENE = 0;
    private static int LOADING_SCENE = 1;
    public static int NEW_YORK_1_FREE_FLIGHT = 2;
    //public static int NEW_YORK_2_FREE_FLIGHT = 3;
    public static int RACE_1 = 3;
    public static int RACE_2 = 4;
    public static int RACE_3 = 5;
    //public static int PROTECT_THE_SKYLORD_1 = 7;
    public static int SURVIVE_DRONE_ATTACK = 6;

    private bool isLoading;



    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex == LOADING_SCENE)
        {
            Debug.LogError($"Build index {LOADING_SCENE} is the index of the loading scene itself!");
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

        yield return SceneManager.LoadSceneAsync(LOADING_SCENE);

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
