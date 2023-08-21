using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 일단 얘도 PoolManager에서 가져온다고 생각해야함. 
    GameObject PlayVoiceSound(string name)
    {
        AudioClip aClip = DataPoolManager._instnace.GetAudioClip(name);
        GameObject go = new GameObject("VoiceSound");       // 이렇게 하면 하이어라키창에 생성이 된다. 근데 이것만 있으면 안되서 밑에 추가로 코드를 적어줘야함

        AudioSource audioPlayer = go.AddComponent<AudioSource>();   // 이렇게 하면 게임오브젝트 밑에 컴포넌트가 붙는다.
        audioPlayer.clip = aClip;
        /// 기타 등드 써두고

        return go; 
        // 하면 이걸 넘겨주고 붙여주는 것이다. 제어는 여기서 하고 붙이는건 거기서 하지만 제어는 다 매니저에서 해야한다. 우리 전에 지웠던것처럼 몬스터 잡아서 지웠던것처럼 정지가 되어있으면
        // 날려주는 것이다. 필요에 의해서 보이스나 따른것도 있으면 위치를 잡아서 그 위치로 게임오브젝트 옮기면 그 공간상에 실시간으로 사운드를 붙여줄 수 있다. 그리고 사운드 플레이가 다 끝나면 지워준다.
    }
}
