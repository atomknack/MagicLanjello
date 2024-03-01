using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using UnityEngine;

public class GNIF_SaverLoader : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private SenderByteDataToClients _dataSender;

    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private SOEvent<string> _saveEvent;

    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private DataBrush_ServerSideBehaviour _serverSideBrush;

    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private DataBrush_ClientSideBehaviour _clientSideBrush;

    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private SOEvent<string> _loadEvent;

    private static char[] gnif = new char[8] { 'g', 'n', 'i', 'f', '0', '0', '0', '1' };
    private static byte[] emptyBytes52 = new byte[52];

    private void OnEnable()
    {
        NullChecks();
        _saveEvent.Subscribe(SaveFile);
        _loadEvent.Subscribe(LoadFile);
    }

    internal void SaveFile(string path)
    {
        var span = _dataSender.AsBytesReadOnlySpan;
        if (span.Length == 0)
            return;

        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(gnif, 0, 8);
            writer.Write(emptyBytes52, 0, 52);
            Int32 len = span.Length;
            writer.Write(len);
            writer.Write(span);
        }
    }

    private void LoadFile(string path)
    {
        byte[] bytes;
        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            reader.ReadBytes(60);
            int len = reader.ReadInt32();
            bytes = reader.ReadBytes(len);
            if (bytes.Length != len)
                throw new System.Exception($"was able to read only {bytes.Length} of {len}, there probably error with file: {path}");
        }
        _serverSideBrush.ClearBrush();
        if (_dataSender.AsBytesReadOnlySpan.Length > 0)
            throw new System.Exception("DataSender should be clean after serverSide brush clear");
        _dataSender.AddToData(bytes);
        (var pos, var placeholder) = ((IDataBrush)_clientSideBrush).GetState();
        ((IDataBrush)_serverSideBrush).SetState(pos, placeholder);
    }

    private void OnDisable()
    {
        NullChecks();
        _saveEvent.UnsubscribeNullSafe(SaveFile);
        _loadEvent.UnsubscribeNullSafe(LoadFile);
    }

    private void NullChecks()
    {
        if (_dataSender == null) throw new System.NullReferenceException(nameof(_dataSender));
        if (_saveEvent == null) throw new System.NullReferenceException(nameof(_saveEvent));
        if (_serverSideBrush == null) throw new System.NullReferenceException(nameof(_serverSideBrush));
        if (_clientSideBrush == null) throw new System.NullReferenceException(nameof(_clientSideBrush));
        if (_loadEvent == null) throw new System.NullReferenceException(nameof(_loadEvent));
    }
}
