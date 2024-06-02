using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dan.Main;
using Dan.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    private const string PRIVATE_CODE = "I_vqDlVLkkmUEJrU0S2FCwG8uMiTRWjUGboCJLeXWMPQ";
    private const string PUBLIC_CODE = "6655214f8f40bb12c85d57c6";
    private const string WEB_URL = "http://dreamlo.com/lb/";
    private const int MAX_LEADER_BOARD_LENGTH = 25;

    private List<LeaderBoardEntry> leaderBoardEntryList;

    [Header("Prefabs")]
    [SerializeField] private GameObject RowEntryPrefab;
    [SerializeField] private GameObject InputRowPrefab;

    [Header("")]
    [SerializeField] private Transform leaderBoardContainer;
    [SerializeField] private ScrollRect scrollRect;


    #region Server Upload/Download

    IEnumerator UploadNewScore(string name, int score)
    {
        Leaderboards.LastToStand.UploadNewEntry(name,score);
        yield return null;
        //UnityWebRequest www = UnityWebRequest.Get(WEB_URL + PRIVATE_CODE + "/add/" + UnityWebRequest.EscapeURL(name) + "/" + score);
        //yield return www.SendWebRequest();
    }


    IEnumerator DownloadLeaderBoard()
    {
        leaderBoardEntryList = new List<LeaderBoardEntry>();
        Leaderboards.LastToStand.GetEntries(ProcessRawDownload);
        
        yield return null;

       //UnityWebRequest www = UnityWebRequest.Get(WEB_URL + PUBLIC_CODE + "/pipe");
       // yield return www.SendWebRequest();

       // string rawDownload = www.downloadHandler.text;
       // ProcessRawDownload(rawDownload);
    }

    private void ProcessRawDownload(Dan.Models.Entry[] entries)
    {
        leaderBoardEntryList = new List<LeaderBoardEntry>();
        entries = entries.ToList().OrderByDescending(o=>o.Rank).ToArray();
        foreach (var entry in entries)
        {
            string entryName = System.Web.HttpUtility.UrlDecode(entry.Username);
            int score =entry.Score;
            leaderBoardEntryList.Add(new LeaderBoardEntry(entryName, score));
        };

        //First empty the leader board
        foreach (Transform child in leaderBoardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //Create new leader board
        int rankCount = 1;
        foreach (LeaderBoardEntry entry in leaderBoardEntryList)
        {
            LeaderBoardRowEntry row = Instantiate(RowEntryPrefab, leaderBoardContainer).GetComponent<LeaderBoardRowEntry>();
            row.Init(rankCount, entry);
            rankCount++;
        }
    }


    private void ProcessRawDownload(string rawDownload)
    {
        leaderBoardEntryList = new List<LeaderBoardEntry>();
        
        string[] entries = rawDownload.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string entryName = System.Web.HttpUtility.UrlDecode(entryInfo[0]);
            int score = int.Parse(entryInfo[1]);
            leaderBoardEntryList.Add(new LeaderBoardEntry(entryName, score));
        }
    }

    #endregion

    #region Creating Leader Board


    public void CreateLeaderBoard()
    {
        StartCoroutine(CreateLeaderBoardCoroutine());
    }


    private IEnumerator CreateLeaderBoardCoroutine()
    {
        //Update the current leader board list
        yield return DownloadLeaderBoard();

        //First empty the leader board
        foreach (Transform child in leaderBoardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //Create new leader board
        int rankCount = 1;
        foreach (LeaderBoardEntry entry in leaderBoardEntryList)
        {
            LeaderBoardRowEntry row = Instantiate(RowEntryPrefab, leaderBoardContainer).GetComponent<LeaderBoardRowEntry>();
            row.Init(rankCount, entry);
            rankCount++;
        }
    }


    public void CreateLeaderBoardWithNewEntry(LeaderBoardEntry newEntry)
    {
        StartCoroutine(CreateLeaderBoardWithNewEntryCoroutine(newEntry));
    }


    private IEnumerator CreateLeaderBoardWithNewEntryCoroutine(LeaderBoardEntry newEntry)
    {
        //Update the current leader board list
        yield return DownloadLeaderBoard();

        //First empty the leader board
        foreach (Transform child in leaderBoardContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Insert the new entry if it qualifies for the leader board
        bool entryInserted = false;
        int newEntryRank = 0;
        foreach (LeaderBoardEntry entry in leaderBoardEntryList)
        {
            if (entry.Score < newEntry.Score)
            {
                leaderBoardEntryList.Insert(newEntryRank, newEntry);               
                entryInserted = true;
                break;
            }
            newEntryRank++;
        }

        //If there is space at the bottom then add the entry
        if (!entryInserted && leaderBoardEntryList.Count < MAX_LEADER_BOARD_LENGTH)
        {
            leaderBoardEntryList.Add(newEntry);
            entryInserted = true;
        }


        // If the new entry was inserted and the list exceeds the max length, remove the last entry
        if (entryInserted && leaderBoardEntryList.Count > MAX_LEADER_BOARD_LENGTH) 
        {          
            leaderBoardEntryList.RemoveAt(leaderBoardEntryList.Count - 1);
        }

        //Create new leader board
        int rankCount = 1;
        foreach (LeaderBoardEntry entry in leaderBoardEntryList)
        {
            if (entryInserted && newEntryRank == (rankCount - 1f))
            {
                LeaderBoardInputRow row = Instantiate(InputRowPrefab, leaderBoardContainer).GetComponent<LeaderBoardInputRow>();
                row.Init(rankCount, newEntry.Score, this);
            }
            else
            {
                LeaderBoardRowEntry row = Instantiate(RowEntryPrefab, leaderBoardContainer).GetComponent<LeaderBoardRowEntry>();
                row.Init(rankCount, entry);               
            }
            rankCount++;
        }

        //scroll to the new rect entry
        if (entryInserted)
        {
            ScrollToRowEntry(newEntryRank);
        }
    }

    #endregion
   
    public void SubmitPlayerName(LeaderBoardEntry entry)
    {
        StartCoroutine(SubmitPlayerNameCoroutine(entry));
    }

    private IEnumerator SubmitPlayerNameCoroutine(LeaderBoardEntry entry)
    {
        yield return StartCoroutine(UploadNewScore(entry.PlayerName, entry.Score));
        StartCoroutine(CreateLeaderBoardCoroutine());
    }

    public void ScrollToRowEntry(int rowEntry)
    {
        float totalEntries = leaderBoardEntryList.Count;

        if (rowEntry < 0 || rowEntry > totalEntries)
        {
            Debug.LogError("Row " + rowEntry + "does not exist" );
        }
        else
        {
            if (totalEntries == 1)
            {
                //do not need to scroll to entry if first entry i.e 1
                return;
            }

            Canvas.ForceUpdateCanvases();

            //If row is valid scroll to it
            float targetNormalizedPosition = 1f - (rowEntry / (totalEntries));

            scrollRect.verticalNormalizedPosition = targetNormalizedPosition;
        }
    }
}

public struct LeaderBoardEntry
{
    public string PlayerName;
    public int Score;

    public LeaderBoardEntry(string playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }
}



