using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    // Variables
    
    public PlayerManager manager;
    public GameObject UIContainer;
    public GameObject ButtonPrefab;

    // Data
    
    List<ItemClass> items;
    public Sprite[] sprites;
    public GameObject[] prefabs;
    
    // Additional Required Item Data
    
    
    // Methods Handling

    void Start()
    {
        items = new List<ItemClass>()
        {
            new ItemClass(prefabs[0], "Conveyer", "Conveyer", sprites[0], true)
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
    
}
