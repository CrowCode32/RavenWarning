using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;



    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        if (instance == null) instance = this;

        keybinds["Jump"] = KeyCode.Space;
        keybinds["Fire1"] = KeyCode.Mouse0;
        keybinds["Dash"] = KeyCode.LeftShift;
        keybinds["Interact"] = KeyCode.E;


    }

    public KeyCode GetKey(string action)
    {
        return keybinds[action];
    }

    public void SetKey(string action, KeyCode newKey)
    {
        keybinds[action] = newKey;
    }
}
