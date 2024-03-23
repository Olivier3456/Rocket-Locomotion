using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunImpactEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;

    private Queue<GunImpactEffect> queue;



    public void Initialize(Queue<GunImpactEffect> queue)
    {
        this.queue = queue;
    }


    public void DoYourEffect(Vector3 position, Vector3 up)
    {
        transform.position = position;
        transform.forward = up;

        gameObject.SetActive(true);
        particle.Play();
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
