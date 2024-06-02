using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardRowEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankTextMesh;
    [SerializeField] private TextMeshProUGUI playerNameTextMesh;
    [SerializeField] private TextMeshProUGUI scoreTextMesh;

    
    public void Init(int rank, LeaderBoardEntry rowEntryData)
    {
        rankTextMesh.text = rank.ToString();
        playerNameTextMesh.text = rowEntryData.PlayerName??string.Empty;
        scoreTextMesh.text = rowEntryData.Score.ToString();
    }
    
}


