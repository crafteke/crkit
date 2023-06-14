using System;
using UnityEngine;
using SocketIOClient;
using System.Text.Json.Serialization;

public class ControllerManager : MonoBehaviour
{
    #region Fields and Properties

    public static SocketIO Client { get; } = new SocketIO("http://localhost:4567/");

    public ArtNet ArtNet { get; private set; }
    
    public ExampleDmx ExampleDmx { get; set; }
    
    #endregion

    #region Unity Methods

    void Start()
    {
        ArtNet = new ArtNet();
        
        // Set up DMX Universes
        // Replace "IP_ADDRESS" with the IP Address
        ExampleDmx.dmx_universe[0] = ArtNet.RegisterUniverse(1, "IP_ADDRESS", 512);
        // You can set up more DMX universes here...

        InitializeSocketIO();
    }

    void Update()
    {
        ArtNet.SendDmxData();
    }

    void OnApplicationQuit()
    {
        Client.DisconnectAsync();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// Initialize SocketIO and set up event handlers.
    /// </summary>
    private async void InitializeSocketIO()
    {
        Client.On("Command", (response) =>
        {
            string message = response.ToString();
            var json = response.GetValue();

            // Handle Command here, likely interacting with DMX.
        });

        Client.OnConnected += async (sender, e) =>
        {
            Debug.Log("Connected to SocketIO server.");
            await Client.EmitAsync("Register", "engine");
            // Possibly Emit more data here...
        };

        await Client.ConnectAsync();
    }

    #endregion
}

public class ControlActionDTO
{
    [JsonPropertyName("controller_id")]
    public string ControllerId { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class ExampleDmx
{
    public ArtNet.DMXUniverse[] dmx_universe = new ArtNet.DMXUniverse[1];

    // You can add more fields and methods related to ExampleDmx here...
}
