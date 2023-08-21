using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;        // TextMeshPro

public class InMessageBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtMessage;
    GameObject _boxBg;

    void Awake()
    {
        _boxBg = transform.GetChild(0).gameObject;
    }

    public void OpenMessageBox(string message)
    {
        _boxBg.SetActive(true);
        _txtMessage.text = message;
    }
    public void CloseMessageBox()
    {
        _boxBg.SetActive(false);
    }

}
