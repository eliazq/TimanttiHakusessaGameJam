using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSelector : MonoBehaviour
{
    public static RandomObjectSelector Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }
    public GameObject GetRandomObjectFromWeightList(List<WeightedGameObject> objects)
    {
        // Calculate the total weight
        float totalWeight = 0f;
        foreach (var obj in objects)
        {
            totalWeight += obj.weight;
        }

        // Pick a random number between 0 and totalWeight
        float randomValue = Random.Range(0, totalWeight);

        // Find the object corresponding to the random value
        float cumulativeWeight = 0f;
        foreach (var obj in objects)
        {
            cumulativeWeight += obj.weight;
            if (randomValue <= cumulativeWeight)
            {
                return obj.gameObject;
            }
        }

        return null; // Shouldn't reach here if weights are assigned correctly
    }
}

