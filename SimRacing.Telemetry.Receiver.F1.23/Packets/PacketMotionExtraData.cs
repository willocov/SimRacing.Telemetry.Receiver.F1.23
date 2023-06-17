using F1_22_UDP_Telemetry_Receiver.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimRacing.Telemetry.Receiver.F1._23.Packets
{
    public class PacketMotionExtraData : Packet
    {

        public PacketMotionExtraData(byte[] data) : base(data)
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
            int byteIndex = HEADER_BYTE_SIZE;

            //Parse out the extra player-only car data
            suspensionPosition = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                suspensionPosition[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }

            suspensionVelocity = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                suspensionVelocity[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }

            suspensionAcceleration = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                suspensionAcceleration[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }

            wheelSpeed = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                wheelSpeed[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }

            wheelSlipRatio = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                wheelSlipRatio[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }

            wheelSlipAngle = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                wheelSlipAngle[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }

            wheelLatForce = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                wheelLatForce[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }

            wheelLongForce = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                wheelLongForce[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }
            heightOfCOGAboveGround = BitConverter.ToSingle(data, byteIndex);            // Velocity in local space
            byteIndex += 4;
            localVelocityX = BitConverter.ToSingle(data, byteIndex);            // Velocity in local space
            byteIndex += 4;
            localVelocityY = BitConverter.ToSingle(data, byteIndex);             // Velocity in local space
            byteIndex += 4;
            localVelocityZ = BitConverter.ToSingle(data, byteIndex);             // Velocity in local space
            byteIndex += 4;
            angularVelocityX = BitConverter.ToSingle(data, byteIndex);      // Angular velocity x-component
            byteIndex += 4;
            angularVelocityY = BitConverter.ToSingle(data, byteIndex);             // Angular velocity y-component
            byteIndex += 4;
            angularVelocityZ = BitConverter.ToSingle(data, byteIndex);             // Angular velocity z-component
            byteIndex += 4;
            angularAccelerationX = BitConverter.ToSingle(data, byteIndex);         // Angular velocity x-component
            byteIndex += 4;
            angularAccelerationY = BitConverter.ToSingle(data, byteIndex);    // Angular velocity y-component
            byteIndex += 4;
            angularAccelerationZ = BitConverter.ToSingle(data, byteIndex);        // Angular velocity z-component
            byteIndex += 4;
            frontWheelsAngle = BitConverter.ToSingle(data, byteIndex);
            byteIndex += 4;
            wheelVertForce = new float[4];
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                wheelVertForce[i] = BitConverter.ToSingle(data, byteIndex);
                byteIndex += 4;
            }
        }

        /// <summary>
        /// The suspension positions for each wheel. Note: All wheel arrays have the following order: RL, RR, FL, FR.
        /// </summary>
        public float[] suspensionPosition; // = new float[4];

        /// <summary>
        /// The suspension velocities for each wheel. The wheel order is RL, RR, FL, FR.
        /// </summary>
        public float[] suspensionVelocity; // = new float[4];

        /// <summary>
        /// The suspension accelerations for each wheel. The wheel order is RL, RR, FL, FR.
        /// </summary>
        public float[] suspensionAcceleration; // = new float[4];

        /// <summary>
        /// The speed of each wheel.
        /// </summary>
        public float[] wheelSpeed; // = new float[4];

        /// <summary>
        /// The slip ratio for each wheel.
        /// </summary>
        public float[] wheelSlipRatio;  //[4];           // Slip ratio for each wheel

        /// <summary>
        /// The slip ratio for each wheel.
        /// </summary>
        public float[] wheelSlipAngle;  //[4];           // Slip angles for each wheel

        /// <summary>
        /// Lateral forces for each wheel
        /// </summary>
        public float[] wheelLatForce;   //[4];            // Lateral forces for each wheel

        /// <summary>
        /// Longitudinal forces for each wheel
        /// </summary>
        public float[] wheelLongForce;  //[4];           // Longitudinal forces for each wheel

        /// <summary>
        /// Height of centre of gravity above ground  
        /// </summary>
        public float heightOfCOGAboveGround;  //;      // Height of centre of gravity above ground    
        
        /// <summary>
        /// The velocity in the local X space.
        /// </summary>
        public float localVelocityX;

        /// <summary>
        /// The velocity in the local Y space.
        /// </summary>
        public float localVelocityY;

        /// <summary>
        /// The velocity in the local Z space.
        /// </summary>
        public float localVelocityZ;

        /// <summary>
        /// The angular velocity x-component.
        /// </summary>
        public float angularVelocityX;

        /// <summary>
        /// The angular velocity y-component.
        /// </summary>
        public float angularVelocityY;

        /// <summary>
        /// The angular velocity z-component.
        /// </summary>
        public float angularVelocityZ;

        /// <summary>
        /// The angular acceleration x-component.
        /// </summary>
        public float angularAccelerationX;

        /// <summary>
        /// The angular acceleration y-component.
        /// </summary>
        public float angularAccelerationY;

        /// <summary>
        /// The angular acceleration z-component.
        /// </summary>
        public float angularAccelerationZ;

        /// <summary>
        /// The current front wheels angle in radians.
        /// </summary>
        public float frontWheelsAngle;

        /// <summary>
        /// Vertical forces for each wheel
        /// </summary>
        public float[] wheelVertForce;  //[4];           // Vertical forces for each wheel

    }

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketMotionExtraDataEventArgs : EventArgs
    {
        public PacketMotionExtraData Packet { get; set; }

        public PacketMotionExtraDataEventArgs(PacketMotionExtraData packet)
        {
            Packet = packet;
        }
    }
}
