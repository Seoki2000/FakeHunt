using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // �ϴ� �굵 PoolManager���� �����´ٰ� �����ؾ���. 
    GameObject PlayVoiceSound(string name)
    {
        AudioClip aClip = DataPoolManager._instnace.GetAudioClip(name);
        GameObject go = new GameObject("VoiceSound");       // �̷��� �ϸ� ���̾��Űâ�� ������ �ȴ�. �ٵ� �̰͸� ������ �ȵǼ� �ؿ� �߰��� �ڵ带 ���������

        AudioSource audioPlayer = go.AddComponent<AudioSource>();   // �̷��� �ϸ� ���ӿ�����Ʈ �ؿ� ������Ʈ�� �ٴ´�.
        audioPlayer.clip = aClip;
        /// ��Ÿ ��� ��ΰ�

        return go; 
        // �ϸ� �̰� �Ѱ��ְ� �ٿ��ִ� ���̴�. ����� ���⼭ �ϰ� ���̴°� �ű⼭ ������ ����� �� �Ŵ������� �ؾ��Ѵ�. �츮 ���� ��������ó�� ���� ��Ƽ� ��������ó�� ������ �Ǿ�������
        // �����ִ� ���̴�. �ʿ信 ���ؼ� ���̽��� �����͵� ������ ��ġ�� ��Ƽ� �� ��ġ�� ���ӿ�����Ʈ �ű�� �� ������ �ǽð����� ���带 �ٿ��� �� �ִ�. �׸��� ���� �÷��̰� �� ������ �����ش�.
    }
}
