using System.Net;
using System.Net.Sockets;
using System.Text;
using Botzilla.Common;

namespace Botzilla.Server;

public class BotzillaServer : IDisposable
{
    public IPEndPoint IpEndPoint { get; init; }
    public string SecretKey { get; init; }

    private Registry _registry = new();

    
    public BotzillaServer(IPEndPoint ipEndPoint, string secretKey)
    {
        IpEndPoint = ipEndPoint;
        SecretKey = secretKey;
    }
    
    public BotzillaServer(string ipAddress, int port, string secretKey)
    {
        IpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        SecretKey = secretKey;
    }
    

    public void Start(CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;

        _ = Task.Run(async () =>
        {
            try
            {
                using Socket listener = new(IpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(IpEndPoint);
                listener.Listen(100);
                
                var handler = await listener.AcceptAsync();
                
                Console.WriteLine("Started Listening");
                
                while (true)
                {
                    var (messageType, bodyBytes, signatureBytes) = Message.DecodeAsBytes(ref handler);
                    
                    if (cancellationToken.Value.IsCancellationRequested)
                    {
                        break;
                    }

                    var opcode = bodyBytes[0];
                    var rest = bodyBytes[1..];

                    _ = Task.Run(async () =>
                    {
                        _ = opcode switch 
                    });
                }

                if (cancellationToken.Value.IsCancellationRequested)
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

    private void RegisterComponents()
    {
        
    }
    
    private void GetComponents()
    {
        
    }

    private void GetComponent()
    {
        
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}