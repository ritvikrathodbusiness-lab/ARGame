using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic; 
using System.Collections; 


public class ARPlaceCube : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    bool isPlacing = false;

    // Update is called once per frame
    void Update()
    {
        if(!raycastManager)
        {
            Debug.LogError("ARRaycastManager is not assigned.");
            return;
        }
         if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0) && !isPlacing)
         {
            isPlacing = true;

            if(Input.touchCount > 0)
            {
                PlaceObject(Input.GetTouch(0).position);
            }
            else
            {
                PlaceObject(Input.mousePosition);
            }

         }
    }

    void PlaceObject(Vector2 touchPosition)
    {
        var rayHits = new List<ARRaycastHit>();
        raycastManager.Raycast(touchPosition, rayHits, TrackableType.AllTypes);

        if(rayHits.Count > 0)
        {
            Vector3 hitPosition = rayHits[0].pose.position;
            Quaternion hitRotation = rayHits[0].pose.rotation;
            GameManager.Instance.Objects.Add(Instantiate(raycastManager.raycastPrefab, hitPosition, hitRotation));

        }

        StartCoroutine(SetIsPlacingFalse());

    }

    IEnumerator SetIsPlacingFalse()
    {
        yield return new WaitForSeconds(0.25f);
        isPlacing = false;
    }
}
