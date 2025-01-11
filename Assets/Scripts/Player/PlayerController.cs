using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    [Header("References")]
    [Space(10)]
    [SerializeField] Camera mainCamera;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] private LayerMask movableLayer;
    [SerializeField] private float clickMaxDistance = 100f;
    [SerializeField] private float walkingSpeed = 1.5f;
    [SerializeField] private float runningSpeed = 2.5f;

    [Header("DEBUG")]
    [SerializeField] Vector3 agentDestination;

    // Events
    public event EventHandler<MovementClickEventArgs> OnMovementClick;
    public class MovementClickEventArgs : EventArgs
    {
        public Vector3 clickPosition;
    }
    public event EventHandler OnDestinationReached;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f; // Maximum stamina
    [SerializeField] private float staminaDrainRate = 1.5f; // Stamina drain rate per second
    [SerializeField] private float staminaRegenRate = 1f; // Stamina regen rate per interval
    [SerializeField] private float regenInterval = 4f; // Interval for stamina regeneration

    [Header("Properties")]
    public float Stamina { get { return currentStamina; } private set { currentStamina = value; } }
    private float currentStamina;
    private float regenTimer;

    public bool InputsActive { get; set; } = true;
    public bool MovementActive { get; set; } = true;
    private float MovementSpeed
    {
        get
        {
            return movementSpeed;
        }
        set
        {
            movementSpeed = value;
            if (movementSpeed > walkingSpeed) IsRunning = true;
            else if (!navAgent.isStopped) isWalking = true;
            else isWalking = false;
            navAgent.speed = movementSpeed;
        }
    }
    public bool IsWalking 
    { 
        get { return isWalking; }
        private set 
        {
            isWalking = value;
            if (isWalking) isRunning = false;
        } 
    }
    public bool IsRunning
    {
        get { return isRunning; }
        set
        {
            isRunning = value;
            if (isRunning) isWalking = false;
        }
    }
    public bool IsMoving { get { return !IsWalking && !IsRunning; } }

    [Header("Variables")]
    private float movementSpeed;
    private bool isWalking;
    private bool isRunning;

    private void Start()
    {
        MovementSpeed = walkingSpeed;
        Stamina = maxStamina; // Initialize stamina
        regenTimer = 0f; // Initialize regeneration timer
    }

    private void Update()
    {
        HandleMovementInput();
        HandleStamina();
        HandleMovementSpeed();
        DebugUpdate();
    }

    private void DebugUpdate()
    {
        agentDestination = navAgent.destination;
    }

    private void HandleMovementSpeed()
    {
        if (!MovementActive)
        {
            MovementSpeed = walkingSpeed;
            navAgent.destination = transform.position;
            return;
        }
        if (Stamina <= 0)
        {
            MovementSpeed = walkingSpeed;
        }
        // Check if reached destination
        float stoppingThreshold = 0.1f;
        if (Vector3.Distance(transform.position, navAgent.destination) < stoppingThreshold)
        {
            OnDestinationReached?.Invoke(this, EventArgs.Empty);
            IsWalking = false;
            navAgent.isStopped = true;
        }
    }

    private void HandleMovementInput()
    {
        if (!InputsActive) return;
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
            OnMovementClick?.Invoke(this, new MovementClickEventArgs { clickPosition = hit.point });
            return hit.point;
        }
        return transform.position;
    }

    public void TriggerRunning()
    {
        if (!IsRunning && Stamina > 0) MovementSpeed = runningSpeed;
        else MovementSpeed = walkingSpeed;
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

    public void EnableMovement()
    {
        MovementActive = true;
    }
    public void DisableMovement()
    {
        MovementActive = false;
        navAgent.destination = transform.position;
    }

}
