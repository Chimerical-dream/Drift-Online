using Fusion;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CarController : NetworkBehaviour
{
    public static UnityEvent<CarController> AnnounceLocalPlayer = new UnityEvent<CarController>();
    public UnityEvent OnDriftStart = new UnityEvent(), OnDriftEnd = new UnityEvent();

    public enum CarType
    {
        FWD, RWD, FourWD
    }
    [SerializeField]
    private CarViewSynchronization carView;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private WheelCollider[] wheelColliders;
    [SerializeField]
    private float torque, maxSteer, downForceCoef, brakingSpeed, freeDriveBraking, handBraking, minDriftAngle;
    [SerializeField]
    private CarType carType;
    [SerializeField]
    private Transform cameraFollowPoint;
    public Transform CameraFollowPoint => cameraFollowPoint;

    private Controls playerControls;

    [Networked] //NetworkBehaviour only saves correctly when it has [Networked] property for some reason...
    private int notUsedInt { get; set; }

    private bool isDrifting = false;
    private Vector2 input;
    private bool isHandbrakeOn = false;
    private Tween handbrakeTween;
    [SerializeField]
    private float steeringProgress, desteerSpeed, steerSpeed, turnRadius, defaultSidewaysStiffness, handbrakeSidewaysStifness;
    [SerializeField]
    private float[] wheelTorque = new float[4], wheelBrake = new float[4];

    private void Awake()
    {
    }

    private void Start()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        carView.InitAsLocalPlayer();

        AnnounceLocalPlayer.Invoke(this);
        playerControls = new Controls();
        playerControls.Driving.Enable();
        playerControls.Driving.Move.performed += OnMoveInput;
        playerControls.Driving.Move.canceled += OnMoveInput;
        playerControls.Driving.HandBrake.started += OnHandBrakeStarted;
        playerControls.Driving.HandBrake.canceled += OnHandBrakeCancelled;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        float motorTorque = input.y * torque;

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].motorTorque = 0;
        }

        switch (carType)
        {
            case CarType.FWD:
                wheelColliders[0].motorTorque = motorTorque;
                wheelColliders[1].motorTorque = motorTorque;
                break;
            case CarType.RWD:
                wheelColliders[2].motorTorque = motorTorque;
                wheelColliders[3].motorTorque = motorTorque;
                break;
            default:
                for (int i = 0; i < wheelColliders.Length; i++)
                {
                    wheelColliders[i].motorTorque = motorTorque;
                }
                break;
        }

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelTorque[i] = wheelColliders[i].motorTorque;
        }

        DoBraking();

        rb.AddForce(-transform.up * Vector3.Project(rb.velocity, transform.forward).magnitude * downForceCoef);

        DoCameraRotation();

        RotateWheels();
        carView.RotateWheels(wheelColliders);
    }

    private void DoCameraRotation()
    {
        Quaternion desiredRot;
        float rbVelocity = rb.velocity.magnitude, startCameraRotateVelocity = 3, maxCameraRotateVelocity = 10;
        if(rbVelocity < startCameraRotateVelocity)
        {
            desiredRot = transform.rotation;
        }
        else
        {
            desiredRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(rb.velocity, Vector3.up).normalized, Vector3.up);
        }

        float cameraRotatingSpeed = Mathf.Clamp01((rbVelocity - startCameraRotateVelocity) / (maxCameraRotateVelocity - startCameraRotateVelocity));
        cameraFollowPoint.rotation = Quaternion.Lerp(cameraFollowPoint.rotation, desiredRot, Time.fixedDeltaTime * 8 * cameraRotatingSpeed);

    }

    private void DoBraking()
    {
        float wheelDirection;
        if(carType == CarType.RWD)
        {
            wheelDirection = Mathf.Abs(wheelColliders[3].rotationSpeed) > 2 ? wheelColliders[3].rotationSpeed : 0;
        }
        else
        {
            wheelDirection = Mathf.Abs(wheelColliders[0].rotationSpeed) > 2 ? wheelColliders[0].rotationSpeed : 0;
        }

        bool isWheelNotSpinningInDesiredDirection = Mathf.Sign(input.y) != Mathf.Sign(wheelDirection) && input.y != 0 && wheelDirection != 0;
        bool isBraking = isWheelNotSpinningInDesiredDirection;

        float brakeTorque = 0;
        if(isBraking) 
        {
            brakeTorque = brakingSpeed;
        }
        else if(input.y == 0)
        {
            brakeTorque = freeDriveBraking;
        }

        if (isHandbrakeOn)
        {
            brakeTorque += handBraking;
        }

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].brakeTorque = brakeTorque + (((i == 2 || i == 3) && isHandbrakeOn) ? handBraking : 0);
            wheelBrake[i] = wheelColliders[i].brakeTorque;
        }
    }

    private void RotateWheels()
    {
        float autoRotateValue = CalculateAutoRotation();

        if(input.x == 0 && autoRotateValue == 0)
        {
            if(Mathf.Abs(steeringProgress) <= Time.deltaTime * desteerSpeed)
            {
                steeringProgress = 0;
            }
            else
            {
                steeringProgress -= Time.deltaTime * desteerSpeed * Mathf.Sign(steeringProgress);
            }
        }
        else if(input.x == 0 && autoRotateValue != 0)
        {
            steeringProgress = Mathf.Clamp(steeringProgress + autoRotateValue * Time.deltaTime * desteerSpeed, -Math.Abs(autoRotateValue), Mathf.Abs(autoRotateValue));
        }
        else
        {
            steeringProgress = Mathf.Clamp(steeringProgress + input.x * Time.deltaTime * steerSpeed, -Math.Abs(input.x), Mathf.Abs(input.x));
        }
        steeringProgress = Mathf.Clamp(steeringProgress, -1, 1);

        float halfCarWidth = 1f, carLength = 2.55f;
        wheelColliders[input.x > 0 ? 0 : 1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(carLength / (turnRadius + halfCarWidth)) * steeringProgress * maxSteer;
        wheelColliders[input.x > 0 ? 1 : 0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(carLength / (turnRadius - halfCarWidth)) * steeringProgress * maxSteer;
    }

    #region input
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();

        if (input.y != 0)
        {
            input.y = Mathf.Sign(input.y);
        }
    }


    private void OnHandBrakeStarted(InputAction.CallbackContext obj)
    {
        handbrakeTween?.Kill();
        handbrakeTween = DOVirtual.Float(defaultSidewaysStiffness, handbrakeSidewaysStifness, .3f, (float x) => SetRearWheelsSidewaysStifness(x));
        isHandbrakeOn = true;
    }

    private void OnHandBrakeCancelled(InputAction.CallbackContext obj)
    {
        handbrakeTween?.Kill();
        handbrakeTween = DOVirtual.Float(handbrakeSidewaysStifness, defaultSidewaysStiffness, .3f, (float x) => SetRearWheelsSidewaysStifness(x));
        isHandbrakeOn = false;
    }
    #endregion


    private void SetRearWheelsSidewaysStifness(float value)
    {
        WheelFrictionCurve wheelFrictionCurve = wheelColliders[2].sidewaysFriction;
        wheelFrictionCurve.stiffness = value;
        wheelColliders[2].sidewaysFriction = wheelFrictionCurve;
        wheelColliders[3].sidewaysFriction = wheelFrictionCurve;
    }
    private void SetIsDrifting(bool value)
    {
        if (value == isDrifting)
        {
            return;
        }

        isDrifting = value;

        if (value)
        {
            OnDriftStart.Invoke();
        }
        else
        {
            OnDriftEnd.Invoke();
        }
    }

    private float CalculateAutoRotation()
    {
        float autoRotateValue = 0;

        Vector3 rbVelocityVector = Vector3.ProjectOnPlane(rb.velocity, transform.up);
        float rbVelocity = rbVelocityVector.magnitude;
        rbVelocityVector.Normalize();

        float driftAngle = Vector3.SignedAngle(transform.forward, rbVelocityVector, transform.up);
        float startAutoRotateSpeed = 3f, speedForMaxAutoRotate = 6f;

        SetIsDrifting(Mathf.Abs(driftAngle) > minDriftAngle && rbVelocity > startAutoRotateSpeed);

        if (rbVelocity > startAutoRotateSpeed)
        {
            float autorotateStrength = Mathf.Clamp01((rbVelocity - startAutoRotateSpeed) / (speedForMaxAutoRotate - startAutoRotateSpeed));
            autoRotateValue = Mathf.Clamp(Mathf.Deg2Rad * driftAngle, -1, 1) * autorotateStrength;
        }

        if (Mathf.Abs(autoRotateValue) < .1f)
        {
            autoRotateValue = 0;
        }
        return autoRotateValue;
    }

}
