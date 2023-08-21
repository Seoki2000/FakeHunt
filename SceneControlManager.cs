using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefineHelper;

public class SceneControlManager: TSingleTon<SceneControlManager>
{
    SceneType _prevScene;
    SceneType _currScene;

    // �ӽ�
    public int _nowSelectStage
    {
        get;set;
        //�̰� ���߿��� �ΰ����ʿ��� �����ϱ� �ϴµ� ������ �ӽ÷� ������ش�. 
    }
    //=======

    /// <summary>
    /// ���� �����Ҷ� ����.
    /// </summary>
    public void StartProjectStart()
    {
        TableManager._instnace.ReadAllTable();                 // AllTable�̶�� ������ �ΰ��ӿ��� ���� �ʿ��� ���� �ִ�. �װ� �츮���� �޸� ������ �� �ϴ� �����̸� ���� ���� �ʿ��� ���̺� ��������
        DataPoolManager._instnace.LoadAudioClip();             

        _prevScene = _currScene;        
        _currScene = SceneType.Villige;

        LoaddingScene("VilligeScne");
    }
    /// <summary>
    /// ������ ������ �ö�.
    /// </summary>
    public void StartVilligeScene()
    {
        _prevScene = _currScene;        // �������� ������� ���� ��Ȳ�̴�.
        _currScene = SceneType.Villige; // ���� ���� Village�� �Ѵ�. 

        LoaddingScene("VilligeScene");
        // DataPoolManager._instnace.LoadDatasScene(SceneType.Villige);�̰� �ƴ� �����ڵ�ó�� �ȴ�.
    }
    /// <summary>
    /// Ingame ������ �ö�.
    /// </summary>
    public void StartIngameScene(int selectStage)
    {
        _prevScene = _currScene;
        _currScene = SceneType.Ingame;

        LoaddingScene("IngameScene", selectStage);
    }
    IEnumerator LoaddingScene(string sceneName, int stageNumber = 0)
    {
        // �ڷ�ƾ�ȿ� �ڷ�ƾ�� �����ּ� �̰Ÿ� ���ؼ� �ε����� �� ���̳����ϰ� ���� �� �ִ�.
        AsyncOperation _aOper;
        Scene activeScene;

        _aOper = SceneManager.LoadSceneAsync(sceneName);        // ���� �̸��� �°� �����ͼ� �ε��ϴ°Ŷ�� ������
        while (!_aOper.isDone)
        {
            yield return null;
        }
        activeScene = SceneManager.GetSceneByName(sceneName);   // �� Ÿ�̹��� ������ ���� �ε尡 �Ϸᰡ �Ȱ��̴�.

        switch (_currScene)
        {
            case SceneType.Villige:
                yield return DataPoolManager._instnace.LoadDatasScene(SceneType.Villige);
                break;
            case SceneType.Ingame:
                string stageName = "Stage" + stageNumber.ToString();    // �̷��� �� �̸��� ������ �ִ°��̴�.
                _aOper = SceneManager.LoadSceneAsync(stageName, LoadSceneMode.Additive);
                // �ڿ� LoadSceneMoad.Additive�� ����� ��Ƽ���� �����ϴ� �⺻���� Single�� �Ǿ��־ ������ �߰����� ������
                // �������� ����� ���ο�� �Ѱ��� �ε��Ѵ�. �׷��� Additive�� ��Ƽ������ �̿��ϴ� ���̴�.
                while (!_aOper.isDone)
                {
                    yield return null;
                }
                activeScene = SceneManager.GetSceneByName(stageName);

                yield return DataPoolManager._instnace.LoadDatasScene(SceneType.Ingame);
                break;
        }
        // if(_currScene == SceneType.Ingame){} switch�� ��� �̷��� ���൵ ������ ������� ���� �� �̻ڰ� ���ֱ� ���ؼ� ����ġ���� ����ϼ̴�.

        SceneManager.SetActiveScene(activeScene);
        // �̰� �츮�� �˰��ִ� ���� Active �ϴ°��̴�. 
        // �� �κ��� ���ξ��� �ε��Ѵ�. 
    }
}
