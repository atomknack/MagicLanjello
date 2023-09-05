using UnityEngine;
using UnityEngine.Events;
using MagicLanjello.CellPlaceholder;
using System;

[AddComponentMenu("MagicLanJello/DataBrush_ClientSideBehaviour")]
internal class DataBrush_ClientSideBehaviour : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<Vector3Int, CellPlaceholderStruct> _gotNewCell;

    private DataBrush _outer;

    private int _bytesIndex;
    private void Awake()
    {
        _outer = new DataBrush();
        Reset();
    }

    public void Reset()
    {
        _bytesIndex = 0;
        _outer.Reset();
    }


    public void GotBytes(System.ArraySegment<byte> bytes)
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
        _outer.TryReadOneFromBytes(bytes, ref index, _gotNewCell.Invoke);

}