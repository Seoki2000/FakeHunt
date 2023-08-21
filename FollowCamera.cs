using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Vector3 _offsetPos = Vector3.zero; // 오프셋 값이 먼저 필요하다(얼마만큼 멀리 둘지)
    [SerializeField] Vector3 _direction = Vector3.zero; // 카메라 각도를 주기 위해서 
    [SerializeField] int _pointsNum = 10;               // 포인트 갯수 
    [SerializeField] float _moveSpeed = 6.5f;           // 카메라 움직이는 속도 

    Transform _tfCam;       // 카메라 위치
    Transform _tfTarget;    // 타겟의 위치

    CameraActionKind _nowKind;
    List<Vector3> _movePoints;
    GameObject _camerObj;


    bool _isArrive;
    

    void Awake()
    {
        _tfCam = transform;
        _movePoints = new List<Vector3>();
        _isArrive = false;
    }
    void Start()
    {
        // 임시
        _nowKind = CameraActionKind.MoveMap;
        //
        for (int n = 0; n < _pointsNum; n++)
        {

            _movePoints.Add(_camerObj.transform.GetChild(n).position);      // 타겟의 위치 전부 다 리스트에 추가해주기 
        }
        _camerObj = GameObject.FindGameObjectWithTag("CameraMovePoint");
    }
    void Update()
    {
        switch (_nowKind)
        {
            case CameraActionKind.MoveMap:
                CameraMoveToPoints();
                break;
            case CameraActionKind.SetPlayPos:
                // 무브맵이 끝났으면 플레이어를 기준으로 옵셋값으로 가야하는데 갑자기 순간이동으로 가면 이상하니까
                // 가면서 카메라도 같이 가서 조정이 되게 하고 도착을 하면 게임이 시작하게 되는 것이다. 
                if (_isArrive)  // 무브맵이 끝났을 경우 카메라를 다시 시작지점으로 만든다.
                {
                    CameraMoveToStartPos();
                }
                break;
            case CameraActionKind.FollowPlayer:
                if (_tfTarget != null)       // 타겟이 없을때가 있으니 있을때만 따라가게 해야한다. 
                {
                    _tfCam.position = _tfTarget.position + _offsetPos;
                    //_tfCam.position = Vector3.Lerp(_tfCam.position, _tfTarget.position + _offsetPos, Time.deltaTime * 5);      // 연출 기법중 한개이다.
                    // Lerp를 사용하면 
                }
                else
                {
                    GameObject p = GameObject.FindGameObjectWithTag("Player");      // 타겟이 없는경우 새로 생성해준다. 
                    if (p != null)
                    {
                        _tfTarget = p.transform;    //이렇게 하면 이거 이후부터는 다 tfTarget이 널이 아닌 경우만 되는것이다.
                        _tfCam.rotation = Quaternion.Euler(_direction);  //현재 각도가 바뀔 일이 없다
                                                                         // Euler를 사용하면 복잡하게 사용하지 않고 편하게 사용 할 수 있다.
                    }
                }
                break;
        }
    }

    void CameraMoveToPoints()
    {
        GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
        for(int n = 0; n < _movePoints.Count; n++)
        {
            go.transform.position = Vector3.MoveTowards(_movePoints[n], _movePoints[n + 1],  _moveSpeed * Time.deltaTime);
        }
        _isArrive = true;
        _nowKind = CameraActionKind.SetPlayPos;
        {
            /*for (int m = 1; m < _movePoints.Count; m++)
            {
                iTween.MoveTo(gameObject, iTween.Hash("y", 44.5f, "x", _movePoints[m].x, "z", _movePoints[m].z, "time", 5.0f));
            }*/


            /*for (int n = 0; n < _pointsNum; n++)
                    {
                        //Debug.Log(pos.transform.GetChild(n).position); 여기까지는 찾아서 Add해주고
                        _movePoints.Add(_camerObj.transform.GetChild(n).position);
                    }
                    for (int m = 1; m < _movePoints.Count; m++)
                    {
                        //Debug.Log(go.transform.position); 여기서 MoveTowards가 아닌 다른걸 해야하나 iTween Move To를 할까 
                        //go.transform.position = Vector3.MoveTowards(_movePoints[m], _movePoints[m + 1], Time.deltaTime);

                        iTween.MoveTo(gameObject, iTween.Hash("y", 44.5f, "x", _movePoints[m].x, "z", _movePoints[m].z, "time", 5.0f));
                        // iTween은 Update에 있으면 안된다. 계속 콜하게 되면 문제이기 때문에 따로 함수를 두고 하는게 좋다. 끝에 도착했을때 콜을 하고 싶다면,
                        // oncomplete 이걸 써서 다시 콜해주는게 좋다. 집에서 따로 함수 만들어서 해보자. CameraMove()여기에서 추가해보자. 
                        // 카메라 본인을 움직이기 때문에 gameObject를 쓰고 Hash로 y값은 30으로 고정시키고 x, z값만 바꿔주자
                    }
                    transform.position = _movePoints[0];
                    // 다시 첫번째로 돌아가야 하니까*/
        }
    }
    void CameraMoveToStartPos()
    {
        Debug.Log(3);
        GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
        go.transform.position = Vector3.MoveTowards(transform.position, _movePoints[0], _moveSpeed * Time.deltaTime);
        // 현재 위치에서 시작위치로 카메라 속도만큼 빠르게 이동한다. 
        _nowKind = CameraActionKind.FollowPlayer;
    }




}
