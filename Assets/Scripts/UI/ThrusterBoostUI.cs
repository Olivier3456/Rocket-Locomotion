using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThrusterBoostUI : MonoBehaviour
{
    [SerializeField] private ThrustersBoostManager boostManager;
    [SerializeField] private RectTransform boostMainRectTransform;
    [SerializeField] private Image boostReserveImage;
    [SerializeField] private Gradient reserveGradient;
    [SerializeField] private AudioClip depletedAudioClip;
    [SerializeField] private AudioClip boostAuthorizedAudioClip;
    [SerializeField] private AudioSource audioSource;


    private void Awake()
    {
        boostManager.OnCanBoostStatusChange.AddListener(OnCanBoostStatusChange);
        MainManager.Instance.OnDeath.AddListener(OnDeath);
    }

    private void OnDestroy()
    {
        boostManager.OnCanBoostStatusChange.RemoveListener(OnCanBoostStatusChange);
        MainManager.Instance.OnDeath.RemoveListener(OnDeath);
    }

    void Update()
    {
        if (MainManager.Instance.IsSimulationRunning)
        {
            float status = boostManager.CurrentReserve / boostManager.MaxReserve;
            boostReserveImage.fillAmount = status;

            if (!boostManager.IsDepleted)
            {
                boostReserveImage.color = reserveGradient.Evaluate(status);
            }
        }
    }

    private void OnCanBoostStatusChange(bool canBoost)
    {
        if (canBoost)
        {
            audioSource.clip = boostAuthorizedAudioClip;
            audioSource.Play();

            if (!isBoostAuthorizedCoroutineRunning)
            {
                StartCoroutine(BoostAuthorizedCoroutine());
            }
        }
        else
        {
            audioSource.clip = depletedAudioClip;
            audioSource.Play();

            if (!isDepletedUICoroutineRunning)
            {
                boostReserveImage.color = Color.gray;
                StartCoroutine(DepletedUICoroutine());
            }
        }
    }


    private bool isDepletedUICoroutineRunning = false;
    private IEnumerator DepletedUICoroutine()
    {
        isDepletedUICoroutineRunning = true;

        float length = 0.25f;
        float iterationsNumber = 2;
        Vector3 normalScale = boostMainRectTransform.localScale;
        Vector3 bigScale = boostMainRectTransform.localScale * 1.5f;

        for (int i = 0; i < iterationsNumber; i++)
        {
            float lerp = 0f;

            while (lerp < 1)
            {
                yield return null;
                lerp += Time.deltaTime / length;
                boostMainRectTransform.localScale = Vector3.Lerp(bigScale, normalScale, lerp);
            }

            boostMainRectTransform.localScale = normalScale;
        }

        isDepletedUICoroutineRunning = false;
    }


    private bool isBoostAuthorizedCoroutineRunning = false;
    private IEnumerator BoostAuthorizedCoroutine()
    {
        isBoostAuthorizedCoroutineRunning = true;

        float length = 0.4f;
        Vector3 normalScale = boostMainRectTransform.localScale;
        Vector3 bigScale = boostMainRectTransform.localScale * 1.75f;

        float lerp = 0f;

        while (lerp < 1)
        {
            yield return null;
            lerp += Time.deltaTime / length;
            boostMainRectTransform.localScale = Vector3.Lerp(bigScale, normalScale, lerp);
        }

        boostMainRectTransform.localScale = normalScale;

        isBoostAuthorizedCoroutineRunning = false;
    }

    private void OnDeath()
    {
        boostMainRectTransform.gameObject.SetActive(false);
    }
}
