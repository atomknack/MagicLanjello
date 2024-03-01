using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UnityEngine;

public class DataBrushCopier : MonoBehaviour
{
    [SerializeField]
    [ValidReference(typeof(IDataBrush))]
    private UnityEngine.Object _from;

    private IDataBrush _fromBrush;

    [SerializeField]
    [ValidReference(typeof(IDataBrush))]
    private UnityEngine.Object _to;

    private IDataBrush _toBrush;

    private void Awake()
    {
        _fromBrush = (IDataBrush)_from;
        if (_fromBrush == null )
            throw new System.ArgumentNullException(nameof(_fromBrush));
        _toBrush = (IDataBrush)_to;
        if (_toBrush == null ) 
            throw new System.ArgumentNullException( nameof(_toBrush));
    }

    public void CopyBrush()
    {
        (var pos, var placeholder) = _fromBrush.GetState();
        _toBrush.SetState(pos, placeholder);
    }
}
