using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum GameEvent { Race1, Race2 };

public static class RaceResultsSaveLoad
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

    private static string path;
    private static int maxResultsNumber = 10;
    private static RaceScores raceScores;
    public static RaceScore ScoreAdded { get; private set; }

    public static RaceScores AddRaceScore(GameEvent gameEvent, float time)
    {
        LoadRaceScoresFromFile(gameEvent);

        RaceScore newRaceResult = new RaceScore();
        newRaceResult.date = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        newRaceResult.time = time;
        newRaceResult.playerName = "No Name Yet";
        ScoreAdded = newRaceResult;

        ReorganizeRaceScores(newRaceResult);

        //Debug.Log($"Race scores added to array: date = {newRaceResult.date}, time = {newRaceResult.time}, player name = {newRaceResult.playerName}. Total race scores number: {raceScores.scores.Length}.");

        SaveRaceScoresToFile();

        return raceScores;
    }


    private static RaceScores LoadRaceScoresFromFile(GameEvent gameEvent)
    {
        if (gameEvent == GameEvent.Race1)
        {
            path = Application.persistentDataPath + "/Race1Score.json";
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

        Debug.Log("Race results loaded from file");

        return raceScores;
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


    public static void WipeData()
    {
        raceScores = new RaceScores();
        SaveRaceScoresToFile();
        Debug.Log("Race results reinitialized");
    }
}
