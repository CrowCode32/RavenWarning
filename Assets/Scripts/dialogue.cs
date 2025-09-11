using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using System;

public class dialogue : MonoBehaviour
{   
    public TextMeshProUGUI textField;
    public string[] lines;
    public float textSpeed;

    public TextAsset dialogueFile;
    int index;
    public bool isRunning;


    void OnEnable()
    {
        textField.text = string.Empty;
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isRunning || lines == null) { return; }
        
        if (Input.GetButtonDown("Interact"))
        {
            if (textField.GetParsedText() == lines[index])
            {
                nextLine();
            } else
            {
                StopAllCoroutines();
                textField.text = lines[index];
            }
        }
    }

    public void startDialogue(TextAsset dialogueInput)
    {
        isRunning = true;

        string allText = dialogueInput.text;
        lines = allText.Split("\n");

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach(char c in lines[index].ToCharArray())
        {
            textField.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    
    void nextLine()
    {
        if(index < lines.Length - 1)
        {
            index++;
            textField.text = string.Empty;
            StartCoroutine(TypeLine());
        } else
        {
            isRunning = false;
            gameObject.SetActive(false);
        }
    }
}
