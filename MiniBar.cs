using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniBar : MonoBehaviour
{
    [SerializeField] Slider _hpBar;
    [SerializeField] Text _txtName;

    Transform _viewRoot;    // 일정 시간이 지나거나 몬스터가 멀어지면 UI를 꺼주기 위해서
    Transform _tfCam;
    float _checkTime = 0;
    float _visibleTime = 4; // 보이는 시간을 위해서


    void Update()
    {
        if (_viewRoot.gameObject.activeSelf)    // 켜져있을 경우
        {
            _checkTime += Time.deltaTime;       // 시간을 체크해주고
            if(_checkTime >= _visibleTime)      // 시간이 지나면
            {
                _checkTime = 0;                 // 체크시간 초기화
                ViewMiniBar(false);             // 미니바 꺼지게 하기
            }
        }
        transform.LookAt(_tfCam);
    }

    public void InitData(string name, float rate = 1)   
    {
        // float rate를 해주는 이유는 이벤트 몬스터처럼 피가 절반으로 시작하는 경우 등 그런게 있으니 기본적으로는 1이고 다른걸 위해서는 이렇게 해주는게 좋다.
        _txtName.text = name;
        _hpBar.value = rate;
        _viewRoot = transform.GetChild(0);
        _tfCam = Camera.main.transform;

        _viewRoot.gameObject.SetActive(false);  // 이렇게하면 컨버스를 끄지 않아도 밑에 있는 녀석들은 다 꺼진다.
    }

    public void ViewMiniBar(bool isOn)  // 끌때나 킬때 무엇을 해야한다면 여기서 하면 된다.
    {
        _viewRoot.gameObject.SetActive(isOn);
    }
    public void OpenMiniBar(float rate)
    {
        // 오픈이 되면 타이머 초기화 
        ViewMiniBar(true);
        _hpBar.value = rate;

        _checkTime = 0;
    }



}
