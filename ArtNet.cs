using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class ArtNet
{
    // SerializeField attribute was there but the variable was commented out
    // Uncomment if needed and assign it a default value
    //[SerializeField]
    //private string broadcastIP = "10.0.0.255"; // Broadcasting address

    private const float DMXFps = 30;
    private readonly UdpClient socket;
    private float lastTxTime = 0;
    private int indexUnivers = 0;

    // Array to store the DMXUniverses
    private readonly DMXUniverse[] registeredUniverses = new DMXUniverse[50];

    // Constructor
    public ArtNet()
    {
        socket = new UdpClient();
        // Uncomment below line to enable broadcasting if needed
        // socket.EnableBroadcast = true;
    }

    /// <summary>
    /// Sends DMX data at a specific interval.
    /// </summary>
    public void SendDMXData()
    {
        float intervalTime = 1f / DMXFps;

        if (Time.time - lastTxTime >= intervalTime)
        {
            lastTxTime = Time.time;
            foreach (DMXUniverse universe in registeredUniverses)
            {
                if (universe != null)
                {
                    universe.Tx(socket);
                }
            }
        }
    }

    /// <summary>
    /// Registers a DMX universe.
    /// </summary>
    /// <param name="universeId">ID of the DMX universe.</param>
    /// <param name="universeIP">IP of the DMX universe.</param>
    /// <param name="universeSize">Size of the DMX universe.</param>
    /// <returns>The registered DMX universe.</returns>
    public DMXUniverse RegisterUniverse(byte universeId, string universeIP, int universeSize)
    {
        if (registeredUniverses[indexUnivers] == null)
        {
            registeredUniverses[indexUnivers] = new DMXUniverse(universeId, universeIP, universeSize);
        }

        DMXUniverse tmpDMXUniverse = registeredUniverses[indexUnivers];
        indexUnivers++;
        return tmpDMXUniverse;
    }

    // Inner class to represent a DMX Universe
    public class DMXUniverse
    {
        private byte universe = 0x1;
        private int universeSize = 512;
        private static byte sequence = 1;
        private readonly byte[] data = new byte[512];
        private byte[] artNetPacket;
        private readonly IPEndPoint target;

        public DMXUniverse(byte universeId, string universeIP, int universeSize)
        {
            universe = universeId;
            target = new IPEndPoint(IPAddress.Parse(universeIP), 6454); // artnet default port is 6454
            this.universeSize = universeSize;
            ForgeArtNetPacket();
        }

        // Set the value for a single channel
        public void SetChannel(int channel, byte value)
        {
            data[channel] = value;
        }

        // Overload to accept int value
        public void SetChannel(int channel, int value)
        {
            data[channel] = (byte)value;
        }

        // Send the data
        public void Tx(UdpClient socket)
        {
            sequence++;

            if (sequence == 0x00)
            {
                sequence++;
            }

            artNetPacket[12] = sequence;

            Buffer.BlockCopy(data, 0, artNetPacket, 18, universeSize);

            try
            {
                socket.Send(artNetPacket, artNetPacket.Length, target);
            }
            catch (Exception e)
            {
                Debug.Log(this + " ARTNET ERROR: " + e);
            }
        }

        // Set DMX data
        public void SetDMXData(byte[] dmxData)
        {
            Buffer.BlockCopy(dmxData, 0, data, 0, dmxData.Length);
        }

        // Forge the ArtNet packet
        private void ForgeArtNetPacket()
        {
            // Rest of the method
            // ...
        }
    }
}
