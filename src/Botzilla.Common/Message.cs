using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Botzilla.Common;

public static class Message
{
    public static byte[] Encode(string message, string secret)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var lengthInBytes = BitConverter.GetBytes(messageBytes.Length);
        var header = lengthInBytes.Concat([(byte)5]).ToArray();
        
        // TODO: Implement signature
        var signature = new byte[32];
        RandomNumberGenerator.Fill(signature);
        var bytePayload = header.Concat(messageBytes).Concat(signature).ToArray();
        return bytePayload;
    }
    
    
    public static (byte MessageType, byte[] Body, byte[] SignatureBytes) DecodeAsBytes(ref Socket socket)
    {
        // parsing header
        var headerBuffer = new Byte[5];
        _ = socket.Receive(headerBuffer, SocketFlags.None);

        var messageBytesCount = BitConverter.ToInt32(headerBuffer[..4]);
        var messageType = headerBuffer[4];
        
        // parsing message body
        var messageBytes = new Byte[messageBytesCount];
        _ = socket.Receive(messageBytes, SocketFlags.None);
        
        // parsing signature
        var signatureBytes = new Byte[32];
        _ = socket.Receive(signatureBytes, SocketFlags.None);

        return (messageType, messageBytes, signatureBytes);
    }
    
    public static (byte MessageType, string Message, byte[] SignatureBytes) Decode(ref Socket socket)
    {
        // parsing header
        var headerBuffer = new Byte[5];
        _ = socket.Receive(headerBuffer, SocketFlags.None);

        var messageBytesCount = BitConverter.ToInt32(headerBuffer[..4]);
        var messageType = headerBuffer[4];
        
        // parsing message body
        var messageBytes = new Byte[messageBytesCount];
        _ = socket.Receive(messageBytes, SocketFlags.None);
        var message = Encoding.UTF8.GetString(messageBytes, 0, messageBytesCount);
        
        // parsing signature
        var signatureBytes = new Byte[32];
        _ = socket.Receive(signatureBytes, SocketFlags.None);

        return (messageType, message, signatureBytes);
    }
}