using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1_22_UDP_Telemetry_Receiver.Packets
{
    /// <summary>
    /// Car telemetry data for a single vehicle.
    /// </summary>
    public class CarTelemetryData
    {
        /// <summary>
        /// Speed of the car in kilometers per hour.
        /// </summary>
        public ushort speed;

        /// <summary>
        /// Amount of throttle applied (0.0 to 1.0).
        /// </summary>
        public float throttle;

        /// <summary>
        /// Steering (-1.0 for full lock left to 1.0 for full lock right).
        /// </summary>
        public float steer;

        /// <summary>
        /// Amount of brake applied (0.0 to 1.0).
        /// </summary>
        public float brake;

        /// <summary>
        /// Amount of clutch applied (0 to 100).
        /// </summary>
        public byte clutch;

        /// <summary>
        /// Gear selected (1-8, N=0, R=-1).
        /// </summary>
        public sbyte gear;

        /// <summary>
        /// Engine RPM.
        /// </summary>
        public ushort engineRPM;

        /// <summary>
        /// DRS status - 0 = off, 1 = on.
        /// </summary>
        public byte drs;

        /// <summary>
        /// Rev lights indicator (percentage).
        /// </summary>
        public byte revLightsPercent;

        /// <summary>
        /// Rev lights (bit 0 = leftmost LED, bit 14 = rightmost LED).
        /// </summary>
        public ushort revLightsBitValue;

        /// <summary>
        /// Brakes temperature (celsius) for each brake.
        /// </summary>
        public ushort[] brakesTemperature; //[4]

        /// <summary>
        /// Tyres surface temperature (celsius) for each tyre.
        /// </summary>
        public byte[] tyresSurfaceTemperature; //[4]

        /// <summary>
        /// Tyres inner temperature (celsius) for each tyre.
        /// </summary>
        public byte[] tyresInnerTemperature; //[4]

        /// <summary>
        /// Engine temperature (celsius).
        /// </summary>
        public ushort engineTemperature;

        /// <summary>
        /// Tyre pressure (PSI) for each tyre.
        /// </summary>
        public float[] tyresPressure; //[4]

        /// <summary>
        /// Driving surface type for each tyre.
        /// </summary>
        public byte[] surfaceType; //[4]
    }

    /// <summary>
    ///Car Telemetry Packet
    ///This packet details telemetry for all the cars in the race. It details various values that would be recorded on the car such as speed, throttle application, DRS etc. Note that the rev light configurations are presented separately as well and will mimic real life driver preferences.
    ///Frequency: Rate as specified in menus
    ///Size: 1347 bytes
    ///Version: 1
    /// </summary>
    public class PacketCarTelemetryData : Packet
    {
        public PacketCarTelemetryData(byte[] data) : base(data)
        {
            Packet headerData = getHeaderData(data.Take(HEADER_BYTE_SIZE).ToArray());

            packetFormat = headerData.packetFormat;            // 2022
            gameMajorVersion = headerData.gameMajorVersion;        // Game major version - "X.00"
            gameMinorVersion = headerData.gameMinorVersion;        // Game minor version - "1.XX"
            packetVersion = headerData.packetVersion;           // Version of this packet type, all start from 1
            packetId = headerData.packetId;                // Identifier for the packet type, see below
            sessionUID = headerData.sessionUID;              // Unique identifier for the session
            sessionTime = headerData.sessionTime;             // Session timestamp
            frameIdentifier = headerData.frameIdentifier;         // Identifier for the frame the data was retrieved on
            playerCarIndex = headerData.playerCarIndex;          // Index of player's car in the array
            secondaryPlayerCarIndex = headerData.secondaryPlayerCarIndex; // Index of secondary player's car in the array (splitscreen)
                                                                          // 255 if no second player

            //Parse the rest of the packet here
            carTelemetryData = new CarTelemetryData[MAX_CARS_ON_TRACK];
            int byteIndex = HEADER_BYTE_SIZE;
            for (int i = 0; i < MAX_CARS_ON_TRACK; i++)
            {
                //Parse the car telemtery data for all cars on track
                CarTelemetryData temp = new CarTelemetryData();
                temp.speed = BitConverter.ToUInt16(data, byteIndex); ;                    // Speed of car in kilometres per hour
                byteIndex += 2;
                temp.throttle = BitConverter.ToSingle(data, byteIndex); ;                 // Amount of throttle applied (0.0 to 1.0)
                byteIndex += 4;
                temp.steer = BitConverter.ToSingle(data, byteIndex); ;                    // Steering (-1.0 (full lock left) to 1.0 (full lock right))
                byteIndex += 4;
                temp.brake = BitConverter.ToSingle(data, byteIndex); ;                    // Amount of brake applied (0.0 to 1.0)
                byteIndex += 4;
                temp.clutch = data[byteIndex]; ;                   // Amount of clutch applied (0 to 100)
                byteIndex++;
                temp.gear = Convert.ToSByte(data[byteIndex]); ;                     // Gear selected (1-8, N=0, R=-1)
                byteIndex++;
                temp.engineRPM = BitConverter.ToUInt16(data, byteIndex); ;                // Engine RPM
                byteIndex += 2;
                temp.drs = data[byteIndex]; ;                      // 0 = off, 1 = on
                byteIndex++;
                temp.revLightsPercent = data[byteIndex]; ;         // Rev lights indicator (percentage)
                byteIndex++;
                temp.revLightsBitValue = BitConverter.ToUInt16(data, byteIndex); ;        // Rev lights (bit 0 = leftmost LED, bit 14 = rightmost LED)
                byteIndex += 2;

                temp.brakesTemperature = new ushort[4];// [4];     // Brakes temperature (celsius)
                for (int x = 0; x < WHEEL_COUNT; x++)
                {
                    temp.brakesTemperature[x] = BitConverter.ToUInt16(data, byteIndex);
                    byteIndex += 2;
                }


                temp.tyresSurfaceTemperature = new byte[4];// [4]; // Tyres surface temperature (celsius)
                for (int x = 0; x < WHEEL_COUNT; x++)
                {
                    temp.tyresSurfaceTemperature[x] = data[byteIndex];
                    byteIndex++;
                }

                temp.tyresInnerTemperature = new byte[4];// [4]; // Tyres inner temperature (celsius)
                for (int x = 0; x < WHEEL_COUNT; x++)
                {
                    temp.tyresInnerTemperature[x] = data[byteIndex];
                    byteIndex++;
                }

                temp.engineTemperature = BitConverter.ToUInt16(data, byteIndex); ;        // Engine temperature (celsius)
                byteIndex += 2;

                temp.tyresPressure = new float[4];// [4];         // Tyres pressure (PSI)
                for (int x = 0; x < WHEEL_COUNT; x++)
                {
                    temp.tyresPressure[x] = BitConverter.ToSingle(data, byteIndex);
                    byteIndex += 4;
                }


                temp.surfaceType = new byte[4];// [4];           // Driving surface, see appendices
                for (int x = 0; x < WHEEL_COUNT; x++)
                {
                    temp.surfaceType[x] = data[byteIndex];
                    byteIndex++;
                }

                carTelemetryData[i] = temp;
            }

            //Parse the player-only MFD data
            mfdPanelIndex = data[byteIndex];
            byteIndex++;
            mfdPanelIndexSecondaryPlayer = data[byteIndex];
            byteIndex++;
            suggestedGear = Convert.ToSByte(data[byteIndex]);
            byteIndex++;
        }

        public CarTelemetryData[] carTelemetryData;// [22];

        public byte mfdPanelIndex;       // Index of MFD panel open - 255 = MFD closed
                                         // Single player, race – 0 = Car setup, 1 = Pits
                                         // 2 = Damage, 3 =  Engine, 4 = Temperatures
                                         // May vary depending on game mode
        public byte mfdPanelIndexSecondaryPlayer;   // See above
        public sbyte suggestedGear;       // Suggested gear for the player (1-8)
                                          // 0 if no gear suggested
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketCarTelemetryDataEventArgs : EventArgs
    {
        public PacketCarTelemetryData Packet { get; set; }

        public PacketCarTelemetryDataEventArgs(PacketCarTelemetryData packet)
        {
            Packet = packet;
        }
    }

}
