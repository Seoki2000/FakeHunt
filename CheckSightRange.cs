using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSightRange : MonoBehaviour
{
    // 몬스터한테만 있을것이다.
    MonsterObj _owner;

    public void InitData(MonsterObj owner)
    {
        _owner = owner;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerObj obj = other.gameObject.GetComponent<PlayerObj>();
            _owner.DiscoverPlayer(obj);
        }
    }
}
