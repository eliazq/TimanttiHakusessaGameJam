using UnityEngine;

public class Gem : Item
{
    [SerializeField] private int cost;
    public int Cost { get { return cost; } }
    public override void OnInteract(Transform interactorTransform)
    {
        Player.Instance.Inventory.AddItem(this);
    }
}
