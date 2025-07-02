using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIhandler : MonoBehaviour
{
    
    public static UpgradeUIhandler instance;
    public GameObject prefab;
    public GameObject UIContainer;
    public TMP_Text RoundText;
    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetUpgrades(List<ItemClass> upgrades)
    {
        foreach (ItemClass upgrade in upgrades)
        {
            GameObject upgradeUI = Instantiate(prefab, UIContainer.transform);
            // Button upgradeButton = upgradeUI.GetComponent<Button>();
            // upgradeButton.onClick.AddListener(() => SelectedUpgrade(upgrade));
        }
    }

    public void SelectedUpgrade(ItemClass upgrade)
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
