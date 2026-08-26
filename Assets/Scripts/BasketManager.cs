using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class BasketManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI basketText;

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            basketText.text = Vector3.Distance(GameManager.Instance.PlayerCam.transform.position, transform.position).ToString("F2") + "m";
            basketText.transform.LookAt(GameManager.Instance.PlayerCam.transform);
            basketText.transform.Rotate(0f, 180f, 0f);
        }
    }
}
