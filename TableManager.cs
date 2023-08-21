using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class TableManager : TSingleTon<TableManager>
{
   /* static TableManager _uniqueInstance;

    public static TableManager _instance
    {
        get { return _uniqueInstance; }
    }*/

    Dictionary<TableType, TableBase> _tableList = new Dictionary<TableType, TableBase>();


    //void Start()
    //{
    //    ReadAllTable();


    //    ShowText();
    //}
    public void ShowText()
    {
        foreach (TableType tType in _tableList.Keys)
        {
            Debug.LogFormat("[{0}]", tType.ToString());
            TableBase _table = _tableList[tType];
            foreach (int index in _tableList[tType].Table.Keys)
            {
                string text = string.Empty;

                foreach (string column in _tableList[tType].Table[index].Keys)
                {

                    text += "{" + column + " : " + _tableList[tType].Table[index][column] + "} ";

                }
                Debug.LogFormat("[{0} : {1}]", index, text);
            }
        }
    }


    public TableBase Get(TableType type)
    {
        if(_tableList.ContainsKey(type))
        {
            return _tableList[type];
        }
        return null;
    }

    public void ReadAllTable()
    {
        Load<LevelAddInfo>(TableType.LevelAddInfo);
        Load<LevelAddInfo>(TableType.MonsterInfo);
        Load<LevelAddInfo>(TableType.RankInfo);
    }


    TableBase Load<T>(TableType type) where T : TableBase, new() 
    {
       
        if (_tableList.ContainsKey(type))
        {
            TableBase tBase = _tableList[type];
            return tBase;
        }
        TextAsset tAsset = Resources.Load<TextAsset>("Datas/Tables/"+type.ToString());
        if (tAsset != null)
        {
            T t = new T();
            t.Load(tAsset.text);

            _tableList.Add(type, t);
        }

        return _tableList[type];
    }
}
