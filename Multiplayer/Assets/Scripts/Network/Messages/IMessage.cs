using System.Collections.Generic;
using System;

using UnityEngine;

public enum MessageType
{
    HandShake = -1,
    Console = 0,
    Position = 1
}

public interface IMessage<T>
{
    public MessageType GetMessageType();
    
    public byte[] Serialize();
    
    public T Deserialize(byte[] message);
}

public class NetHandShake : IMessage<(long, int)>
{
    (long, int) data; //Se llama tupla long es la ip y el int es la puerto
    
    public (long, int) Deserialize(byte[] message) //Nosotros tenemos que programar un sistema que agarre los primeros 4 bytes que vendrìa a ser el tipo de mensaje
    {
        (long, int) outData;
        //(long a, int a) outData; //Se puede hacer esto
        //Y referenciarlos asi outData.Item1 o outData.a

        outData.Item1 = BitConverter.ToInt64(message, 4); //Por eso aca empieza a leer desde el byte 4
        outData.Item2 = BitConverter.ToInt32(message, 12);

        return outData;
    }

    public MessageType GetMessageType()
    {
       return MessageType.HandShake;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));

        return outData.ToArray();
    }
}

public class NetVector3 : IMessage<UnityEngine.Vector3>
{
    private static ulong lastMsgID = 0;
    
    private Vector3 data;

    public NetVector3(Vector3 data)
    {
        this.data = data;
    }

    public Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        //Falta una parte que interprete la metadata

        outData.x = BitConverter.ToSingle(message, 8); 
        outData.y = BitConverter.ToSingle(message, 12);
        outData.z = BitConverter.ToSingle(message, 16);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Position;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();
               
        //Estos primeros 2 datos son la metadata
        outData.AddRange(BitConverter.GetBytes((int)GetMessageType())); //Tipo de mensaje
        
        outData.AddRange(BitConverter.GetBytes(lastMsgID++)); 
        
        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));

        return outData.ToArray();
    }

    //Dictionary<Client,Dictionary<msgType,int>>
}