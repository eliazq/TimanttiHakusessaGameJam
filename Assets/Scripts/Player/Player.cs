using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("References")]
    [SerializeField] private Inventory inventory;

    [SerializeField] int startMoney = 2400;


    [Header("Properties")]
    public PlayerController Controller { get; private set; }
    public Inventory Inventory { get { return inventory; } }

    public bool IsMining { get { return isMining; } }

    float timer = 0;
    [SerializeField] float rockGatherInterval = 2;

    bool isMining;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        Controller = GetComponent<PlayerController>();

        Inventory.AddItem(ItemManager.CreateItem("Coin", startMoney));
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
        Controller.InputsActive = false;
        Controller.MovementActive = false;
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
            Inventory.AddItem(ItemManager.CreateItem("Rock"));
        }
    }
    public void StopMiningRock()
    {
        isMining = false;
        Controller.InputsActive = true;
        Controller.MovementActive = true;
    }
}
