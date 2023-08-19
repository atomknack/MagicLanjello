using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using UnityEngine;

public class TargetMoveRelativeToCamera : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOEvent<Vector2> _inputMoveXZ;

    private Vector2 _valueOfInputMoveXZ = Vector2.zero;

    [SerializeField]
    private Vector2 _speedXZ = Vector2.one;

    [SerializeField]
    [ValidReference]
    private SOEvent<float> _inputMoveY;

    private float _valueOfinputMoveY = 0f;

    [SerializeField]
    private float _speedYZ = 1f;

    [SerializeField]
    [ValidReference]
    Transform _target;


    public void OnEnable()
    {
        InitVariablesOrThrow();


        void InitVariablesOrThrow()
        {
            if (_inputMoveXZ == null)
                throw new System.ArgumentNullException(nameof(_inputMoveXZ));
            if (_inputMoveY == null)
                throw new System.ArgumentNullException(nameof(_inputMoveY));

            _inputMoveXZ.Subscribe(MoveXZ);
            _inputMoveY.Subscribe(MoveY);
        }
    }


    private void Update()
    {
        if (_valueOfInputMoveXZ.magnitude < 0.00001f && MathF.Abs(_valueOfinputMoveY) < 0.001f)
            return;
        Camera camera = Camera.main;
        if (camera == null)
            return;

    }



    private void MoveY(float y)
    {
        _valueOfinputMoveY = y;
    }

    private void MoveXZ(Vector2 xz)
    {
        _valueOfInputMoveXZ = xz;
    }
}
