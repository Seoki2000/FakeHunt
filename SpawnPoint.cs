using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] float _genTime = 5;
    [SerializeField] float _genMaxCount = 3;
    [SerializeField] float _limitLiveMonCount = 1;
    [SerializeField] CharPrefabName _prefabName;
    [SerializeField] RoammingType _roammingType;

    List<GameObject> _generateObjs;

    GameObject _prefabMon;
    Transform _rootPoint;
    float _checkTime;
    List<Vector3> _points;

    void Awake()
    {
        _generateObjs = new List<GameObject>();
        _checkTime = 0;
        _rootPoint = transform.GetChild(0);
        _points = new List<Vector3>();
    }

    void Start()
    {
        _prefabMon = DataPoolManager._instnace.GetPrefab(_prefabName.ToString());
        if (_prefabMon == null)
            Debug.Log("_prefabMon 이 null 입니다.");

        for (int n = 0; n < _rootPoint.childCount; n++)
            _points.Add(_rootPoint.GetChild(n).position);
    }

    void Update()
    {
        if(_genMaxCount > 0)
        {
            if(_generateObjs.Count < _limitLiveMonCount)
            {
                _checkTime += Time.deltaTime;
                if (_checkTime >= _genTime)
                {
                    if (_prefabMon != null)
                    {
                        Transform tf = transform.GetChild(0);
                        GameObject go = Instantiate(_prefabMon, tf.position, tf.rotation);
                        MonsterObj mon = go.GetComponent<MonsterObj>();
                        mon.InitDataSet(1,_roammingType, _points);
                        _generateObjs.Add(go);
                        _genMaxCount--;
                    }
                    _checkTime = 0;
                }
            }
        }
    }

    void LateUpdate()
    {
        for(int n = 0; n < _generateObjs.Count; n++)
        {
            if (_generateObjs[n] == null)
            {
                _generateObjs.RemoveAt(n);
                break;
            }
        }
    }
}
