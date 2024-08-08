using ChimeraGames.Fusion;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    private static LobbyController instance;
    public static LobbyController Instance => instance;

    private Dictionary<int, CarSynchronizer> cars = new Dictionary<int, CarSynchronizer>();

    private void Awake()
    {
        instance = this;
        CarSynchronizer.AnnounceCar.AddListener(OnCarAnnounced);
        FusionConnection.OnPlayerLeftSession.AddListener(OnPlayerLeftSession);
    }


    private void OnPlayerLeftSession(PlayerRef player)
    {
        if (cars.ContainsKey(player.PlayerId))
        {
            cars.Remove(player.PlayerId);
        }
    }

    private void OnCarAnnounced(CarSynchronizer car)
    {
        cars.Add(car.NetworkedPlayerData.PlayerId, car);
    }
}
