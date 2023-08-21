using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class IngameManager : MonoBehaviour
{
    // �� �����ϰ� ���� ������� �ʿ��Ҷ� �����ϰ� �ٸ����� ���ִ� �װŸ� ���൵ �ȴ�. ���� �����Ǹ� �ʿ��Ѿֵ� ȣ��,
    // ȣ���Ѿֵ��� �ʿ��Ѱ� ���������ϱ� �̷������� 

    static IngameManager _uniqueInstance;
    // ����
    PlayerObj _player;
    Transform _posSpawnPlayer;
    Transform _movePosCam;
    Transform _rootMonSpawn;
    FollowCamera _followCam;

    // box��
    MiniPlayerInfoBox _miniInfoBox;
    MiniMapBox _miniMapBox;
    InMessageBox _inMsgBox;


    // ����
    MainFlowState _curMainState;
    List<SpawnPoint> _spawnPointList;

    public static IngameManager _instnace
    {
        get { return _uniqueInstance; }
    }

    void Awake()
    {
        _uniqueInstance = this;
        // DontDestory������ ���ص� �ȴ�. �ֳ��ϸ� ���� �ٲ�� ���������ϴϱ�

        //�ӽ�        ���߿� Start�ʿ� ������� �����̴�.
       
        //===
    }

    /// <summary>
    /// Ingame�� ���� ������ �ʱ�ȭ���� �ϴ� �κ�.
    /// </summary>
    public void InitSettingGame()           // Stage1���� �ε��� �������Ÿ� �������� ������ ȣ���� �ȴٰ� �����Ѵ�. 
    {
        // �ʿ��� �� ����
        _curMainState = MainFlowState.InitSetting;

        GameObject go = GameObject.FindGameObjectWithTag("PlayerStartPos");         // �÷��̾� ��ŸƮ������ ���ӿ�����Ʈ�� �����´�.
        _posSpawnPlayer = go.transform;                                             // _posSpawnPlayer�� ��ŸƮ������ Transform�� �־��ش�
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

        // �ʿ��� �� ����
        _spawnPointList = new List<SpawnPoint>();

        // ���� ����
        for(int n = 0; n < _rootMonSpawn.childCount; n++)
        {
            SpawnPoint sp = _rootMonSpawn.GetChild(n).GetComponent<SpawnPoint>();
            _spawnPointList.Add(sp);
        }
        // _followCam
        _miniInfoBox.CloseBox();    // ���� �÷��̾ ������ ���� �ʾ����ϱ� �ϴ��� �̷��� �־��ش�.
        //_miniMapBox.ClickInOutButton();
        _inMsgBox.CloseMessageBox();
    }
    /// <summary>
    /// ���� ������ ���� �غ� �ܰ�� �ϴ� ȭ�鿡 �غ� ���� �޼��� ��� 
    /// </summary>
    public void ReadGame()
    {
        _curMainState = MainFlowState.Ready;
    }
    /// <summary>
    /// �÷��̾��� ���� �� ������Ʈ ����. 
    /// ������ ��������Ʈ�� Ȱ��ȭ. 
    /// </summary>
    public void SpawnStart()
    {
        _curMainState = MainFlowState.SpawnStart;
    }
    /// <summary>
    /// ī�޶��� ������ ���� �뷫���� ���� ���� �� ������ ������ ���� �� �� �ֵ��� �Ѵ�.  
    /// FollowCamera���� ������ ���Ḧ �˷��� ���� �������� �Ѿ �� �ֵ��� �����. 
    /// ���� �� �����ְ� ���� �ٽ� ī�޶� �÷��̾����� ���� ��찡 �����Ŵ�. ���������� �ٸ��� ��ٷ��� �Ѵ�. Start������ ī�޶󿡼� ��������Ѵ�. 
    /// </summary>
    public void ShowMapCamera()
    {
        _curMainState = MainFlowState.ShowMap;
    }
    /// <summary>
    /// ī�޶� ���ΰ��� ��ġ�� �����ϰ� ���� �ؼ� �÷��̸� �ٷ� ������ �� �ֵ��� �غ��Ű��(������) �ܰ�.
    /// </summary>
    public void StartGame()
    {
        _curMainState = MainFlowState.Start;
    }
    /// <summary>
    /// ������ �������� ������ ���� ������ �÷��� �ϴ� �ܰ�.
    /// �� ���������� ���� ���� ������ ���� �ൿ�� �ϰ� �ϴ� �ܰ�. 
    /// Ȥ�� �����ΰ� ���ٸ� ��ȹ�ڿ��� �ѹ������� ������� �Ǵϱ� ���� ���ӽ�ŸƮ�� �ڿ������� �Ѿ�ų�, ������ Ȯ���� �������ϰų� �̷� �͵��� ����
    /// ��쿡 ����� ���̴�. 
    /// </summary>
    public void PlayGame()
    {
        _curMainState = MainFlowState.Play;
    }
    /// <summary>
    /// �߰� �̺�Ʈ�� ����� �ִ� ��쿡 ���Ǵ� �ܰ�� ���ٸ� ���� ����� �ʿ䰡 ����.
    /// �̺�Ʈ ������� �ߵ�.
    /// </summary>
    public void Intermission()
    {
        _curMainState = MainFlowState.Intermission;
    }
    /// <summary>
    /// ���������� ������ �޼��ϰų� �÷��̾��� ����� ȣ��ȴ�.
    /// </summary>
    /// <param name="isSucces">���� �޼��� true</param>
    public void EndPlay(bool isSucces)
    {
        _curMainState = MainFlowState.End;
    }
    /// <summary>
    /// �ش� ���������� Ȱ�� ������ ���� ������ ȭ�鿡 ����Ͽ� �����ֵ��� �Ѵ�.
    /// </summary>
    public void ResultGame()
    {
        _curMainState = MainFlowState.Result;
    }

}
