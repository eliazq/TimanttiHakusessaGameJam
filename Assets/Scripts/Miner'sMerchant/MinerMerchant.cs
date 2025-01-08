using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinerMerchant : MonoBehaviour, IInteractable
{
    [SerializeField] int rockCost = 750;
    [SerializeField] GameObject minerSellingUI;
    [SerializeField] Transform rockButtonsContainer;
    [SerializeField] GameObject rockButton;
    [SerializeField] TextMeshProUGUI rockPriceText;
    [SerializeField] GameObject soldOutText;

    public bool isDiamondHolder = false;
    
    public List<Gem> Gems = new List<Gem>();
    [SerializeField] List<WeightedGameObject> gems;

    int maxRocks = 4;
    int rocks;

    private void Awake()
    {
        rocks = Random.Range(0, maxRocks + 1);

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
        rockPriceText.text = $"= {rockCost}$";
        foreach( WeightedGameObject gem in gems)
        {
            Gems.Add(gem.gameObject.GetComponent<Gem>());
        }
    }

    private void Start()
    {
        if (rocks <= 0 && isDiamondHolder)
        {
            rocks = 1;
            GameObject newRockButton = Instantiate(rockButton, rockButtonsContainer);
            newRockButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (rockButtonsContainer.GetComponent<GridLayoutGroup>().enabled) rockButtonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
                if (TrySellRock())
                    Destroy(newRockButton);
            });
        }
        
        if (rocks <= 0) soldOutText.SetActive(true);
        
        
    }
    private bool TrySellRock()
    {
        if (Player.Instance.Inventory.TryGetItem("Coin", out Item coin) && coin.Amount >= rockCost)
        {
            coin.Amount -= rockCost;
        }
        else return false;

        // GIVE DIAMOND IF HOLDER
        if (rocks <= 1 && isDiamondHolder && !Player.Instance.Inventory.HasItem("Cullinan Diamond"))
        {
            Player.Instance.Inventory.AddItem(ItemManager.CreateItem("Cullinan Diamond"));
        }
        else if (isDiamondHolder && !Player.Instance.Inventory.HasItem("Cullinan Diamond"))
        {
            int rndTwo = Random.Range(0, 3); // 0, 1, 2
            if (rndTwo == 1)
            {
                Player.Instance.Inventory.AddItem(ItemManager.CreateItem("Cullinan Diamond"));
            }
            else
            {
                GameObject randomGem = Instantiate(RandomObjectSelector.Instance.GetRandomObjectFromWeightList(gems));
                Player.Instance.Inventory.AddItem(randomGem.GetComponent<Item>());
            }
        }
        else
        {
            GameObject randomGem = Instantiate(RandomObjectSelector.Instance.GetRandomObjectFromWeightList(gems));
            Player.Instance.Inventory.AddItem(randomGem.GetComponent<Item>());
        }
        rocks--;
        
        if (rocks <= 0) soldOutText.SetActive(true);
        return true;
    }

    public void BuyRock()
    {
        if (Player.Instance.Inventory.TryGetItem("Rock", out Item rock))
        {
            Player.Instance.Inventory.AddItem(ItemManager.CreateItem("Coin", rock.GetComponent<Rock>().Price));
            Player.Instance.Inventory.DestroyItem(rock);
        }
    }

    public string GetInteractText()
    {
        if (minerSellingUI.activeSelf)
            return "Close Seller's Menu";
        else return "Open Seller's Menu";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        if (minerSellingUI.activeSelf)
        {
            Player.Instance.Controller.InputsActive = true;
            Player.Instance.Controller.MovementActive = true;
        }
        else
        {
            Player.Instance.Controller.InputsActive = false;
            Player.Instance.Controller.MovementActive = false;
        }
        TriggerSellerUI();
    }

    private void TriggerSellerUI()
    {
        minerSellingUI.SetActive(!minerSellingUI.activeSelf);
    }
}
