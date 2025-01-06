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
    [SerializeField] GameObject diamondPrefab;
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
                SellRock();
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
                SellRock();
                Destroy(newRockButton);
            });
        }
        
        if (rocks <= 0) soldOutText.SetActive(true);
        
        
    }

    private void SellRock()
    {
        Player.Instance.Inventory.TryGetItem("Coin", out Item coin);

        if (coin != null && coin.Amount >= rockCost) coin.Amount -= rockCost;
        else return;

        if (coin.Amount <= 0) Player.Instance.Inventory.DestroyItem(coin);

        // GIVE DIAMOND IF HOLDER
        if (rocks <= 1 && isDiamondHolder && !Player.Instance.Inventory.HasItem("Cullinan Diamond"))
        {
            Player.Instance.Inventory.AddItem(Instantiate(diamondPrefab, null).GetComponent<Item>());
        }
        else if (isDiamondHolder && !Player.Instance.Inventory.HasItem("Cullinan Diamond"))
        {
            int rndTwo = Random.Range(0, 3); // 0, 1, 2
            if (rndTwo == 1)
            {
                Player.Instance.Inventory.AddItem(Instantiate(diamondPrefab, null).GetComponent<Item>());
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
            Player.Instance.controller.InputsActive = true;
            Player.Instance.controller.MovementActive = true;
        }
        else
        {
            Player.Instance.controller.InputsActive = false;
            Player.Instance.controller.MovementActive = false;
        }
        TriggerSellerUI();
    }

    private void TriggerSellerUI()
    {
        minerSellingUI.SetActive(!minerSellingUI.activeSelf);
    }
}
