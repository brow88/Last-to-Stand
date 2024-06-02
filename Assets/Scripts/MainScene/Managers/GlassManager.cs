using UnityEngine;

public class GlassManager : MonoBehaviour
{
    [Header("Player 1 Positions")]
    public Transform StartPositionPlayer1;
    public Transform  EndPositionPlayer1;
    public Transform  CatchPositionPlayer1;

    [Header("Player 2 Positions")]
    public Transform StartPositionPlayer2;
    public Transform EndPositionPlayer2;
    public Transform CatchPositionPlayer2;

    public static GlassManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public Vector3 GetPlayer1StartPos()
    {
        return new Vector3(StartPositionPlayer1.position.x, StartPositionPlayer1.position.y, StartPositionPlayer1.position.z);
    }

    public Vector3 GetPlayer1EndPos()
    {
        return new Vector3(EndPositionPlayer1.position.x, EndPositionPlayer1.position.y, EndPositionPlayer1.position.z);
    }

    public Vector3 GetPlayer2StartPos()
    {
        return new Vector3(StartPositionPlayer1.position.x, StartPositionPlayer1.position.y, StartPositionPlayer1.position.z);
    }

    public Vector3 GetPlayer2EndPos()
    {
        return new Vector3(EndPositionPlayer1.position.x, EndPositionPlayer1.position.y, EndPositionPlayer1.position.z);
    }

}