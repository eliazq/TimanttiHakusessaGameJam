using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinerMerchant : MonoBehaviour, IInteractable
{
    [SerializeField] int rockCost = 750;
    [SerializeField] string interactText = string.Empty;
    [SerializeField] GameObject minerSellingUI;
    [SerializeField] Transform rockButtonsContainer;
    [SerializeField] GameObject rockButton;
    [SerializeField] TextMeshProUGUI rockPriceText;
    [SerializeField] GameObject diamondPrefab;

    public bool isDiamondHolder = false;

    [SerializeField] List<WeightedGameObject> gems;

    int maxRocks = 4;
    int rocks;

    private void Awake()
    {
        rocks = Random.Range(0, maxRocks);

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
    }

    private void Start()
    {
        if (rocks <= 0 && isDiamondHolder) rocks = 1;

        GameObject newRockButton = Instantiate(rockButton, rockButtonsContainer);
        newRockButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (rockButtonsContainer.GetComponent<GridLayoutGroup>().enabled) rockButtonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
            SellRock();
            Destroy(newRockButton);
        });

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
            return;
        }
        else if(isDiamondHolder && !Player.Instance.Inventory.HasItem("Cullinan Diamond"))
        {
            int rndTwo = Random.Range(0, 3); // 0, 1, 2
            if (rndTwo == 1)
            {
                Player.Instance.Inventory.AddItem(Instantiate(diamondPrefab, null).GetComponent<Item>());
                return;
            }
        }

        GameObject randomGem = Instantiate(RandomObjectSelector.Instance.GetRandomObjectFromWeightList(gems));
        Player.Instance.Inventory.AddItem(randomGem.GetComponent<Item>());

    }

    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        TriggerSellerUI();
    }

    private void TriggerSellerUI()
    {
        minerSellingUI.SetActive(!minerSellingUI.activeSelf);
    }
}
