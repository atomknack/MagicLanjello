using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using UnityEngine;

public class PlayerNetworkInput : NetworkBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOEvent<Vector2> _inputMoveXZ;

    private Vector2 _valueOfInputMoveXZ = Vector2.zero;

    [SerializeField]
    [ValidReference]
    private SOEvent<float> _inputMoveY;

    private float _valueOfinputMoveY = 0f;

    [SerializeField]
    [ValidReference]
    private SOEvent<Vector2> _screenLookDelta;

    private Vector2 _valueOfScreenLookDelta = Vector2.zero;

    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private Transform _target;

    [SerializeField]
    [ValidReference]
    [DisableEditingInPlaymode]
    private Transform _localFollowerOfTarget;

    [SerializeField]
    private float rotationPower = 3f;
    [SerializeField]
    public float rotationLerp = 0.5f;

    [SerializeField]
    private float _speed = 1f;

    private Camera _camera;

    private bool _componentInitialized = false;

    public override void OnStartLocalPlayer()
    {
        InitVariablesOrThrow();

        //_InputJumpEvent.Subscribe(OnInputJump);
        _componentInitialized = true;

        void InitVariablesOrThrow()
        {
            if (_inputMoveXZ == null)
                throw new System.ArgumentNullException(nameof(_inputMoveXZ));
            if (_inputMoveY == null)
                throw new System.ArgumentNullException(nameof(_inputMoveY));
            if (_screenLookDelta == null)
                throw new System.ArgumentNullException(nameof(_screenLookDelta));

            if (_target == null)
                throw new System.ArgumentNullException("Gameobject has no assigned _target");
            if (_localFollowerOfTarget == null)
                throw new System.ArgumentNullException("Gameobject has no assigned _followerOfTarget");
            if (_localFollowerOfTarget.IsChildOf(_target) == false)
                throw new System.Exception($"{_target.name} should be parent of {_localFollowerOfTarget.name}");
            _camera = Camera.main;
            if (_camera == null)
                throw new System.ArgumentNullException("cannot assign camera");

            _inputMoveXZ.Subscribe(MoveXZ);
            _inputMoveY.Subscribe(MoveY);
            _screenLookDelta.Subscribe(Look);
        }
    }

    private void OnDisable() => OnStopClient();
    public override void OnStopClient()
    {
        if (_componentInitialized == false)
            return;

        _inputMoveXZ.UnsubscribeNullSafe(MoveXZ);
        _inputMoveY.UnsubscribeNullSafe(MoveY);
        _screenLookDelta.UnsubscribeNullSafe(Look);

        _componentInitialized = false;
    }

    private void Update()
    {
        var look = _valueOfScreenLookDelta;
        //_valueOfScreenLookDelta = Vector2.zero;
        var moveXZ = _valueOfInputMoveXZ;
        //_valueOfInputMoveXZ = Vector2.zero;
        var moveY = _valueOfinputMoveY;
        //_valueOfinputMoveY = 0f;

        if (isLocalPlayer == false)
            return;

        //var look = _screenLookDelta.GetValue();
        _localFollowerOfTarget.rotation *= Quaternion.AngleAxis(look.x * rotationPower, Vector3.up);
        _localFollowerOfTarget.rotation *= Quaternion.AngleAxis(look.y * rotationPower, Vector3.right);


        var angles = _localFollowerOfTarget.localEulerAngles;
        angles.z = 0;
        angles.x = ClampUpDown(angles.x);

        _localFollowerOfTarget.localEulerAngles = angles;



        //var moveXZ = _inputMoveXZ.GetValue();
        if (moveXZ.x == 0 && moveXZ.y == 0)
        {
            return;
        }
        float moveSpeed = _speed / 100f;
        Vector3 position = (_camera.transform.forward * moveXZ.y * moveSpeed) + (_camera.transform.right * moveXZ.x * moveSpeed);
        position.y = 0;
        //Debug.Log($"{transform.position} {position}");
        _target.position = _target.position + position;


        //Set the player rotation based on the look transform
        _target.rotation = Quaternion.Euler(0, _localFollowerOfTarget.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        _localFollowerOfTarget.localEulerAngles = new Vector3(angles.x, 0, 0);

        float ClampUpDown(float angle)
        {
            if (angle < 180 && angle > 70)
                return 70;

            if (angle > 180 && angle < 345)
                return 345;

            return angle;
        }
    }

    private void Look(Vector2 accumulatedDelta)
    {
        _valueOfScreenLookDelta = accumulatedDelta;
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
