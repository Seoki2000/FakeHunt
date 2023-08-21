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
    [SerializeField] float _followDistance = 20;        // 원래 자리에서 일정거리 이상 따라가면 다시 원래자리로 돌아가는것을 위한것이다.
    [Header("Essential Param")]
    [SerializeField] BoxCollider[] _attackRng;
    [SerializeField] SphereCollider _sightRng;

    // 스탯 정보
    int _nowHP;
    int _monRank;

    // 참조 변수
    Animator _animator;
    NavMeshAgent _navAgent;
    CheckSightRange _chkSight;
    List<Vector3> _roammingPoints;
    PlayerObj _targetPlayer;
    MiniBar _uiMini;

    // 정보 변수
    AniType _nowAniType;
    Vector3 _genPosition;
    Vector3 _battleStartPos;  // 여기에서부터 20이 되는 거리가 떨어지면 돌아오도록 할 예정이다. 
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
        // 여기에서 if문을 주거나 switch문으로 공격패턴에 따라 데미지를 추가해줘도 좋다. 
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
            /*// 어떤 상황에서든 돌아가야하니까 Update에서 진행할 생각이다.
            // 만약 현재 위치가 젠위치에서 _followDistance보다 더 멀어진 경우, 다시 젠 위치로 돌아와서 대기해야 한다.
            if (Vector3.Distance(_genPosition, transform.position) >= _followDistance)    // 젠위치에서 현재위치의 거리차이가 followDistance보다 큰 경우
            {
                transform.LookAt(_genPosition); // 젠포지션으로 바라보고 
                _animator.speed = _runSpeed * 2; // 애니메이션 속도도 두배로 빠르게 해준다
                ChangeAniFromType(AniType.RUN); // 애니메이션을 런으로 바꾸고
                transform.position = Vector3.MoveTowards(transform.position, _genPosition, _runSpeed * 2);  // 젠위치로 다시 돌아간다. 
                if (Vector3.Distance(_genPosition, transform.position) <= _backCheck) // 만약 젠위치까지 근접했다면  
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
                if (Vector3.Distance(transform.position, _battleStartPos) > _followDistance)  // =은 뺀 이유가 =이면 때려야하니까. 
                {
                    ChangeAniFromType(AniType.BACKHOME);
                    _navAgent.destination = _battleStartPos;   
                    //_navAgent.destination = _targetPlayer.transform.position; 이래야지 계속 갱신이 되는 것이다. 
                }
                else
                {
                    if(Vector3.Distance(transform.position, _targetPlayer.transform.position) > _attRange)  // 타겟이 공격 범위가 벗어난 경우
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
                    // 현재 타겟과 나와의 거리 차이가 attRange값보다 작다면 (즉 범위 안에 들어왔다면) 
                    ChangeAniFromType(AniType.ATTACK);
                    transform.LookAt(_targetPlayer.transform);
                }
                else
                {
                    // 범위에서 벗어났다면, 따라가기를 해야한다.
                    ChangeAniFromType(AniType.RUN);
                    _navAgent.destination = _targetPlayer.transform.position;
                }
                break;
            
            case AniType.BACKHOME:
                if(_navAgent.remainingDistance <= 0)
                {
                    _isSelect = false;
                    // 백홈으로 해주면 도착을 했을 때 Select를 false(무조건 true인 상태이였을테니)로 바꿔줌으로 인해서 다시 동작을 시작할 수 있게 해준다
                    _sightRng.enabled = true;   // 체크범위 다시 켜주기. 
                }
                break;
        }
        ProcessSelect();    // 여기서 체크범위를 다시 켜주는것도 괜찮은데 일단은 도착하자마자 키는걸로 한다(ProcessSelect에서 해도 괜찮을거 같다.)
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
           rng.enable = false; 이렇게 꺼두면 나중에 더 좋다. 왜냐하면 쓸데없는 연산을 안할 수 있다. Track도 마찬가지다.
#endif  //이렇게 하면 에디터에서만 동작한다.
        _charType = eCharacterType.Monster;     // 몬스터는 이렇게 해줘야한다. 
        _roammType = type;
        _roammingPoints = roamPoint;
        for (int n = 0; n < _attackRng.Length; n++)
        {
            CheckAttackRange attRng = _attackRng[n].GetComponent<CheckAttackRange>();
            attRng.InitializeData(this);
        }
        _uiMini = gameObject.GetComponentInChildren<MiniBar>(); // 이렇게 해서 밑에서 찾아온다(현재는 1개)

        _sightRng.radius = _sightRange;
        _chkSight = _sightRng.GetComponent<CheckSightRange>();
        _chkSight.InitData(this);
        _uiMini.InitData(_name);    // 여기에서 미니바에 데이터를 넣어준다
    }

    /*public void InitializeSet(int rank, string name)
    {
        //name = TableManager._instnace.Get(TableType.MonsterInfo).
        
        float att = TableManager._instnace.Get(TableType.MonsterInfo).ToI(rank, "AttPower");
        float def = TableManager._instnace.Get(TableType.MonsterInfo).ToI(rank, "DefPower");
        float hp = TableManager._instnace.Get(TableType.MonsterInfo).ToI(rank, "HP");
        

        //float gvExp = TableManager._instnace.Get(TableType            GiveExp
        //Prefab
        // 랭크인포에 스케일에 맞춰서 정보가 들어가야한다.
        // 예를 들어 3랭크이면 att 2.2, def 3.8, hp 3, exp 5 이렇게 들어가야한다.
        

        // 이렇게 따로 만들 필요 없이 그냥 위에서 한번에 계산해주는게 더 좋다. 플레이어와 다르게 몬스터가 랭크업하는 경우는 없다고 했음.
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
                _navAgent.speed = _runSpeed * 2;        // 돌아가는 속도 두배로
                _animator.speed = 2;                    // 속도가 두배니까 애니메이션 재생 속도도 두배로 
                _animator.SetInteger("AniType", (int)AniType.RUN);
                break;
        }
        _nowAniType = type;
        if(type != AniType.BACKHOME)    // BACKHOME이면 RUN이여서 쓸 수 없어서 잠시 막아뒀다. 
        {
            _animator.SetInteger("AniType", (int)type);
        }
    }
    public void DiscoverPlayer(PlayerObj obj)
    {
        _targetPlayer = obj;                                        // 범위안에 들어온 타겟 설정
        ChangeAniFromType(AniType.RUN);                             // 이때 런이 된다.(애니메이션 런으로 변경) 
        _sightRng.enabled = false;                                  // 체크된 타겟이 있으니 범위 Off
        _navAgent.destination = _targetPlayer.transform.position;   // 타겟의 위치를 설정
        _battleStartPos = transform.position;                       // 전투 시작 위치를 저장하여 쫓는 거리 기준점으로
    }
    int GetAttackNumber()
    {
        // 나중에 Define에서 성격을 따로 만들어서 성격에 따라 공격패턴을 주로 어떤것을 더 해줘야하는지.
        // 공격 루틴에 따라서 데미지가 다르면 그부분에 대한것이 CheckAttackRange에서 해줘야하긴한다.
        // 몬스터가 final데미지를 줄 때 공격패턴에 따라 데미지를 다르게 주는것도 해보자.
        return Random.Range(0, _attackRng.Length);  // 공격갯수만큼 랜덤으로 가져오는것 
    }

    Vector3 GetRandomPos()
    {
        float px = Random.Range(-_limitWidth, _limitWidth);
        float pz = Random.Range(-_limitFrontBack, _limitFrontBack);

        return _genPosition + new Vector3(px, 0, pz);   // 범위 일정범위 반환 
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

