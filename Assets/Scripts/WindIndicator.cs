using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindIndicator : MonoBehaviour
{
    [SerializeField] private Transform windIndicatorVisual;
    [SerializeField] private TextMeshProUGUI speedText;

    [SerializeField, Tooltip("The length of the arrow when ")] private float maximumLocalScale = 2f;

    private Wind wind;

    private Transform cam;

    private float scaleUnit;

    void Start()
    {
        wind = FindObjectOfType<Wind>();

        if (wind == null)
        {
            Destroy(windIndicatorVisual.gameObject);
            Destroy(speedText.gameObject);
            Destroy(this);
            return;
        }

        cam = Camera.main.transform;

        scaleUnit = maximumLocalScale / wind.MaximumSpeed;
    }


    void Update()
    {
        if (wind == null)
        {
            return;
        }

        Vector3 direction = wind.Direction;
        float speed = wind.Magnitude;
        float xLocalScale = speed * scaleUnit;

        windIndicatorVisual.localScale = new Vector3(xLocalScale, windIndicatorVisual.localScale.y, windIndicatorVisual.localScale.z);
        windIndicatorVisual.rotation = Quaternion.LookRotation(direction);

        float speedKmH = speed * 3.6f;

        speedText.text = speedKmH.ToString("0");

        //Quaternion worldRotation = Quaternion.LookRotation(direction);
        //Quaternion localRotation = worldRotation * Quaternion.Inverse(cam.rotation);
        //windIndicatorVisual.rotation = localRotation;
    }
}
