using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeUIhandler : MonoBehaviour
{
    
    public static UpgradeUIhandler Instance;
    
    public GameObject prefab;
    public GameObject UIContainer;
    public TMP_Text RoundText;
    
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        PlayerManager.Instance.SetUpgrades();
    }

    public void SetUpgrades(List<ItemClass> upgrades, float round)
    {
        RoundText.text = $"You've made it to Round {round}!";
        foreach (ItemClass upgrade in upgrades)
        {
            GameObject upgradeUI = Instantiate(prefab, UIContainer.transform);
            Button buttonobject = upgradeUI.GetComponent<Button>();
            Transform Item = upgradeUI.transform.Find("Item");
            Transform UpgradeName = upgradeUI.transform.Find("UpgradeName");
            Transform ItemDescription = upgradeUI.transform.Find("ItemDescription");
            
            Item.GetComponent<Image>().sprite = upgrade.ItemIcon;
            UpgradeName.GetComponent<TMP_Text>().text = upgrade.ItemUpgradeName;
            ItemDescription.GetComponent<TMP_Text>().text = upgrade.ItemDescription;
            
            ItemClass CapturedUpgrade = upgrade;
            buttonobject.onClick.AddListener(() => SelectedUpgrade(CapturedUpgrade));
        }
    }
    
    public void SelectedUpgrade(ItemClass upgrade)
    {
        PlayerManager.Instance.AllowUpgrades(upgrade);
        PlayerManager.Instance.StartNextRound();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
