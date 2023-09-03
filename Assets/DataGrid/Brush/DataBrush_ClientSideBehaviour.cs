using MagicLanjello.CellPlaceholder;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("MagicLanJello/DataBrush_ClientSideBehaviour")]
internal class DataBrush_ClientSideBehaviour : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<Vector3Int, CellPlaceholderStruct> _gotNewCell;

    private DataBrush _outer;
    private void Awake()
    {
        _outer = new DataBrush();
        _outer.Reset();
    }

    public bool TryReadOneFromBytes(ReadOnlySpan<byte> bytes, ref int index) =>
        _outer.TryReadOneFromBytes(bytes, ref index, _gotNewCell.Invoke);

}