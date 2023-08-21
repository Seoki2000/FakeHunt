using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPos : MonoBehaviour
{
    [SerializeField] float _checkStayTime;       // �÷��̾ ���� �ȿ��� ����ϴ� �ð�üũ
    [SerializeField] float _succesTime = 10;     // ����Ŭ���� ���ǿ� ���� �ð�
    // [SerializeField]�� ���ϰ� �����غ���. 

    SphereCollider _goalRange;  // �� ���� �ȿ� ���� ��� 
    // ������ �˷�����ϴµ� �̰� ��� �����;�����... PlayerObj���� OnHit�� ���𰡸� �Ѱ��ٸ��Ѱ� �����ؾ��� 
    PlayerObj _player;
    TimerBox _timer;

    bool _isSucess;     // �������� ��� ������ ���� 

    public bool _sucessGame     // ���ǿ� �´� �ð����� ��ƾ ���. 
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
        if (other.CompareTag("Player"))     // ���� �÷��̾ ���� ��� 
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
            _checkStayTime += Time.deltaTime;   // üũ�ð�
            /* if (_player.CheckHitPlayer())       // ���� �¾��� ���
             {
                 _checkStayTime = 0;             // üũ�ð��� �ٽ� �ʱ�ȭ��Ų��. 
             }*/
            if(_checkStayTime >= _succesTime)
            {
                _isSucess = true;
            }
            if (_player._isHitPlayer)       // �´� ��� OnHit���� true�� �ٲ��ְ� OnHit�� ���� ��� _isHitPlayer�� false�� �ȴ�. 
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
