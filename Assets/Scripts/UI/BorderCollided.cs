using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderCollided : MonoBehaviour
{
    [SerializeField] private GameObject borderColliderImageGameObject;
    [SerializeField] private PhysicalContactsManager physicalContactManager;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip borderColliderAudioClip;

    private WaitForSeconds wait = new WaitForSeconds(0.33f);

    void Start()
    {
        borderColliderImageGameObject.SetActive(false);
        physicalContactManager.OnBorderCollided.AddListener(OnBorderCollided);
    }


    private void OnBorderCollided()
    {
        StartCoroutine(BorderColliderCoroutine());
    }

    IEnumerator BorderColliderCoroutine()
    {
        playerAudioSource.PlayOneShot(borderColliderAudioClip);
        borderColliderImageGameObject.SetActive(true);
        yield return wait;
        borderColliderImageGameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        physicalContactManager.OnBorderCollided.RemoveListener(OnBorderCollided);
    }
}
