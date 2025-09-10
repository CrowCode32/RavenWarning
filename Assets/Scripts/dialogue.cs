using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;

public class dialogue : MonoBehaviour
{
    [SerializeField] TextAsset dialogueFile;
    public TextMeshProUGUI textField;
    public string[] lines;
    public float textSpeed;

    int index;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textField.text = string.Empty;
        startDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (textField.text == lines[index])
            {
                nextLine();
            } else
            {
                StopAllCoroutines();
                textField.text = lines[index];
            }
        }
    }

    public void startDialogue()
    {
        index = 0;

        string allText = dialogueFile.text;
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
            gameObject.SetActive(false);
        }
    }
}
