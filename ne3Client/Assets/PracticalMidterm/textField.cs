using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class textField : MonoBehaviour
{
    private string str;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayText()
    {
        str = this.gameObject.GetComponent<TMP_InputField>().text;
        Debug.Log(str);
    }
}
