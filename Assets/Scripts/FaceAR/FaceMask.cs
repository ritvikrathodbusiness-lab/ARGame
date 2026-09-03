using UnityEngine;
using System.Collections.Generic;

public class FaceMask : MonoBehaviour
{
    public List<GameObject> faceMasks = new List<GameObject>();

    void Start()
    {
        FaceManager.Instance.facePrefab = this.gameObject;
    }

    public void SetFace(int i)
    {
        for (int j = 0; j < faceMasks.Count; j++)
        {
            faceMasks[j].SetActive(j == i);
        }
    }
}
