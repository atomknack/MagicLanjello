using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UKnack.Attributes;
using UKnack.Events;


public class OnlyCallIfWasNoOther : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOPublisher _composite;

    private bool _shouldIDoIt = false;
    private bool _iForbid = false;

    public void DoSomeAction()
    {
        _shouldIDoIt = true;
    }

    public void ForbidDoingSomeAction()
    {
        _iForbid = true;
    }

    private void Update()
    {
        if (_shouldIDoIt && ( ! _iForbid) )
            _composite.Publish();
        _shouldIDoIt = false;
        _iForbid = false;
    }
}

