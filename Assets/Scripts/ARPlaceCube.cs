using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlaceCube : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject cubePrefab;

    private GameObject currentObject;
    private static readonly List<ARRaycastHit> rayHits = new List<ARRaycastHit>();

    void Update()
    {
        if (!raycastManager)
        {
            Debug.LogError("ARRaycastManager is not assigned.");
            return;
        }

        // --- Touch input ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TryPlaceObject(touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    TryMoveObject(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    currentObject = null; // release, so next touch places a new cube
                    break;
            }
        }
        // --- Mouse input (editor testing) ---
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceObject(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                TryMoveObject(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                currentObject = null;
            }
        }
    }

    void TryPlaceObject(Vector2 screenPosition)
    {
        if (raycastManager.Raycast(screenPosition, rayHits, TrackableType.PlaneWithinPolygon) && GameManager.Instance.Objects.Count < 1)
        {
            Pose hitPose = rayHits[0].pose;
            currentObject = Instantiate(cubePrefab, hitPose.position, hitPose.rotation);
            GameManager.Instance.Objects.Add(currentObject);
        }
    }

    void TryMoveObject(Vector2 screenPosition)
    {
        if (currentObject == null) return;

        if (raycastManager.Raycast(screenPosition, rayHits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = rayHits[0].pose;
            currentObject.transform.position = hitPose.position;
            currentObject.transform.rotation = hitPose.rotation;
        }
    }
}