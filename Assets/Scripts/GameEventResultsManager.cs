using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum GameEvent { Race1, Race2, Race3, ProtectTheSkyLord1, SurviveDroneAttack1 };

public static class GameEventResultsManager
{
    [Serializable]
    public class RaceScores
    {
        public RaceScore[] scores;
    }

    [Serializable]
    public class RaceScore
    {
        public string playerName;
        public string date;
        public float time;
    }

    [Serializable]
    public class SurviveDroneAttackScores
    {
        public SurviveDroneAttackScore[] scores;
    }

    [Serializable]
    public class SurviveDroneAttackScore
    {
        public string playerName;
        public string date;
        public int kills;
    }


    private static string path;
    private static int maxResultsNumber = 10;
    private static RaceScores raceScores;
    public static RaceScore RaceScoreAdded { get; private set; }

    private static SurviveDroneAttackScores surviveDroneAttackScores;
    public static SurviveDroneAttackScore SurviveDroneAttackScoreAdded { get; private set; }


    public static RaceScores AddRaceScore(GameEvent gameEventRace, float time)
    {
        LoadRaceScoresFromFile(gameEventRace);

        RaceScore newRaceResult = new RaceScore();
        newRaceResult.date = DateTime.Now.ToString("MM/dd/yyyy");
        newRaceResult.time = time;
        newRaceResult.playerName = "No Name Yet";
        RaceScoreAdded = newRaceResult;

        ReorganizeRaceScores(newRaceResult);

        //Debug.Log($"Race scores added to array: date = {newRaceResult.date}, time = {newRaceResult.time}, player name = {newRaceResult.playerName}. Total race scores number: {raceScores.scores.Length}.");

        SaveRaceScoresToFile();

        return raceScores;
    }

    public static SurviveDroneAttackScores AddDroneAttackScore(GameEvent gameEventSurviveDroneAttack, int kills)
    {
        LoadSurviveDroneAttackScoresFromFile(gameEventSurviveDroneAttack);

        SurviveDroneAttackScore newScore = new SurviveDroneAttackScore();
        newScore.date = DateTime.Now.ToString("MM/dd/yyyy");
        newScore.kills = kills;
        newScore.playerName = "No Name Yet";
        SurviveDroneAttackScoreAdded = newScore;

        ReorganizeSurviveDroneAttackScores(newScore);

        //Debug.Log($"Survive drone attack scores added to array: date = {newScore.date}, kills = {newScore.kills}, player name = {newScore.playerName}. Total race scores number: {surviveDroneAttackScores.scores.Length}.");

        SaveSurviveDroneAttackScoresToFile();

        return surviveDroneAttackScores;
    }



    private static RaceScores LoadRaceScoresFromFile(GameEvent gameEvent)
    {
        if (gameEvent == GameEvent.Race1)
        {
            path = Application.persistentDataPath + "/Race1Score.json";
        }
        else if (gameEvent == GameEvent.Race2)
        {
            path = Application.persistentDataPath + "/Race2Score.json";
        }
        else if (gameEvent == GameEvent.Race3)
        {
            path = Application.persistentDataPath + "/Race3Score.json";
        }

        if (!File.Exists(path))
        {
            Debug.Log("No file to load yet");
            raceScores = new RaceScores();
            return raceScores;
        }

        string jsonData = File.ReadAllText(path);

        try
        {
            raceScores = JsonUtility.FromJson<RaceScores>(jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading JSON data: " + e.Message);
            raceScores = new RaceScores();
        }

        //Debug.Log("Race results loaded from file");

        return raceScores;
    }

    private static SurviveDroneAttackScores LoadSurviveDroneAttackScoresFromFile(GameEvent gameEvent)
    {
        if (gameEvent == GameEvent.SurviveDroneAttack1)
        {
            path = Application.persistentDataPath + "/SurviveDroneAttackScore1.json";
        }

        if (!File.Exists(path))
        {
            //Debug.Log("No file to load yet");
            surviveDroneAttackScores = new SurviveDroneAttackScores();
            return surviveDroneAttackScores;
        }

        string jsonData = File.ReadAllText(path);

        try
        {
            surviveDroneAttackScores = JsonUtility.FromJson<SurviveDroneAttackScores>(jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading JSON data: " + e.Message);
            surviveDroneAttackScores = new SurviveDroneAttackScores();
        }

        //Debug.Log("Survive drone attack scores loaded from file");

        return surviveDroneAttackScores;
    }



    private static void ReorganizeRaceScores(RaceScore newRaceResult) // (Chat GPT 3.5)
    {
        // S'il n'y a pas encore de r�sultats, on cr�e un tableau avec juste le nouveau r�sultat.
        if (raceScores.scores == null)
        {
            raceScores.scores = new RaceScore[1] { newRaceResult };
            return; // Fin de la fonction apr�s l'initialisation.
        }

        // Ajouter le nouveau r�sultat � la liste existante
        List<RaceScore> tempList = raceScores.scores.ToList();
        tempList.Add(newRaceResult);
        raceScores.scores = tempList.ToArray();

        // Trier les r�sultats par temps
        Array.Sort(raceScores.scores, (x, y) => x.time.CompareTo(y.time));


        // Limiter la taille des r�sultats au maximum autoris�
        if (raceScores.scores.Length > maxResultsNumber)
        {
            Array.Resize(ref raceScores.scores, maxResultsNumber);
        }
    }

    private static void ReorganizeSurviveDroneAttackScores(SurviveDroneAttackScore newScore)
    {
        if (surviveDroneAttackScores.scores == null)
        {
            surviveDroneAttackScores.scores = new SurviveDroneAttackScore[1] { newScore };
            return; // Fin de la fonction apr�s l'initialisation.
        }

        // Ajouter le nouveau r�sultat � la liste existante
        List<SurviveDroneAttackScore> tempList = surviveDroneAttackScores.scores.ToList();
        tempList.Add(newScore);
        surviveDroneAttackScores.scores = tempList.ToArray();

        // Trier les r�sultats par nombre de drones tu�s
        Array.Sort(surviveDroneAttackScores.scores, (x, y) => y.kills.CompareTo(x.kills));

        // Limiter la taille des r�sultats au maximum autoris�
        if (surviveDroneAttackScores.scores.Length > maxResultsNumber)
        {
            Array.Resize(ref surviveDroneAttackScores.scores, maxResultsNumber);
        }
    }



    private static void SaveRaceScoresToFile()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        string jsonData = JsonUtility.ToJson(raceScores, true);
        File.WriteAllText(path, jsonData);

        Debug.Log("Race results saved in file " + path);
        path = null;
    }

    private static void SaveSurviveDroneAttackScoresToFile()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        string jsonData = JsonUtility.ToJson(surviveDroneAttackScores, true);
        File.WriteAllText(path, jsonData);

        Debug.Log("Survive drones attack results saved in file " + path);
        path = null;
    }


    //public static void WipeData()
    //{
    //    raceScores = new RaceScores();
    //    SaveRaceScoresToFile();
    //    Debug.Log("Race results reinitialized");
    //}
}
