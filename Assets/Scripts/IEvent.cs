using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{
    void RegisterToMainManager();
    void UnregisterToMainManager();
    bool IsPauseAllowed();
    bool IsUnpauseAllowed();
    bool IsEventStarted();
    bool IsEventFinished();
}
