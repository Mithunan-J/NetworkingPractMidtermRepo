using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextChat : MonoBehaviour
{
    private static TextChat instance;
    public GameObject TextBoxPanel;
    public GameObject TextBoxPrefab;
    
    public static TextChat GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateTextBox(string _msg)
    {
        //Debug.Log("Creating Text box");
        GameObject newTextBox = GameObject.Instantiate(TextBoxPrefab, TextBoxPanel.transform);
        newTextBox.GetComponent<TextMeshProUGUI>().text = _msg;
        //Debug.Log("Created Text box");
    }
}
