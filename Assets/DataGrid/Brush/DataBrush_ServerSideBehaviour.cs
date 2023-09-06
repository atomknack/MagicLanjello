using MagicLanjello.CellPlaceholder;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("MagicLanJello/DataBrush_ServerSideBehaviour")]

internal class DataBrush_ServerSideBehaviour : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    SOEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient> _putCellServerEvent;

    [SerializeField]
    UnityEvent<ArraySegment<byte>> _haveBytesToSend;

    private DataBrush _outer;

    public void Reset()
    {
        Debug.Log("Reset for server side called");
        _outer.Reset();
    }

    private void Awake()
    {
        _outer = new DataBrush();
        _outer.Reset();
    }

    private void ToBytes(Vector3Int pos, CellPlaceholderStruct placeholder, NetworkConnectionToClient client)
    {
        Debug.Log($"Got request to put cell on server {pos}, {placeholder}, {client.connectionId}");
        _outer.ToBytes(pos, placeholder, _haveBytesToSend.Invoke);
    }


    private void OnEnable()
    {
        if (_putCellServerEvent == null)
            throw new NullReferenceException(nameof(_putCellServerEvent));
        if (_haveBytesToSend == null)
            throw new NullReferenceException(nameof(_haveBytesToSend));
        _putCellServerEvent.Subscribe(ToBytes);
    }

    private void OnDisable()
    {
        _putCellServerEvent.UnsubscribeNullSafe(ToBytes);
    }
}