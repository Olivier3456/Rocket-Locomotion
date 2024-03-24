using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunImpactEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioSource audioSource;

    private Queue<GunImpactEffect> queue;



    public void Initialize(Queue<GunImpactEffect> queue)
    {
        this.queue = queue;
    }


    public void DoYourEffect(RaycastHit hit)
    {
        transform.position = hit.point;
        transform.forward = hit.normal;

        gameObject.SetActive(true);
        particle.Play();

        if (hit.distance < 100f)
        {
            float pitchDelta = 0.4f;
            float audioSourceRandomPitch = Random.Range(1 - (pitchDelta * 0.5f), 1 + (pitchDelta * 0.5f));
            audioSource.pitch = audioSourceRandomPitch;
            audioSource.Play();
        }

        StartCoroutine(WaitForParticleEnd_Coroutine());
    }


    private IEnumerator WaitForParticleEnd_Coroutine()
    {
        while (particle.isPlaying)
        {
            yield return null;
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        queue.Enqueue(this);
        gameObject.SetActive(false);
    }
}
