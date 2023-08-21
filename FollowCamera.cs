using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Vector3 _offsetPos = Vector3.zero; // ������ ���� ���� �ʿ��ϴ�(�󸶸�ŭ �ָ� ����)
    [SerializeField] Vector3 _direction = Vector3.zero; // ī�޶� ������ �ֱ� ���ؼ� 
    [SerializeField] int _pointsNum = 10;               // ����Ʈ ���� 
    [SerializeField] float _moveSpeed = 6.5f;           // ī�޶� �����̴� �ӵ� 

    Transform _tfCam;       // ī�޶� ��ġ
    Transform _tfTarget;    // Ÿ���� ��ġ

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
        // �ӽ�
        _nowKind = CameraActionKind.MoveMap;
        //
        for (int n = 0; n < _pointsNum; n++)
        {

            _movePoints.Add(_camerObj.transform.GetChild(n).position);      // Ÿ���� ��ġ ���� �� ����Ʈ�� �߰����ֱ� 
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
                // ������� �������� �÷��̾ �������� �ɼ°����� �����ϴµ� ���ڱ� �����̵����� ���� �̻��ϴϱ�
                // ���鼭 ī�޶� ���� ���� ������ �ǰ� �ϰ� ������ �ϸ� ������ �����ϰ� �Ǵ� ���̴�. 
                if (_isArrive)  // ������� ������ ��� ī�޶� �ٽ� ������������ �����.
                {
                    CameraMoveToStartPos();
                }
                break;
            case CameraActionKind.FollowPlayer:
                if (_tfTarget != null)       // Ÿ���� �������� ������ �������� ���󰡰� �ؾ��Ѵ�. 
                {
                    _tfCam.position = _tfTarget.position + _offsetPos;
                    //_tfCam.position = Vector3.Lerp(_tfCam.position, _tfTarget.position + _offsetPos, Time.deltaTime * 5);      // ���� ����� �Ѱ��̴�.
                    // Lerp�� ����ϸ� 
                }
                else
                {
                    GameObject p = GameObject.FindGameObjectWithTag("Player");      // Ÿ���� ���°�� ���� �������ش�. 
                    if (p != null)
                    {
                        _tfTarget = p.transform;    //�̷��� �ϸ� �̰� ���ĺ��ʹ� �� tfTarget�� ���� �ƴ� ��츸 �Ǵ°��̴�.
                        _tfCam.rotation = Quaternion.Euler(_direction);  //���� ������ �ٲ� ���� ����
                                                                         // Euler�� ����ϸ� �����ϰ� ������� �ʰ� ���ϰ� ��� �� �� �ִ�.
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
                        //Debug.Log(pos.transform.GetChild(n).position); ��������� ã�Ƽ� Add���ְ�
                        _movePoints.Add(_camerObj.transform.GetChild(n).position);
                    }
                    for (int m = 1; m < _movePoints.Count; m++)
                    {
                        //Debug.Log(go.transform.position); ���⼭ MoveTowards�� �ƴ� �ٸ��� �ؾ��ϳ� iTween Move To�� �ұ� 
                        //go.transform.position = Vector3.MoveTowards(_movePoints[m], _movePoints[m + 1], Time.deltaTime);

                        iTween.MoveTo(gameObject, iTween.Hash("y", 44.5f, "x", _movePoints[m].x, "z", _movePoints[m].z, "time", 5.0f));
                        // iTween�� Update�� ������ �ȵȴ�. ��� ���ϰ� �Ǹ� �����̱� ������ ���� �Լ��� �ΰ� �ϴ°� ����. ���� ���������� ���� �ϰ� �ʹٸ�,
                        // oncomplete �̰� �Ἥ �ٽ� �����ִ°� ����. ������ ���� �Լ� ���� �غ���. CameraMove()���⿡�� �߰��غ���. 
                        // ī�޶� ������ �����̱� ������ gameObject�� ���� Hash�� y���� 30���� ������Ű�� x, z���� �ٲ�����
                    }
                    transform.position = _movePoints[0];
                    // �ٽ� ù��°�� ���ư��� �ϴϱ�*/
        }
    }
    void CameraMoveToStartPos()
    {
        Debug.Log(3);
        GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
        go.transform.position = Vector3.MoveTowards(transform.position, _movePoints[0], _moveSpeed * Time.deltaTime);
        // ���� ��ġ���� ������ġ�� ī�޶� �ӵ���ŭ ������ �̵��Ѵ�. 
        _nowKind = CameraActionKind.FollowPlayer;
    }




}
