using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] GameObject coinPrefab;

    [SerializeField] int startMoney = 2400;

    [SerializeField] private Inventory inventory;

    [SerializeField] GameObject pickaxeVisual;

    public PlayerController controller;
    public Inventory Inventory { get { return inventory; } }

    public bool IsMining { get { return isMining; } }

    float timer = 0;
    [SerializeField] float rockGatherInterval = 2;
    [SerializeField] GameObject rockPrefab;

    bool isMining;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        controller = GetComponent<PlayerController>();

        if (startMoney <= 0) return;
        GameObject coinObj = Instantiate(coinPrefab, null);
        Coin coin = coinObj.GetComponent<Coin>();
        coin.Amount = startMoney;
        inventory.AddItem(coin);
    }

    private void Update()
    {
        if (isMining)
        {
            AddRocksToPlayerAfterTime();
        }
    }

    public void StartMiningRock(RockMine targetRockMine)
    {
        isMining = true;
        pickaxeVisual.SetActive(true);
        controller.InputsActive = false;
        controller.MovementActive = false;
        Transform target = targetRockMine.transform;
        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y; // Keep the current object's Y position
        transform.LookAt(targetPosition); // Rotate the object to face the target
    }

    private void AddRocksToPlayerAfterTime()
    {
        timer += Time.deltaTime;
        if (timer > rockGatherInterval)
        {
            timer = 0;
            Item rock = Instantiate(rockPrefab).GetComponent<Item>();
            Inventory.AddItem(rock);
        }
    }
    public void StopMiningRock()
    {
        isMining = false;
        pickaxeVisual.SetActive(false);
        controller.InputsActive = true;
        controller.MovementActive = true;
    }
}
