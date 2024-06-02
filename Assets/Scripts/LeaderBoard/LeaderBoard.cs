using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dan.Main;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
  
    private const int MAX_LEADER_BOARD_LENGTH = 25;

    private List<LeaderBoardEntry> leaderBoardEntryList;

    [Header("Prefabs")]
    [SerializeField] private GameObject RowEntryPrefab;
    [SerializeField] private GameObject InputRowPrefab;

    [Header("")]
    [SerializeField] private Transform leaderBoardContainer;
    [SerializeField] private ScrollRect scrollRect;

    private DateTime lastGameTime;

    public void Start()
    {
       StartCoroutine(DownloadLeaderBoard());
    }

    #region Server Upload/Download

    IEnumerator UploadNewScore(string username, int score)
    {
        Leaderboards.LastToStand.UploadNewEntry(username,score);
        yield return null;
    }


    IEnumerator DownloadLeaderBoard()
    {
        if ((DateTime.Now - lastGameTime).TotalSeconds<30)
        {
            yield break;
        }
        leaderBoardEntryList = new List<LeaderBoardEntry>();
        Leaderboards.LastToStand.GetEntries(ProcessDownload);
        yield return null;
    }

    private void ProcessDownload(Dan.Models.Entry[] entries)
    {
        lastGameTime = DateTime.Now;
        leaderBoardEntryList = new List<LeaderBoardEntry>();
        entries = entries.ToList().OrderBy(o=>o.Rank).ToArray();
        foreach (var entry in entries)
        {
            string entryName = System.Web.HttpUtility.UrlDecode(entry.Username);
            int score = entry.Score;
            leaderBoardEntryList.Add(new LeaderBoardEntry(entryName, score));
        };

        //First empty the leader board
        foreach (Transform child in leaderBoardContainer.transform)
        {
            if (child.gameObject.GetComponent<LeaderBoardInputRow>() != null)
            {
                continue;
            }
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
    }


    public void CreateLeaderBoardWithNewEntry(LeaderBoardEntry newEntry)
    {
        StartCoroutine(CreateLeaderBoardWithNewEntryCoroutine(newEntry));
    }


    private IEnumerator CreateLeaderBoardWithNewEntryCoroutine(LeaderBoardEntry newEntry)
    {
        //Update the current leader board list
        yield return DownloadLeaderBoard();
        
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



