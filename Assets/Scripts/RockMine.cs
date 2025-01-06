using UnityEngine;

public class RockMine : MonoBehaviour, IInteractable
{
    public string GetInteractText()
    {
        if (Player.Instance.IsMining)
            return "Stop Mining";
        else
            return "Start Mining";
    } 

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        if (Player.Instance.IsMining) Player.Instance.StopMiningRock();
        else 
            Player.Instance.StartMiningRock(this);
    }
}
