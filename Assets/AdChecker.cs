using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdChecker : MonoBehaviour {
    
    private Button button;
    private Text buttonText;
    private string text;
    private string waitingText = "Waiting...";
    
	void Start () {

        button = GetComponent<Button>();
        buttonText = transform.Find("Text").GetComponent<Text>();
        text = buttonText.text;
    }
	
	void Update () {
        if (Advertisement.IsReady())
        {
            buttonText.text = text;
            button.interactable = true;
        }
        else
        {
            buttonText.text = waitingText;
            button.interactable = false;
        }

    }
}
