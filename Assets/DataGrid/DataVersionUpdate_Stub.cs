using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UnityEngine;
using UnityEngine.Events;

public class DataVersionUpdate_Stub : NetworkBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOEvent _onServerUpDataVersion;

    [SerializeField]
    private UnityEvent _onServerUpDataVersionPressed;

    [SerializeField]
    private UnityEvent notHostClientAfterDoneWithData;

    public void DoSomethingWithDataBeforeDataChange(ArraySegment<byte> data)
    {
        Debug.Log($"did something with data {data.Count}");
        notHostClientAfterDoneWithData.Invoke();
    }

    private void UpDataVersion()
    {
        _onServerUpDataVersionPressed?.Invoke();
    }

    public override void OnStartServer()
    {
        _onServerUpDataVersion.Subscribe(UpDataVersion);
        base.OnStartServer();
    }
    public override void OnStopServer()
    {
        _onServerUpDataVersion.UnsubscribeNullSafe(UpDataVersion);
        base.OnStopServer();
    }
}
