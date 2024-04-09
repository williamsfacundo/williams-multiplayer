using System.Collections.Generic;
using System.Net;
using System;

using UnityEngine;

public struct Client
{
    public IPEndPoint ipEndPoint;

    public float timeStamp;

    public int id;
    
    public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
    {
        this.timeStamp = timeStamp;
        
        this.id = id;
        
        this.ipEndPoint = ipEndPoint;
    }
}

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{
    public Action<byte[], IPEndPoint> OnReceiveEvent;
    
    public int TimeOut = 30;
    
    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();
    
    private UdpConnection connection;

    private int clientId = 0; // This id should be generated during first handshake

    public IPAddress ipAddress
    {
        get; private set;
    }

    public int port
    {
        get; private set;
    }

    public bool isServer
    {
        get; private set;
    }    

    public void StartServer(int port)
    {
        this.port = port;
        
        isServer = true;
        
        connection = new UdpConnection(port, this);
    }

    public void StartClient(IPAddress ip, int port)
    {
        this.port = port;
        this.ipAddress = ip;

        isServer = false;
        
        connection = new UdpConnection(ip, port, this);

        AddClient(new IPEndPoint(ip, port)); //Soy yo quien me agrego como cliente? O mando un handshake para preguntar al server para que me agregue
    }

    void AddClient(IPEndPoint ip)
    {
        if (!ipToId.ContainsKey(ip))
        {
            Debug.Log("Adding client: " + ip.Address);

            int id = clientId;

            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, id, Time.realtimeSinceStartup));

            clientId++;
        }
    }

    void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        AddClient(ip);

        if (OnReceiveEvent != null)
            OnReceiveEvent.Invoke(data, ip);
    }

    public void SendToServer(byte[] data)
    {
        connection.Send(data);
    }

    public void Broadcast(byte[] data)
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                connection.Send(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
    }
}
