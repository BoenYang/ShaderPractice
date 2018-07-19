using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SSR : MonoBehaviour
{

    public InputField input;

	
	// Update is called once per frame
	void Update ()
	{
        input.onValidateInput += OnValidateInput;
	    string tt = "fdafdsa";
	}

    private char OnValidateInput(string text, int charIndex, char addedChar)
    {
        Debug.Log(text + " " + charIndex);
        return addedChar;
    }
}
