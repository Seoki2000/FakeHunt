using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineHelper;

public class MiniMapBox : MonoBehaviour
{
    [SerializeField] RectTransform _miniMapPos;
    [SerializeField] Camera _miniMapCamera;
    // 플러스 버튼, 마이너스 버튼이 있어야 하고 이걸 통해서 미니맵의 확대 및 축소를 해야한다. 
    // 카메라의 orthographicSize를 줄이거나 늘려서 줌을 하는 방식으로 해보자. 
    // 현재 카메라를 못가지고 온다는 에러가 뜨는데 명령어가 계속 실행될려고함(카메라 가져오는 방식을 바꿔야한다.)

    float _maxZoomIn = 5;
    float _maxZoomOut = 54.5f;
    float _zoomClick = 5;
    bool _isOpen;

    void Awake()
    {
        _miniMapCamera = GetComponent<Camera>();    // 이렇게 가져오는게 맞을텐데.....
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
            //_miniMapCamera.orthographicSize = _miniMapCamera.orthographicSize - _zoomClick;   이렇게 해도 못가지고 온다 
            _miniMapCamera.orthographicSize += _zoomClick;
        }
    }


}
