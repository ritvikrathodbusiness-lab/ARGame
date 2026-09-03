using UnityEngine;
using System.Collections.Generic; 
using TMPro;
using System.Collections; 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public List<GameObject> Objects = new List<GameObject>();
    [SerializeField] TextMeshProUGUI scoreText;
    public GameObject PlayerCam;
    [SerializeField] int score;
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

    public void ScoreBasket()
    {
        score++;
        scoreText.text = "Score: " + score.ToString();
        Debug.Log("Score: " + score);
    }
}
