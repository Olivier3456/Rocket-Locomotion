using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] private GameObject rayInteractor;
    [Space(20)]
    [SerializeField] private GameObject thruster;
    [Space(20)]
    [SerializeField] private GameObject gun;
    [Space(20)]
    [SerializeField] private InputActionReference switchThrusterGun;

    public enum HandObject { RayInteractor, Thruster, Gun };
    private HandObject handObject;
    private HandObject handObject_EXCEPT_RayInteractor;

    public HandObject ObjectInHand { get { return handObject; } }


    private void OnEnable()
    {
        if (gun != null)
        {
            switchThrusterGun.action.started += SwitchThrusterGun_Action_started;
            switchThrusterGun.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (gun != null)
        {
            switchThrusterGun.action.Disable();
            switchThrusterGun.action.started -= SwitchThrusterGun_Action_started;
        }

        if (MainManager.Instance.GameMenu != null)
        {
            MainManager.Instance.GameMenu.OnShow.RemoveListener(OnMenuShow);
            MainManager.Instance.GameMenu.OnHide.RemoveListener(OnMenuHide);
        }
    }

    private void Start()
    {
        MainManager.Instance.GameMenu.OnShow.AddListener(OnMenuShow);
        MainManager.Instance.GameMenu.OnHide.AddListener(OnMenuHide);

        //if (SceneManager.GetActiveScene().buildIndex != MySceneManager.MAIN_MENU_SCENE_BUILD_INDEX ||
        //    SceneManager.GetActiveScene().buildIndex != MySceneManager.LOADING_SCENE_BUILD_INDEX)
        //{
        SwitchHandObject(HandObject.Thruster);
        //}
    }


    private void SwitchThrusterGun_Action_started(InputAction.CallbackContext obj)
    {
        switch (handObject)
        {
            case HandObject.RayInteractor:
                return;
            case HandObject.Thruster:
                SwitchHandObject(HandObject.Gun);
                break;
            case HandObject.Gun:
                SwitchHandObject(HandObject.Thruster);
                break;
        }
    }


    private void OnMenuShow()
    {
        SwitchHandObject(HandObject.RayInteractor);
    }
    private void OnMenuHide()
    {
        SwitchHandObject(handObject_EXCEPT_RayInteractor);
    }


    private void SwitchHandObject(HandObject objectToActivate)
    {
        switch (objectToActivate)
        {
            case HandObject.RayInteractor:
                handObject = HandObject.RayInteractor;
                break;
            case HandObject.Thruster:
                handObject = HandObject.Thruster;
                handObject_EXCEPT_RayInteractor = handObject;
                break;
            case HandObject.Gun:
                handObject = HandObject.Gun;
                handObject_EXCEPT_RayInteractor = handObject;
                break;
        }

        rayInteractor.SetActive(handObject == HandObject.RayInteractor);
        thruster.SetActive(handObject == HandObject.Thruster);

        if (gun != null)
        {
            gun.SetActive(handObject == HandObject.Gun);
        }
    }
}
