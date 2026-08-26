using UnityEngine;

public class BallThrower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ballPrefab; // needs Rigidbody + Collider
    [SerializeField] private Transform playerCam;   // drag the actual Camera transform here

    [Header("Spawn Point (relative to camera)")]
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, -0.3f, 0.5f);

    [Header("Throw Tuning")]
    [SerializeField] private float minThrowSpeed = 2f;
    [SerializeField] private float maxThrowSpeed = 12f;
    [SerializeField] private float throwSpeedMultiplier = 8f; // scales swipe speed -> launch speed
    [SerializeField] private float maxArcAngle = 35f;          // how much upward swipe tilts the throw

    private GameObject currentBall;
    private Rigidbody currentBallRb;
    private Vector2 swipeStartPos;
    private float swipeStartTime;
    private bool isSwiping = false;

    void Update()
    {
        // Only allow throwing once a basket exists
        if (GameManager.Instance == null || GameManager.Instance.Objects.Count == 0)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    BeginSwipe(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isSwiping) EndSwipe(touch.position);
                    break;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                BeginSwipe(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0) && isSwiping)
            {
                EndSwipe(Input.mousePosition);
            }
        }
    }

    void BeginSwipe(Vector2 screenPos)
    {
        if (ballPrefab == null || playerCam == null) return;

        isSwiping = true;
        swipeStartPos = screenPos;
        swipeStartTime = Time.time;

        Vector3 spawnPos = playerCam.TransformPoint(spawnOffset);
        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        currentBallRb = currentBall.GetComponent<Rigidbody>();
        if (currentBallRb != null)
        {
            currentBallRb.isKinematic = true; // frozen while aiming
        }
    }

    void EndSwipe(Vector2 screenPos)
    {
        isSwiping = false;

        if (currentBall == null) return;

        Vector2 swipeDelta = screenPos - swipeStartPos;
        float swipeTime = Mathf.Max(Time.time - swipeStartTime, 0.01f);

        // Normalize by screen height so speed feels consistent across devices
        float swipeSpeed = (swipeDelta.magnitude / Screen.height) / swipeTime;
        float launchSpeed = Mathf.Clamp(swipeSpeed * throwSpeedMultiplier, minThrowSpeed, maxThrowSpeed);

        // Vertical swipe component controls arc angle (upward swipe = more arc)
        float verticalRatio = Mathf.Clamp01(swipeDelta.y / Screen.height);
        float arcAngle = verticalRatio * maxArcAngle;

        // Horizontal swipe nudges direction left/right slightly
        float horizontalRatio = Mathf.Clamp(swipeDelta.x / Screen.width, -1f, 1f);

        Vector3 baseDirection = playerCam.forward;
        Vector3 throwDirection = Quaternion.AngleAxis(-arcAngle, playerCam.right) * baseDirection;
        throwDirection = Quaternion.AngleAxis(horizontalRatio * 15f, Vector3.up) * throwDirection;
        throwDirection.Normalize();

        if (currentBallRb != null)
        {
            currentBallRb.isKinematic = false;
            currentBallRb.linearVelocity = throwDirection * launchSpeed;
        }

        currentBall = null;
        currentBallRb = null;
    }
}