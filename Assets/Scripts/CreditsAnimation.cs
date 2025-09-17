using UnityEngine;

public class CreditsAnimation : MonoBehaviour
{
    public void endCredits()
    {
        GameManager.instance.loadMainMenu();
    }
}
