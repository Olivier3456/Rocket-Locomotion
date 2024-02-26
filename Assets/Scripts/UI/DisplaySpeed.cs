using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DisplaySpeed : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerLife playerLife;

    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;

    private float speedRefreshDelay = 0.2f;
    private float speedRefreshTimer = 0;


    void Update()
    {
        if (!playerLife.IsAlive)
        {
            return;
        }

        speedRefreshTimer += Time.deltaTime;
        if (speedRefreshTimer > speedRefreshDelay)
        {
            speedRefreshTimer = 0;

            float magnitude = rb.velocity.magnitude;
            float speedKmH = magnitude * 3.6f;

            float maxSpeedForColor = 200f;
            float lerp = speedKmH / maxSpeedForColor;

            speedText.color = Color.Lerp(minColor, maxColor, lerp);
            speedText.text = speedKmH.ToString("0");
        }
    }
}
