using ChimeraGames.Fusion;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChimeraGames.UI.MainMenu
{
    public class OnlineScreen : Tab
    {
        public static UnityEvent<string> OnPlayerNicknameChanged = new UnityEvent<string>();

        [SerializeField]
        private InputField nicknameInput;
        [SerializeField]
        private List<JoinLobbyButton> joinLobbyButtons = new List<JoinLobbyButton>();
        [SerializeField]
        private Button refreshBtn;


        private void Awake()
        {
            nicknameInput.SetTextWithoutNotify(SaveSystem.SaveData.Nickname);
            nicknameInput.onValueChanged.AddListener(OnNicknameChanged);

            RefreshLobbies();
        }

        public async void RefreshLobbies()
        {
            refreshBtn.interactable = false;

            foreach(var sessionBtn in joinLobbyButtons)
            {
                sessionBtn.Hide();
            }

            int ind = 0;
            foreach(var session in FusionConnection.Instance.sessionInfos)
            {
                await Task.Delay(100);

                AddSessionBtn(ind, session);
                ind++;
            }



            await Task.Delay(1000);
            if(refreshBtn == null) //user switched scenes
            {
                return;
            }
            refreshBtn.interactable = true;
        }

        private void AddSessionBtn(int ind, SessionInfo sessionInfo)
        {
            while(joinLobbyButtons.Count < (ind + 1))
            {
                joinLobbyButtons.Add(Instantiate(joinLobbyButtons[0], joinLobbyButtons[0].transform.parent));
            }

            joinLobbyButtons[ind].Show(sessionInfo);
        }

        public void OnNewLobbyClick()
        {
            FusionConnection.Instance.CreateNewLobby();
        }

        private void OnNicknameChanged(string value)
        {
            OnPlayerNicknameChanged.Invoke(value);
        }
    }
}
