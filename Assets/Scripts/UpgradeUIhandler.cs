using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    public void SetUpgrades(List<ItemClass> upgrades)
    {
        foreach (ItemClass upgrade in upgrades)
        {
            GameObject upgradeUI = Instantiate(prefab, UIContainer.transform);
            Button buttonobject = upgradeUI.GetComponent<Button>();
            buttonobject.onClick.AddListener(() => SelectedUpgrade(upgrade));
        }
    }

    public void SelectedUpgrade(ItemClass upgrade)
    {
        print("Add Upgrade and Start Next Round");
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
