using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    // Variables
    
    public PlayerManager manager;
    public GameObject UIContainer;
    public GameObject ButtonPrefab;
    public TMP_Text FuelText;
    public TMP_Text TimeLeft;
    public GameObject ToolTipText;

    // Data
    
    List<ItemClass> items;
    public Sprite[] sprites;
    public GameObject[] prefabs;
    
    // Methods Handling

    void Start()
    {
        items = new List<ItemClass>()
        {
            new ItemClass(prefabs[0], "Conveyer", "Conveyer", sprites[0], true),
            new ItemClass(prefabs[1], "Miner", "Miner", sprites[1], true)
        };
        CreateUIButtons();
    }

    void CreateUIButtons()
    {
        foreach (ItemClass item in items)
        {
            if (item.Unlocked)
            {
                GameObject button = Instantiate(ButtonPrefab, UIContainer.transform);
                Button buttonobject = button.GetComponent<Button>();
                Transform Item = button.transform.Find("Item");
                Item.GetComponent<Image>().sprite = item.ItemIcon;
                ItemClass selecteditem = item;
                buttonobject.onClick.AddListener(() => ButtonClicked(selecteditem));
            }
        }
    }

    void ButtonClicked(ItemClass item)
    {
        manager.ItemPlacingPreview(item);
    }

    public void UpdateScoreUI(float Time, float Score, float Scoregoal)
    {
        TimeLeft.text = $"{Mathf.RoundToInt(Time)}";
        FuelText.text = $"{Score} / {Scoregoal}";
    }

    public void ToggleToopTip(bool CurrentlyPlacing)
    {
        if (CurrentlyPlacing == true)
        {
            ToolTipText.SetActive(true);
        }
        else
        {
            ToolTipText.SetActive(false);
        }
    }
    
}
