using UnityEngine;

public interface ISellable
{
    public int price { get; set; }
    void Sell();
}
