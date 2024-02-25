using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThrusterBoostUI : MonoBehaviour
{
    [SerializeField] private ThrustersBoostManager boostManager;
    [SerializeField] private RectTransform boostMainRectTransform;
    [SerializeField] private Image boostBackgroundImage;
    [SerializeField] private Image boostReserveImage;
    [SerializeField] private Gradient reserveGradient;
    [SerializeField] private AudioClip depletedAudioClip;
    [SerializeField] private AudioClip boostAuthorizedAudioClip;
    [SerializeField] private AudioSource audioSource;

   
    private void Awake()
    {
        boostManager.OnDepleted.AddListener(BoostDepleted);
        boostManager.OnCanBoostAgain.AddListener(CanBoostAgain);
    }

    private void OnDestroy()
    {
        boostManager.OnDepleted.RemoveListener(BoostDepleted);
        boostManager.OnCanBoostAgain.RemoveListener(CanBoostAgain);
    }

    void Update()
    {
        float status = boostManager.CurrentReserve / boostManager.MaxReserve;
        boostReserveImage.fillAmount = status;
        boostReserveImage.color = reserveGradient.Evaluate(status);
    }

    private void BoostDepleted()
    {
        audioSource.clip = depletedAudioClip;
        audioSource.Play();

        if (!isDepletedUICoroutineRunning)
        {
            StartCoroutine(DepletedUICoroutine());
        }
    }

    private void CanBoostAgain()
    {
        audioSource.clip = boostAuthorizedAudioClip;
        audioSource.Play();

        if (!isBoostAuthorizedCoroutineRunning)
        {
            StartCoroutine(BoostAuthorizedCoroutine());
        }
    }


    private bool isDepletedUICoroutineRunning = false;
    private IEnumerator DepletedUICoroutine()
    {
        isDepletedUICoroutineRunning = true;

        float length = 0.25f;
        float iterationsNumber = 2;
        Vector3 normalScale = boostMainRectTransform.localScale;
        Vector3 bigScale = boostMainRectTransform.localScale * 1.25f;

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
        Vector3 bigScale = boostMainRectTransform.localScale * 1.5f;

        Color boostBackgroundImageColor = boostBackgroundImage.color;
        Color boostBackgroundImageTransparent = new Color(boostBackgroundImageColor.r, boostBackgroundImageColor.g, boostBackgroundImageColor.b, 0f);
        Color boostReserveImageColor = boostReserveImage.color;
        Color boostReserveImageTransparent = new Color(boostReserveImageColor.r, boostReserveImageColor.g, boostReserveImageColor.b, 0f);

        float lerp = 0f;

        while (lerp < 1)
        {
            yield return null;
            lerp += Time.deltaTime / length;
            boostMainRectTransform.localScale = Vector3.Lerp(bigScale, normalScale, lerp);
            boostReserveImage.color = Color.Lerp(boostReserveImageTransparent, boostReserveImageColor, lerp);
            boostBackgroundImage.color = Color.Lerp(boostBackgroundImageTransparent, boostBackgroundImageColor, lerp);
        }

        boostMainRectTransform.localScale = normalScale;
        boostReserveImage.color = boostReserveImageColor;
        boostBackgroundImage.color = boostBackgroundImageColor;

        isBoostAuthorizedCoroutineRunning = false;
    }
}
