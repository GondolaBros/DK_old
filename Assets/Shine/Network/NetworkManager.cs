using UnityEngine;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime.Types;
using System;
using System.Text;
using Amazon.GameLift.Model.Internal.MarshallTransformations;
using UnityEditor;
using Amazon.Runtime;
using Aws.GameLift.Realtime.Network;
using Amazon;

public class NetworkManager : MonoBehaviour
{
    public Client RealtimeClient { get; private set; }
    public AmazonGameLiftClient GameLiftClient { get; private set; }
    public CreateGameSessionRequest GameSessionRequest { get; private set; }
    public CreatePlayerSessionRequest PlayerSessionRequest { get; private set; }
    public bool OnCloseReceived { get; private set; }
    public string Username { get; private set; }
    
    public bool debug;

    // An opcode defined by client and your server script that represents a custom message type
    private const int MY_TEST_OP_CODE = 0x0;
 
    void Start()
    {
        this.OnCloseReceived = false;
        this.Username = "aoinoikaz";

        ClientLogger.LogHandler = (x) => Debug.Log(x);

        AmazonGameLiftConfig config = new AmazonGameLiftConfig();
        GameLiftClient = new AmazonGameLiftClient();

        if (debug)
        {
            GameSessionRequest = new CreateGameSessionRequest();
            GameSessionRequest.FleetId = "fleet-293cc1d7-3487-4a90-b285-1bbc3b966da0";
            GameSessionRequest.Name = Guid.NewGuid().ToString();
            GameSessionRequest.MaximumPlayerSessionCount = 2;

            var createGameSessionResponse = GameLiftClient.CreateGameSession(GameSessionRequest);
            if (createGameSessionResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Debug.Log("Created game session: " + createGameSessionResponse.ToString());

                /*
                ConnectionStatus playerConnectionStatus = Connect(
                   // DNS name used instead of ip because we're using TLS encryption
                   createGameSessionResponse.GameSession.DnsName,
                   // TCP port for gamelift interactions
                   createGameSessionResponse.GameSession.Port,
                   // UDP port for real time data
                   3456,
                   // Server-generated-identifier used to validate the player
                   createGameSessionResponse.PlayerSession.PlayerSessionId,
                   // Extra data for the connection payload
                   StringToBytes("TestConnectionPayload"));*/
            }

        }
        else
        {
            string playerId = this.Username + Guid.NewGuid().ToString();
            DesiredPlayerSession session = new DesiredPlayerSession();
            session.PlayerId = playerId;
            session.PlayerData = "PlayerCombatLevel:5|Champion:Nimmi";

            PlayerLatency playerLatency = new PlayerLatency();
            playerLatency.PlayerId = playerId;
            playerLatency.LatencyInMilliseconds = 15f;

            var startGameSessionPlacementRequest = new StartGameSessionPlacementRequest();
            startGameSessionPlacementRequest.GameSessionName = Guid.NewGuid().ToString();
            startGameSessionPlacementRequest.PlacementId = Guid.NewGuid().ToString();
            startGameSessionPlacementRequest.MaximumPlayerSessionCount = 2;
            startGameSessionPlacementRequest.GameSessionQueueName = "arn:aws:gamelift:ca-central-1:561339566466:gamesessionqueue/DKQueue";
            startGameSessionPlacementRequest.DesiredPlayerSessions.Add(session);
            
            //TODO: adding the player latencies causes the server to fail to activate game session ! 
            //startGameSessionPlacementRequest.PlayerLatencies.Add(playerLatency);

            var startGameSessionPlacementResponse = GameLiftClient.StartGameSessionPlacement(startGameSessionPlacementRequest);
            if (startGameSessionPlacementResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                Debug.Log("GameSessionPlacement failed");
            }
            else
            {
                Debug.Log("SGS State: " + startGameSessionPlacementResponse.GameSessionPlacement.Status.ToString());
                Debug.Log("StartGameSessionPlacementResponse: " + startGameSessionPlacementResponse.HttpStatusCode);
                Debug.Log("SGS: " + startGameSessionPlacementResponse.GameSessionPlacement.GameSessionQueueName);
                Debug.Log("SGS: " + startGameSessionPlacementResponse.GameSessionPlacement.GameSessionName);
            }
        }
    }


    // Connect to realtime gameserver
    ConnectionStatus Connect(string endpoint, int tcpPort, int localUdpPort, string playerSessionId, byte[] connectionPayload)
    {
        ConnectionToken token = new ConnectionToken(playerSessionId, connectionPayload);
        return RealtimeClient.Connect(endpoint, tcpPort, localUdpPort, token);
    }


    public void Disconnect()
    {
        if (RealtimeClient != null && RealtimeClient.Connected)
        {
            RealtimeClient.Disconnect();
        }
    }


    public bool IsConnected()
    {
        return RealtimeClient.Connected;
    }


    private void OnApplicationQuit()
    {
        this.Disconnect();
        
    }


    /// <summary>
    /// Example of sending to a custom message to the server.
    /// 
    /// Server could be replaced by known peer Id etc.
    /// </summary>
    /// <param name="intent">Choice of delivery intent ie Reliable, Fast etc. </param>
    /// <param name="payload">Custom payload to send with message</param>
    public void SendMessage(DeliveryIntent intent, string payload)
    {
        RealtimeClient.SendMessage(RealtimeClient.NewMessage(MY_TEST_OP_CODE)
            .WithDeliveryIntent(intent)
            .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
            .WithPayload(StringToBytes(payload)));
    }

    /**
     * Handle connection open events
     */
    public void OnOpenEvent(object sender, EventArgs e)
    {
    }

    /**
     * Handle connection close events
     */
    public void OnCloseEvent(object sender, EventArgs e)
    {
        OnCloseReceived = true;
    }

    /**
     * Handle Group membership update events 
     */
    public void OnGroupMembershipUpdate(object sender, GroupMembershipEventArgs e)
    {
    }

    /**
     *  Handle data received from the Realtime server 
     */
    public virtual void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        switch (e.OpCode)
        {
            // handle message based on OpCode
            default:
                break;

        }
    }

    /**
     * Helper method to simplify task of sending/receiving payloads.
     */
    public static byte[] StringToBytes(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    /**
     * Helper method to simplify task of sending/receiving payloads.
     */
    public static string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
    // Start is called before the first frame update

}
