using UnityEngine;

public class UIHandler : MonoBehaviour
{

    public PlayerManager manager;
    public GameObject Conveyer;

    public void PlaceConveyerClick()
    {
        manager.StartPlacing(Conveyer);
    }


}
