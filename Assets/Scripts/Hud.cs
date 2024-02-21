using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Hud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private GameObject deadText;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerLife playerLife;

    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;


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
        float magnitude = rb.velocity.magnitude;
        float speedKmH = magnitude * 3.6f;

        float maxSpeedForColor = 200f;
        float lerp = speedKmH / maxSpeedForColor;

        speedText.color = Color.Lerp(minColor, maxColor, lerp);
        speedText.text = speedKmH.ToString("0");
    }


    private IEnumerator HandleDeathCoroutine()
    {
        deadText.SetActive(true);
        TextMeshProUGUI deadTMP = deadText.GetComponent<TextMeshProUGUI>();

        yield return new WaitForSeconds(1);

        float timer = 0;
        while (timer < 2)
        {
            yield return null;
            deadTMP.fontSize += Time.deltaTime;
            deadTMP.color = new Color(deadTMP.color.r, deadTMP.color.g, deadTMP.color.b, deadTMP.color.a + (Time.deltaTime * 0.5f));
            timer += Time.deltaTime;
        }

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
