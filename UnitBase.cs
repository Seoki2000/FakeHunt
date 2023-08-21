using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public abstract class UnitBase : MonoBehaviour
{
    // 무조건 hit 되는걸로 만들 예정이다
    protected string _name;

    protected int _attPow;
    protected int _defPow;
    protected int _maxHP;

    protected bool _isDead;

    public abstract string _myName
    {
        get;
    }
    public abstract bool _isDeath
    {
        get;
    }
    public abstract float _hpRate
    {
        get;
    }

    public eCharacterType _charType
    {
        get;set;
    }
    public virtual void InitializeInfoSet(string name, int att, int def, int hp)
    {
        _name = name;
        _attPow = att;
        _defPow = def;
        _maxHP = hp;
    }

    public abstract void OnHit(UnitBase oppon);
    
}
