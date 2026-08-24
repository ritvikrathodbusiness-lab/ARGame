using UnityEngine;
using System.Collections.Generic; 
using System.Collections; 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> Objects = new List<GameObject>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearObjects()
    {
        foreach (var obj in Objects)
        {
            Destroy(obj);
        }
        Objects.Clear();
    }
}
