using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
public class FaceManager : MonoBehaviour
{
    public static FaceManager Instance { get; private set; }

    [SerializeField] ARFaceManager faceManager;
    public GameObject facePrefab;
    [SerializeField] TextMeshProUGUI faceFilterText;
    int index = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeFaceFilter()
    {
        faceFilterText.text = "Function Start";

        if(index >= facePrefab.GetComponent<FaceMask>().faceMasks.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }

        facePrefab.GetComponent<FaceMask>().SetFace(index);
        
        faceFilterText.text = "Face Filter: " + (index + 1);
    }
}
