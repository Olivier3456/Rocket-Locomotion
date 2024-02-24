using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrusterBoostCanvas : MonoBehaviour
{
    [SerializeField] private ThrustersBoostManager boostManager;
    [SerializeField] private Image boostReserveImage;
    [SerializeField] private Gradient reserveGradient;

    void Update()
    {
        float status = boostManager.CurrentReserve / boostManager.MaxReserve;
        boostReserveImage.color = reserveGradient.Evaluate(status);
        boostReserveImage.fillAmount = status;
    }
}
