using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCheckpoint : MonoBehaviour
{
    [SerializeField] private EventRace race;
    [Space(20)]
    [SerializeField] private Renderer[] renderers;
    [Space(20)]
    [SerializeField] private Material nextCheckpointMat;

    private const string PLAYER_TAG = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            race.CheckpointReached(this);
        }
    }

    public void YouAreNext()
    {
        foreach (var rdr in renderers)
        {
            rdr.material = nextCheckpointMat;
        }
    }
}
