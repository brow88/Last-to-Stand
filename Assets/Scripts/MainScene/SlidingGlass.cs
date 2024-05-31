using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SlidingGlass : MonoBehaviour
{
    [FormerlySerializedAs("startPosition")] [Header("Positions")]
    public Transform StartPosition;
    public Transform EndPosition;
    public Transform CatchPosition;

    [Header("Settings")]
    public float Speed = 50.0f;
    [SerializeField] private float CatchRadius = 1.0f;
    [SerializeField] private bool moveToStart = true;
    public Player Player;

    private Vector3 targetPosition;
    private bool isCaught = false;
    private bool isMoving = false;

    private bool isReady = false;

    private void Start()
    {
       
    }

    public void SetReady()
    {
        isReady = true;
        // Set the initial position based on the moveToStart flag
        transform.position = moveToStart ? StartPosition.position : EndPosition.position;
        targetPosition = moveToStart ? EndPosition.position : StartPosition.position;
        isMoving = true;
        gameObject.PlaySound(SoundManager.Instance.FindClip("GlassSlide"));
    }

    private void Update()
    {
        if (!isReady)
            return;
        if (isMoving && !isCaught)
        {
            MoveGlass();
        }

        if (InputManager.Instance.CatchGlassPlayerOneInput())
        {
            TryCatchGlass();
        }
    }

    private void MoveGlass()
    {

        // Move the glass towards the target position
        var newPos = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
        transform.position = newPos;

        // Check if the glass has reached the end position
        var distanceToEnd = Vector3.Distance(newPos, targetPosition);
        if (distanceToEnd < 0.1f)
        {
            isMoving = false;
            OnReachEndPosition();
        }
    }

    private void TryCatchGlass()
    {
        // Check if the glass is at the catch position
        var distanceToCatch = Vector3.Distance(transform.position, CatchPosition.position);
        if (distanceToCatch < CatchRadius) 
        {
            isCaught = true;
            isMoving = false;
            OnGlassCaught();
        }
        else
        {
            Debug.Log($"Glass too far away to be caught! -> distance: {distanceToCatch}");
        }
    }

    private void OnGlassCaught()
    {
        Debug.Log("Glass Caught! -> give it to the player");
        Player.GotDrink();
        Destroy(this.gameObject);
    }

    private void OnReachEndPosition()
    {
        if (!isCaught)
        {
            Debug.Log("Glass fell on the floor! get ready to be slapped!");
            gameObject.GetComponent<SpriteRenderer>().sprite = null;
            gameObject.PlaySound(SoundManager.Instance.FindClip("GlassBreak"));
            Bartender.Instance.NotifyThatPlayerDroppedGlass(Player);
        }
    }

    public void ResetGlass()
    {
        // Reset the glass to the initial state
        isCaught = false;
        isMoving = true;
        transform.position = moveToStart ? StartPosition.position : EndPosition.position;
        targetPosition = moveToStart ? EndPosition.position : StartPosition.position;
    }
}
