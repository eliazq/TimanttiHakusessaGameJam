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
    public bool IsWalking { get; private set; } = true;
    public bool IsRunning { get; private set; }
    public bool IsMoving { get { return !IsWalking && !IsRunning; } }

    [Header("DEBUG")]
    Vector3 lastClickPoint;

    GameObject lastFlag;

    private void Start()
    {
        navAgent.speed = walkingSpeed;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mouseClickInWorld = GetMouseClickPointInWorld();
            if (mouseClickInWorld != transform.position)
            {
                navAgent.destination = mouseClickInWorld;
            }
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

    private Vector3 GetMouseClickPointInWorld()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, clickMaxDistance, movableLayer))
        {
            lastClickPoint = hit.point;
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
        if (!IsRunning)
        {
            navAgent.speed = runningSpeed;
        }
        else
        {
            navAgent.speed = walkingSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastClickPoint, 0.5f);
    }
}
