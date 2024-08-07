using ChimeraGames.Fusion;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChimeraGames.UI.MainMenu
{
    public class JoinLobbyButton : MonoBehaviour
    {
        [SerializeField]
        private Text lobbyName, playerCount;
        [SerializeField]
        private Button joinButton;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(SessionInfo info)
        {
            gameObject.SetActive(true);
            lobbyName.text = info.Name;
            playerCount.text = info.PlayerCount + "/" + info.MaxPlayers;
            joinButton.interactable = info.IsOpen && info.PlayerCount < info.MaxPlayers && info.IsValid;
        }

        public void OnClick()
        {
            FusionConnection.Instance.JoinLobby(lobbyName.text);
        }
    }
}
