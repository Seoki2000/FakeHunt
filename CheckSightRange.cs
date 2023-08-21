using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSightRange : MonoBehaviour
{
    // �������׸� �������̴�.
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
