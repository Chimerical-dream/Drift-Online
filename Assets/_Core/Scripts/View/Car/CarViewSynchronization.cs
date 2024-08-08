using ChimeraGames.Fusion;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine;
using DG.Tweening;

public class CarViewSynchronization : NetworkBehaviour
{
    [SerializeField]
    private Transform[] wheelModels;
    [SerializeField]
    private Transform[] wheelRotateModels;
    [SerializeField]
    private CarViewSetupper carViewSetupper;
    [SerializeField]
    private ParticleSystem[] wheelSlipFxs;
    [SerializeField]
    private float minSlipToActivateFx = 2, slipFxMultiplier = 3f;
    private float[] wheelsRotationSpeed = new float[4];

    [Networked, Capacity(4)]
    private NetworkArray<WheelSynchronizationData> networkedWheelsSynchronizationData { get; }

    public void InitAsLocalPlayer()
    {
        carViewSetupper.SetPrint(SaveSystem.CarViewData.CarPrintImage);

        enabled = false;
    }

    public void SetPrint(byte[] imageBytes)
    {
        carViewSetupper.SetPrint(imageBytes);
    }



    public void SetupPlayerData(PlayerData playerData)
    {
        carViewSetupper.SetColor(playerData.SyncedCarViewData.CarBodyColor);
        carViewSetupper.SetSpoiler(playerData.SyncedCarViewData.SpoilerIndex);
        carViewSetupper.SetPrintValues(playerData.SyncedCarViewData.CarPrintValues);
    }

    public void RotateWheels(WheelCollider[] wheelColliders)
    {
        WheelSynchronizationData[] synchronizationDatas = new WheelSynchronizationData[networkedWheelsSynchronizationData.Length];
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            Vector3 pos;
            Quaternion rot;
            wheelColliders[i].GetWorldPose(out pos, out rot);
            //wheelModels[i].SetPositionAndRotation(pos, rot);
            wheelModels[i].rotation = rot;


            float driftAmount = GetDriftAmount(wheelColliders[i]);
            float fxRateOverDist = driftAmount > 0 ? 1 : 0;
            float fxRateOverTime = driftAmount * slipFxMultiplier;

            var emission = wheelSlipFxs[i].emission;
            emission.rateOverTime = fxRateOverTime;
            emission.rateOverDistance = fxRateOverDist;


            synchronizationDatas[i].wheelsRotationSpeed = wheelColliders[i].rotationSpeed;
            Vector3 projection = transform.InverseTransformDirection(Vector3.ProjectOnPlane(wheelModels[i].forward, transform.up));
            if(projection.z < 0)
            {
                projection.z *= -1;
                projection.x *= -1;
            }
            projection = transform.TransformDirection(projection);

            float yRot = Vector3.SignedAngle(transform.forward, projection, transform.up);
            if(yRot == 180)
            {
                yRot = 0;
            }
            synchronizationDatas[i].wheelsYrotation = yRot;
            synchronizationDatas[i].driftFxRateOverDist = fxRateOverDist;
            synchronizationDatas[i].driftFxRateOverTime = fxRateOverTime;
        }

        for(int i =0; i< synchronizationDatas.Length; i++)
        {
            networkedWheelsSynchronizationData.Set(i, synchronizationDatas[i]);
        }
    }

    private float GetDriftAmount(WheelCollider wheelCollider)
    {
        WheelHit wheelHit;
        wheelCollider.GetGroundHit(out wheelHit);
        float driftAmount = Mathf.Abs(wheelHit.sidewaysSlip) + Mathf.Abs(wheelHit.forwardSlip) * .3f;
        if (driftAmount < minSlipToActivateFx)
        {
            driftAmount = 0;
        }
        return driftAmount;
    }

    private void FixedUpdate()
    {
        SimulateOnOtherClients();
    }

    private void SimulateOnOtherClients()
    {
        //WheelSynchronizationData[] synchronizationDatas = networkedWheelsSynchronizationData.ToArray();
        for (int i = 0; i < wheelSlipFxs.Length; i++)
        {
            var emission = wheelSlipFxs[i].emission;
            emission.rateOverTime = networkedWheelsSynchronizationData.Get(i).driftFxRateOverTime;
            emission.rateOverDistance = networkedWheelsSynchronizationData.Get(i).driftFxRateOverDist;
        }

        for (int i = 0; i < wheelModels.Length; i++)
        {
            var wheel = wheelModels[i];
            wheel.localEulerAngles = new Vector3(0, networkedWheelsSynchronizationData.Get(i).wheelsYrotation, 0);


            float lerpSpeed = Mathf.Abs(networkedWheelsSynchronizationData[i].wheelsRotationSpeed) > Mathf.Abs(wheelsRotationSpeed[i]) ? 1 : Time.deltaTime * 5f;
            wheelsRotationSpeed[i] = Mathf.Lerp(wheelsRotationSpeed[i], networkedWheelsSynchronizationData[i].wheelsRotationSpeed, lerpSpeed);
            wheelRotateModels[i].localRotation = Quaternion.Euler(wheelRotateModels[i].localEulerAngles + Vector3.right * wheelsRotationSpeed[i] * Time.deltaTime);
        }
    }

    [System.Serializable]
    public struct WheelSynchronizationData : INetworkStruct
    {
        public float driftFxRateOverTime;
        public float driftFxRateOverDist;
        public float wheelsRotationSpeed;
        public float wheelsYrotation;
    }
}
