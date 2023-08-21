using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using UnityEngine.AI;

public class MonsterObj : UnitBase
{
    [Header("Edit Param")]
    [SerializeField] float _limitWidth = 8;
    [SerializeField] float _limitFrontBack = 8;
    [SerializeField] float _minWaitTime = 4;
    [SerializeField] float _maxWaitTime = 10;
    [SerializeField] float _walkSpeed = 1;
    [SerializeField] float _runSpeed = 4;
    [SerializeField] float _attRange = 2;
    [SerializeField] float _sightRange = 8;
    [SerializeField] float _followDistance = 20;        // ���� �ڸ����� �����Ÿ� �̻� ���󰡸� �ٽ� �����ڸ��� ���ư��°��� ���Ѱ��̴�.
    [Header("Essential Param")]
    [SerializeField] BoxCollider[] _attackRng;
    [SerializeField] SphereCollider _sightRng;

    // ���� ����
    int _nowHP;
    int _monRank;

    // ���� ����
    Animator _animator;
    NavMeshAgent _navAgent;
    CheckSightRange _chkSight;
    List<Vector3> _roammingPoints;
    PlayerObj _targetPlayer;
    MiniBar _uiMini;

    // ���� ����
    AniType _nowAniType;
    Vector3 _genPosition;
    Vector3 _battleStartPos;  // ���⿡������ 20�� �Ǵ� �Ÿ��� �������� ���ƿ����� �� �����̴�. 
    RoammingType _roammType;
    float _waitTime;
    bool _isSelect;
    int _walkRate = 60;
    int _nextIndex;
    bool _isBack;
    int _attNumber = 0;
    float _expPoint;




    public override string _myName
    {
        get { return _name; }
    }
    public override bool _isDeath
    {
        get { return _isDead; }
    }
    public override float _hpRate
    {
        get { return _nowHP / (float)_maxHP; }
    }
    
    public int _finalDamage
    {
        // ���⿡�� if���� �ְų� switch������ �������Ͽ� ���� �������� �߰����൵ ����. 
        get { return _attPow; }
    }

    void Awake()
    {
        _genPosition = transform.position;
        _animator = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();

        _isSelect = false;
        _isBack = false;
    }
    void Start()
    {
        InitDataSet(_monRank, _roammType, _roammingPoints);
    }
    void Update()
    {
        {
            /*// � ��Ȳ������ ���ư����ϴϱ� Update���� ������ �����̴�.
            // ���� ���� ��ġ�� ����ġ���� _followDistance���� �� �־��� ���, �ٽ� �� ��ġ�� ���ƿͼ� ����ؾ� �Ѵ�.
            if (Vector3.Distance(_genPosition, transform.position) >= _followDistance)    // ����ġ���� ������ġ�� �Ÿ����̰� followDistance���� ū ���
            {
                transform.LookAt(_genPosition); // ������������ �ٶ󺸰� 
                _animator.speed = _runSpeed * 2; // �ִϸ��̼� �ӵ��� �ι�� ������ ���ش�
                ChangeAniFromType(AniType.RUN); // �ִϸ��̼��� ������ �ٲٰ�
                transform.position = Vector3.MoveTowards(transform.position, _genPosition, _runSpeed * 2);  // ����ġ�� �ٽ� ���ư���. 
                if (Vector3.Distance(_genPosition, transform.position) <= _backCheck) // ���� ����ġ���� �����ߴٸ�  
                {
                    ChangeAniFromType(AniType.IDLE);
                    _nowAniType = AniType.IDLE;
                    _animator.speed = 1;
                }
            }*/
        }
        switch (_nowAniType)
        {
            case AniType.IDLE:
                _waitTime -= Time.deltaTime;
                if (_waitTime <= 0)
                    _isSelect = false;
                break;
            case AniType.WALK:
                AllOffAttack();
                _navAgent.speed = _walkSpeed;
                break;
            case AniType.RUN:
                if (Vector3.Distance(transform.position, _battleStartPos) > _followDistance)  // =�� �� ������ =�̸� �������ϴϱ�. 
                {
                    ChangeAniFromType(AniType.BACKHOME);
                    _navAgent.destination = _battleStartPos;   
                    //_navAgent.destination = _targetPlayer.transform.position; �̷����� ��� ������ �Ǵ� ���̴�. 
                }
                else
                {
                    if(Vector3.Distance(transform.position, _targetPlayer.transform.position) > _attRange)  // Ÿ���� ���� ������ ��� ���
                    {
                        ChangeAniFromType(AniType.RUN);                             
                        _navAgent.destination = _targetPlayer.transform.position;     
                    }
                    else
                    {
                        ChangeAniFromType(AniType.ATTACK);
                    }
                }
                break;
            case AniType.ATTACK:
                if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _attRange)
                {
                    // ���� Ÿ�ٰ� ������ �Ÿ� ���̰� attRange������ �۴ٸ� (�� ���� �ȿ� ���Դٸ�) 
                    ChangeAniFromType(AniType.ATTACK);
                    transform.LookAt(_targetPlayer.transform);
                }
                else
                {
                    // �������� ����ٸ�, ���󰡱⸦ �ؾ��Ѵ�.
                    ChangeAniFromType(AniType.RUN);
                    _navAgent.destination = _targetPlayer.transform.position;
                }
                break;
            
            case AniType.BACKHOME:
                if(_navAgent.remainingDistance <= 0)
                {
                    _isSelect = false;
                    // ��Ȩ���� ���ָ� ������ ���� �� Select�� false(������ true�� �����̿����״�)�� �ٲ������� ���ؼ� �ٽ� ������ ������ �� �ְ� ���ش�
                    _sightRng.enabled = true;   // üũ���� �ٽ� ���ֱ�. 
                }
                break;
        }
        ProcessSelect();    // ���⼭ üũ������ �ٽ� ���ִ°͵� �������� �ϴ��� �������ڸ��� Ű�°ɷ� �Ѵ�(ProcessSelect���� �ص� �������� ����.)
    }

    public void InitDataSet(int monIndex, RoammingType type, List<Vector3> roamPoint)
    {
        string name = TableManager._instnace.Get(TableType.MonsterInfo).ToS(monIndex, "Name");
        int att = TableManager._instnace.Get(TableType.MonsterInfo).ToI(monIndex, "AttPower");
        int def = TableManager._instnace.Get(TableType.MonsterInfo).ToI(monIndex, "DefPower");
        int hp = TableManager._instnace.Get(TableType.MonsterInfo).ToI(monIndex, "HP");
        int exp = TableManager._instnace.Get(TableType.MonsterInfo).ToI(monIndex, "GiveExp");
        _monRank = TableManager._instnace.Get(TableType.MonsterInfo).ToI(monIndex, "Rank");
        att = (int)(att * TableManager._instnace.Get(TableType.RankInfo).ToF(_monRank, "AttScale"));
        def = (int)(att * TableManager._instnace.Get(TableType.RankInfo).ToF(_monRank, "AttScale"));
        hp = (int)(att * TableManager._instnace.Get(TableType.RankInfo).ToF(_monRank, "AttScale"));
        _expPoint = (int)(att * TableManager._instnace.Get(TableType.RankInfo).ToF(_monRank, "ExpScale"));

        InitializeInfoSet(name, att, def, hp);

#if UNITY_EDITOR
        RangeGizmo rng = _sightRng.GetComponent<RangeGizmo>();
        rng._radius = _sightRange;
#else
           RangeGizmo rng = _sightRng.GetComponent<RangeGizmo>();
           rng.enable = false; �̷��� ���θ� ���߿� �� ����. �ֳ��ϸ� �������� ������ ���� �� �ִ�. Track�� ����������.
#endif  //�̷��� �ϸ� �����Ϳ����� �����Ѵ�.
        _charType = eCharacterType.Monster;     // ���ʹ� �̷��� ������Ѵ�. 
        _roammType = type;
        _roammingPoints = roamPoint;
        for (int n = 0; n < _attackRng.Length; n++)
        {
            CheckAttackRange attRng = _attackRng[n].GetComponent<CheckAttackRange>();
            attRng.InitializeData(this);
        }
        _uiMini = gameObject.GetComponentInChildren<MiniBar>(); // �̷��� �ؼ� �ؿ��� ã�ƿ´�(����� 1��)

        _sightRng.radius = _sightRange;
        _chkSight = _sightRng.GetComponent<CheckSightRange>();
        _chkSight.InitData(this);
        _uiMini.InitData(_name);    // ���⿡�� �̴Ϲٿ� �����͸� �־��ش�
    }

    /*public void InitializeSet(int rank, string name)
    {
        //name = TableManager._instnace.Get(TableType.MonsterInfo).
        
        float att = TableManager._instnace.Get(TableType.MonsterInfo).ToI(rank, "AttPower");
        float def = TableManager._instnace.Get(TableType.MonsterInfo).ToI(rank, "DefPower");
        float hp = TableManager._instnace.Get(TableType.MonsterInfo).ToI(rank, "HP");
        

        //float gvExp = TableManager._instnace.Get(TableType            GiveExp
        //Prefab
        // ��ũ������ �����Ͽ� ���缭 ������ �����Ѵ�.
        // ���� ��� 3��ũ�̸� att 2.2, def 3.8, hp 3, exp 5 �̷��� �����Ѵ�.
        

        // �̷��� ���� ���� �ʿ� ���� �׳� ������ �ѹ��� ������ִ°� �� ����. �÷��̾�� �ٸ��� ���Ͱ� ��ũ���ϴ� ���� ���ٰ� ����.
    }*/

    public override void InitializeInfoSet(string name, int att, int def, int hp)
    {
        _name = name;
        _attPow = att;
        _defPow = def;
        _nowHP = _maxHP = hp;
    }
    public override void OnHit(UnitBase Oppon)
    {
        if (_isDead)
        {
            return;
        }
        if(Oppon._charType == eCharacterType.Player)
        {
            PlayerObj obj = (PlayerObj)Oppon;
            int finishDam = obj._finalDamage - _defPow;
            finishDam = finishDam < 1 ? 1 : finishDam;
            _nowHP -= finishDam;
            if(_nowHP <= 0)
            {
                _nowHP = 0;
                ChangeAniFromType(AniType.DEATH);
            }

            _uiMini.OpenMiniBar(_hpRate);
        }
    }

    public void ChangeAniFromType(AniType type)
    {
        _animator.speed = 1;
        switch (type)
        {
            case AniType.WALK:
                AllOffAttack();
                _navAgent.stoppingDistance = 0;
                _navAgent.speed = _walkSpeed;
                break;
            case AniType.RUN:
                AllOffAttack();
                _navAgent.stoppingDistance = _attRange;
                _navAgent.speed = _runSpeed;
                break;
            case AniType.ATTACK:
                _animator.SetInteger("AttackType", _attNumber);
                break;
            case AniType.DEATH:
                _animator.SetTrigger("IsDead");
                break;
            case AniType.BACKHOME:
                _navAgent.speed = _runSpeed * 2;        // ���ư��� �ӵ� �ι��
                _animator.speed = 2;                    // �ӵ��� �ι�ϱ� �ִϸ��̼� ��� �ӵ��� �ι�� 
                _animator.SetInteger("AniType", (int)AniType.RUN);
                break;
        }
        _nowAniType = type;
        if(type != AniType.BACKHOME)    // BACKHOME�̸� RUN�̿��� �� �� ��� ��� ���Ƶ״�. 
        {
            _animator.SetInteger("AniType", (int)type);
        }
    }
    public void DiscoverPlayer(PlayerObj obj)
    {
        _targetPlayer = obj;                                        // �����ȿ� ���� Ÿ�� ����
        ChangeAniFromType(AniType.RUN);                             // �̶� ���� �ȴ�.(�ִϸ��̼� ������ ����) 
        _sightRng.enabled = false;                                  // üũ�� Ÿ���� ������ ���� Off
        _navAgent.destination = _targetPlayer.transform.position;   // Ÿ���� ��ġ�� ����
        _battleStartPos = transform.position;                       // ���� ���� ��ġ�� �����Ͽ� �Ѵ� �Ÿ� ����������
    }
    int GetAttackNumber()
    {
        // ���߿� Define���� ������ ���� ���� ���ݿ� ���� ���������� �ַ� ����� �� ������ϴ���.
        // ���� ��ƾ�� ���� �������� �ٸ��� �׺κп� ���Ѱ��� CheckAttackRange���� ������ϱ��Ѵ�.
        // ���Ͱ� final�������� �� �� �������Ͽ� ���� �������� �ٸ��� �ִ°͵� �غ���.
        return Random.Range(0, _attackRng.Length);  // ���ݰ�����ŭ �������� �������°� 
    }

    Vector3 GetRandomPos()
    {
        float px = Random.Range(-_limitWidth, _limitWidth);
        float pz = Random.Range(-_limitFrontBack, _limitFrontBack);

        return _genPosition + new Vector3(px, 0, pz);   // ���� �������� ��ȯ 
    }
    Vector3 GetNextPos()
    {
        Vector3 goalPos = Vector3.zero;

        switch (_roammType)
        {
            case RoammingType.RandomPosition:
                goalPos = GetRandomPos();
                break;
            case RoammingType.RandomIndex:
                goalPos = _roammingPoints[Random.Range(0, _roammingPoints.Count)];
                break;
            case RoammingType.Sequens:
                goalPos = _roammingPoints[_nextIndex];
                if (++_nextIndex >= _roammingPoints.Count)
                    _nextIndex = 0;
                break;
            case RoammingType.OtherWay:
                goalPos = _roammingPoints[_nextIndex];
                if (--_nextIndex < 0)
                    _nextIndex = _roammingPoints.Count;
                break;
            case RoammingType.BackNForth:
                goalPos = _roammingPoints[_nextIndex];
                if (_isBack)
                {
                    if (--_nextIndex < 0)
                    {
                        _isBack = false;
                        _nextIndex = 1;
                    }
                }
                else
                {
                    if (++_nextIndex >= _roammingPoints.Count)
                    {
                        _isBack = true;
                        _nextIndex -= 2;
                    }
                }
                break;
            default:
                goalPos = transform.position;
                break;
        }

        return goalPos;
    }
    void ProcessSelect()
    {
        if (!_isSelect)
        {
            if (Random.Range(0, 100) >= _walkRate)
            {
                ChangeAniFromType(AniType.IDLE);
                _waitTime = Random.Range(_minWaitTime, _maxWaitTime);
            }
            else
            {
                ChangeAniFromType(AniType.WALK);
                _navAgent.destination = GetNextPos();// GetRandomPos();
            }
            _isSelect = true;
        }
    }

    void AllOffAttack()
    {
        for (int n = 0; n < _attackRng.Length; n++)
        {
            _attackRng[n].enabled = false;
        }
    }
    void AttackRangeOn(int id)
    {
        AllOffAttack();
        _attackRng[id].enabled = true;
        _attNumber = GetAttackNumber();
        Debug.Log(_attNumber);
    }
    void AttackRangeOff(int id)
    {
        _attackRng[id].enabled = false;
    }



    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 150, 40), "IDLE"))
    //        ChangeAniFromType(AniType.IDLE);
    //    if (GUI.Button(new Rect(0, 45, 150, 40), "WALK"))
    //        ChangeAniFromType(AniType.WALK);
    //    if (GUI.Button(new Rect(0, 90, 150, 40), "RUN"))
    //        ChangeAniFromType(AniType.RUN);
    //    if (GUI.Button(new Rect(0, 135, 150, 40), "DEAD"))
    //        ChangeAniFromType(AniType.DEATH);
    //    if (GUI.Button(new Rect(Screen.width  - 150, 0, 150, 40), "ATTACK1"))
    //        ChangeAniFromType(AniType.ATTACK);
    //    if (GUI.Button(new Rect(Screen.width - 150, 45, 150, 40), "ATTACK2"))
    //        ChangeAniFromType(AniType.ATTACK, 1);
    //    if (GUI.Button(new Rect(Screen.width - 150, 90, 150, 40), "ATTACK3"))
    //        ChangeAniFromType(AniType.ATTACK, 2);
    //}
}

