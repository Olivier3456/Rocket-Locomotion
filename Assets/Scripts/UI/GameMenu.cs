using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuGameObject;
    [SerializeField] private RectTransform menuRectTransform;
    [SerializeField] private GameObject continueButtonGameObject;

    [SerializeField] private float distanceFromCamera = 1f;
    [SerializeField] private float menuSize = 0.5f;

    [SerializeField] private InputActionReference showOrHideMenu;

    public UnityEvent OnShow = new UnityEvent();
    public UnityEvent OnHide = new UnityEvent();

    private void Awake()
    {
        showOrHideMenu.action.started += ShowOrHideMenu_Action_Started;
        showOrHideMenu.action.Enable();
    }

    private void OnDestroy()
    {
        showOrHideMenu.action.Disable();
        showOrHideMenu.action.started -= ShowOrHideMenu_Action_Started;
    }

    private void ShowOrHideMenu_Action_Started(InputAction.CallbackContext obj)
    {
        if (SceneManager.GetActiveScene().buildIndex > MySceneManager.LOADING_SCENE_BUILD_INDEX)  // No effect if we are in Main Menu Scene or Loading scene
        {
            if (MainManager.Instance.IsPlayerAlive) // It must not be possible to deactivate this menu after player death
            {
                ToggleShowHide();
            }
        }
    }

    private void Start()
    {
        menuGameObject.SetActive(false);
    }


    public void Show(bool withoutPause = false)
    {        
        if (withoutPause || MainManager.Instance.Pause(true))
        {
            Transform cameraTransform = Camera.main.transform;

            menuRectTransform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
            menuRectTransform.forward = cameraTransform.forward;

            menuRectTransform.localScale = Vector3.one * distanceFromCamera * menuSize;
            menuGameObject.SetActive(true);

            continueButtonGameObject.SetActive(MainManager.Instance.CanUnpause);

            OnShow.Invoke();
        }
    }

    public void Hide(bool withoutUnpause = false)
    {
        if (withoutUnpause || MainManager.Instance.Pause(false))
        {
            menuGameObject.SetActive(false);
            OnHide.Invoke();
        }
    }

    public void ToggleShowHide()
    {
        if (MainManager.Instance.IsPaused)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    
    // ==================================== BUTTONS FUNCTIONS ====================================
    public void ContinueButton()
    {
        Hide();
    }

    public void ReturnToMainMenuButton()
    {
        MainManager.Instance.LoadScene(MySceneManager.MAIN_MENU_SCENE_BUILD_INDEX);
    }

    public void RestartButton()
    {
        MainManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
