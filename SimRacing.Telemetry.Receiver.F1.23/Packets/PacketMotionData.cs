using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
    /// <summary>
    /// Represents the motion data for a car.
    /// </summary>
    public class CarMotionData
    {
        /// <summary>
        /// World space X position - metres.
        /// </summary>
        public float worldPositionX;

        /// <summary>
        /// World space Y position.
        /// </summary>
        public float worldPositionY;

        /// <summary>
        /// World space Z position.
        /// </summary>
        public float worldPositionZ;

        /// <summary>
        /// Velocity in world space X - metres/s
        /// </summary>
        public float worldVelocityX;

        /// <summary>
        /// Velocity in world space Y.
        /// </summary>
        public float worldVelocityY;

        /// <summary>
        /// Velocity in world space Z.
        /// </summary>
        public float worldVelocityZ;

        /// <summary>
        /// World space forward X direction (normalized).
        /// </summary>
        public short worldForwardDirX;

        /// <summary>
        /// World space forward Y direction (normalized).
        /// </summary>
        public short worldForwardDirY;

        /// <summary>
        /// World space forward Z direction (normalized).
        /// </summary>
        public short worldForwardDirZ;

        /// <summary>
        /// World space right X direction (normalized).
        /// </summary>
        public short worldRightDirX;

        /// <summary>
        /// World space right Y direction (normalized).
        /// </summary>
        public short worldRightDirY;

        /// <summary>
        /// World space right Z direction (normalized).
        /// </summary>
        public short worldRightDirZ;

        /// <summary>
        /// Lateral G-Force component.
        /// </summary>
        public float gForceLateral;

        /// <summary>
        /// Longitudinal G-Force component.
        /// </summary>
        public float gForceLongitudinal;

        /// <summary>
        /// Vertical G-Force component.
        /// </summary>
        public float gForceVertical;

        /// <summary>
        /// Yaw angle in radians.
        /// </summary>
        public float yaw;

        /// <summary>
        /// Pitch angle in radians.
        /// </summary>
        public float pitch;

        /// <summary>
        /// Roll angle in radians.
        /// </summary>
        public float roll;

    };



    /// <summary>
    /// Motion Packets (ID = 0)
    /// The motion packet gives physics data for all the cars being driven.There is additional data for the car being driven with the goal of being able to drive a motion platform setup.
    /// N.B.For the normalised vectors below, to convert to float values divide by 32767.0f – 16-bit signed values are used to pack the data and on the assumption that direction values are always between -1.0f and 1.0f.
    /// Frequency: Rate as specified in menus
    /// Size: 1464 bytes
    /// Version: 1   
    /// </summary>
    public class PacketMotionData : Packet
    {
        public PacketMotionData(byte[] data) : base(data)
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

            //Get car motion data for all 22 cars (max that will be on track)
            carMotionData = new CarMotionData[MAX_CARS_ON_TRACK];
            int byteIndex = HEADER_BYTE_SIZE;
            for (int i = 0; i < MAX_CARS_ON_TRACK; i++)
            {
                CarMotionData temp = new CarMotionData();
                temp.worldPositionX = BitConverter.ToSingle(data, byteIndex);             // Session timestamp
                byteIndex += 4;
                temp.worldPositionY = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
                temp.worldPositionZ = BitConverter.ToSingle(data, byteIndex);           // World space Z position
                byteIndex += 4;
                temp.worldVelocityX = BitConverter.ToSingle(data, byteIndex);           // Velocity in world space X
                byteIndex += 4;
                temp.worldVelocityY = BitConverter.ToSingle(data, byteIndex);           // Velocity in world space Y
                byteIndex += 4;
                temp.worldVelocityZ = BitConverter.ToSingle(data, byteIndex);           // Velocity in world space Z
                byteIndex += 4;
                temp.worldForwardDirX = BitConverter.ToInt16(data, byteIndex);         // World space forward X direction (normalised)
                byteIndex += 2;
                temp.worldForwardDirY = BitConverter.ToInt16(data, byteIndex);         // World space forward Y direction (normalised)
                byteIndex += 2;
                temp.worldForwardDirZ = BitConverter.ToInt16(data, byteIndex);         // World space forward Z direction (normalised)
                byteIndex += 2;
                temp.worldRightDirX = BitConverter.ToInt16(data, byteIndex);           // World space right X direction (normalised)
                byteIndex += 2;
                temp.worldRightDirY = BitConverter.ToInt16(data, byteIndex);           // World space right Y direction (normalised)
                byteIndex += 2;
                temp.worldRightDirZ = BitConverter.ToInt16(data, byteIndex);           // World space right Z direction (normalised)
                byteIndex += 2;
                temp.gForceLateral = BitConverter.ToSingle(data, byteIndex);            // Lateral G-Force component
                byteIndex += 4;
                temp.gForceLongitudinal = BitConverter.ToSingle(data, byteIndex);       // Longitudinal G-Force component
                byteIndex += 4;
                temp.gForceVertical = BitConverter.ToSingle(data, byteIndex);            // Vertical G-Force component
                byteIndex += 4;
                temp.yaw = BitConverter.ToSingle(data, byteIndex);                     // Yaw angle in radians
                byteIndex += 4;
                temp.pitch = BitConverter.ToSingle(data, byteIndex);                    // Pitch angle in radians
                byteIndex += 4;
                temp.roll = BitConverter.ToSingle(data, byteIndex);                   // Roll angle in radians
                byteIndex += 4;
                carMotionData[i] = temp;
            }

            

        }

        public CarMotionData[] carMotionData;// = new CarMotionData[22];      // Data for all cars on track
       
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketMotionDataEventArgs : EventArgs
    {
        public PacketMotionData Packet { get; set; }

        public PacketMotionDataEventArgs(PacketMotionData packet)
        {
            Packet = packet;
        }
    }
}
