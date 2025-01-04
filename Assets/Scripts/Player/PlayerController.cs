using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] private float clickMaxDistance = 100f;
    [SerializeField] private LayerMask movableLayer;
    [SerializeField] private GameObject flagVisual;
    [SerializeField] private float walkingSpeed = 1.5f;
    [SerializeField] private float runningSpeed = 2.5f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f; // Maximum stamina
    [SerializeField] private float staminaDrainRate = 1.5f; // Stamina drain rate per second
    [SerializeField] private float staminaRegenRate = 1f; // Stamina regen rate per interval
    [SerializeField] private float regenInterval = 4f; // Interval for stamina regeneration
    
    public float Stamina { get { return currentStamina; } private set { currentStamina = value; } }
    private float currentStamina;
    private float regenTimer;

    public bool IsWalking { get; private set; } = true;
    public bool IsRunning { get; private set; }
    public bool IsMoving { get { return !IsWalking && !IsRunning; } }

    GameObject lastFlag;

    private void Start()
    {
        navAgent.speed = walkingSpeed;
        Stamina = maxStamina; // Initialize stamina
        regenTimer = 0f; // Initialize regeneration timer
    }

    private void Update()
    {
        HandleMovementInput();
        HandleStamina();
        HandleMovementSpeed();
    }

    private void HandleMovementSpeed()
    {
        if (Stamina <= 0)
        {
            navAgent.speed = walkingSpeed;
            IsRunning = false;
            IsWalking = true;
        }
        float stoppingThreshold = 0.1f;
        if (Vector3.Distance(transform.position, navAgent.destination) < stoppingThreshold)
        {
            IsWalking = false;
            IsRunning = false;
            Destroy(lastFlag);
        }
        else
        {
            IsWalking = false;
            IsRunning = false;
            if (navAgent.speed == walkingSpeed) IsWalking = true;
            else if (navAgent.speed == runningSpeed) IsRunning = true;
        }
    }

    private void HandleMovementInput()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            MoveToMousePosition();
        }
    }

    private void MoveToMousePosition()
    {
        Vector3 mouseClickInWorld = GetMouseClickPointInWorld();
        if (mouseClickInWorld != transform.position)
        {
            navAgent.destination = mouseClickInWorld;
        }
    }

    private Vector3 GetMouseClickPointInWorld()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, clickMaxDistance, movableLayer))
        {
            if (lastFlag != null) Destroy(lastFlag);
            lastFlag = Instantiate(flagVisual, hit.point, Quaternion.identity);
            float randomAngle = Random.Range(0f, 360f);
            lastFlag.transform.Rotate(Vector3.up, randomAngle);
            return hit.point;
        }
        return transform.position;
    }

    public void TriggerRunning()
    {
        if (!IsRunning && Stamina > 0)
        {
            navAgent.speed = runningSpeed;
        }
        else
        {
            navAgent.speed = walkingSpeed;
        }
    }

    private void HandleStamina()
    {
        if (IsRunning && Stamina > 0)
        {
            // Drain stamina while running
            Stamina -= staminaDrainRate * Time.deltaTime;
        }
        else if (!IsRunning)
        {
            // Regenerate stamina if not running
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenInterval)
            {
                Stamina = Mathf.Min(Stamina + staminaRegenRate, maxStamina);
                regenTimer = 0f; // Reset the regeneration timer
            }
        }

        // Clamp stamina between 0 and maxStamina
        Stamina = Mathf.Clamp(Stamina, 0, maxStamina);
    }

}
