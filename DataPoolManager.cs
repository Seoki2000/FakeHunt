using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;


public class DataPoolManager : TSingleTon<DataPoolManager>
{
    /*static DataPoolManager _uniqueInstance;

    public static DataPoolManager _instance
    {
        get { return _uniqueInstance; }
    }
*/

    Dictionary<string, GameObject> _prefabDatas;
    Dictionary<string, AudioClip> _audioClips;     // ������ ���̺�� ������ �ִ°� ����� ���� ����

    protected override void Init()
    {
        base.Init();

        _prefabDatas = new Dictionary<string, GameObject>();
        _audioClips = new Dictionary<string, AudioClip>();
    }

    public IEnumerator LoadDatasScene(SceneType type)
    {
        ReleaseAllDictionary();
        yield return null;
        // prefab Loadding
        if (type == SceneType.Ingame)
        {
            for (int n = 0; n < (int)CharPrefabName.Count; n++)
            {
                string path = "Models/Characters/";
                string key = ((CharPrefabName)n).ToString();
                GameObject prefab = Resources.Load(path + key) as GameObject;
                _prefabDatas.Add(key, prefab);
                yield return null;
            }
            for(int n = 0; n < (int)ItemPrefabName.Count; n++)
            {
                string path = "Models/Weapons/";
                string key = ((ItemPrefabName)n).ToString();
                GameObject prefab = Resources.Load(path + key) as GameObject;
                _prefabDatas.Add(key, prefab);
                yield return null;
            }
        }
        //
    }
    public IEnumerator LoadAudioClip()
    {
        yield return null;
    }
    public GameObject GetPrefab(string prefabName)
    {
        if (_prefabDatas.ContainsKey(prefabName))
            return _prefabDatas[prefabName];
        return null;
    }
    public AudioClip GetAudioClip(string name)
    {
        if (_audioClips.ContainsKey(name))
        {
            return _audioClips[name];   // ���� �ش��ϴ� Ű�� ������ audio ����
        }
        return null;    
    }
    void ReleaseAllDictionary()
    {
        if (_prefabDatas.Count > 0)
            _prefabDatas.Clear();
    }
}
