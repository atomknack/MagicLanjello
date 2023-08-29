using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DebugLogByteArray : MonoBehaviour
{
    Label _bytesArrayLog;
    Label _numberOfBytes;

    public void LogSegment(ArraySegment<byte> segment)
    {
        if (_bytesArrayLog == null || _numberOfBytes == null)
        {
            Debug.Log("trying to write to unitialized component");
            return;
        }


        _bytesArrayLog.text = String.Join(',',segment.Array);
        _numberOfBytes.text = segment.Count.ToString();
    }

    private void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;
        _numberOfBytes = root.Q<Label>("NumberOfBytes");
        if (_numberOfBytes == null)
            throw new System.ArgumentNullException(nameof(_numberOfBytes));
        _bytesArrayLog = root.Q<Label>("Bytes");
        if (_bytesArrayLog == null)
            throw new System.ArgumentNullException(nameof(_bytesArrayLog));
    }


}
