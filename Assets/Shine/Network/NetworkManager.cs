using UnityEngine;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using System;
using System.Collections;
using LiteNetLib;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public AmazonGameLiftClient GameLiftClient { get; private set; }
    public EventBasedNetListener RealtimeListener { get; private set; }
    public NetManager RealtimeClient { get; private set; }
    public NetPeer OurPeer{ get; private set;}
    public string ServerIP { get; private set; }
    public string TestServerIP { get; private set; }
    public int RealtimeUdpPort { get; private set; }
    public string Username { get; private set; }
    public string PlayerSessionId { get; private set; }

    public bool debug;
    string placementId;
    string testFleetId = "fleet-1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d";
    string fleetId = "fleet-1dffcfc7-caa2-44d3-9d14-e8061272042c";
    string gameSessionQueueName = "";

    // An opcode defined by client and your server script that represents a custom message type
    private const int MY_TEST_OP_CODE = 0x0;

    public int MaxConcurrentProcesses;

    public Dictionary<int, int> TcpToUdpMap;

    void Start()
    {
        this.placementId = null;
        this.PlayerSessionId = null;
        this.Username = "aoinoikaz";
        this.RealtimeUdpPort = 0;
        this.ServerIP = null;
        this.TestServerIP = "127.0.0.1";
        this.OurPeer = null;
        this.TcpToUdpMap = new Dictionary<int, int>(MaxConcurrentProcesses);

        for (int i = 0; i < MaxConcurrentProcesses; i++)
        {
            TcpToUdpMap.Add(9080 + i, 3456 + i);
        }

        if (debug)
        {
            AmazonGameLiftConfig gameliftConfig = new AmazonGameLiftConfig();
            gameliftConfig.ServiceURL = "http://127.0.0.1:9080";
            GameLiftClient = new AmazonGameLiftClient(gameliftConfig);
        }
        else
        {
            GameLiftClient = new AmazonGameLiftClient();
        }

        this.RealtimeListener = new EventBasedNetListener();
        this.RealtimeListener.NetworkReceiveEvent += RealtimeListener_NetworkReceiveEvent;
        this.RealtimeListener.PeerConnectedEvent += RealtimeListener_PeerConnectedEvent;
        this.RealtimeListener.PeerDisconnectedEvent += RealtimeListener_PeerDisconnectedEvent;
        this.RealtimeClient = new NetManager(this.RealtimeListener);

        if (!this.RealtimeClient.Start())
        {
            Debug.Log("Error trying to start client listener on port: " + this.RealtimeUdpPort);
        }
    }


    private void Update()
    {
        if (this.RealtimeClient != null && this.OurPeer != null)
        {
            this.RealtimeClient.PollEvents();
        }
    }


    public async void FindMatch()
    {
        CreateGameSessionRequest cgsr = new CreateGameSessionRequest();
        cgsr.FleetId = debug ? testFleetId : fleetId;
        cgsr.Name = Guid.NewGuid().ToString();
        cgsr.MaximumPlayerSessionCount = 2;

        Debug.Log("Creating game session async... awaiting response...");
        CreateGameSessionResponse cgsR =  await GameLiftClient.CreateGameSessionAsync(cgsr);
        //CreateGameSessionResponse cgsR = GameLiftClient.CreateGameSession(cgsr);

        if (cgsR.GameSession != null)
        {
            Debug.Log("Created game session: " + cgsR.GameSession.CreationTime + " | " + cgsR.GameSession.GameSessionId);

            // we need to assign this client the returned ip/port to connect to
            this.ServerIP = cgsR.GameSession.IpAddress;
                
            // Our server runtime configurations will always have a tcp and udp port matching.
            _ = this.TcpToUdpMap.TryGetValue(cgsR.GameSession.Port, out int udp);
            this.RealtimeUdpPort = udp;
            
            Debug.Log(this.ServerIP + " | " + cgsR.GameSession.Port + " | " + this.RealtimeUdpPort);

            CreatePlayerSessionRequest cpsr = new CreatePlayerSessionRequest();
            cpsr.GameSessionId = cgsR.GameSession.GameSessionId;
            cpsr.PlayerId = Guid.NewGuid().ToString();
            cpsr.PlayerData = "PlayerCombatLevel:5|Champion:Nimmi";

            Debug.Log("Creating player session async... awaiting response...");
            CreatePlayerSessionResponse cpsR = await GameLiftClient.CreatePlayerSessionAsync(cpsr);
            //CreatePlayerSessionResponse cpsR = GameLiftClient.CreatePlayerSession(cpsr);

            if (cpsR.PlayerSession != null)
            {
                Debug.Log("Created player session: " + cpsR.PlayerSession.CreationTime + " | " + cpsR.PlayerSession.PlayerSessionId);
                this.PlayerSessionId = cpsR.PlayerSession.PlayerSessionId;
                StartCoroutine(Connect());
            }
        }
    }


    public IEnumerator Connect()
    {
        Debug.Log("Attempting to connect to server");

        yield return new WaitForSeconds(1.5f);

        try
        {
            OurPeer = this.RealtimeClient.Connect(debug ? this.TestServerIP : this.ServerIP, this.RealtimeUdpPort, this.PlayerSessionId);
            Debug.Log(OurPeer.ToString());
            Debug.Log(OurPeer.ConnectionState.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Caught exception when trying to connect: " + e.ToString());
        }
    }


    public void Disconnect()
    {
        if (RealtimeClient != null && this.IsConnected())
        {
            RealtimeClient.Stop();
        }
    }


    public bool IsConnected()
    {
        return this.OurPeer != null && this.OurPeer.ConnectionState == ConnectionState.Connected;
    }


    private void RealtimeListener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("PeerDisconnectedEvent: " + disconnectInfo.Reason.ToString());
    }


    private void RealtimeListener_PeerConnectedEvent(NetPeer peer)
    {
        Debug.Log("Successfully connected to remote server: " + peer.EndPoint.ToString());
        Giggity.Instance.FindMatch.gameObject.SetActive(false);
        Giggity.Instance.ConnectionState.text = "Connected to game server";
    }


    private void RealtimeListener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        Debug.LogFormat("RealtimeListener_NetworkReceiveEvent: {0}", reader.GetString(100 /* max length of string */));
        reader.Recycle();
    }


    private void OnApplicationQuit()
    {
        this.Disconnect();
    }
}
