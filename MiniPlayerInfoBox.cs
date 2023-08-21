using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniPlayerInfoBox : MonoBehaviour
{
    [SerializeField] Text _txtName;
    [SerializeField] Text _txtAttValue;
    [SerializeField] Text _txtDefValue;


    void Awake()
    {
        _txtName = GetComponent<Text>();
        _txtAttValue = GetComponent<Text>();
        _txtDefValue = GetComponent<Text>();
    }
    
    public void InitSetData(string name, int att, int def)
    {
        gameObject.SetActive(true);
        _txtName.text = name;
        _txtAttValue.text = att.ToString();
        _txtDefValue.text = def.ToString();
    }

    public void CloseBox()
    {
        gameObject.SetActive(false);
    }

}

