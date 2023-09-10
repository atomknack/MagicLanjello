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

internal class DataBrush_ServerSideBehaviour : MonoBehaviour, IDataBrush
{
    [SerializeField]
    [ValidReference]
    SOEvent<Vector3Int, CellPlaceholderStruct, NetworkConnectionToClient> _putCellServerEvent;

    [SerializeField]
    UnityEvent<ArraySegment<byte>> _haveBytesToSend;

    [SerializeField]
    private UnityEvent _afterClearIsCalled;

    private DataBrush _brush;

    public void Clear()
    {
        Debug.Log("Server side brush Clear called");
        ClearWithoutNotify();
        _afterClearIsCalled.Invoke();
    }

    private void ClearWithoutNotify()
    {
        _brush.Reset();
    }

    private void Awake()
    {
        _brush = new DataBrush();
        ClearWithoutNotify();
    }

    private void ToBytes(Vector3Int pos, CellPlaceholderStruct placeholder, NetworkConnectionToClient client)
    {
        string id = client == null ? "null" : client.connectionId.ToString();
        Debug.Log($"Got request to put cell on server {pos}, {placeholder}, {id}");
        _brush.ToBytes(pos, placeholder, _haveBytesToSend.Invoke);
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

    void IDataBrush.SetState(Vector3Int position, CellPlaceholderStruct placeholder) => _brush.SetState(position, placeholder);
    (Vector3Int position, CellPlaceholderStruct placeholder) IDataBrush.GetState() => _brush.GetState();
}