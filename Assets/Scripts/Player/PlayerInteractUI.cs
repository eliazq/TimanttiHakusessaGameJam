using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerInteractUI : MonoBehaviour {

    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactTextMeshProUGUI;
    [SerializeField] private TextMeshProUGUI interactKeyTextMeshProUGUI;

    private void Update() {
        if (playerInteract.GetInteractableObject() != null) {

            if (playerInteract.GetInteractableObject().GetTransform().TryGetComponent(out Item item))
            {
                // if interactable item is not in player inventory show interact text 
                if (!Player.Instance.Inventory.HasItem(item))
                {
                    Show(playerInteract.GetInteractableObject());
                }
            }
            else
            {
                Show(playerInteract.GetInteractableObject());
            }
        }
        else
        {
            Hide();
        }
    }

    private void Show(IInteractable interactable) {
        containerGameObject.SetActive(true);
        interactTextMeshProUGUI.text = interactable.GetInteractText();
    }

    private void Hide() {
        containerGameObject.SetActive(false);
    }

}