using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllersManager : MonoBehaviour
{
    [SerializeField] GameObject rayInteractorLeft;
    [SerializeField] GameObject rayInteractorRight;
    [Space(20)]
    [SerializeField] GameObject thrusterLeft;
    [SerializeField] GameObject thrusterRight;

    void Start()
    {
        MainManager.Instance.MainMenu.OnShow.AddListener(OnMenuShow);
        MainManager.Instance.MainMenu.OnHide.AddListener(OnMenuHide);

        if (SceneManager.GetActiveScene().buildIndex != MySceneManager.MAIN_MENU_SCENE_BUILD_INDEX)
        {
            ActivateRays(false);
            ActivateThrusters(true);
        }
    }

    private void OnDestroy()
    {
        MainManager.Instance.MainMenu.OnShow.RemoveListener(OnMenuShow);
        MainManager.Instance.MainMenu.OnHide.RemoveListener(OnMenuHide);
    }

    private void OnMenuShow()
    {
        ActivateRays(true);
        ActivateThrusters(false);
    }
    private void OnMenuHide()
    {
        ActivateRays(false);
        ActivateThrusters(true);
    }





    private void ActivateRays(bool active)
    {
        rayInteractorLeft.SetActive(active);
        rayInteractorRight.SetActive(active);
    }

    private void ActivateThrusters(bool active)
    {
        thrusterLeft.SetActive(active);
        thrusterRight.SetActive(active);
    }
}
