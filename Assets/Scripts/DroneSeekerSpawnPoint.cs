using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSeekerSpawnPoint : MonoBehaviour
{
    //private const string PLAYER_TAG = "Player";

    private static List<DroneSeekerSpawnPoint> droneSeekerSpawnPointsInPlayerRange = new List<DroneSeekerSpawnPoint>();
    public static List<DroneSeekerSpawnPoint> DroneSeekerSpawnPointsInPlayerRange { get { return droneSeekerSpawnPointsInPlayerRange; } }


    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag(PLAYER_TAG)) // No need to compare tags: Drone Spawn Point layer only reacts with Player layer.
        //{
        if (!droneSeekerSpawnPointsInPlayerRange.Contains(this))
        {
            droneSeekerSpawnPointsInPlayerRange.Add(this);
        }
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag(PLAYER_TAG))
        //{
        if (droneSeekerSpawnPointsInPlayerRange.Contains(this))
        {
            droneSeekerSpawnPointsInPlayerRange.Remove(this);
        }
        //}
    }
}
