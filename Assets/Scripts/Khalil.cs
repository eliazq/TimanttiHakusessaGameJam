using UnityEngine;

public class Khalil : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject winningScreen;

    bool winning = false;
    public string GetInteractText()
    {
        if (Player.Instance.Inventory.HasItem("Cullinan Diamond"))
            return "Give Diamond To Khalil";
        else return "Find Cullinan Diamond";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void Update()
    {
        if (winning)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) Application.Quit();
        }
    }

    public void Interact(Transform interactorTransform)
    {
        if (!Player.Instance.Inventory.HasItem("Cullinan Diamond")) return;
        // Player Has Diamond
        WinGame();
    }

    public void WinGame()
    {
        winning = true;
        winningScreen.SetActive(true);
    }
}
