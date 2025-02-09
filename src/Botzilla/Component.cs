using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Botzilla.Common;

namespace Botzilla;

public class Component
{
    public IPEndPoint ServerIpEndpoint { get; private init; }
    public IPEndPoint IpEndpoint { get; private init; }
    public string Name { get; private init; }
    public string SecretKey { get; private init; }
    public Action<string> OnMessageReceived { get; set; } = _ => { };

    private CancellationTokenSource _cancellationTokenSource = new();
    
    
    public Component(IPEndPoint serverIpEndpoint, IPEndPoint ipEndpoint, string secretKey, string name)
    {
        ServerIpEndpoint = serverIpEndpoint;
        IpEndpoint = ipEndpoint;
        Name = name;
        SecretKey = secretKey;
    }
    
    public Component(string serverIpAddress, int serverPort, string ipAddress, int port, string secretKey, string name)
    {
        ServerIpEndpoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort);
        IpEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        Name = name;
        SecretKey = secretKey;
    }


    public void StartListening()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var cancellationToken = _cancellationTokenSource.Token;
                
                using Socket listener = new(IpEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(IpEndpoint);
                listener.Listen(100);
                
                var handler = await listener.AcceptAsync(cancellationToken);
                
                Console.WriteLine("Started Listening");
                
                while (true)
                {
                    var (messageType, message, signatureBytes) = Message.Decode(ref handler);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    
                    OnMessageReceived.Invoke(message);
                    // Console.WriteLine($"[{Name}]: {messageType} | {message} | {BitConverter.ToString(signatureBytes)}");
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    // log some message here
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"TODO: {exception.Message}");
            }
        });
    }

    public void StopListening()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }

    public async Task SendMessage(string message, string componentName)
    {
        // 1. ask server for component information
        var bytePayload = Message.Encode(message, SecretKey);
        using Socket client = new(ServerIpEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await client.ConnectAsync(ServerIpEndpoint);

        client.Send(bytePayload, SocketFlags.None);
        client.Shutdown(SocketShutdown.Both);
    }

    public void BroadcastMessage()
    {
        
    }

    public static async Task RegisterComponent(IPEndPoint serverIpEndpoint, Component component)
    {
        var payload = new Dictionary<string, object>
        {
            { "name", component.Name },
            { "ipaddress", component.IpEndpoint.Address.ToString() },
            { "port", component.IpEndpoint.Port }
        };
        
        var bytePayload = Message.Encode(JsonSerializer.Serialize(payload), component.SecretKey);
        using Socket client = new(serverIpEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await client.ConnectAsync(serverIpEndpoint);

        client.Send(bytePayload, SocketFlags.None);
        client.Shutdown(SocketShutdown.Both);
    }
}
