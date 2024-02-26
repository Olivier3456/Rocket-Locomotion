using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private GameObject deadText;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerLife playerLife;

    private void Awake()
    {
        playerLife.OnDeath.AddListener(HandleDeath);
    }

    private void OnDisable()
    {
        playerLife.OnDeath.RemoveListener(HandleDeath);
    }

    private Coroutine handleDeathCoroutine;
    private void HandleDeath()
    {
        if (handleDeathCoroutine == null)
        {
            handleDeathCoroutine = StartCoroutine(HandleDeathCoroutine());
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
