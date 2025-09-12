using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class trinketDescription : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button button;
    [SerializeField] Image desc;

    public void Start()
    {
        if (desc != null)
        {
            desc.gameObject.SetActive(false);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (desc != null)
        {
            desc.gameObject.SetActive(true);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (desc != null)
        {
            desc.gameObject.SetActive(false);
        }

    }
}
