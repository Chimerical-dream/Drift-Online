using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ChimeraGames.Fusion
{
    public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static UnityEvent<ReliableKey, PlayerRef, ArraySegment<byte>> OnDataReceived = new UnityEvent<ReliableKey, PlayerRef, ArraySegment<byte>>();
        public static UnityEvent<PlayerRef> OnPlayerJoinedSession = new UnityEvent<PlayerRef>(), OnPlayerLeftSession = new UnityEvent<PlayerRef>();
        private static FusionConnection instance;
        public static FusionConnection Instance => instance;
        public static NetworkRunner NetworkRunner => instance.networkRunner;

        [SerializeField]
        private NetworkRunner networkRunner;
        [SerializeField]
        private NetworkObject playerPrefab;

        private static bool isCreatingNewLobby = false;
        private static bool isJoiningLobby = false;
        private static string lobbyToJoinName = "";

        public List<SessionInfo> sessionInfos = new List<SessionInfo>();

        private void Awake()
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            networkRunner.JoinSessionLobby(SessionLobby.Shared);
        }

        private void OnLevelWasLoaded(int level)
        {
            if (isCreatingNewLobby)
            {
                CreateSession();
                return;
            }
            if (isJoiningLobby)
            {
                JoinSession();
                return;
            }
        }

        public void CreateNewLobby()
        {
            isCreatingNewLobby = true;
            SceneManager.LoadScene(1);
        }

        public void JoinLobby(string name)
        {
            isJoiningLobby = true;
            lobbyToJoinName = name;
            SceneManager.LoadScene(1);
        }

        private async void JoinSession()
        {
            isJoiningLobby = false;
            string sessionName = lobbyToJoinName;
            await networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = sessionName,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        private async void CreateSession()
        {
            isCreatingNewLobby = false;
            string sessionName = "Room" + UnityEngine.Random.Range(10, 100) + " of " + SaveSystem.SaveData.Nickname;
            await networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = sessionName,
                PlayerCount = 4,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }


        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("On connected to server");
            var player = runner.Spawn(playerPrefab, playerPrefab.transform.position, playerPrefab.transform.rotation);

            runner.SetPlayerObject(runner.LocalPlayer, player);
        }



        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            sessionInfos = sessionList;
        }


        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            OnDataReceived.Invoke(key, player, data);
        }


        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            OnPlayerJoinedSession.Invoke(player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            OnPlayerLeftSession.Invoke(player);
        }


        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
             
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
             
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
             
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
             
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
             
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
             
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
             
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
             
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
             
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
             
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
             
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
             
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
             
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
             
        }
    }
}
