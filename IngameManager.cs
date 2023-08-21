using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class IngameManager : MonoBehaviour
{
    // 다 제어하고 싶은 사람들은 필요할때 성생하고 다를때는 꺼주는 그거를 해줘도 된다. 씬이 생성되면 필요한애들 호출,
    // 호출한애들이 필요한거 가져오게하기 이런식으로 

    static IngameManager _uniqueInstance;
    // 참조
    PlayerObj _player;
    Transform _posSpawnPlayer;
    Transform _movePosCam;
    Transform _rootMonSpawn;
    FollowCamera _followCam;

    // box들
    MiniPlayerInfoBox _miniInfoBox;
    MiniMapBox _miniMapBox;
    InMessageBox _inMsgBox;


    // 정보
    MainFlowState _curMainState;
    List<SpawnPoint> _spawnPointList;

    public static IngameManager _instnace
    {
        get { return _uniqueInstance; }
    }

    void Awake()
    {
        _uniqueInstance = this;
        // DontDestory까지는 안해도 된다. 왜냐하면 씬이 바뀌면 없어져야하니까

        //임시        나중에 Start쪽에 만들어줄 예정이다.
       
        //===
    }

    /// <summary>
    /// Ingame에 사용될 참조와 초기화들을 하는 부분.
    /// </summary>
    public void InitSettingGame()           // Stage1까지 로딩이 끝났을거를 기준으로 했을때 호출이 된다고 봐야한다. 
    {
        // 필요한 것 참조
        _curMainState = MainFlowState.InitSetting;

        GameObject go = GameObject.FindGameObjectWithTag("PlayerStartPos");         // 플레이어 스타트포스로 게임오브젝트를 가져온다.
        _posSpawnPlayer = go.transform;                                             // _posSpawnPlayer에 스타트포스의 Transform을 넣어준다
        go = GameObject.FindGameObjectWithTag("CameraMovePoint");
        _movePosCam = go.transform;
        go = GameObject.FindGameObjectWithTag("SpawnMonRoot");
        _rootMonSpawn = go.transform;
        _followCam = Camera.main.GetComponent<FollowCamera>();
        go = GameObject.FindGameObjectWithTag("UIMiniInfoBox");
        _miniInfoBox = go.GetComponent<MiniPlayerInfoBox>();
        go = GameObject.FindGameObjectWithTag("UIMiniMapBox");
        _miniMapBox = go.GetComponent<MiniMapBox>();
        go = GameObject.FindGameObjectWithTag("UIMessageBox");
        _inMsgBox = go.GetComponent<InMessageBox>();

        // 필요한 것 생성
        _spawnPointList = new List<SpawnPoint>();

        // 정보 저장
        for(int n = 0; n < _rootMonSpawn.childCount; n++)
        {
            SpawnPoint sp = _rootMonSpawn.GetChild(n).GetComponent<SpawnPoint>();
            _spawnPointList.Add(sp);
        }
        // _followCam
        _miniInfoBox.CloseBox();    // 아직 플레이어가 생성이 되지 않았으니까 일단은 이렇게 넣어준다.
        //_miniMapBox.ClickInOutButton();
        _inMsgBox.CloseMessageBox();
    }
    /// <summary>
    /// 다음 스텝을 위한 준비 단계로 일단 화면에 준비에 대한 메세지 출력 
    /// </summary>
    public void ReadGame()
    {
        _curMainState = MainFlowState.Ready;
    }
    /// <summary>
    /// 플레이어의 생성 및 스테이트 설정. 
    /// 몬스터의 스폰포인트의 활성화. 
    /// </summary>
    public void SpawnStart()
    {
        _curMainState = MainFlowState.SpawnStart;
    }
    /// <summary>
    /// 카메라의 무빙을 통해 대략적인 맵의 형태 및 간략한 정보를 보여 줄 수 있도록 한다.  
    /// FollowCamera에서 무빙의 종료를 알려서 다음 스탭으로 넘어갈 수 있도록 만든다. 
    /// 맵을 다 보여주고 나서 다시 카메라가 플레이어한테 갔을 경우가 끝난거다. 이전까지와 다르게 기다려야 한다. Start게임을 카메라에서 던져줘야한다. 
    /// </summary>
    public void ShowMapCamera()
    {
        _curMainState = MainFlowState.ShowMap;
    }
    /// <summary>
    /// 카메라가 주인공의 위치에 도착하고 정리 해서 플레이를 바로 시작할 수 있도록 준비시키는(유저를) 단계.
    /// </summary>
    public void StartGame()
    {
        _curMainState = MainFlowState.Start;
    }
    /// <summary>
    /// 유저가 적극적인 개입을 통해 게임을 플레이 하는 단계.
    /// 각 스테이지별 게임 종료 목적에 따른 행동을 하게 하는 단계. 
    /// 혹시 무엇인가 없다면 기획자에게 한번정도는 물어봐도 되니까 만약 게임스타트가 자연스럽게 넘어가거나, 유저가 확인을 눌러야하거나 이런 것들이 없을
    /// 경우에 물어보는 것이다. 
    /// </summary>
    public void PlayGame()
    {
        _curMainState = MainFlowState.Play;
    }
    /// <summary>
    /// 중간 이벤트나 페이즈가 있는 경우에 사용되는 단계로 없다면 굳이 사용할 필요가 없다.
    /// 이벤트 방식으로 발동.
    /// </summary>
    public void Intermission()
    {
        _curMainState = MainFlowState.Intermission;
    }
    /// <summary>
    /// 스테이지의 목적을 달성하거나 플레이어의 사망시 호출된다.
    /// </summary>
    /// <param name="isSucces">목적 달성시 true</param>
    public void EndPlay(bool isSucces)
    {
        _curMainState = MainFlowState.End;
    }
    /// <summary>
    /// 해당 스테이지의 활동 내역에 대한 정보를 화면에 출력하여 보여주도록 한다.
    /// </summary>
    public void ResultGame()
    {
        _curMainState = MainFlowState.Result;
    }

}
