using Fusion;
using UnityEngine;


[System.Serializable]
public struct PlayerData : INetworkStruct
{
    public NetworkString<_128> Name;
    public SyncedCarViewData SyncedCarViewData;
    public int PlayerId;

    public PlayerData(NetworkString<_128> name, CarViewData carViewData, int playerId)
    {
        Name = name;
        SyncedCarViewData = new SyncedCarViewData(carViewData);
        PlayerId = playerId;
    }
}
