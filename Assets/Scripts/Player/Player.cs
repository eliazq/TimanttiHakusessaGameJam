using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] GameObject coinPrefab;

    [SerializeField] int startMoney = 5500;

    [SerializeField] private Inventory inventory;
    public Inventory Inventory { get { return inventory; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        GameObject coinObj = Instantiate(coinPrefab, null);
        Coin coin = coinObj.GetComponent<Coin>();
        coin.Amount = startMoney;
        inventory.AddItem(coin);
    }
}
