using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UnityEngine;

[AddComponentMenu("MouseOrbitImproved For SOEvents")]
public class MouseOrbitImproved : MonoBehaviour
{
    [SerializeField]
    [ValidReference]
    private SOEvent<Vector2> _screenLookDelta;

    private Vector2 _lookValue = Vector2.zero;

    [SerializeField]
    [ValidReference]
    private SOEvent<float> _BackAwayProvider;

    private float _backAwayValue = 0f;

    public Transform target;
    public float distance = 5.0f;
    public float distanceStep = 5.0f;

    [SerializeField]
    private Vector2 _speed = new Vector2(120, 120);

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public float flyOffdistance = 4.0f;
    public float flyOffSpeed = 1.0f;

    private Rigidbody _rigidbody;

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _screenLookDelta.Subscribe(Look);
        _BackAwayProvider.Subscribe(BackAway);
    }

    private void BackAway(float scroll)
    {
        _backAwayValue = scroll;
    }

    private void Look(Vector2 xy)
    {
        _lookValue = xy;
    }

    void LateUpdate()
    {

        x += _lookValue.x * _speed.x * distance * 0.02f;
        y -= _lookValue.y * _speed.y * 0.02f;

        _lookValue = Vector2.zero;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        if (distance < flyOffdistance)
        {
            RaycastHit hitback;
            if (!Physics.Linecast(transform.position, transform.position - (Vector3.forward * (flyOffSpeed * Time.deltaTime * 2)), out hitback))
            {
                //distance -= hitback.distance; //TODO: use real distance to calsulate how far camera can move
                distance += flyOffSpeed * Time.deltaTime; //P: slowly fly off from tracked object
            }

        }

        

        distance = Mathf.Clamp(distance - _backAwayValue * distanceStep, distanceMin, distanceMax);

        _backAwayValue = 0f;

        RaycastHit hit;
        if (Physics.Linecast(target.position, transform.position, out hit))
        {
            distance -= distance - hit.distance;
        }
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F; //P possible bug should be MOD of angle be used?
        if (angle > 360F)
            angle -= 360F; //P possible bug should be MOD of angle be used?
        return Mathf.Clamp(angle, min, max);
    }
}
