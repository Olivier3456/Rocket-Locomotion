using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS_Counter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public float updateInterval = 0.5f;

    private float framesCount = 0;
    private float elapsedTime = 0;

    //private void Start()
    //{
    //    if (head == null)
    //    {
    //        head = FindObjectOfType<Camera>().gameObject;
    //    }

    //    if (head != null)
    //    {
    //        transform.parent = head.transform;
    //    }

    //    if (fpsText == null)
    //    {
    //        Debug.LogError("FPSCounter: text component not assigned!");
    //        enabled = false;
    //        return;
    //    }
    //}

    private void Update()
    {
        framesCount++;
        elapsedTime += Time.deltaTime;

        if (elapsedTime > updateInterval)
        {
            float fps = framesCount / elapsedTime;
            fpsText.text = $"FPS: {fps:F2}";

            framesCount = 0;
            elapsedTime = 0;
        }
    }
}
