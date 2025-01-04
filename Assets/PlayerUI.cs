using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    PlayerController playerController;

    [SerializeField] TextMeshProUGUI staminaText;

    private void Start()
    {
        playerController = Player.Instance.GetComponent<PlayerController>();
    }

    private void Update()
    {
        staminaText.text = $"{(int)playerController.Stamina}%";
    }
}
