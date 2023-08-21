using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineHelper;

public class MiniMapBox : MonoBehaviour
{
    [SerializeField] RectTransform _miniMapPos;
    [SerializeField] Camera _miniMapCamera;
    // �÷��� ��ư, ���̳ʽ� ��ư�� �־�� �ϰ� �̰� ���ؼ� �̴ϸ��� Ȯ�� �� ��Ҹ� �ؾ��Ѵ�. 
    // ī�޶��� orthographicSize�� ���̰ų� �÷��� ���� �ϴ� ������� �غ���. 
    // ���� ī�޶� �������� �´ٴ� ������ �ߴµ� ��ɾ ��� ����ɷ�����(ī�޶� �������� ����� �ٲ���Ѵ�.)

    float _maxZoomIn = 5;
    float _maxZoomOut = 54.5f;
    float _zoomClick = 5;
    bool _isOpen;

    void Awake()
    {
        _miniMapCamera = GetComponent<Camera>();    // �̷��� �������°� �����ٵ�.....
        _miniMapPos = GetComponent<RectTransform>();
        transform.position = _miniMapPos.transform.position;
        _isOpen = true;
    }
    public void MiniMapOpenClose()
    {
        if (_isOpen)
        {
            _miniMapPos.anchoredPosition = Vector2.zero;
        }
        else
        {
            _miniMapPos.anchoredPosition = new Vector3(1, -1, -1);
        }
    }
   

    public void MiniMapOn()
    {
        GameObject go = GetComponentInChildren<GameObject>();
        go.SetActive(true);
    }
    public void MiniMapOff()
    {
        GameObject go = GetComponentInChildren<GameObject>();
        go.SetActive(false);
    }
    public void ZoomInMap()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Camera");
        if(_maxZoomIn < _miniMapCamera.orthographicSize)
        {
            _miniMapCamera.orthographicSize -= _zoomClick;
        }
    }
    public void ZoomOutMap()
    {
        if (_maxZoomOut > _miniMapCamera.orthographicSize)
        {
            //_miniMapCamera.orthographicSize = _miniMapCamera.orthographicSize - _zoomClick;   �̷��� �ص� �������� �´� 
            _miniMapCamera.orthographicSize += _zoomClick;
        }
    }


}
