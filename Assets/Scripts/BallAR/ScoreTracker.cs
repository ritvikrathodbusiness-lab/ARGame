using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            GameManager.Instance.ScoreBasket();
        }
    }
}
