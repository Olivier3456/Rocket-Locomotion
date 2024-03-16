using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaceCheckpoint : MonoBehaviour
{
    [SerializeField] private EventRace race;
    [Space(20)]
    [SerializeField] private Renderer[] renderers;
    [Space(20)]
    [SerializeField] private Material nextCheckpointMat;
    [Space(20)]
    [SerializeField] private ParticleSystem particles;

    private const string PLAYER_TAG = "Player";

    public static UnityEvent<RaceCheckpoint> OnCheckpointReached = new UnityEvent<RaceCheckpoint>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            OnCheckpointReached.Invoke(this);
        }
    }

    public void YouAreNext()
    {
        foreach (var rdr in renderers)
        {
            rdr.material = nextCheckpointMat;
            particles.Play();
        }
    }
}
