using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Values;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetMoveRelativeToCamera : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOEvent<Vector2> _moveXZProvider;

    private Vector2 _moveXZ = Vector2.zero;

    [SerializeField]
    private Vector2 _speedXZ = Vector2.one;

    [SerializeField]
    [ValidReference]
    private SOEvent<float> _moveYProvider;

    private float _moveY = 0f;

    [SerializeField]
    private float _speedY = 1f;

    [SerializeField]
    [ValidReference]
    Transform _target;

    private bool _initialized = false;

    private void Update()
    {
        if (_initialized == false)
            return;
        if (_moveXZ.magnitude < 0.00001f && MathF.Abs(_moveY) < 0.001f)
            return;
        Camera camera = Camera.main;
        if (camera == null)
            return;

        float delta = Time.deltaTime;

        Vector3 forward = camera.transform.forward;
        forward = new Vector3(forward.x, 0, forward.z).normalized *_moveXZ.y * _speedXZ.y * delta;

        Vector3 right = camera.transform.right;
        right = new Vector3(right.x, 0, right.z).normalized * _moveXZ.x * _speedXZ.x * delta;

        Vector3 up = camera.transform.up;
        up = new Vector3(0, up.y, 0).normalized * _moveY * _speedY * delta;

        _target.position += forward + right + up ;
    }


    public void OnEnable()
    {
        if (_moveYProvider == null)
            throw new System.ArgumentNullException(nameof(_moveYProvider));
        if (_moveXZProvider == null)
            throw new System.ArgumentNullException(nameof(_moveXZProvider));
        if (_target == null)
            throw new System.ArgumentNullException(nameof(_target));

        _moveXZProvider.Subscribe(MoveXZ);
        _moveYProvider.Subscribe(MoveY);

        _initialized = true;
    }

    public void OnDisable()
    {
        if (_initialized == false)
            return;
        _moveXZProvider.UnsubscribeNullSafe(MoveXZ);
        _moveYProvider.UnsubscribeNullSafe(MoveY);
    }


    private void MoveY(float y)
    {
        _moveY = y;
    }

    private void MoveXZ(Vector2 xz)
    {
        _moveXZ = xz;
    }
}
