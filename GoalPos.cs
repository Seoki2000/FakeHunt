using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPos : MonoBehaviour
{
    [SerializeField] float _checkStayTime;       // 플레이어가 범위 안에서 대기하는 시간체크
    [SerializeField] float _succesTime = 10;     // 게임클리어 조건에 대한 시간
    // [SerializeField]로 편하게 조절해보자. 

    SphereCollider _goalRange;  // 골 범위 안에 들어온 경우 
    // 맞으면 알려줘야하는데 이걸 어디서 가져와야하지... PlayerObj에서 OnHit에 무언가를 넘겨줄만한걸 생각해야함 
    PlayerObj _player;
    TimerBox _timer;

    bool _isSucess;     // 성공했을 경우 리턴을 위해 

    public bool _sucessGame     // 조건에 맞는 시간동안 버틴 경우. 
    {
        get { return _isSucess; }
    }


    void Awake()
    {
        GameObject go = GameObject.FindGameObjectWithTag("GoalRange");
        _goalRange = go.GetComponent<SphereCollider>();
        
    }
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))     // 만약 플레이어가 들어온 경우 
        {
            OnTriggerStay(other);
            _timer.InitData(_succesTime, _checkStayTime);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))     
        {
            _timer.OnTimer();
            _checkStayTime += Time.deltaTime;   // 체크시간
            /* if (_player.CheckHitPlayer())       // 만약 맞았을 경우
             {
                 _checkStayTime = 0;             // 체크시간을 다시 초기화시킨다. 
             }*/
            if(_checkStayTime >= _succesTime)
            {
                _isSucess = true;
            }
            if (_player._isHitPlayer)       // 맞는 경우 OnHit에서 true로 바꿔주고 OnHit가 끝난 경우 _isHitPlayer는 false가 된다. 
            {
                _checkStayTime = 0;
            }

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _checkStayTime = 0;
        }
    }
}
