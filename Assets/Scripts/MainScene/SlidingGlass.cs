using Unity.VisualScripting;
using UnityEngine;

public class SlidingGlass : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private Transform catchPosition;

    [Header("Settings")]
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float CatchRadius = 1.0f;
    [SerializeField] private bool moveToStart = true;
    [SerializeField] private Player player;

    private Vector3 targetPosition;
    private bool isCaught = false;
    private bool isMoving = false;

    private void Start()
    {
        // Set the initial position based on the moveToStart flag
        transform.position = moveToStart ? startPosition.position : endPosition.position;
        targetPosition = moveToStart ? endPosition.position : startPosition.position;
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving && !isCaught)
        {
            MoveGlass();
        }

        if (InputManager.Instance.CatchGlassInput())
        {
            TryCatchGlass();
        }
    }

    private void MoveGlass()
    {

        // Move the glass towards the target position
        var newPos = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
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
        var distanceToCatch = Vector3.Distance(transform.position, catchPosition.position);
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
        player.GotDrink();
        Destroy(this.gameObject);
    }

    private void OnReachEndPosition()
    {
        if (!isCaught)
        {
            Debug.Log("Glass fell on the floor! get ready to be slapped!");
            Bartender.Instance.NotifyThatPlayerDroppedGlass(player);
        }
    }

    public void ResetGlass()
    {
        // Reset the glass to the initial state
        isCaught = false;
        isMoving = true;
        transform.position = moveToStart ? startPosition.position : endPosition.position;
        targetPosition = moveToStart ? endPosition.position : startPosition.position;
    }
}
