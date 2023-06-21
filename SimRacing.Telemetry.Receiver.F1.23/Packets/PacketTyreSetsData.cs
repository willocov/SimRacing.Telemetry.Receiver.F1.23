using F1_22_UDP_Telemetry_Receiver.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
    /// <summary>
    /// Represents data related to a set of tyres.
    /// </summary>
    public class TyreSetData
    {
        /// <summary>
        /// Actual tyre compound used.
        /// </summary>
        public byte actualTyreCompound;

        /// <summary>
        /// Visual tyre compound used.
        /// </summary>
        public byte visualTyreCompound;

        /// <summary>
        /// Tyre wear represented as a percentage.
        /// </summary>
        public byte wear;

        /// <summary>
        /// Indicates whether this set of tyres is currently available. 
        /// </summary>
        public byte available;

        /// <summary>
        /// Recommended session for this tyre set.
        /// </summary>
        public byte recommendedSession;

        /// <summary>
        /// Represents the number of laps left in this tyre set.
        /// </summary>
        public byte lifeSpan;

        /// <summary>
        /// Maximum number of laps recommended for this tyre compound.
        /// </summary>
        public byte usableLife;

        /// <summary>
        /// Represents the lap delta time in milliseconds compared to the fitted set.
        /// </summary>
        public short lapDeltaTime;

        /// <summary>
        /// Indicates whether the tyre set is fitted or not.
        /// </summary>
        public byte fitted;
    };


    /// <summary>
    /// Represents packet data related to tyre sets, inherited from the Packet class.
    /// </summary>
    public class PacketTyreSetData : Packet
    {
        /// <summary>
        /// Index of the car to which the tyre sets belong.
        /// </summary>
        public byte carIndex;

        /// <summary>
        /// An array of TyreSetData objects, representing different tyre sets. It can contain up to 13 dry and 7 wet tyre sets.
        /// </summary>
        public TyreSetData[] tyreSetData;

        /// <summary>
        /// The index into the tyreSetData array that points to the currently fitted tyre set.
        /// </summary>
        public byte fittedIndex;
        public PacketTyreSetData(byte[] data) : base(data)
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
            int byteIndex = HEADER_BYTE_SIZE;
            TyreSetData[] tyreSetData = new TyreSetData[20];

            carIndex = data[byteIndex];
            byteIndex++;

            //Parse the tire set data
            for (int i = 0; i < 20; i++) //20 = The max size of the array
            {
                TyreSetData temp = new TyreSetData();

                temp.actualTyreCompound = data[byteIndex];
                byteIndex++;
                temp.visualTyreCompound = data[byteIndex];
                byteIndex++;
                temp.wear = data[byteIndex]; byteIndex++;
                temp.available = data[byteIndex]; byteIndex++;
                temp.recommendedSession = data[byteIndex]; byteIndex++;
                temp.lifeSpan = data[byteIndex]; byteIndex++;
                temp.usableLife = data[byteIndex]; byteIndex++;
                temp.lapDeltaTime = BitConverter.ToInt16(data, byteIndex);
                byteIndex += 2;
                temp.fitted = data[byteIndex];
                byteIndex++;

                tyreSetData[i] = temp;
            }

            fittedIndex = data[byteIndex];
        }
    }

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketTyreSetDataEventArgs : EventArgs
    {
        public PacketTyreSetData Packet { get; set; }

        public PacketTyreSetDataEventArgs(PacketTyreSetData packet)
        {
            Packet = packet;
        }
    }
}
