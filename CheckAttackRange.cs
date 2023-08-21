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
                Debug.Log(other.gameObject.name + "������");
                MonsterObj Mon = other.GetComponent<MonsterObj>();      // �̷��� �غ� �صΰ� ���߿� �Լ��� ������ �ɰ��̴�. 
                Mon.OnHit(_owner);
            }
        }
        else if(_owner._charType == eCharacterType.Monster)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log(other.gameObject.name + "������");
                PlayerObj player = other.gameObject.GetComponent<PlayerObj>();
                player.OnHit(_owner);
            }
        }
    }
}
