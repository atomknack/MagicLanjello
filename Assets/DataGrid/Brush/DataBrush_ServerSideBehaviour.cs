using MagicLanjello.CellPlaceholder;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UnityEngine;


[AddComponentMenu("MagicLanJello/DataBrush_ServerSideBehaviour")]

internal class DataBrush_ServerSideBehaviour : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    SOEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient> _putCellServerEvent;

    [SerializeField]
    [ValidReference]
    SOPublisher<ArraySegment<byte>> _haveBytes;

    private DataBrush _outer;
    private void Awake()
    {
        _outer = new DataBrush();
        _outer.Reset();
    }

    private void ToBytes(Vector3Int pos, CellPlaceholderStruct placeholder, NetworkConnectionToClient client) =>
        _outer.ToBytes(pos, placeholder, _haveBytes.Publish);

    private void OnEnable()
    {
        if (_putCellServerEvent == null)
            throw new NullReferenceException(nameof(_putCellServerEvent));
        if (_haveBytes == null)
            throw new NullReferenceException(nameof(_haveBytes));
        _putCellServerEvent.Subscribe(ToBytes);
    }

    private void OnDisable()
    {
        _putCellServerEvent.UnsubscribeNullSafe(ToBytes);
    }
}