using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Photon.Realtime;
using ChimeraGames.Fusion;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LobbyInfoScreen : MonoBehaviour
{
    [SerializeField]
    private List<LobbyPlayerInfo> playerInfos = new List<LobbyPlayerInfo>();
    [SerializeField]
    private Text roomTitle;
    private Cooldown updateCd = new Cooldown(.5f);

    private void Awake()
    {
        Init();
    }

    private async void Init()
    {
        enabled = false;

        while (!FusionConnection.NetworkRunner.IsConnectedToServer)
        {
            await Task.Delay(500);
        }

        roomTitle.text = FusionConnection.NetworkRunner.SessionInfo.Name; 
        enabled = true;
    }

    private void FixedUpdate()
    {
        if (!updateCd.IsElapsed)
        {
            return;
        }
        updateCd.Reset();

        int ind = 0;
        foreach(var playerRef in FusionConnection.NetworkRunner.ActivePlayers)
        {
            var car = LobbyController.Instance.GetCar(playerRef.PlayerId);
            if(car == null)
            {
                continue;
            }
            UpdateInfo(ind, car.NetworkedPlayerData.Name.ToString(), car.NetworkedScore, FusionConnection.NetworkRunner.GetPlayerRtt(playerRef));
            ind++;
        }

        for(int i = ind; i < playerInfos.Count; i++)
        {
            playerInfos[i].Hide();
        }
    }

    private void UpdateInfo(int ind, string name, int score, double ping)
    {
        while (playerInfos.Count < (ind + 1))
        {
            playerInfos.Add(Instantiate<LobbyPlayerInfo>(playerInfos[0], playerInfos[0].transform.parent));
        }

        playerInfos[ind].ShowInfo(name, (int)(ping * 1000), score);
    }
}
