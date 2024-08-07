using Fusion;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CarController : SimulationBehaviour
{
    public static UnityEvent<CarController> AnnounceLocalPlayer = new UnityEvent<CarController>();

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
    private float torque, maxSteer, downForceCoef, brakingSpeed, freeDriveBraking;
    [SerializeField]
    private CarType carType;
    [SerializeField]
    private Transform cameraFollowPoint;
    public Transform CameraFollowPoint => cameraFollowPoint;

    private Controls playerControls;

    private Vector2 input;
    [SerializeField]
    private float steeringProgress, desteerSpeed, steerSpeed, turnRadius;
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
        if(rb.velocity.magnitude < 5)
        {
            desiredRot = transform.rotation;
        }
        else
        {
            desiredRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(rb.velocity, Vector3.up).normalized, Vector3.up);
        }
        cameraFollowPoint.rotation = Quaternion.Lerp(cameraFollowPoint.rotation, desiredRot, Time.fixedDeltaTime * 8);

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
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].brakeTorque = brakeTorque;
            wheelBrake[i] = wheelColliders[i].brakeTorque;
        }
    }

    private void RotateWheels()
    {
        float autoRotateValue = 0;

        Vector3 rbVelocityVector = Vector3.ProjectOnPlane(rb.velocity, transform.up);
        float rbVelocity = rbVelocityVector.magnitude;
        rbVelocityVector.Normalize();

        if(rbVelocity > 3f)
        {
            autoRotateValue = Mathf.Clamp(Mathf.Deg2Rad * Vector3.SignedAngle(transform.forward, rbVelocityVector, transform.up), -1, 1);
        }

        if(Mathf.Abs(autoRotateValue) < .1f)
        {
            autoRotateValue = 0;
        }
        


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
        //float turnRadius = Mathf.Clamp(this.turnRadius * rb.velocity.magnitude, 2, 50);// carLength / Mathf.Tan(maxSteer);
        wheelColliders[input.x > 0 ? 0 : 1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(carLength / (turnRadius + halfCarWidth)) * steeringProgress * maxSteer;
        wheelColliders[input.x > 0 ? 1 : 0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(carLength / (turnRadius - halfCarWidth)) * steeringProgress * maxSteer;
    }



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
        throw new NotImplementedException();
    }

    private void OnHandBrakeCancelled(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }
}
