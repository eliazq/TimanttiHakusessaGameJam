using UnityEngine;

public class PlayerMovementVisual : MonoBehaviour
{

    [SerializeField] GameObject clickWorldVisualPrefab;
    GameObject visualObject;

    private void Start()
    {
        visualObject = Instantiate(clickWorldVisualPrefab, null);
        Player.Instance.Controller.OnMovementClick += Controller_OnMovementClick;
        Player.Instance.Controller.OnDestinationReached += Controller_OnDestinationReached;
    }

    private void Controller_OnDestinationReached(object sender, System.EventArgs e)
    {
        visualObject.SetActive(false);
    }

    private void Controller_OnMovementClick(object sender, PlayerController.MovementClickEventArgs e)
    {
        Vector3 clickPosition = e.clickPosition;

        if (!visualObject.activeSelf) visualObject.SetActive(true);
        visualObject.transform.position = clickPosition;
        visualObject.transform.Rotate(Vector3.up * Random.Range(0, 360f));
    }
}
