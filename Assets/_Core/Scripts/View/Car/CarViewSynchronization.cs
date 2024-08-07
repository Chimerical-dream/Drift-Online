using ChimeraGames.Fusion;
using Fusion;
using System;
using UnityEngine;

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
    private NetworkArray<SynchronizationData> synchronizationData { get; }
    [Networked(OnChanged = nameof(PlayerDataChanged))]
    private PlayerData playerData { get; set; }

    private void Awake()
    {
        FusionConnection.OnDataReceived.AddListener(OnDataRecieved);
    }

    public void InitAsLocalPlayer()
    {
        playerData = new PlayerData(SaveSystem.SaveData.Nickname, SaveSystem.CarViewData, FusionConnection.NetworkRunner.LocalPlayer.PlayerId);
        
        foreach(var player in FusionConnection.NetworkRunner.ActivePlayers)
        {
            if (player.Equals(FusionConnection.NetworkRunner.LocalPlayer))
            {
                continue;
            }
            FusionConnection.NetworkRunner.SendReliableDataToPlayer(player, SaveSystem.CarViewData.ImageBytes);
        }
        carViewSetupper.SetPrint(SaveSystem.CarViewData.CarPrintImage);
        SetupPlayerData();


        enabled = false;
    }

    public static void PlayerDataChanged(Changed<CarViewSynchronization> changed)
    {
        changed.Behaviour.SetupPlayerData();
    }


    private void SetupPlayerData()
    {
        carViewSetupper.SetColor(playerData.SyncedCarViewData.CarBodyColor);
        carViewSetupper.SetSpoiler(playerData.SyncedCarViewData.SpoilerIndex);
        carViewSetupper.SetPrintValues(playerData.SyncedCarViewData.CarPrintValues);
    }


    private void OnDataRecieved(PlayerRef playerRef, ArraySegment<byte> data)
    {
        //if (playerRef.PlayerId == playerData.PlayerId)
        //{
        //    return;
        //}

        Debug.Log(FusionConnection.NetworkRunner.LocalPlayer.PlayerId + " GOT DATA FROM " + playerRef.PlayerId);

        carViewSetupper.SetPrint(data.Array);
    }

    public void RotateWheels(WheelCollider[] wheelColliders)
    {
        SynchronizationData[] synchronizationDatas = new SynchronizationData[synchronizationData.Length];
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
            synchronizationData.Set(i, synchronizationDatas[i]);
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
        SynchronizationData[] synchronizationDatas = synchronizationData.ToArray();
        for(int i = 0; i< wheelSlipFxs.Length; i++)
        {
            var emission = wheelSlipFxs[i].emission;
            emission.rateOverTime = synchronizationDatas[i].driftFxRateOverTime;
            emission.rateOverDistance = synchronizationDatas[i].driftFxRateOverDist;
        }

        for(int i = 0; i < wheelModels.Length; i++)
        {
            var wheel = wheelModels[i];
            wheel.localEulerAngles = new Vector3(0, synchronizationDatas[i].wheelsYrotation, 0);


            float lerpSpeed = Mathf.Abs(synchronizationData[i].wheelsRotationSpeed) > Mathf.Abs(wheelsRotationSpeed[i]) ? 1 : Time.deltaTime * 5f;
            wheelsRotationSpeed[i] = Mathf.Lerp(wheelsRotationSpeed[i], synchronizationData[i].wheelsRotationSpeed, lerpSpeed);
            wheelRotateModels[i].localRotation = Quaternion.Euler(wheelRotateModels[i].localEulerAngles + Vector3.right * wheelsRotationSpeed[i] * Time.deltaTime);
        }
    }

    [System.Serializable]
    public struct SynchronizationData : INetworkStruct
    {
        public float driftFxRateOverTime;
        public float driftFxRateOverDist;
        public float wheelsRotationSpeed;
        public float wheelsYrotation;
    }
}
