using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllersVibrations : MonoBehaviour
{
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;
    [Space(20)]
    [SerializeField] private ThrustersBoostManager thrustersBoostManager;
    [SerializeField] private PlayerLife playerLife;
    [SerializeField] private PhysicalContactsManager physicalContactsManager;

    private WaitForSeconds waitForNextVibration = new WaitForSeconds(0.18f);


    private void Awake()
    {
        physicalContactsManager.OnCollision.AddListener(OnCollision);
        thrustersBoostManager.OnCanBoostStatusChange.AddListener(OnCanBoostAgain);
    }


    private void OnCanBoostAgain(bool canBoost)
    {
        if (canBoost)
        {
            StartCoroutine(CanBoostAgainVibrationCoroutine());
        }
        else
        {
            float amplitude = 1f;
            float duration = 0.33f;
            leftController.SendHapticImpulse(amplitude, duration);
            rightController.SendHapticImpulse(amplitude, duration);
        }
    }


    private IEnumerator CanBoostAgainVibrationCoroutine()
    {
        float amplitude = 1f;
        float duration = 0.1f;

        leftController.SendHapticImpulse(amplitude, duration);
        rightController.SendHapticImpulse(amplitude, duration);

        yield return waitForNextVibration;

        leftController.SendHapticImpulse(amplitude, duration);
        rightController.SendHapticImpulse(amplitude, duration);
    }


    private void OnCollision(float collisionForce)
    {
        float amplitude = Mathf.Clamp01(collisionForce / playerLife.RelativeVelocityThresholdToLoseLife);
        float duration = 0.25f;

        leftController.SendHapticImpulse(amplitude, duration);
        rightController.SendHapticImpulse(amplitude, duration);
    }


    private void OnDestroy()
    {
        physicalContactsManager.OnCollision.RemoveListener(OnCollision);
        thrustersBoostManager.OnCanBoostStatusChange.RemoveListener(OnCanBoostAgain);
    }
}
