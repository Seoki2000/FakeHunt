using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefineHelper 
{
    public enum WeaponType
    {
        OneHandSwrod = 0,
        OneHandMace
    }

    #region[ĳ���Ϳ�]
    public enum eCharacterType
    {
        Player              = 0,
        Monster,
        Dummy
    }

    public enum AniType
    {
        IDLE            = 0,
        RUN,
        ATTACK,
        SKILL1,             // ����
        SKILL2,             // ���� ��ų
        SKILL3,             // ���� ���� ��ų
        WALK,
        BACKHOME,
        DEATH           = 99
    }
    public enum RoammingType
    {
        RandomPosition = 0,   // ������ ��ġ�� �̵�
        RandomIndex,          // ������ �ε����� ��ġ�� �̵�
        Sequens,              // ��ġ ������� �̵�
        OtherWay,             // ��ġ ������ �ݴ�� �̵�
        BackNForth,           // ������� ���ٰ� ������ ������ �ݴ��...(ó������ ���ƿ��� �������)
    }
    public enum CameraActionKind
    {
        none                = 0,
        MoveMap,
        SetPlayPos,
        FollowPlayer
    }
    #endregion[ĳ���Ϳ�]

    #region[���̺��]

    
    public enum TableType
    {
        LevelAddInfo            = 0,
        MonsterInfo,
        RankInfo,

        count
    }
    
    #endregion[���̺��]

    #region[�Ŵ�����]
    

    public enum SceneType
    {
        Villige = 0,
        Ingame,
    }

    public enum CharPrefabName
    {
        PlayerPaladin,
        MonsterTitan,

        Count
    }
    public enum ItemPrefabName
    {
        OneHandSword,

        Count
    }
    public enum MainFlowState
    {
        none                    = 0,
        InitSetting,            
        Ready,
        SpawnStart,
        ShowMap,
        Start,
        Play,
        Intermission,
        End,
        Result
    }
    #endregion[�Ŵ�����]
}
