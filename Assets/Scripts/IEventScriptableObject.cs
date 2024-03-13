using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IEvents/Base IEvent")]
public class IEventScriptableObject : ScriptableObject
{
    public int sceneIndex;
    public GameObject IEventPrefab;
}
