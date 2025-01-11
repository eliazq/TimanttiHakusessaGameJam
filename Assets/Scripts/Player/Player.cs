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

    bool isMining;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        Controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        Inventory.AddItem(ItemManager.CreateItem("Coin", startMoney));
    }

    public void StartMiningRock(RockMine targetRockMine)
    {
        isMining = true;
        DisableMovement();
        LookTowards(targetRockMine.transform.position);
    }

    private void LookTowards(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y; // Keep the current object's Y position
        transform.LookAt(targetPosition); // Rotate the object to face the target
    }

    public void StopMiningRock()
    {
        isMining = false;
        EnableMovement();
    }

    private void EnableMovement()
    {
        Controller.InputsActive = true;
        Controller.MovementActive = true;       
    }
    private void DisableMovement()
    {
        Controller.InputsActive = false;
        Controller.MovementActive = false;       
    }
}
