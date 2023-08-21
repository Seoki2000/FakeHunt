using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _minText;
    [SerializeField] TextMeshProUGUI _secText;

    void Awake()
    {
        gameObject.SetActive(false);
    }
    public void OnTimer()
    {
        gameObject.SetActive(true);
    }

    public void InitData(float min, float sec)
    {
        _minText.text = min.ToString();
        _secText.text = sec.ToString();
    }
    
    



}
