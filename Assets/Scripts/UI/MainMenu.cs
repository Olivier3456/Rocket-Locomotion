using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject gameModesChoice;
    [SerializeField] private GameObject freeFlightLevelsChoice;
    [SerializeField] private GameObject raceLevelsChoice;
    [Space(20), Header("Game Event Prefabs")]
    [SerializeField] private EventNone eventNone_NY1_StrongWind;
    [SerializeField] private EventRace eventRace1;



    private void Start()
    {
        main.SetActive(true);
        freeFlightLevelsChoice.SetActive(false);
    }



    // MAIN MENU BUTTONS
    public void MainMenu_StartButton()
    {
        main.SetActive(false);
        gameModesChoice.SetActive(true);
    }

    public void MainMenu_SettingsButton()
    {

    }



    // GAME MODE CHOICE MENU BUTTONS
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



    // FREE FLIGHT MENU BUTTONS
    public void FreeFlightMenu_NY1Button()
    {
        MainManager.Instance.LoadScene(MySceneManager.NY1_SCENE_BUILD_INDEX, eventNone_NY1_StrongWind);
    }

    public void FreeFlightMenu_NY2Button()
    {
        //MainManager.Instance.LoadScene(3);
    }

    public void FreeFlightMenu_BackButton()
    {
        freeFlightLevelsChoice.SetActive(false);
        gameModesChoice.SetActive(true);
    }



    // RACE MENU BUTTONS
    public void RaceMenu_Race1Button()
    {
        MainManager.Instance.LoadScene(MySceneManager.NY1_SCENE_BUILD_INDEX, eventRace1);
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
