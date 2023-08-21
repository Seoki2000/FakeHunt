using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeGizmo : MonoBehaviour
{
    [SerializeField] Color _color = Color.yellow;

    public float _radius
    {
        get;set;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}

