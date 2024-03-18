using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePlayerReferencesToMainManager : MonoBehaviour
{
    [SerializeField] private GameMenu gameMenu;
    [SerializeField] private Rigidbody playerRigidbody;

    private void Awake()
    {
        MainManager.Instance.SetPlayerRigidbody(playerRigidbody);
        MainManager.Instance.SetGameMenu(gameMenu);
    }

    private void OnDestroy()
    {
        MainManager.Instance.SetPlayerRigidbody(null);
        MainManager.Instance.SetGameMenu(null);
    }
}
