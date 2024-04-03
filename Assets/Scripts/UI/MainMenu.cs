using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject gameModesChoice;
    [SerializeField] private GameObject freeFlightLevelsChoice;
    [SerializeField] private GameObject windChoice;
    [SerializeField] private GameObject raceLevelsChoice;
    [SerializeField] private GameObject combatMissionsChoice;
    [Space(20)]
    [SerializeField] private Slider windForceMinSlider;
    [SerializeField] private Slider windForceMaxSlider;
    [SerializeField] private Slider windDirectionChangeSlider;



    private void Start()
    {
        main.SetActive(true);
        gameModesChoice.SetActive(false);
        freeFlightLevelsChoice.SetActive(false);
        windChoice.SetActive(false);
        raceLevelsChoice.SetActive(false);
        combatMissionsChoice.SetActive(false);
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
        MainManager.Instance.windParameters = new WindParameters(0f, 0f, 0f);
        gameModesChoice.SetActive(false);
        freeFlightLevelsChoice.SetActive(true);
    }

    public void GameModeChoiceMenu_RaceButton()
    {
        gameModesChoice.SetActive(false);
        raceLevelsChoice.SetActive(true);
    }

    public void GameModeChoiceMenu_ProtectButton()
    {
        gameModesChoice.SetActive(false);
        combatMissionsChoice.SetActive(true);
    }

    public void GameModeChoiceMenu_BackButton()
    {
        gameModesChoice.SetActive(false);
        main.SetActive(true);
    }



    // FREE FLIGHT MENU BUTTONS
    private int freeFlightMapIndex;
    public void FreeFlightMenu_NY1Button()
    {
        //MainManager.Instance.LoadScene(MySceneManager.NEW_YORK_1_FREE_FLIGHT);
        freeFlightMapIndex = MySceneManager.NEW_YORK_1_FREE_FLIGHT;
        windChoice.SetActive(true);
    }

    public void FreeFlightMenu_NY2Button()
    {
        //MainManager.Instance.LoadScene(MySceneManager.NEW_YORK_2_FREE_FLIGHT);
        freeFlightMapIndex = MySceneManager.NEW_YORK_2_FREE_FLIGHT;
        windChoice.SetActive(true);
    }

    public void FreeFlightMenu_BackButton()
    {
        MainManager.Instance.windParameters = null;
        freeFlightLevelsChoice.SetActive(false);
        gameModesChoice.SetActive(true);
    }



    // WIND CHOICE MENU BUTTONS
    public void WindChoiceMenu_StartButton()
    {
        MainManager.Instance.LoadScene(freeFlightMapIndex);
    }

    public void WindChoiceMenu_BackButton()
    {
        windChoice.SetActive(false);
    }

    public void WindChoiceMenu_WindForceMinSlider(float value)
    {
        MainManager.Instance.windParameters.windForceMin = value;
        MainManager.Instance.windParameters.windForceMax = MainManager.Instance.windParameters.windForceMax < MainManager.Instance.windParameters.windForceMin ? MainManager.Instance.windParameters.windForceMin : MainManager.Instance.windParameters.windForceMax;
        windForceMaxSlider.value = MainManager.Instance.windParameters.windForceMax;
    }

    public void WindChoiceMenu_WindForceMaxSlider(float value)
    {
        MainManager.Instance.windParameters.windForceMax = value;
        MainManager.Instance.windParameters.windForceMin = MainManager.Instance.windParameters.windForceMin > MainManager.Instance.windParameters.windForceMax ? MainManager.Instance.windParameters.windForceMax : MainManager.Instance.windParameters.windForceMin;
        windForceMinSlider.value = MainManager.Instance.windParameters.windForceMin;
    }

    public void WindChoiceMenu_WindDirectionChangeSlider(float value)
    {
        MainManager.Instance.windParameters.windDirectionChangeDelta = value;
    }



    // RACE MENU BUTTONS
    public void RaceMenu_Race1Button()
    {
        MainManager.Instance.LoadScene(MySceneManager.RACE_1);
    }
    public void RaceMenu_Race2Button()
    {
        MainManager.Instance.LoadScene(MySceneManager.RACE_2);
    }
    public void RaceMenu_Race3Button()
    {
        MainManager.Instance.LoadScene(MySceneManager.RACE_3);
    }
    public void RaceMenu_BackButton()
    {
        raceLevelsChoice.SetActive(false);
        gameModesChoice.SetActive(true);
    }



    // COMBAT MISSIONS MENU BUTTONS
    public void CombatMissionsMenu_Protect1Button()
    {
        MainManager.Instance.LoadScene(MySceneManager.PROTECT_THE_SKYLORD_1);
    }
    public void CombatMissionsMenu_SurviveDroneAttackButton()
    {
        MainManager.Instance.LoadScene(MySceneManager.SURVIVE_DRONE_ATTACK);
    }
    public void CombatMissionsMenu_BackButton()
    {
        combatMissionsChoice.SetActive(false);
        gameModesChoice.SetActive(true);
    }
}
