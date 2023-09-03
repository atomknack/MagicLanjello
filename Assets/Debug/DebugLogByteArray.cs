using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DebugLogByteArray : MonoBehaviour
{
    TextField _bytesArrayLog;
    Label _numberOfBytes;

    ArraySegment<byte> _segment = ArraySegment<byte>.Empty;
    StringBuilder sb = new StringBuilder();

    private const int everyNFrames = 100;
    private int frameCounter = 0;

    public void LogSegment(ArraySegment<byte> segment)
    {
        _segment = segment;
    }

    private void Update()
    {
        frameCounter++;
        if (frameCounter < everyNFrames)
            return;
        frameCounter = 0;
        LogSegment();
    }

    private void LogSegment()
    {
        if (_bytesArrayLog == null || _numberOfBytes == null)
        {
            Debug.Log("trying to write to unitialized component");
            return;
        }

        _numberOfBytes.text = _segment.Count.ToString();

        
        var temp = _segment;

        if (temp.Count > 121)
            temp = temp.Slice(temp.Count - 120);

        sb.Clear();
        for (int i = 0; i < temp.Count; i++)
        {
            sb.Append(temp[i]);
            sb.Append(',');
            if (i % 30 == 0 && i!=0)
                sb.Append("\n");

        }

        _bytesArrayLog.SetValueWithoutNotify(sb.ToString());
        
    }

    private void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;
        _numberOfBytes = root.Q<Label>("NumberOfBytes");
        if (_numberOfBytes == null)
            throw new System.ArgumentNullException(nameof(_numberOfBytes));
        _bytesArrayLog = root.Q<TextField>("Bytes");
        if (_bytesArrayLog == null)
            throw new System.ArgumentNullException(nameof(_bytesArrayLog));
    }


}
