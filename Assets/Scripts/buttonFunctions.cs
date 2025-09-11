using UnityEngine;

public class buttonFunctions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void featherSelect()
    {
        GameManager.instance.OnFeatherDropdownChanged();

    }
    public void trinketSelect()
    {
        GameManager.instance.OnTrinketDropdownChanged();
       

    }
}
