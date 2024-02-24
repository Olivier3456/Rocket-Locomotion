using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrusterBoostCanvas : MonoBehaviour
{
    [SerializeField] private ThrusterBoostManager boostManager;
    [SerializeField] private Image boostReserveImage;
    [SerializeField] private Gradient reserveGradient;

    private Transform cam;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        float status = boostManager.CurrentReserve / boostManager.MaxReserve;
        boostReserveImage.color = reserveGradient.Evaluate(status);
        boostReserveImage.fillAmount = status;

        transform.LookAt(cam);
    }
}
