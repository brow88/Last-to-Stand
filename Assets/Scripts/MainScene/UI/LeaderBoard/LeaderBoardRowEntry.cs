using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardRowEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rankTextMesh;
    [SerializeField] TextMeshProUGUI playerNameTextMesh;
    [SerializeField] TextMeshProUGUI scoreTextMesh;

    public void Init(RowEntryData rowEntryData)
    {
        rankTextMesh.text = rowEntryData.Rank.ToString();
        playerNameTextMesh.text = rowEntryData.LeaderBoardEntry.PlayerName;
        scoreTextMesh.text = rowEntryData.LeaderBoardEntry.Score.ToString();
    }
}


