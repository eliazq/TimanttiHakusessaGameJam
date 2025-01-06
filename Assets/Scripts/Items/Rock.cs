using UnityEngine;

public class Rock : Item
{
    public int Price { get { return price; } }
    [SerializeField] int price = 100;
    public override void OnInteract(Transform interactorTransform)
    {
        Player.Instance.Inventory.AddItem(this);
    }
}
