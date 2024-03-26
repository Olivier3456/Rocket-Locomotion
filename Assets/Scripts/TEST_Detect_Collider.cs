using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TEST_Detect_Collider : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MovingLoopCoroutine());
    }


    private IEnumerator MovingLoopCoroutine()
    {
        while (true)
        {
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(-0.1f, 0.1f);
            float randomZ = Random.Range(-1f, 1f);
            Vector3 randomDirection = new Vector3(randomX, randomY, randomZ).normalized;
            float radius = 0.5f;
            float randomDistance = Random.value * 50f;

            if (!Physics.SphereCast(transform.position, radius, randomDirection, out RaycastHit hit, randomDistance))
            {
                Debug.Log("Target Location is OK: no obstacle on the way.");
                Vector3 originalPosition = transform.position;
                Vector3 destination = originalPosition + (randomDirection * randomDistance);

                float lerp = 0f;
                float speed = 4f;
                float distance = Vector3.Distance(originalPosition, destination);
                float length = distance / speed;

                while (lerp < 1f)
                {
                    yield return null;
                    lerp += Time.deltaTime / length;
                    transform.position = Vector3.Lerp(originalPosition, destination, lerp);
                }
            }
            else
            {
                Debug.Log("An obstacle is on the way to target location. Waiting next frame to find new valid destination.");
                yield return null;
            }
        }
    }
}
