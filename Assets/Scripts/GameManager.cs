using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private MinerMerchant[] miners;

    private void Awake()
    {
        MinerMerchant diamondMiner = miners[Random.Range(0, miners.Length)];
    }
}
