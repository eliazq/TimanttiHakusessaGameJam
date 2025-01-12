using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinerMerchantUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MinerMerchant minerMerchant;
    [SerializeField] GameObject merchantUI;
    [SerializeField] GameObject allRocksSoldText;
    [SerializeField] Transform spritePricesParent;
    [SerializeField] Transform spritePriceContainer;


    private void Start()
    {
        if (minerMerchant == null) GetComponent<MinerMerchant>();

        for (int i = 0; i < minerMerchant.Gems.Count; i++)
        {
            Transform newSpritePriceContainer = Instantiate(spritePriceContainer, spritePricesParent);

            newSpritePriceContainer.GetChild(0).GetComponent<Image>().sprite = minerMerchant.Gems[i].Data.itemIcon;
            newSpritePriceContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = FormatMoney(minerMerchant.Gems[i].Cost);
        }

        minerMerchant.OnInteract += MinerMerchant_OnInteract;
        minerMerchant.OnRockSold += MinerMerchant_OnRockSold;
    }

    private void MinerMerchant_OnRockSold(object sender, MinerMerchant.RockSoldEventArgs e)
    {
        if (e.allRocksSold) allRocksSoldText.SetActive(true);
    }

    private void MinerMerchant_OnInteract(object sender, System.EventArgs e)
    {
        merchantUI.SetActive(!merchantUI.activeSelf);
    }

    string FormatMoney(int amount)
    {
        if (amount >= 1000000)
            return (amount / 1000000f).ToString("0.#") + "M";
        else if (amount >= 1000)
            return (amount / 1000f).ToString("0.#") + "k";
        else
            return amount.ToString();
    }

}
