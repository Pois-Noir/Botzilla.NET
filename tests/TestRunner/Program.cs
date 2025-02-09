using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Botzilla;
using Botzilla.Common;
using Botzilla.Server;
 
/*
const int port = 8080;
var hostName = Dns.GetHostName();
IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
IPAddress localIpAddress = localhost.AddressList[0]; 
IPEndPoint ipEndPoint = new(localIpAddress, port);

var cancellationTokenSource = new CancellationTokenSource();
var botzillaServer = new BotzillaServer(ipEndPoint, "joyal");
botzillaServer.Start(cancellationTokenSource.Token);


var messageBytes = Encoding.UTF8.GetBytes(message);
var lengthInBytes = BitConverter.GetBytes(messageBytes.Length);
var header = lengthInBytes.Concat([(byte)5]).ToArray();
byte[] signature = new byte[32];
RandomNumberGenerator.Fill(signature);
var bytePayload = header.Concat(messageBytes).Concat(signature).ToArray();


await Task.Delay(TimeSpan.FromSeconds(1));

var message = "i love my brother joyal";
var bytePayload = Message.Encode(message, "joyal");

using Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
client.Connect(ipEndPoint);

client.Send(bytePayload, SocketFlags.None);
Console.WriteLine($"Socket client sent message: \"{message}\"");

await Task.Delay(TimeSpan.FromSeconds(1));

cancellationTokenSource.Cancel();
client.Shutdown(SocketShutdown.Both);
*/


const int serverPort = 8080;
var hostName = Dns.GetHostName();
IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
IPAddress localIpAddress = localhost.AddressList[0]; 
IPEndPoint serverIpEndpoint = new(localIpAddress, serverPort);

var cancellationTokenSource = new CancellationTokenSource();
var botzillaServer = new BotzillaServer(serverIpEndpoint, "joyal");
botzillaServer.Start(cancellationTokenSource.Token);


IPEndPoint component1IpEndpoint = new(localIpAddress, 6006);
var component1 = new Component(serverIpEndpoint, component1IpEndpoint, "joyal", "component1");
await Component.RegisterComponent(serverIpEndpoint, component1);
component1.StartListening();

IPEndPoint component2IpEndpoint = new(localIpAddress, 6007);
var component2 = new Component(serverIpEndpoint, component2IpEndpoint, "joyal", "component2");
await Component.RegisterComponent(serverIpEndpoint, component2);
component2.StartListening();

await component1.SendMessage("wassup man", "component2");


await Task.Delay(TimeSpan.FromSeconds(1));
cancellationTokenSource.Cancel();
