using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNone : MonoBehaviour, IGameEvent
{

    public bool IsEventFinished()
    {
        return false;
    }

    public bool IsEventStarted()
    {
        return true;
    }

    public bool IsPauseAllowed()
    {
        return true;
    }

    public bool IsUnpauseAllowed()
    {
        return true;
    }

    //public void RegisterToMainManager()
    //{
    //    MainManager.Instance.RegisterOngoingEvent(this);
    //}

    //public void UnregisterToMainManager()
    //{
    //    MainManager.Instance.UnregisterOngoingEvent(this);
    //}    
}
