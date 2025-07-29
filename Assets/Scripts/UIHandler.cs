using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    public static UIHandler Instance;
    
    // Variables
    
    public GameObject UIContainer;
    public GameObject ButtonPrefab;
    public TMP_Text FuelText;
    public TMP_Text TimeLeft;
    public GameObject ToolTipText;
    public AudioSource audioSource;
    public AudioClip ToggleSound;

    // Data
    
    // Methods Handling

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateUIButtons(List<ItemClass> items )
    {
        foreach (Transform item in UIContainer.transform)
        {
            Destroy(item.gameObject);
        }
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

    public void StartRound(List<ItemClass> items)
    {
        CreateUIButtons(items);
    }
    
    void ButtonClicked(ItemClass item)
    {
        PlayerManager.Instance.ItemPlacingPreview(item);
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(ToggleSound);
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
