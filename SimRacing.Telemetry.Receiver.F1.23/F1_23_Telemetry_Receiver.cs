using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimRacing.Telemetry.Receiver.F1_23.Packets;

namespace Sim_Racing_UDP_Receiver.Games.F1_2022
{
    /// <summary>
    /// HOW TO USE: Create an object. Subscribe to events to receive packets. Call StartReceiving()
    /// </summary>
    public class F1_23_Telemetry_Receiver
    {
        #region Properties
        private CancellationTokenSource cts = new CancellationTokenSource();    //used to cancel loop that listens for UDP packets
        private const int DEFAULT_PORT = 20777;

        private string ipAddress { get; set; }
        private int port { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for the F1_2022_UDP_Receiver class.
        /// Initializes the IPEndpoint to listen for datagrams sent from any source.
        /// May encounter network/firewall issues when receiving on a separate device than the game is running on.
        /// </summary>

        public F1_23_Telemetry_Receiver()
        {
            ipAddress = null;
            port = DEFAULT_PORT;

            cts = new CancellationTokenSource();
        }


        /// <summary>
        /// Parameterized constructor for the F1_2022_UDP_Receiver class.
        /// Initializes the IPEndpoint and port with the provided values.
        /// Recommended when running from a separate device than the game is running on.
        /// </summary>
        /// <param name="_ipAddress">The IP address to bind the UDP receiver to.</param>
        /// <param name="_portNbr">The port number to listen on.</param>

        public F1_23_Telemetry_Receiver(string _ipAddress, int _portNbr)
        {
            ipAddress = _ipAddress;
            port = _portNbr;

            cts = new CancellationTokenSource();
        }
        #endregion

        #region Events

        /// <summary>
        /// Assigns a function to handle the PacketReceived event.
        /// The function should expect an EventHandler<PacketEventArgs> delegate.
        /// The event provides access to packet data through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketEventArgs> PacketReceived;

        /// <summary>
        /// Assigns a function to handle the CarDamageDataPacketReceived event.
        /// The function should expect an EventHandler<PacketCarDamageDataEventArgs> delegate.
        /// The event provides access to car damage data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about car damage data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketCarDamageDataEventArgs> CarDamageDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the CarSetupDataPacketReceived event.
        /// The function should expect an EventHandler<PacketCarSetupDataEventArgs> delegate.
        /// The event provides access to car setup data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about car setup data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketCarSetupDataEventArgs> CarSetupDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the CarStatusDataPacketReceived event.
        /// The function should expect an EventHandler<PacketCarStatusDataEventArgs> delegate.
        /// The event provides access to car status data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about car status data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketCarStatusDataEventArgs> CarStatusDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the CarTelemetryDataPacketReceived event.
        /// The function should expect an EventHandler<PacketCarTelemetryDataEventArgs> delegate.
        /// The event provides access to car telemetry data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about car telemetry data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketCarTelemetryDataEventArgs> CarTelemetryDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the EventDataPacketReceived event.
        /// The function should expect an EventHandler<PacketEventDataEventArgs> delegate.
        /// The event provides access to event data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about event data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketEventDataEventArgs> EventDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the FinalClassificationDataPacketReceived event.
        /// The function should expect an EventHandler<PacketFinalClassificationDataEventArgs> delegate.
        /// The event provides access to final classification data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about final classification data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketFinalClassificationDataEventArgs> FinalClassificationDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the LapDataPacketReceived event.
        /// The function should expect an EventHandler<PacketLapDataEventArgs> delegate.
        /// The event provides access to lap data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about lap data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketLapDataEventArgs> LapDataPacketReceived;


        /// <summary>
        /// Assigns a function to handle the LobbyInfoDataPacketReceived event.
        /// The function should expect an EventHandler<PacketLobbyInfoDataEventArgs> delegate.
        /// The event provides access to lobby info data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about lobby info data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketLobbyInfoDataEventArgs> LobbyInfoDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the MotionDataPacketReceived event.
        /// The function should expect an EventHandler<PacketMotionDataEventArgs> delegate.
        /// The event provides access to motion data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about motion data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketMotionDataEventArgs> MotionDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the ParticipantsDataPacketReceived event.
        /// The function should expect an EventHandler<PacketParticipantsDataEventArgs> delegate.
        /// The event provides access to participants data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about participants data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketParticipantsDataEventArgs> ParticipantsDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the SessionDataPacketReceived event.
        /// The function should expect an EventHandler<PacketSessionDataEventArgs> delegate.
        /// The event provides access to session data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about session data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketSessionDataEventArgs> SessionDataPacketReceived;

        /// <summary>
        /// Assigns a function to handle the SessionHistoryDataPacketReceived event.
        /// The function should expect an EventHandler<PacketSessionHistoryDataEventArgs> delegate.
        /// The event provides access to session history data packet through EventArgs.Packet property.
        /// Refer to the documentation for detailed information about session history data packet properties.
        /// </summary>
        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketSessionHistoryDataEventArgs> SessionHistoryDataPacketReceived;

        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketTyreSetDataEventArgs> TyreSetDataPacketReceived;

        public event EventHandler<SimRacing.Telemetry.Receiver.F1_23.Packets.PacketMotionExtraDataEventArgs> MotionExtraDataPacketReceived;

        #endregion

        #region Methods
        //Public Methods

        /// <summary>
        /// Starts listening for incoming UDP packets in a background thread.
        /// For each received packet, it creates corresponding packet data object based on packet type,
        /// and invokes the associated event handlers. Any error during packet handling is logged to console.
        /// </summary>        
        public void StartReceiving()
        {
            try
            {
                //Start a loop in a background thread that continously listens for incoming UDP packets.
                Task task = Task.Run(() =>
                {
                    //Creates a UdpClient for reading incoming data.
                    UdpClient receivingUdpClient = new UdpClient(port);

                    //Creates an IPEndPoint to record the IP Address and port number of the sender.
                    IPEndPoint RemoteIpEndPoint = null;
                    if (ipAddress == null)
                    {
                        //Address is null, use IPAddress.Any
                        // The IPEndPoint will allow you to read datagrams sent from any source.
                        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    }
                    else
                    {
                        //IPAddress is defined, used the specified address
                        RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 0);
                    }

                    // The IPEndPoint will allow you to read datagrams sent from any source.
                    //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    try
                    {
                        while (!cts.IsCancellationRequested)
                        {
                            byte[] udpPacketBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                            //Get the packet header data
                            Packet header = new Packet(udpPacketBytes);

                            switch ((PacketType)header.packetId)
                            {
                                case PacketType.Motion: //Motion
                                    {
                                        try
                                        {
                                            PacketMotionData packetMotionData = new PacketMotionData(udpPacketBytes);
                                            MotionDataPacketReceived?.Invoke(this, new PacketMotionDataEventArgs(packetMotionData));
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("Motion Packet Error: " + ex.Message);
                                        }
                                        break;
                                    }
                                case PacketType.Session: //Session
                                    {
                                        try
                                        {
                                            PacketSessionData packetSessionData = new PacketSessionData(udpPacketBytes);
                                            SessionDataPacketReceived?.Invoke(this, new PacketSessionDataEventArgs(packetSessionData));
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("Session Packet Error: " + ex.Message);
                                        }
                                        break;
                                    }
                                case PacketType.LapData: //Lap Data
                                    {
                                        try
                                        {
                                            PacketLapData packetLapData = new PacketLapData(udpPacketBytes);
                                            LapDataPacketReceived?.Invoke(this, new PacketLapDataEventArgs(packetLapData));
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("Lap Packet Error: " + ex.Message);
                                        }
                                        break;
                                    }
                                case PacketType.Event: //Event
                                    {
                                        try
                                        {
                                            PacketEventData packetEventData = new PacketEventData(udpPacketBytes);
                                            EventDataPacketReceived?.Invoke(this, new PacketEventDataEventArgs(packetEventData));

                                        }
                                        catch (Exception ex) { Debug.WriteLine("Event Packet Error: " + ex.Message); }
                                        break;
                                    }
                                case PacketType.Participants: //Participants
                                    {
                                        try
                                        {
                                            PacketParticipantsData packetParticipantsData = new PacketParticipantsData(udpPacketBytes);
                                            ParticipantsDataPacketReceived?.Invoke(this, new PacketParticipantsDataEventArgs(packetParticipantsData));
                                        }
                                        catch (Exception ex) { Debug.WriteLine("Participant Packet Error: " + ex.Message); }
                                        break;

                                    }
                                case PacketType.CarSetups: //Car Setup
                                    {
                                        try
                                        {
                                            PacketCarSetupData packetCarSetupData = new PacketCarSetupData(udpPacketBytes);
                                            CarSetupDataPacketReceived?.Invoke(this, new PacketCarSetupDataEventArgs(packetCarSetupData));
                                        }
                                        catch (Exception ex) { Debug.WriteLine("Setup Packet Error: " + ex.Message); }
                                        break;
                                    }
                                case PacketType.CarTelemetery: //Car Telemetry
                                    {
                                        try
                                        {
                                            PacketCarTelemetryData packetCarTelemetryData = new PacketCarTelemetryData(udpPacketBytes);
                                            CarTelemetryDataPacketReceived?.Invoke(this, new PacketCarTelemetryDataEventArgs(packetCarTelemetryData));
                                        }
                                        catch (Exception ex) { Debug.WriteLine("Car Telemetry Packet Error: " + ex.Message); }
                                        break;
                                    }
                                case PacketType.CarStatus: //Car Status
                                    {
                                        try
                                        {
                                            PacketCarStatusData packetCarStatusData = new PacketCarStatusData(udpPacketBytes);
                                            CarStatusDataPacketReceived?.Invoke(this, new PacketCarStatusDataEventArgs(packetCarStatusData));
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("Car Status Packet Error: " + ex.Message);
                                        }
                                        break;

                                    }
                                case PacketType.FinalClassification: //Final Classification
                                    {
                                        try
                                        {
                                            PacketFinalClassificationData packetFinalClassificationData = new PacketFinalClassificationData(udpPacketBytes);
                                            FinalClassificationDataPacketReceived?.Invoke(this, new PacketFinalClassificationDataEventArgs(packetFinalClassificationData));

                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("Final Classification Packet Error: " + ex.Message);
                                        }
                                        break;
                                    }
                                case PacketType.LobbyInfo: //Lobby Info
                                    {
                                        try
                                        {
                                            PacketLobbyInfoData packetLobbyInfoData = new PacketLobbyInfoData(udpPacketBytes);
                                            LobbyInfoDataPacketReceived?.Invoke(this, new PacketLobbyInfoDataEventArgs(packetLobbyInfoData));
                                        }
                                        catch (Exception ex) { Debug.WriteLine("Lobby Info Packet Error: " + ex.Message); }
                                        break;
                                    }
                                case PacketType.CarDamage: //Car Damage
                                    {
                                        try
                                        {
                                            PacketCarDamageData packetCarDamageData = new PacketCarDamageData(udpPacketBytes);
                                            CarDamageDataPacketReceived?.Invoke(this, new PacketCarDamageDataEventArgs(packetCarDamageData));
                                        }
                                        catch (Exception ex) { Debug.WriteLine("Car Damage Packet Error: " + ex.Message); }
                                        break;

                                    }
                                case PacketType.SessionHistory: //Session History
                                    {
                                        try
                                        {
                                            PacketSessionHistoryData packetSessionData = new PacketSessionHistoryData(udpPacketBytes);
                                            SessionHistoryDataPacketReceived?.Invoke(this, new PacketSessionHistoryDataEventArgs(packetSessionData));
                                        }
                                        catch (Exception ex) { Debug.WriteLine("Session History Packet Error: " + ex.Message); }
                                        break;
                                    }
                                case PacketType.TyreSets:
                                    try
                                    {
                                        PacketTyreSetData packetTyreSetData = new PacketTyreSetData(udpPacketBytes);
                                        TyreSetDataPacketReceived?.Invoke(this, new PacketTyreSetDataEventArgs(packetTyreSetData));
                                    }
                                    catch (Exception ex) { 
                                        Debug.WriteLine("Tyre Set Packet Error: " + ex.Message);
                                    }
                                    break;
                                case PacketType.MotionExtra:
                                    try
                                    {
                                        PacketMotionExtraData packetMotionExtraData = new PacketMotionExtraData(udpPacketBytes);
                                        MotionExtraDataPacketReceived?.Invoke(this, new PacketMotionExtraDataEventArgs(packetMotionExtraData));
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("Motion Extra Packet Error: " + ex.Message);
                                    }
                                    break;
                                default: //Something went wrong...    
                                    {
                                        Debug.WriteLine(" Packet ID does not match a specified packet type: " + header.packetId.ToString());
                                        break;
                                    }
                            }

                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                });

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
        
        /// <summary>
        /// Attempts to stop the background task that listens for incoming UDP packets.
        /// If there is an error during the operation, the error message is logged to console.
        /// </summary>
        public void StopReceiving()
        {
            try
            {
                cts.Cancel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop receiving: " + ex.Message);
            }
        }
        #endregion
    }
}
