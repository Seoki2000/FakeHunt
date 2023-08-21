using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointsTrack : MonoBehaviour
{

    [SerializeField] Color _colorLine = Color.red;

    Transform[] _points;

    void OnDrawGizmos() // �����Ϳ����� ���̴� ���� 
    {
        Gizmos.color = _colorLine;

        _points = GetComponentsInChildren<Transform>();
                 // ���ӿ�����Ʈ(�ڽ�)�� �������� �ڱ� �ڽ��� ���� Transform�� �� ������ �����´�. 

        int nextIdx = 1; // 0���� �ڱ��ڽ� 1������ �ڽĿ�����Ʈ
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
