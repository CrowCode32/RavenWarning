using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonHighlight : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI highlight;

    public void Start()
    {
        if (highlight != null)
        {
            highlight.enabled = false;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (highlight != null)
        {
            highlight.enabled = true;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlight != null)
        {
            highlight.enabled = false;
        }

    }
}
