using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinerMerchant : MonoBehaviour, IInteractable
{
    [SerializeField] int rockCost = 750;
    [SerializeField] Transform rockButtonsContainer;
    [SerializeField] GameObject rockButton;

    // Events
    public event EventHandler<RockSoldEventArgs> OnRockSold;
    public class RockSoldEventArgs : EventArgs
    {
        public bool allRocksSold;
    }
    public event EventHandler OnInteract;

    public List<Gem> Gems = new List<Gem>();
    [SerializeField] List<WeightedGameObject> gems;

    int maxRocks = 4;
    int rocks;
    bool isInteracting;

    private void Awake()
    {
        rocks = UnityEngine.Random.Range(1, maxRocks + 1);
        
        foreach(WeightedGameObject gem in gems)
            Gems.Add(gem.gameObject.GetComponent<Gem>());
    }

    private void Start()
    {
        CreateRockButtons();
    }
    
    private void CreateRockButtons()
    {
        for (int i = 0; i < rocks; i++)
        {
            GameObject newRockButton = Instantiate(rockButton, rockButtonsContainer);
            newRockButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (rockButtonsContainer.GetComponent<GridLayoutGroup>().enabled) rockButtonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
                if (TrySellRock())
                    Destroy(newRockButton);
            });
        }

    }

    private bool TrySellRock()
    {
        if (Player.Instance.Inventory.TryGetItem("Coin", out Item coin) && coin.Amount >= rockCost)
        {
            coin.Amount -= rockCost;
        }
        else return false;

        GameObject randomGem = 
            Instantiate(RandomObjectSelector.Instance.GetRandomObjectFromWeightList(gems));
        Player.Instance.Inventory.AddItem(randomGem.GetComponent<Item>());

        rocks--;
        
        OnRockSold?.Invoke(this, new RockSoldEventArgs { allRocksSold = rocks <= 0 });
        return true; // Rock Sold!
    }

    public void BuyRock()
    {
        if (Player.Instance.Inventory.TryGetItem("Rock", out Item rock))
        {
            Item coin = ItemManager.CreateItem("Coin", rock.GetComponent<Rock>().Price);
            Player.Instance.Inventory.AddItem(coin);
            Player.Instance.Inventory.DestroyItem(rock);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        if (Player.Instance.Controller.MovementActive) { Player.Instance.Controller.DisableMovement(); isInteracting = true; }
        else { Player.Instance.Controller.EnableMovement(); isInteracting = false; }

        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    public string GetInteractText()
    {
        if (!isInteracting) return "Open Merchant's Menu";
        else return "Close Merchant's Menu";
    }
}
