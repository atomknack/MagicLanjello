using UnityEngine;
using UnityEngine.Events;
using MagicLanjello.CellPlaceholder;
using System;

internal interface IDataUpdater<T>
{
    public void UpdateData(T data);
    public void Reset();
    public static IDataUpdater<T> Cast(System.Object data) => (IDataUpdater<T>)data;
}

[AddComponentMenu("MagicLanJello/DataBrush_ClientSideBehaviour")]
internal class DataBrush_ClientSideBehaviour : MonoBehaviour, IDataUpdater<ArraySegment<byte>>, IDataBrush
{
    [SerializeField]
    private UnityEvent<Vector3Int, CellPlaceholderStruct> _gotNewCell;

    [SerializeField]
    private UnityEvent _afterClearIsCalled;

    private DataBrush _brush;

    private int _bytesIndex;

    private void Awake()
    {
        _brush = new DataBrush();
        ClearWithoutNotify();
    }

    public void Reset()
    {
        Debug.Log("Client side brush Clear called");
        ClearWithoutNotify();
        _afterClearIsCalled.Invoke();
    }

    private void ClearWithoutNotify()
    {

        _bytesIndex = 0;
        _brush.Reset();
    }

    public void UpdateData(ArraySegment<byte> bytes)
    {
        if (bytes.Count < _bytesIndex)
            throw new System.ArgumentException("Did you forget to Reset() this after resetting level?");
        var span = bytes.AsSpan();
        int prevIndex = _bytesIndex;
        while (TryReadOneFromBytes(span, ref _bytesIndex))
        {
            //Debug.Log(prevIndex);
            if (_bytesIndex == prevIndex)
                throw new System.Exception($"there no advance on {prevIndex}, of Segment byte {bytes[prevIndex]} of total number of bytes {bytes.Count}");
            prevIndex = _bytesIndex;
        }
    }

    private bool TryReadOneFromBytes(System.ReadOnlySpan<byte> bytes, ref int index) =>
        _brush.TryReadOneFromBytes(bytes, ref index, _gotNewCell.Invoke);


    void IDataBrush.SetState(Vector3Int position, CellPlaceholderStruct placeholder) => _brush.SetState(position, placeholder);
    (Vector3Int position, CellPlaceholderStruct placeholder) IDataBrush.GetState() => _brush.GetState();
}