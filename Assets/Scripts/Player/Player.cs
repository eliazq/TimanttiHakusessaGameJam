using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] GameObject coinPrefab;

    [SerializeField] int startMoney = 5500;

    [SerializeField] private Inventory inventory;

    public PlayerController controller;
    public Inventory Inventory { get { return inventory; } }

    public bool IsMining { get { return isMining; } }

    bool isMining;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        GameObject coinObj = Instantiate(coinPrefab, null);
        Coin coin = coinObj.GetComponent<Coin>();
        coin.Amount = startMoney;
        inventory.AddItem(coin);
    }

    private void Update()
    {
        if (isMining)
        {
            AddRocksToInventoryAfterTime();
        }
    }

    private void AddRocksToInventoryAfterTime()
    {
        // TODO, ROCK ITEM, rock is only sellable, do interface, in script when sell check if item is ISellable
    }

    public void StartMiningRock(RockMine targetRockMine)
    {
        isMining = true;
        controller.InputsActive = false;
        controller.MovementActive = false;
        Transform target = targetRockMine.transform;
        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y; // Keep the current object's Y position
        transform.LookAt(targetPosition); // Rotate the object to face the target
    }
    public void StopMiningRock()
    {
        isMining = false;
        controller.InputsActive = true;
        controller.MovementActive = true;
    }
}
