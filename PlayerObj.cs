using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using UnityEngine.AI;

public class PlayerObj : UnitBase
{
    /*IEnumerator Start()
    {
        // 이렇게 하는 방법도 있다 
        return Action();
    }

    IEnumerator Action()
    {

    }*/
    /* void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 150, 40), "NoWeapon"))
        {
            _aniController.SetBool("IsWeapon", false);
        }
        if (GUI.Button(new Rect(0, 45, 150, 40), "Weapon"))
        {
            _aniController.SetBool("IsWeapon", true);
        }
        if (GUI.Button(new Rect(550, 0, 150, 40), "NOWeaponRun"))
        {
            _aniController.SetBool("IsWeapon", false);
            _aniController.SetInteger("AniType", 1);
        }
        if (GUI.Button(new Rect(550, 45, 150, 40), "WeaponRun"))
        {
            _aniController.SetBool("IsWeapon", true);
            _aniController.SetInteger("AniType", 1);
        }
        if (GUI.Button(new Rect(550, 305, 150, 40), "NoWeaponIdle"))
        {
            _aniController.SetBool("IsWeapon", false);
            _aniController.SetInteger("AniType", 0);
        }
        if (GUI.Button(new Rect(550, 350, 150, 40), "WeaponIdle"))
        {
            _aniController.SetBool("IsWeapon", true);
            _aniController.SetInteger("AniType", 0);
        }
        if(GUI.Button(new Rect(0, 305, 150, 40), "Attack"))
        {
            _aniController.SetInteger("AniType", 2);
        }
        

    }*/ // 애니메이션 확인용

    [Header("Edit Param")]  // 편집할 수 있는 파라메타들을 두기 위해서 이렇게 만들었다.
    [SerializeField] float _movSpeed = 5;   // 애니메이션과 속도를 맞출거라서 
    // 임시 리로스 툴을 이용해서 나중에 따로 만들 예정이다. 
    [SerializeField] GameObject _prefabWeapon;  
    //======
    // 임시
    [SerializeField] float _attackRng = 1.5f;   // 공격범위를 뜻함
    //=====
    [Header("Essential Param")]
    [SerializeField] Transform _posWeapon;
    [SerializeField] BoxCollider[] _checkAttacks;   // 여러개인 녀석은 여러개를 추가하기 위해서 이렇게 둔다. 
    // ====

    // 스탯정보
    int _level;
    int _nowHP;
    int _nowXP;
    int _nextLVXP;
    int _perRecoveryVal;

    // 참조 변수
    Animator _aniController;
    NavMeshAgent _navAgent;
    //CharacterController _charController;
    /* 나중에는 다른 방식으로 할 예정이라 없앨것이지만 지금 임시로 이용
    CharacterController에는 Move, SimpleMove 이렇게 두가지 방식으로 움직일 수 있다. 
    Move는 중력계산을 하지않고 SimpleMove는 중력을 계산한다.
    Move는 이번 프레임에서 움직여야하는 것을 알려주는것이고(환경적 요인까지 계산해야하면 Move를 써야함) 
    SimpleMove는 작업할때 훨씬 편하다 내가 계산을 하지 않아도 괜찮다(물리엔진을 이용해서 알아서 계산해줌) 절대적 좌표에 대한 방향이다*/
    Camera _mainCam;
    GameObject _weapon;
    MiniPlayerInfoBox _MiniPlayerBox;

    // 정보 변수
    AniType _currentAniType;
    bool _isEquip;  // 무기 있는지 없는지 확인을 위해 
    bool _isBattle;
    bool _isMaxHP;  // 최대체력인지 체크 
    bool _isHit = false;
    float _checkTimeForHeal;    // 체력회복을 위해서 시간 체크


    public override string _myName
    {
        get { return _name; }   // 나중에 호칭같은게 붙으면 여기다가 추가해도 괜찮다. 
    }
    public override bool _isDeath
    {
        get { return _isDeath; }
    }
    public override float _hpRate
    {
        //get { return _nowHP / (float)_maxHP; }

        // 게임에 따라서 다르게 사용할 수도 있다. 현재 HP가 MaxHP보다 큰 경우가 있다. 밑에 코드처럼 하는 경우도 있다. 
         get 
         { 
            float rate = _nowHP / (float) _maxHP;
            return rate > 1 ? 1 : rate; 
         }
    }
    public int _finalDamage
    {
        get { return _attPow; }
    }
    public bool _isHitPlayer
    {
        get { return _isHit; }
    }

    void Awake()
    {
        _aniController = GetComponent<Animator>();
        //_charController = GetComponent<CharacterController>();  // 가져와서 뭘 하진 않을건데 Rigidbody, Collrider 역할을 위해서만 해두는 것.
        _navAgent = GetComponent<NavMeshAgent>();

        _isEquip = false;
        _isBattle = false;
        _isMaxHP = true;
        // _mainCam = Camera.main;  나중에는 Awake가 하는게 맞는데 지금은 같은 Hierarchy창에 있어서 어느게 먼저 생성될지 몰라서 
    }
    void Start()
    {
        _mainCam = Camera.main;

        // 임시
        InitializeSet(1, "김철수", _MiniPlayerBox);
        //Debug.Log(TableManager._instnace.Get(TableType.LevelAddInfo).ToS(3, "AttPower"));
        //======
        StartCoroutine(SelfHeal());
        // 이걸 StopCoroutine(SelfHeal()); 이거나 ("SelfHeal"); 이렇게 해서 update에 박아두고 if문으로 제어해도 괜찮다(bool형 변수 한개 만들어서 사용)
    }
    void Update()
    {
        if (_isDead)
        {
            return;
        }

        _checkTimeForHeal += Time.deltaTime;        // 자가치유를 위해서 시간 체크

        if (_navAgent.remainingDistance <= 0)
        {
            // 이걸 이용하면 못가는 지역을 클릭해도 못가는 지역근처까지 간다. 그리고 근처까지 가서 런이 아닌 아이들로 바꿔줌.
            if (_isBattle)
            {
                if(_navAgent.remainingDistance <= _attackRng)
                {
                    transform.LookAt(_navAgent.destination);
                    ChangeAniFromType(AniType.ATTACK);
                }
                
            }
            else
            {
                if(_navAgent.remainingDistance <= 0)
                {
                    ChangeAniFromType(AniType.IDLE);
                }
                if(_checkTimeForHeal >= 1.0f)       // 1초가 지났을 경우
                {
                    SelfHealing(_isMaxHP);          // true일 경우 return false일 경우만 _nowHp += _perRecoveryVal 이 된다. 
                    _checkTimeForHeal = 0;          // 시간 체크를 다시 한다. 
                }
            }
        }

        // Layer Map & Floor만 감지해서 터치하도록 한다.
        // 터치시 캐릭터가 해당 위치로 이동.
        if (Input.GetButtonDown("Fire1"))    // Input.GetMouseButton(0)
        {
            RaycastHit rHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int lMask = 1 << LayerMask.NameToLayer("Map") | LayerMask.NameToLayer("Floor");
            lMask |= 1 << LayerMask.NameToLayer("Monster");
            if(Physics.Raycast(ray, out rHit, Mathf.Infinity, lMask))
            {
                //transform.position = rHit.point;   
                _navAgent.destination = rHit.point;
                if (rHit.transform.CompareTag("DummyObj"))    // Tag가 더미일 경우 
                {
                    _isBattle = true;
                    ChangeAniFromType(AniType.RUN, _attackRng);
                }
                else
                {
                    _isBattle = false;  // 이래야 IDLE로 변한다
                    ChangeAniFromType(AniType.RUN);
                }
            } 
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(!_isEquip)
            {
                EquipWeapon(WeaponType.OneHandSwrod, !_isEquip);
            }
        }
    }

    public override void InitializeInfoSet(string name, int att, int def, int hp)
    {
        _name = name;
        _attPow = att;
        _defPow = def;
        _nowHP = _maxHP = hp;

        for (int n = 0; n < _checkAttacks.Length; n++)
        {
            CheckAttackRange chkAttRng = _checkAttacks[n].GetComponent<CheckAttackRange>();
            chkAttRng.InitializeData(this);
            _checkAttacks[n].enabled = false;   // 처음에는 꺼져있어야하기 때문에. 가져오면서 먼저 꺼둔다. 
        }
    }

    // 임시
    public void InitializeSet(int nowLevel, string name, MiniPlayerInfoBox miniBox)
    {
        _charType = eCharacterType.Player;  // 플레이어라는것을 걸어준다. 
        _level = nowLevel;
        _nowXP = 0;
        int att = TableManager._instnace.Get(TableType.LevelAddInfo).ToI(nowLevel, "AtkPower");
        int def = TableManager._instnace.Get(TableType.LevelAddInfo).ToI(nowLevel, "DefPower");
        int hp = TableManager._instnace.Get(TableType.LevelAddInfo).ToI(nowLevel, "HP");
        _nextLVXP = TableManager._instnace.Get(TableType.LevelAddInfo).ToI(nowLevel, "NextLevel");
        _perRecoveryVal = 1;    // 초당 1씩 피가 차는것을 해준다.
        miniBox.InitSetData(name, att, def);
        InitializeInfoSet(name, att, def, hp);
    }

    //====

    public void EquipWeapon(WeaponType type, bool equip = true)
    {
        // type에 따른 프래펩 가져와서 손에 쥐어주기 
        switch (type)
        {
            case WeaponType.OneHandSwrod:
                if(equip)
                {
                    _weapon = Instantiate(_prefabWeapon, _posWeapon);
                    _weapon.transform.localPosition = Vector3.zero;
                    _weapon.transform.localRotation = Quaternion.identity;
                    // 이렇게 초기화를 먼저 해준다. 위치가 이상해지고 오류가 생기는 일이 있어서 혹시 몰라서 이렇게 설정을 해두는 것이다 localP,localR
                }
                else
                {
                    Destroy(_weapon);
                }
                break;
            case WeaponType.OneHandMace:
                break;
        }
        _isEquip = equip;
        _aniController.SetBool("IsWeapon", _isEquip);

    }
    public void ChangeAniFromType(AniType type, float dist = 0)
    {
        if (_isDead)
        {
            return;
        }
        switch (type)
        {
            case AniType.IDLE:
                break;
            case AniType.RUN:
                AllOffAttack(); // 중간에 애니메이션이 풀릴 수가 있어서 이렇게 해준다. 
                _navAgent.speed = _movSpeed;
                _navAgent.stoppingDistance = dist; // 내가 찍은 위치로 가야하니까 어택인 경우는 공격범위까지 가야한다 두개의 차이점 
                break;
            case AniType.ATTACK:
                break;
            case AniType.DEATH:
                AllOffAttack();
                _isDead = true;
                break;
        }
        _currentAniType = type;
        _aniController.SetInteger("AnyType", (int)type);
    }
    public override void OnHit(UnitBase oppon)
    {
        if (_isDead)
        {
            return;
        }

        if(oppon._charType == eCharacterType.Monster)
        {
            _isHit = true;
            // 몬스터에 관련한.....
            MonsterObj mon = (MonsterObj)oppon;
            int finishDam = mon._finalDamage - _defPow;
            finishDam = finishDam < 1 ? 1 : finishDam;  // def가 너무 높으면 맞으면 오히려 체력이 찰 수 있어서 이렇게 해준다.
            _nowHP -= finishDam;
            if(_nowHP <= 0)
            {
                _nowHP = 0;
                ChangeAniFromType(AniType.DEATH);
            }
            Debug.Log(_nowHP);
        }
        _isHit = false; // 맞는 계산이 끝났으면 다시 false로 바꿔준다.
    }

    public void SelfHealing(bool _isMAx)
    {
        _isMAx = _isMaxHP;  
        if (!_isMAx)    // 현재 체력이 최대가 아닌 경우 
        {
            _nowHP += _perRecoveryVal;
            if(_nowHP >= _maxHP)
            {
                _isMaxHP = true;
            }
        }
    }


    
    IEnumerator SelfHeal()
    {
        yield return new WaitForSeconds(1.0f);

        _nowHP += _perRecoveryVal;
        if(_nowHP >= _maxHP)
        {
            yield break;
        }
    }       // 이런식으로 코루틴을 사용해도 똑같이 자체힐링 사용 가능하지않을까 

    /*public bool CheckHitPlayer()
    {
        int temp = _nowHP;  // temp에 현재 HP 저장 들어와있는동안 매 프레임마다 다시 temp가 새로 생성되서 같을 예정이다. 즉 의미없을듯 
        if (temp > _hpRate)  // 만약 체력이 아까보다 떨어졌다면 
        {
            return true;    // 맞으면 true
        }
        return false;   // 아니면 false를 return해준다. 
        // 이게 문제점이 temp로 계속 만들어지니까 그냥 안쓰고 위에 public bool 형으로 get return해서 받아주기로함
    }*/

    void AllOffAttack()
    {
        for(int n = 0; n < _checkAttacks.Length; n++)
        {
            _checkAttacks[n].enabled = false;   
        }
    }
    void OnAttackRange(int id)
    {
        AllOffAttack(); // 원래는 꺼지는게 맞는데 혹시 안꺼질수도 있으니 이렇게 해둔것이다. 
        _checkAttacks[id].enabled = true;
    }
    void AttackRangeOff(int id)
    {
        _checkAttacks[id].enabled = false;
    }

   











    /*void Update()
   {
       //Axisraw가 아니라 일단은 그냥 Axis로 한다.
       float mz = Input.GetAxis("Vertical");  
       float mx = Input.GetAxis("Horizontal");

       Vector3 dv = new Vector3(mx, 0, mz);    // dv = 다이렉션 벡터
       dv = (dv.magnitude > 1) ? dv.normalized : dv;
       //Vector3 mv = (transform.forward * mx + transform.forward * mz) * _movSpeed;
       Vector3 mv = transform.rotation * dv * _movSpeed;   // transform.rotation 을 안넣어주면 절대좌표로 움직여서 이상하게 간다. 
       _charController.SimpleMove(mv); // transRatate랑은 다르다. 누구를 기준으로 움직이게 할 것이냐를 해야함. 
       //_charController.Move(mv * Time.deltaTime);   // 만약 move를 쓸거면, mv만 하면 날라가니까 Time.delatTime을 곱해줘야한다.

       // 이동이 없을경우 IDLE 즉 mz가 0보다 큰경우, Run 그게 아니면 IDLE
       // 일단 우리는 사이드워크가 없으니까 따로 만들어주는 일이 없어서 mz를 기준으로 한 것이다.
       if(mv.magnitude > 0)
       {
           ChangeAniFromType(AniType.RUN);
       }
       else
       {
           ChangeAniFromType(AniType.IDLE);
       }

   }*/
}
