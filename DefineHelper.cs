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

    #region[캐릭터용]
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
        SKILL1,             // 버프
        SKILL2,             // 광역 스킬
        SKILL3,             // 단일 공격 스킬
        WALK,
        BACKHOME,
        DEATH           = 99
    }
    public enum RoammingType
    {
        RandomPosition = 0,   // 임의의 위치로 이동
        RandomIndex,          // 임위의 인덱스의 위치로 이동
        Sequens,              // 위치 순서대로 이동
        OtherWay,             // 위치 순서의 반대로 이동
        BackNForth,           // 순서대로 갔다가 마지막 도착시 반대로...(처음으로 돌아오면 순서대로)
    }
    public enum CameraActionKind
    {
        none                = 0,
        MoveMap,
        SetPlayPos,
        FollowPlayer
    }
    #endregion[캐릭터용]

    #region[테이블용]

    
    public enum TableType
    {
        LevelAddInfo            = 0,
        MonsterInfo,
        RankInfo,

        count
    }
    
    #endregion[테이블용]

    #region[매니저용]
    

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
    #endregion[매니저용]
}
