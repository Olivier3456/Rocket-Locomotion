using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private GameObject deadText;

    private void Start()
    {
        MainManager.Instance.OnDeath.AddListener(HandleDeath);
    }

    private void OnDestroy()
    {
        MainManager.Instance.OnDeath.RemoveListener(HandleDeath);
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
        float length = 2;
        while (timer < length)
        {
            yield return null;
            deadTMP.fontSize += Time.deltaTime * 0.1f;
            deadTMP.color = new Color(deadTMP.color.r, deadTMP.color.g, deadTMP.color.b, deadTMP.color.a + (Time.deltaTime / length));
            timer += Time.deltaTime;

        }

        timer = 0;
        length = 0.5f;
        while (timer < length)
        {
            yield return null;
            deadTMP.fontSize += Time.deltaTime * 0.1f;
            deadTMP.color = new Color(deadTMP.color.r, deadTMP.color.g, deadTMP.color.b, deadTMP.color.a - (Time.deltaTime / length));
            timer += Time.deltaTime;
        }

        bool withoutPause = true;
        MainManager.Instance.GameMenu.Show(withoutPause);
    }
}
