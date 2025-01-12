using System.Collections;
using UnityEngine;

public class RockMine : MonoBehaviour, IInteractable
{
    [Header("Gather Settings")]
    [SerializeField] float rockGatherIntervalMin = 1f;
    [SerializeField] float rockGatherIntervalMax = 8f;

    public string GetInteractText()
    {
        if (Player.Instance.IsMining)
            return "Stop Mining";
        else
            return "Start Mining";
    } 

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {

        if (!Player.Instance.IsMining)
        {
            Player.Instance.StartMiningRock(this);
            StartCoroutine(StartAddingRocksToPlayer());
        }
        else Player.Instance.StopMiningRock();
    }

    IEnumerator StartAddingRocksToPlayer()
    {
        float timer = 0;
        float gatherTime = Random.Range(rockGatherIntervalMin, rockGatherIntervalMax);

        while (Player.Instance.IsMining)
        {
            timer += Time.deltaTime;
            if (timer > gatherTime)
            {
                timer = 0;
                gatherTime = Random.Range(rockGatherIntervalMin, rockGatherIntervalMax);
                Player.Instance.Inventory.AddItem(ItemManager.CreateItem("Rock"));
            }
            yield return null;
        }
    }
}
