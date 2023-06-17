using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1_22_UDP_Telemetry_Receiver.Packets
{
    /// <summary>
    /// Car setup data for each vehicle in the session.
    /// </summary>
    public class CarSetupData
    {
        /// <summary>
        /// Front wing aero.
        /// </summary>
        public byte frontWing;

        /// <summary>
        /// Rear wing aero.
        /// </summary>
        public byte rearWing;

        /// <summary>
        /// Differential adjustment on throttle (percentage).
        /// </summary>
        public byte onThrottle;

        /// <summary>
        /// Differential adjustment off throttle (percentage).
        /// </summary>
        public byte offThrottle;

        /// <summary>
        /// Front camber angle (suspension geometry).
        /// </summary>
        public float frontCamber;

        /// <summary>
        /// Rear camber angle (suspension geometry).
        /// </summary>
        public float rearCamber;

        /// <summary>
        /// Front toe angle (suspension geometry).
        /// </summary>
        public float frontToe;

        /// <summary>
        /// Rear toe angle (suspension geometry).
        /// </summary>
        public float rearToe;

        /// <summary>
        /// Front suspension.
        /// </summary>
        public byte frontSuspension;

        /// <summary>
        /// Rear suspension.
        /// </summary>
        public byte rearSuspension;

        /// <summary>
        /// Front anti-roll bar.
        /// </summary>
        public byte frontAntiRollBar;

        /// <summary>
        /// Rear anti-roll bar.
        /// </summary>
        public byte rearAntiRollBar;

        /// <summary>
        /// Front ride height.
        /// </summary>
        public byte frontSuspensionHeight;

        /// <summary>
        /// Rear ride height.
        /// </summary>
        public byte rearSuspensionHeight;

        /// <summary>
        /// Brake pressure (percentage).
        /// </summary>
        public byte brakePressure;

        /// <summary>
        /// Brake bias (percentage).
        /// </summary>
        public byte brakeBias;

        /// <summary>
        /// Rear left tyre pressure (PSI).
        /// </summary>
        public float rearLeftTyrePressure;

        /// <summary>
        /// Rear right tyre pressure (PSI).
        /// </summary>
        public float rearRightTyrePressure;

        /// <summary>
        /// Front left tyre pressure (PSI).
        /// </summary>
        public float frontLeftTyrePressure;

        /// <summary>
        /// Front right tyre pressure (PSI).
        /// </summary>
        public float frontRightTyrePressure;

        /// <summary>
        /// Ballast.
        /// </summary>
        public byte ballast;

        /// <summary>
        /// Fuel load.
        /// </summary>
        public float fuelLoad;
    }
    /// <summary>
    ///Car Setups Packet
    ///This packet details the car setups for each vehicle in the session. Note that in multiplayer games, other player cars will appear as blank, you will only be able to see your car setup and AI cars.
    ///Frequency: 2 per second
    ///Size: 1102 bytes
    ///Version: 1    
    /// </summary>
    public class PacketCarSetupData : Packet
    {
        public PacketCarSetupData(byte[] data) : base(data)
        {
            Packet headerData = getHeaderData(data.Take(HEADER_BYTE_SIZE).ToArray());

            packetFormat = headerData.packetFormat;            // 2022
            gameMajorVersion = headerData.gameMajorVersion;        // Game major version - "X.00"
            gameMinorVersion = headerData.gameMinorVersion;        // Game minor version - "1.XX"
            packetVersion = headerData.packetVersion;           // Version of this packet type, all start from 1
            packetId = headerData.packetId;                // Identifier for the packet type, see below
            packetType = headerData.packetType;
            sessionUID = headerData.sessionUID;              // Unique identifier for the session
            sessionTime = headerData.sessionTime;             // Session timestamp
            frameIdentifier = headerData.frameIdentifier;         // Identifier for the frame the data was retrieved on
            playerCarIndex = headerData.playerCarIndex;          // Index of player's car in the array
            secondaryPlayerCarIndex = headerData.secondaryPlayerCarIndex; // Index of secondary player's car in the array (splitscreen)
                                                                          // 255 if no second player

            //Parse the rest of the packet here

            //loop for 22 cars
            carSetups = new CarSetupData[MAX_CARS_ON_TRACK];
            int byteIndex = HEADER_BYTE_SIZE;
            for (int i = 0; i < MAX_CARS_ON_TRACK; i++)
            {
                CarSetupData temp = new CarSetupData();

                temp.frontWing = data[byteIndex];                // Front wing aero
                byteIndex++;
                temp.rearWing = data[byteIndex];                // Rear wing aero
                byteIndex++;
                temp.onThrottle = data[byteIndex];               // Differential adjustment on throttle (percentage)
                byteIndex++;
                temp.offThrottle = data[byteIndex];              // Differential adjustment off throttle (percentage)
                byteIndex++;
                temp.frontCamber = BitConverter.ToSingle(data, byteIndex);           // Front camber angle (suspension geometry)
                byteIndex += 4;
                temp.rearCamber = BitConverter.ToSingle(data, byteIndex);               // Rear camber angle (suspension geometry)
                byteIndex += 4;
                temp.frontToe = BitConverter.ToSingle(data, byteIndex);                 // Front toe angle (suspension geometry)
                byteIndex += 4;
                temp.rearToe = BitConverter.ToSingle(data, byteIndex);                 // Rear toe angle (suspension geometry)
                byteIndex += 4;

                temp.frontSuspension = data[byteIndex];             // Front suspension
                byteIndex++;
                temp.rearSuspension = data[byteIndex];               // Rear suspension
                byteIndex++;
                temp.frontAntiRollBar = data[byteIndex];             // Front anti-roll bar
                byteIndex++;
                temp.rearAntiRollBar = data[byteIndex];              // Front anti-roll bar
                byteIndex++;
                temp.frontSuspensionHeight = data[byteIndex];        // Front ride height
                byteIndex++;
                temp.rearSuspensionHeight = data[byteIndex];         // Rear ride height
                byteIndex++;
                temp.brakePressure = data[byteIndex];                // Brake pressure (percentage)
                byteIndex++;
                temp.brakeBias = data[byteIndex];                    // Brake bias (percentage)
                byteIndex++;

                temp.rearLeftTyrePressure = BitConverter.ToSingle(data, byteIndex);     // Rear left tyre pressure (PSI)
                byteIndex += 4;
                temp.rearRightTyrePressure = BitConverter.ToSingle(data, byteIndex);    // Rear right tyre pressure (PSI)
                byteIndex += 4;
                temp.frontLeftTyrePressure = BitConverter.ToSingle(data, byteIndex);    // Front left tyre pressure (PSI)
                byteIndex += 4;
                temp.frontRightTyrePressure = BitConverter.ToSingle(data, byteIndex);   // Front right tyre pressure (PSI)
                byteIndex += 4;
                temp.ballast = data[byteIndex];                      // Ballast
                byteIndex++;

                temp.fuelLoad = BitConverter.ToSingle(data, byteIndex);                 // Fuel load
                byteIndex += 4;
                carSetups[i] = temp;
            }
        }

        public CarSetupData[] carSetups;// [22];
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketCarSetupDataEventArgs : EventArgs
    {
        public PacketCarSetupData Packet { get; set; }

        public PacketCarSetupDataEventArgs(PacketCarSetupData packet)
        {
            Packet = packet;
        }
    }
}
