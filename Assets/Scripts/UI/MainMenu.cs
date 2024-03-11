using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject gameModesChoice;
    [SerializeField] private GameObject freeFlightLevelsChoice;
    [SerializeField] private GameObject raceLevelsChoice;


    private void Start()
    {
        main.SetActive(true);
        freeFlightLevelsChoice.SetActive(false);
    }



    // MAIN MENU
    public void MainMenu_StartButton()
    {
        main.SetActive(false);
        gameModesChoice.SetActive(true);
    }

    public void MainMenu_SettingsButton()
    {

    }



    // GAME MODE CHOICE MENU
    public void GameModeChoiceMenu_FreeFlightButton()
    {
        gameModesChoice.SetActive(false);
        freeFlightLevelsChoice.SetActive(true);
    }

    public void GameModeChoiceMenu_RaceButton()
    {
        gameModesChoice.SetActive(false);
        raceLevelsChoice.SetActive(true);
    }

    public void GameModeChoiceMenu_BackButton()
    {
        gameModesChoice.SetActive(false);
        main.SetActive(true);
    }



    // FREE FLIGHT MENU
    public void FreeFlightMenu_NY1Button()
    {
        MainManager.Instance.LoadScene(2);
    }

    public void FreeFlightMenu_NY2Button()
    {
        MainManager.Instance.LoadScene(3);
    }

    public void FreeFlightMenu_BackButton()
    {
        freeFlightLevelsChoice.SetActive(false);
        gameModesChoice.SetActive(true);
    }



    // RACE MENU
    public void RaceMenu_Race1Button()
    {

    }
    public void RaceMenu_Race2Button()
    {

    }
    public void RaceMenu_BackButton()
    {
        raceLevelsChoice.SetActive(false);
        gameModesChoice.SetActive(true);
    }
}
