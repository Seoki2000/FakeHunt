using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniBar : MonoBehaviour
{
    [SerializeField] Slider _hpBar;
    [SerializeField] Text _txtName;

    Transform _viewRoot;    // ���� �ð��� �����ų� ���Ͱ� �־����� UI�� ���ֱ� ���ؼ�
    Transform _tfCam;
    float _checkTime = 0;
    float _visibleTime = 4; // ���̴� �ð��� ���ؼ�


    void Update()
    {
        if (_viewRoot.gameObject.activeSelf)    // �������� ���
        {
            _checkTime += Time.deltaTime;       // �ð��� üũ���ְ�
            if(_checkTime >= _visibleTime)      // �ð��� ������
            {
                _checkTime = 0;                 // üũ�ð� �ʱ�ȭ
                ViewMiniBar(false);             // �̴Ϲ� ������ �ϱ�
            }
        }
        transform.LookAt(_tfCam);
    }

    public void InitData(string name, float rate = 1)   
    {
        // float rate�� ���ִ� ������ �̺�Ʈ ����ó�� �ǰ� �������� �����ϴ� ��� �� �׷��� ������ �⺻�����δ� 1�̰� �ٸ��� ���ؼ��� �̷��� ���ִ°� ����.
        _txtName.text = name;
        _hpBar.value = rate;
        _viewRoot = transform.GetChild(0);
        _tfCam = Camera.main.transform;

        _viewRoot.gameObject.SetActive(false);  // �̷����ϸ� �������� ���� �ʾƵ� �ؿ� �ִ� �༮���� �� ������.
    }

    public void ViewMiniBar(bool isOn)  // ������ ų�� ������ �ؾ��Ѵٸ� ���⼭ �ϸ� �ȴ�.
    {
        _viewRoot.gameObject.SetActive(isOn);
    }
    public void OpenMiniBar(float rate)
    {
        // ������ �Ǹ� Ÿ�̸� �ʱ�ȭ 
        ViewMiniBar(true);
        _hpBar.value = rate;

        _checkTime = 0;
    }



}
