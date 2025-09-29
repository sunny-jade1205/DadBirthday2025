using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner networkRunner = null;

    [SerializeField]
    private NetworkPrefabRef playerNetworkPrefab;

    [SerializeField]
    //private List<NetworkPrefabRef> playerList = new List<NetworkPrefabRef>();
    private Dictionary<PlayerRef, NetworkObject> playerDictionary = new Dictionary<PlayerRef, NetworkObject>();

    [SerializeField]
    private GameObject ballsPool;
    [SerializeField]
    private GameObject ballsPrefab;

    private void Start()
    {
        StartGame(GameMode.AutoHostOrClient);
    }
    async void StartGame(GameMode mode)
    {
        networkRunner.ProvideInput = true;

        // 把 buildIndex 轉成 SceneRef
        var sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            Scene = sceneRef,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            SpawnPlayer(runner, player);
        }
        else if (player == runner.LocalPlayer)
        {
            // Client 端請求 Host 幫忙生成
            RpcRequestSpawnPlayer(runner, player);
        }
        if (playerDictionary.Count == 2)
        {
            StartCoroutine("BurnBalls", runner);

        }
    }
    IEnumerator BurnBalls(NetworkRunner runner)
    {
        for (int i = 0; i <= 1000; i++)
        {
            Vector3 spawnPosition = new Vector3(0f, 0.5f, 0f);
            NetworkObject networkBallObject = runner.Spawn(ballsPrefab, spawnPosition);
            if (runner.IsServer && networkBallObject != null)
            {
                var spawnedRigidbody = networkBallObject.GetComponent<Rigidbody>();
                if (spawnedRigidbody != null)
                {
                    spawnedRigidbody.position = spawnPosition;

                    Vector3 randomVelocity = new Vector3(0,-0.5f,0);
                    spawnedRigidbody.velocity = randomVelocity;
                }
            }
            yield return new WaitForSeconds(0.8f);
        }

    }
    private void RpcRequestSpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            SpawnPlayer(runner, player);
        }
    }
    private void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        NetworkObject networkPlayerObject = runner.Spawn(playerNetworkPrefab, Vector3.zero, Quaternion.identity, player);
        playerDictionary.Add(player, networkPlayerObject);

        Quaternion playerQuaternion = Quaternion.Euler(0, (360 / playerDictionary.Count) * GetKeyIndex<PlayerRef, NetworkObject>(playerDictionary, player) - 360, 0);
        networkPlayerObject.transform.rotation = playerQuaternion;
        var playerNetwork = networkPlayerObject.GetComponent<PlayerNetwork>();

        //playerNetwork.playerIndex = GetKeyIndex<PlayerRef, NetworkObject>(playerDictionary, player);
        //if (playerNetwork != null)
        //{
        //    playerNetwork.RpcRequestChangeMaterial(playerNetwork.playerIndex);
        //}

    }
    int GetKeyIndex<TKey, TValue>(Dictionary<TKey, TValue> playerDict, TKey key)
    {
        int i = 0;
        foreach (var playerIndex in playerDict.Keys)
        {
            if (EqualityComparer<TKey>.Default.Equals(playerIndex, key))
                return i;
            i++;
        }
        return -1; // 找不到回 -1
    }
    //[Networked(OnChanged = nameof(OnPlayerIndexChanged))]
    //public int playerIndex { get; set; }

    //private static void OnPlayerIndexChanged(Changed<PlayerNetwork> changed)
    //{
    //    changed.Behaviour.UpdateMaterial();
    //}
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            if (playerDictionary.TryGetValue(player, out NetworkObject networkPlayerObject))
            {
                runner.Despawn(networkPlayerObject);
                playerDictionary.Remove(player);
            }
        }

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
