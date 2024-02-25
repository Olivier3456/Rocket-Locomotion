using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HudManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private GameObject deadText;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerLife playerLife;

    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;

    private float speedRefreshDelay = 0.2f;
    private float speedRefreshTimer = 0;


    private void Awake()
    {
        playerLife.OnDeath.AddListener(DisplayDeadText);
    }

    private void OnDisable()
    {
        playerLife.OnDeath.RemoveListener(DisplayDeadText);
    }


    private Coroutine handleDeathCoroutine;
    private void DisplayDeadText()
    {
        if (handleDeathCoroutine == null)
        {
            handleDeathCoroutine = StartCoroutine(HandleDeathCoroutine());
        }
    }


    void Update()
    {
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


    private IEnumerator HandleDeathCoroutine()
    {
        deadText.SetActive(true);
        TextMeshProUGUI deadTMP = deadText.GetComponent<TextMeshProUGUI>();

        yield return new WaitForSeconds(1);

        float timer = 0;
        bool sceneLoading = false;
        while (true)
        {
            yield return null;
            deadTMP.fontSize += Time.deltaTime * 0.1f;
            deadTMP.color = new Color(deadTMP.color.r, deadTMP.color.g, deadTMP.color.b, deadTMP.color.a + (Time.deltaTime * 0.5f));
            timer += Time.deltaTime;

            if (timer > 2 && !sceneLoading)
            {
                SceneManager.LoadSceneAsync(0);
                sceneLoading = true;
            }
        }
    }
}
