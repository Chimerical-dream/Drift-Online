using ChimeraGames.Fusion;
using DG.Tweening;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarSynchronizer : NetworkBehaviour
{
    public static UnityEvent<CarSynchronizer> AnnounceCar = new UnityEvent<CarSynchronizer>();
    [SerializeField]
    private CarViewSynchronization carViewSynchronization;

    ReliableKey reliableKey => ReliableKey.FromInts(thisClassStamp, FusionConnection.NetworkRunner.LocalPlayer.PlayerId, networkedPlayerData.PlayerId);
    private int thisClassStamp => nameof(CarSynchronizer).GetHashCode();


    [Networked]
    private PlayerData networkedPlayerData { get; set; }
    public PlayerData NetworkedPlayerData => networkedPlayerData;

    private void Start()
    {
        if (!HasStateAuthority)
        {
            FusionConnection.OnDataReceived.AddListener(OnDataRecieved);

            DOVirtual.DelayedCall(2f, FinishIniting);
            return;
        }


        networkedPlayerData = new PlayerData(SaveSystem.SaveData.Nickname, SaveSystem.CarViewData, FusionConnection.NetworkRunner.LocalPlayer.PlayerId);

        foreach (var player in FusionConnection.NetworkRunner.ActivePlayers)
        {
            if (player.PlayerId == FusionConnection.NetworkRunner.LocalPlayer.PlayerId)
            {
                continue;
            }

            FusionConnection.NetworkRunner.SendReliableDataToPlayer(player, reliableKey, SaveSystem.CarViewData.ImageBytes);
        }

        FusionConnection.OnPlayerJoinedSession.AddListener(OnNewPlayerJoined);
        FinishIniting();
    }

    private void FinishIniting()
    {
        carViewSynchronization.SetupPlayerData(networkedPlayerData);
        AnnounceCar.Invoke(this);
    }



    private void OnNewPlayerJoined(PlayerRef playerRef)
    {
        if (playerRef.PlayerId == FusionConnection.NetworkRunner.LocalPlayer.PlayerId)
        {
            return;
        }
        FusionConnection.NetworkRunner.SendReliableDataToPlayer(playerRef, reliableKey, SaveSystem.CarViewData.ImageBytes);
    }

    private void OnDataRecieved(ReliableKey key, PlayerRef playerRef, ArraySegment<byte> data)
    {
        int key1, key2, key3, key4;
        key.GetInts(out key1, out key2, out key3, out key4);
        if (key1 != thisClassStamp)
        {
            Debug.LogWarning("WRONG KEY");
            return;
        }
        if (key3 != networkedPlayerData.PlayerId)
        {
            return;
        }

        Debug.Log(FusionConnection.NetworkRunner.LocalPlayer.PlayerId + " GOT DATA FROM " + key2);

        carViewSynchronization.SetPrint(data.Array);
    }
}
