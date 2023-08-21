using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefineHelper;

public class SceneControlManager: TSingleTon<SceneControlManager>
{
    SceneType _prevScene;
    SceneType _currScene;

    // 임시
    public int _nowSelectStage
    {
        get;set;
        //이게 나중에는 인게임쪽에도 가야하긴 하는데 없으니 임시로 만들어준다. 
    }
    //=======

    /// <summary>
    /// 앱이 시작할때 실행.
    /// </summary>
    public void StartProjectStart()
    {
        TableManager._instnace.ReadAllTable();                 // AllTable이라고 했지만 인게임에서 따로 필요할 수도 있다. 그걸 우리들이 메모리 관리를 더 하는 차원이면 각각 씬에 필요한 테이블만 가져오자
        DataPoolManager._instnace.LoadAudioClip();             

        _prevScene = _currScene;        
        _currScene = SceneType.Villige;

        LoaddingScene("VilligeScne");
    }
    /// <summary>
    /// 빌리지 씬으로 올때.
    /// </summary>
    public void StartVilligeScene()
    {
        _prevScene = _currScene;        // 이전씬과 현재씬이 같은 상황이다.
        _currScene = SceneType.Villige; // 현재 씬은 Village로 한다. 

        LoaddingScene("VilligeScene");
        // DataPoolManager._instnace.LoadDatasScene(SceneType.Villige);이게 아닌 위에코드처럼 된다.
    }
    /// <summary>
    /// Ingame 씬으로 올때.
    /// </summary>
    public void StartIngameScene(int selectStage)
    {
        _prevScene = _currScene;
        _currScene = SceneType.Ingame;

        LoaddingScene("IngameScene", selectStage);
    }
    IEnumerator LoaddingScene(string sceneName, int stageNumber = 0)
    {
        // 코루틴안에 코루틴을 만들어둬서 이거를 통해서 로딩씬을 더 다이나믹하게 만들 수 있다.
        AsyncOperation _aOper;
        Scene activeScene;

        _aOper = SceneManager.LoadSceneAsync(sceneName);        // 씬을 이름에 맞게 가져와서 로드하는거라고 생각함
        while (!_aOper.isDone)
        {
            yield return null;
        }
        activeScene = SceneManager.GetSceneByName(sceneName);   // 이 타이밍이 왔으면 씬은 로드가 완료가 된것이다.

        switch (_currScene)
        {
            case SceneType.Villige:
                yield return DataPoolManager._instnace.LoadDatasScene(SceneType.Villige);
                break;
            case SceneType.Ingame:
                string stageName = "Stage" + stageNumber.ToString();    // 이렇게 씬 이름을 가지고 있는것이다.
                _aOper = SceneManager.LoadSceneAsync(stageName, LoadSceneMode.Additive);
                // 뒤에 LoadSceneMoad.Additive로 해줘야 멀티씬이 가능하다 기본값이 Single로 되어있어서 저것을 추가하지 않으면
                // 이전씬을 지우고 새로운씬 한개만 로드한다. 그래서 Additive로 멀티씬으로 이용하는 것이다.
                while (!_aOper.isDone)
                {
                    yield return null;
                }
                activeScene = SceneManager.GetSceneByName(stageName);

                yield return DataPoolManager._instnace.LoadDatasScene(SceneType.Ingame);
                break;
        }
        // if(_currScene == SceneType.Ingame){} switch문 대신 이렇게 해줘도 좋지만 강사님이 조금 더 이쁘게 해주기 위해서 스위치문을 사용하셨다.

        SceneManager.SetActiveScene(activeScene);
        // 이게 우리가 알고있는 씬을 Active 하는것이다. 
        // 이 부분은 메인씬을 로드한다. 
    }
}
