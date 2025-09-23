using UnityEngine;
using UnityEngine.UI;

public class pickups : MonoBehaviour
{
    GameData gameData = GameManager.instance.gameData;

    [SerializeField] trinket trinket;
    [SerializeField] feather feather;
    
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null && trinket != null)
        {
            Debug.Log("Yep thats a trinket");
            pickupable.getTrinket(trinket);
            Debug.Log(trinket.trinketNum);
            gameData.trinketStat++;

            GameManager.instance.trinketSlots[trinket.trinketNum].name.text = trinket.name;
            GameManager.instance.trinketSlots[trinket.trinketNum].description.text = trinket.trinketDesc;
            GameManager.instance.trinketSlots[trinket.trinketNum].unlocked.image.sprite = trinket.sprite;
            GameManager.instance.EnableJournalEntryTrinket(trinket.trinketNum);
            GameManager.instance.trinketsAquired.Add(trinket);
            GameManager.instance.UpdateTrinketDropdown();
            Destroy(gameObject);
        }
        else if(pickupable != null && feather != null)
        {
            Debug.Log("Nah thats a feather");
            gameData.featherStat++;
            pickupable.getFeather(feather);
            GameManager.instance.feathersAquired.Add(feather);
            GameManager.instance.UpdateFeatherDropdown();
            Destroy(gameObject);
        }
    }
}
