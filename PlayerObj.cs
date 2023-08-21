using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using UnityEngine.AI;

public class PlayerObj : UnitBase
{
    /*IEnumerator Start()
    {
        // �̷��� �ϴ� ����� �ִ� 
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
        

    }*/ // �ִϸ��̼� Ȯ�ο�

    [Header("Edit Param")]  // ������ �� �ִ� �Ķ��Ÿ���� �α� ���ؼ� �̷��� �������.
    [SerializeField] float _movSpeed = 5;   // �ִϸ��̼ǰ� �ӵ��� ����Ŷ� 
    // �ӽ� ���ν� ���� �̿��ؼ� ���߿� ���� ���� �����̴�. 
    [SerializeField] GameObject _prefabWeapon;  
    //======
    // �ӽ�
    [SerializeField] float _attackRng = 1.5f;   // ���ݹ����� ����
    //=====
    [Header("Essential Param")]
    [SerializeField] Transform _posWeapon;
    [SerializeField] BoxCollider[] _checkAttacks;   // �������� �༮�� �������� �߰��ϱ� ���ؼ� �̷��� �д�. 
    // ====

    // ��������
    int _level;
    int _nowHP;
    int _nowXP;
    int _nextLVXP;
    int _perRecoveryVal;

    // ���� ����
    Animator _aniController;
    NavMeshAgent _navAgent;
    //CharacterController _charController;
    /* ���߿��� �ٸ� ������� �� �����̶� ���ٰ������� ���� �ӽ÷� �̿�
    CharacterController���� Move, SimpleMove �̷��� �ΰ��� ������� ������ �� �ִ�. 
    Move�� �߷°���� �����ʰ� SimpleMove�� �߷��� ����Ѵ�.
    Move�� �̹� �����ӿ��� ���������ϴ� ���� �˷��ִ°��̰�(ȯ���� ���α��� ����ؾ��ϸ� Move�� �����) 
    SimpleMove�� �۾��Ҷ� �ξ� ���ϴ� ���� ����� ���� �ʾƵ� ������(���������� �̿��ؼ� �˾Ƽ� �������) ������ ��ǥ�� ���� �����̴�*/
    Camera _mainCam;
    GameObject _weapon;
    MiniPlayerInfoBox _MiniPlayerBox;

    // ���� ����
    AniType _currentAniType;
    bool _isEquip;  // ���� �ִ��� ������ Ȯ���� ���� 
    bool _isBattle;
    bool _isMaxHP;  // �ִ�ü������ üũ 
    bool _isHit = false;
    float _checkTimeForHeal;    // ü��ȸ���� ���ؼ� �ð� üũ


    public override string _myName
    {
        get { return _name; }   // ���߿� ȣĪ������ ������ ����ٰ� �߰��ص� ������. 
    }
    public override bool _isDeath
    {
        get { return _isDeath; }
    }
    public override float _hpRate
    {
        //get { return _nowHP / (float)_maxHP; }

        // ���ӿ� ���� �ٸ��� ����� ���� �ִ�. ���� HP�� MaxHP���� ū ��찡 �ִ�. �ؿ� �ڵ�ó�� �ϴ� ��쵵 �ִ�. 
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
        //_charController = GetComponent<CharacterController>();  // �����ͼ� �� ���� �����ǵ� Rigidbody, Collrider ������ ���ؼ��� �صδ� ��.
        _navAgent = GetComponent<NavMeshAgent>();

        _isEquip = false;
        _isBattle = false;
        _isMaxHP = true;
        // _mainCam = Camera.main;  ���߿��� Awake�� �ϴ°� �´µ� ������ ���� Hierarchyâ�� �־ ����� ���� �������� ���� 
    }
    void Start()
    {
        _mainCam = Camera.main;

        // �ӽ�
        InitializeSet(1, "��ö��", _MiniPlayerBox);
        //Debug.Log(TableManager._instnace.Get(TableType.LevelAddInfo).ToS(3, "AttPower"));
        //======
        StartCoroutine(SelfHeal());
        // �̰� StopCoroutine(SelfHeal()); �̰ų� ("SelfHeal"); �̷��� �ؼ� update�� �ھƵΰ� if������ �����ص� ������(bool�� ���� �Ѱ� ���� ���)
    }
    void Update()
    {
        if (_isDead)
        {
            return;
        }

        _checkTimeForHeal += Time.deltaTime;        // �ڰ�ġ���� ���ؼ� �ð� üũ

        if (_navAgent.remainingDistance <= 0)
        {
            // �̰� �̿��ϸ� ������ ������ Ŭ���ص� ������ ������ó���� ����. �׸��� ��ó���� ���� ���� �ƴ� ���̵�� �ٲ���.
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
                if(_checkTimeForHeal >= 1.0f)       // 1�ʰ� ������ ���
                {
                    SelfHealing(_isMaxHP);          // true�� ��� return false�� ��츸 _nowHp += _perRecoveryVal �� �ȴ�. 
                    _checkTimeForHeal = 0;          // �ð� üũ�� �ٽ� �Ѵ�. 
                }
            }
        }

        // Layer Map & Floor�� �����ؼ� ��ġ�ϵ��� �Ѵ�.
        // ��ġ�� ĳ���Ͱ� �ش� ��ġ�� �̵�.
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
                if (rHit.transform.CompareTag("DummyObj"))    // Tag�� ������ ��� 
                {
                    _isBattle = true;
                    ChangeAniFromType(AniType.RUN, _attackRng);
                }
                else
                {
                    _isBattle = false;  // �̷��� IDLE�� ���Ѵ�
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
            _checkAttacks[n].enabled = false;   // ó������ �����־���ϱ� ������. �������鼭 ���� ���д�. 
        }
    }

    // �ӽ�
    public void InitializeSet(int nowLevel, string name, MiniPlayerInfoBox miniBox)
    {
        _charType = eCharacterType.Player;  // �÷��̾��°��� �ɾ��ش�. 
        _level = nowLevel;
        _nowXP = 0;
        int att = TableManager._instnace.Get(TableType.LevelAddInfo).ToI(nowLevel, "AtkPower");
        int def = TableManager._instnace.Get(TableType.LevelAddInfo).ToI(nowLevel, "DefPower");
        int hp = TableManager._instnace.Get(TableType.LevelAddInfo).ToI(nowLevel, "HP");
        _nextLVXP = TableManager._instnace.Get(TableType.LevelAddInfo).ToI(nowLevel, "NextLevel");
        _perRecoveryVal = 1;    // �ʴ� 1�� �ǰ� ���°��� ���ش�.
        miniBox.InitSetData(name, att, def);
        InitializeInfoSet(name, att, def, hp);
    }

    //====

    public void EquipWeapon(WeaponType type, bool equip = true)
    {
        // type�� ���� ������ �����ͼ� �տ� ����ֱ� 
        switch (type)
        {
            case WeaponType.OneHandSwrod:
                if(equip)
                {
                    _weapon = Instantiate(_prefabWeapon, _posWeapon);
                    _weapon.transform.localPosition = Vector3.zero;
                    _weapon.transform.localRotation = Quaternion.identity;
                    // �̷��� �ʱ�ȭ�� ���� ���ش�. ��ġ�� �̻������� ������ ����� ���� �־ Ȥ�� ���� �̷��� ������ �صδ� ���̴� localP,localR
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
                AllOffAttack(); // �߰��� �ִϸ��̼��� Ǯ�� ���� �־ �̷��� ���ش�. 
                _navAgent.speed = _movSpeed;
                _navAgent.stoppingDistance = dist; // ���� ���� ��ġ�� �����ϴϱ� ������ ���� ���ݹ������� �����Ѵ� �ΰ��� ������ 
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
            // ���Ϳ� ������.....
            MonsterObj mon = (MonsterObj)oppon;
            int finishDam = mon._finalDamage - _defPow;
            finishDam = finishDam < 1 ? 1 : finishDam;  // def�� �ʹ� ������ ������ ������ ü���� �� �� �־ �̷��� ���ش�.
            _nowHP -= finishDam;
            if(_nowHP <= 0)
            {
                _nowHP = 0;
                ChangeAniFromType(AniType.DEATH);
            }
            Debug.Log(_nowHP);
        }
        _isHit = false; // �´� ����� �������� �ٽ� false�� �ٲ��ش�.
    }

    public void SelfHealing(bool _isMAx)
    {
        _isMAx = _isMaxHP;  
        if (!_isMAx)    // ���� ü���� �ִ밡 �ƴ� ��� 
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
    }       // �̷������� �ڷ�ƾ�� ����ص� �Ȱ��� ��ü���� ��� �������������� 

    /*public bool CheckHitPlayer()
    {
        int temp = _nowHP;  // temp�� ���� HP ���� �����ִµ��� �� �����Ӹ��� �ٽ� temp�� ���� �����Ǽ� ���� �����̴�. �� �ǹ̾����� 
        if (temp > _hpRate)  // ���� ü���� �Ʊ�� �������ٸ� 
        {
            return true;    // ������ true
        }
        return false;   // �ƴϸ� false�� return���ش�. 
        // �̰� �������� temp�� ��� ��������ϱ� �׳� �Ⱦ��� ���� public bool ������ get return�ؼ� �޾��ֱ����
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
        AllOffAttack(); // ������ �����°� �´µ� Ȥ�� �Ȳ������� ������ �̷��� �صа��̴�. 
        _checkAttacks[id].enabled = true;
    }
    void AttackRangeOff(int id)
    {
        _checkAttacks[id].enabled = false;
    }

   











    /*void Update()
   {
       //Axisraw�� �ƴ϶� �ϴ��� �׳� Axis�� �Ѵ�.
       float mz = Input.GetAxis("Vertical");  
       float mx = Input.GetAxis("Horizontal");

       Vector3 dv = new Vector3(mx, 0, mz);    // dv = ���̷��� ����
       dv = (dv.magnitude > 1) ? dv.normalized : dv;
       //Vector3 mv = (transform.forward * mx + transform.forward * mz) * _movSpeed;
       Vector3 mv = transform.rotation * dv * _movSpeed;   // transform.rotation �� �ȳ־��ָ� ������ǥ�� �������� �̻��ϰ� ����. 
       _charController.SimpleMove(mv); // transRatate���� �ٸ���. ������ �������� �����̰� �� ���̳ĸ� �ؾ���. 
       //_charController.Move(mv * Time.deltaTime);   // ���� move�� ���Ÿ�, mv�� �ϸ� ���󰡴ϱ� Time.delatTime�� ��������Ѵ�.

       // �̵��� ������� IDLE �� mz�� 0���� ū���, Run �װ� �ƴϸ� IDLE
       // �ϴ� �츮�� ���̵��ũ�� �����ϱ� ���� ������ִ� ���� ��� mz�� �������� �� ���̴�.
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
