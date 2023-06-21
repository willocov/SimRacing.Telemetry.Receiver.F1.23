using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using Sim_Racing_UDP_Receiver.Games.F1_2022;
using System.Security.Cryptography.X509Certificates;

namespace Sim_Racing_UDP_Receiver
{
    /// <summary>
    /// The main entry point of the console application. Serves as a sample usage for the F1_23_UDP_Receiver class library. 
    /// It instantiates an F1_2023_UDP_Receiver object and a TestEventHandlers object, 
    /// starts the process of receiving packets, waits for a user's key press before stopping the process. 
    /// If any exceptions occur, they are written to the Debug console.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                F1_23_Telemetry_Receiver uDP_Receiver = new F1_23_Telemetry_Receiver();
                TestEventHandlers test = new TestEventHandlers();
                uDP_Receiver.CarTelemetryDataPacketReceived += test.CarTelemetryPacketReceived;
                uDP_Receiver.StartReceiving();
                Console.ReadKey();
                uDP_Receiver.StopReceiving();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }
        public class TestEventHandlers
        {
            public void CarTelemetryPacketReceived(object sender, SimRacing.Telemetry.Receiver.F1_23.Packets.PacketCarTelemetryDataEventArgs e)
            {
                SimRacing.Telemetry.Receiver.F1_23.Packets.PacketCarTelemetryData packetCarTelemetryData = e.Packet;
                Debug.WriteLine("Car Telemetry Data:");
                Debug.WriteLine($"Speed: " + packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].speed + " km/h");
                Debug.WriteLine($"Throttle: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].throttle}");
                Debug.WriteLine($"Steer: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].steer}");
                Debug.WriteLine($"Brake: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].brake}");
                Debug.WriteLine($"Clutch: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].clutch}");
                Debug.WriteLine($"Gear: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].gear}");
                Debug.WriteLine($"Engine RPM: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].engineRPM}");
                Debug.WriteLine($"DRS: {(packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].drs == 0 ? "Off" : "On")}");
                Debug.WriteLine($"Rev lights percent: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].revLightsPercent}%");
                Debug.WriteLine($"Rev lights bit value: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].revLightsBitValue}");

                Debug.WriteLine("Brakes temperature: [{0}]", string.Join(", ", packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].brakesTemperature));
                Debug.WriteLine("Tyres surface temperature: [{0}]", string.Join(", ", packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].tyresSurfaceTemperature));
                Debug.WriteLine("Tyres inner temperature: [{0}]", string.Join(", ", packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].tyresInnerTemperature));

                Debug.WriteLine($"Engine temperature: {packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].engineTemperature}°C");

                Debug.WriteLine("Tyres pressure: [{0}]", string.Join(", ", packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].tyresPressure));
                Debug.WriteLine("Surface type: [{0}]", string.Join(", ", packetCarTelemetryData.carTelemetryData[packetCarTelemetryData.playerCarIndex].surfaceType));

            }
        }

    }
}