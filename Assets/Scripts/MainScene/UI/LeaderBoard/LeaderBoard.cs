using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] private GameObject RowEntryPrefab;
    [SerializeField] private Transform leaderBoardContainer;

    public void CreateLeaderBoard(List<LeaderBoardEntry> currentLeaderBoardData, LeaderBoardEntry newleaderBoardEntry)
    {
        //First empty the leader board
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //Create a list of RowEntryDatas for the leader board
        RowEntryData data = new RowEntryData();
        foreach (LeaderBoardEntry leaderBoardEntry in currentLeaderBoardData)
        {

        }

        //Add the new leader board entry to the RowEntryDatas



        //Create new leader board
        int rankCount = 0;
        foreach (LeaderBoardEntry leaderBoardEntry in currentLeaderBoardData)
        {
            rankCount++;
            LeaderBoardRowEntry rowEntry = Instantiate(RowEntryPrefab, leaderBoardContainer).GetComponent<LeaderBoardRowEntry>();
            rowEntry.Init(new RowEntryData() { Editable = false, Rank = rankCount, LeaderBoardEntry = leaderBoardEntry});
        }
    }

    public void GoToRowEntry(int rowEntry)
    {

    }
}

public struct RowEntryData
{
    public bool Editable;
    public int Rank;
    public LeaderBoardEntry LeaderBoardEntry;
}

public struct LeaderBoardEntry
{
    public string PlayerName;
    public int Score;
}



