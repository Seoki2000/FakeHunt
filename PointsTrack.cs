using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointsTrack : MonoBehaviour
{

    [SerializeField] Color _colorLine = Color.red;

    Transform[] _points;

    void OnDrawGizmos() // 에디터에서만 보이는 내용 
    {
        Gizmos.color = _colorLine;

        _points = GetComponentsInChildren<Transform>();
                 // 게임오브젝트(자신)을 기준으로 자기 자신을 포함 Transform이 들어간 모든것을 가져온다. 

        int nextIdx = 1; // 0번은 자기자신 1번부터 자식오브젝트
        Vector3 currPos = _points[nextIdx].position;
        Vector3 nextPos;

        for(int i = 0; i<= _points.Length; i++)
        {
            nextPos = (++nextIdx >= _points.Length) ? _points[1].position : _points[nextIdx].position ;
            Gizmos.DrawLine(currPos, nextPos);
            currPos = nextPos;
        }
    }




}
