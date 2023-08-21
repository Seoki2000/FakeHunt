using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class CheckAttackRange : MonoBehaviour
{
    UnitBase _owner;

    public void InitializeData(UnitBase owner)
    {
        _owner = owner;
    }
    void OnTriggerEnter(Collider other)
    {
        if (_owner._charType == eCharacterType.Player)
        {
            if (other.CompareTag("Monster") || other.CompareTag("DummyObj"))
            {
                Debug.Log(other.gameObject.name + "때렸음");
                MonsterObj Mon = other.GetComponent<MonsterObj>();      // 이렇게 준비를 해두고 나중에 함수를 날리면 될것이다. 
                Mon.OnHit(_owner);
            }
        }
        else if(_owner._charType == eCharacterType.Monster)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log(other.gameObject.name + "때렸음");
                PlayerObj player = other.gameObject.GetComponent<PlayerObj>();
                player.OnHit(_owner);
            }
        }
    }
}
