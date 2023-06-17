using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1_22_UDP_Telemetry_Receiver.Packets
{

    public class CarDamageData
    {
        /// <summary>
        /// Tyre wear (percentage) for each tyre position.
        /// </summary>
        public float[] tyresWear; //[4];

        /// <summary>
        /// Tyre damage (percentage) for each tyre position.
        /// </summary>
        public byte[] tyresDamage; //[4];

        /// <summary>
        /// Brakes damage (percentage) for each brake.
        /// </summary>
        public byte[] brakesDamage; //[4];

        /// <summary>
        /// Front left wing damage (percentage).
        /// </summary>
        public byte frontLeftWingDamage;

        /// <summary>
        /// Front right wing damage (percentage).
        /// </summary>
        public byte frontRightWingDamage;

        /// <summary>
        /// Rear wing damage (percentage).
        /// </summary>
        public byte rearWingDamage;

        /// <summary>
        /// Floor damage (percentage).
        /// </summary>
        public byte floorDamage;

        /// <summary>
        /// Diffuser damage (percentage).
        /// </summary>
        public byte diffuserDamage;

        /// <summary>
        /// Sidepod damage (percentage).
        /// </summary>
        public byte sidepodDamage;

        /// <summary>
        /// Indicator for DRS fault, 0 = OK, 1 = fault.
        /// </summary>
        public byte drsFault;

        /// <summary>
        /// Indicator for ERS fault, 0 = OK, 1 = fault.
        /// </summary>
        public byte ersFault;

        /// <summary>
        /// Gear box damage (percentage).
        /// </summary>
        public byte gearBoxDamage;

        /// <summary>
        /// Engine damage (percentage).
        /// </summary>
        public byte engineDamage;

        /// <summary>
        /// Engine wear MGU-H (percentage).
        /// </summary>
        public byte engineMGUHWear;

        /// <summary>
        /// Engine wear ES (percentage).
        /// </summary>
        public byte engineESWear;

        /// <summary>
        /// Engine wear CE (percentage).
        /// </summary>
        public byte engineCEWear;

        /// <summary>
        /// Engine wear ICE (percentage).
        /// </summary>
        public byte engineICEWear;

        /// <summary>
        /// Engine wear MGU-K (percentage).
        /// </summary>
        public byte engineMGUKWear;

        /// <summary>
        /// Engine wear TC (percentage).
        /// </summary>
        public byte engineTCWear;

        /// <summary>
        /// Engine blown, 0 = OK, 1 = fault.
        /// </summary>
        public byte engineBlown;

        /// <summary>
        /// Engine seized, 0 = OK, 1 = fault.
        /// </summary>
        public byte engineSeized;
    };

    /// <summary>
    /// Car Damage Packet
    /// This packet details car damage parameters for all the cars in the race.
    /// Frequency: 2 per second
    /// Size: 948 bytes
    /// Version: 1    
    /// </summary>
    public class PacketCarDamageData : Packet
    {
        public PacketCarDamageData(byte[] data) : base(data)
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
            carDamageData = new CarDamageData[MAX_CARS_ON_TRACK];
            int byteIndex = HEADER_BYTE_SIZE;
            for (int i = 0; i < MAX_CARS_ON_TRACK; i++)
            {
                CarDamageData temp = new CarDamageData();

                temp.tyresWear = new float[4];// Tyre wear (percentage)
                for (int x = 0; x < WHEEL_COUNT; x++)
                {
                    temp.tyresWear[x] = BitConverter.ToSingle(data, byteIndex);
                    byteIndex += 4;
                }



                temp.tyresDamage = new byte[4];// Tyre damage (percentage)
                for (int x = 0; x < WHEEL_COUNT; x++)
                {
                    temp.tyresDamage[x] = data[byteIndex];
                    byteIndex++;
                }

                temp.brakesDamage = new byte[4];  // Brakes damage (percentage)
                for (int x = 0; x < WHEEL_COUNT; x++)
                {
                    temp.brakesDamage[x] = data[byteIndex];
                    byteIndex++;
                }

                temp.frontLeftWingDamage = data[byteIndex];              // Front left wing damage (percentage)
                byteIndex++;
                temp.frontRightWingDamage = data[byteIndex];             // Front right wing damage (percentage)
                byteIndex++;
                temp.rearWingDamage = data[byteIndex];                   // Rear wing damage (percentage)
                byteIndex++;
                temp.floorDamage = data[byteIndex];                      // Floor damage (percentage)
                byteIndex++;
                temp.diffuserDamage = data[byteIndex];                   // Diffuser damage (percentage)
                byteIndex++;
                temp.sidepodDamage = data[byteIndex];                    // Sidepod damage (percentage)
                byteIndex++;
                temp.drsFault = data[byteIndex];                         // Indicator for DRS fault, 0 = OK, 1 = fault
                byteIndex++;
                temp.ersFault = data[byteIndex];                          // Indicator for ERS fault, 0 = OK, 1 = fault
                byteIndex++;
                temp.gearBoxDamage = data[byteIndex];                     // Gear box damage (percentage)
                byteIndex++;
                temp.engineDamage = data[byteIndex];                      // Engine damage (percentage)
                byteIndex++;
                temp.engineMGUHWear = data[byteIndex];                    // Engine wear MGU-H (percentage)
                byteIndex++;
                temp.engineESWear = data[byteIndex];                      // Engine wear ES (percentage)
                byteIndex++;
                temp.engineCEWear = data[byteIndex];                      // Engine wear CE (percentage)
                byteIndex++;
                temp.engineICEWear = data[byteIndex];                     // Engine wear ICE (percentage)
                byteIndex++;
                temp.engineMGUKWear = data[byteIndex];                    // Engine wear MGU-K (percentage)
                byteIndex++;
                temp.engineTCWear = data[byteIndex];                      // Engine wear TC (percentage)
                byteIndex++;
                temp.engineBlown = data[byteIndex];                       // Engine blown, 0 = OK, 1 = fault
                byteIndex++;
                temp.engineSeized = data[byteIndex];                      // Engine seized, 0 = OK, 1 = fault
                byteIndex++;
                carDamageData[i] = temp;
            }
        }

        public CarDamageData[] carDamageData;// [22];
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketCarDamageDataEventArgs : EventArgs
    {
        public PacketCarDamageData Packet { get; set; }

        public PacketCarDamageDataEventArgs(PacketCarDamageData packet)
        {
            Packet = packet;
        }
    }
}
