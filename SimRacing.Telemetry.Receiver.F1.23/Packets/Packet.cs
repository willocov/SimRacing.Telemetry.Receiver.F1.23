using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
    //This is the packet header that is included in each packet
    //Packet ID (packetId) defines what structure is used for the packet body. See definitions below

    //The packets IDs are as follows:
    //Packet Name               Value Description
    //
    //Motion	                0	Contains all motion data for player’s car – only sent while player is in control
    //Session                   1	Data about the session – track, time left
    //Lap Data                  2	Data about all the lap times of cars in the session
    //Event	                    3	Various notable events that happen during a session
    //Participants	            4	List of participants in the session, mostly relevant for multiplayer
    //Car Setups	            5	Packet detailing car setups for cars in the race
    //Car Telemetry             6	Telemetry data for all cars
    //Car Status                7	Status data for all cars
    //Final Classification      8	Final classification confirmation at the end of a race
    //Lobby Info	            9	Information about players in a multiplayer lobby
    //Car Damage	            10	Damage status for all cars
    //Session History           11	Lap and tyre data for session
    public class Packet
    {

        public Packet() { }
        public Packet(byte[] data)
        {
            packetFormat = BitConverter.ToUInt16(data, 0);
            gameYear = data[2];
            gameMajorVersion = data[3];
            gameMinorVersion = data[4];
            packetVersion = data[5];                                        // Version of this packet type, all start from 1
            packetId = data[6];                                             // Identifier for the packet type, see below
            sessionUID = BitConverter.ToUInt64(data, 7);              // Unique identifier for the session
            sessionTime = BitConverter.ToSingle(data, 15);             // Session timestamp
            frameIdentifier = BitConverter.ToUInt32(data, 19);         // Identifier for the frame the data was retrieved on
            playerCarIndex = data[23];          // Index of player's car in the array
            secondaryPlayerCarIndex = data[24]; // Index of secondary player's car in the array (splitscreen)
                                                // 255 if no second player
        }
        /// <summary>
        /// The number of bytes used to deserialize header data.
        /// </summary>
        public const int HEADER_BYTE_SIZE = 29;

        /// <summary>
        /// The maximum number of cars on track. Used for array sizes related to grid data.
        /// </summary>
        public const int MAX_CARS_ON_TRACK = 22;

        /// <summary>
        /// The number of wheels per car. Used for array sizes related to wheel data.
        /// </summary>
        public const int WHEEL_COUNT = 4;

        /// <summary>
        /// The total number of tyre stints.
        /// </summary>
        public const int TYRE_STINT_TOTAL = 8;

        /// <summary>
        /// The packet format (year) for the packet data.
        /// </summary>
        public ushort packetFormat;

        /// <summary>
        /// Game year - last 2 digits. e.g. 23
        /// </summary>
        public byte gameYear;

        /// <summary>
        /// The major version of the game.
        /// </summary>
        public byte gameMajorVersion;

        /// <summary>
        /// The minor version of the game.
        /// </summary>
        public byte gameMinorVersion;

        /// <summary>
        /// The version of this packet type.
        /// </summary>
        public byte packetVersion;

        /// <summary>
        /// The identifier for the packet type.
        /// </summary>
        public byte packetId;

        /// <summary>
        /// The unique identifier for the session.
        /// </summary>
        public ulong sessionUID;

        /// <summary>
        /// The timestamp of the session.
        /// </summary>
        public float sessionTime;

        /// <summary>
        /// The identifier for the frame the data was retrieved on.
        /// </summary>
        public uint frameIdentifier;

        /// <summary>
        /// Overall identifier for the frame, doesn't go back after flashback
        /// </summary>
        public uint overallFrameIdentifier;

        /// <summary>
        /// The index of the player's car in the array.
        /// </summary>
        public byte playerCarIndex;

        /// <summary>
        /// The index of the secondary player's car in the array (splitscreen).
        /// </summary>
        public byte secondaryPlayerCarIndex;

        /// <summary>
        /// The type of packet.
        /// </summary>
        public SimRacing.Telemetry.Receiver.F1_23.Packets.PacketType packetType;
        public Packet getHeaderData(byte[] data)
        {
            //This method was added to have a single code base handle the header deserialization across all packet subclasses.
            //Deserialize the packet header data with this method, and then pass the returned Packet object into the constructor for the packet types subclass
            try
            {

                Packet packet = new Packet
                {
                    packetFormat = BitConverter.ToUInt16(data, 0),
                    gameMajorVersion = data[2],
                    gameMinorVersion = data[3],
                    packetVersion = data[4],           // Version of this packet type, all start from 1
                    packetId = data[5],                // Identifier for the packet type, see below
                    sessionUID = BitConverter.ToUInt64(data, 6),              // Unique identifier for the session
                    sessionTime = BitConverter.ToSingle(data, 14),             // Session timestamp
                    frameIdentifier = BitConverter.ToUInt32(data, 18),         // Identifier for the frame the data was retrieved on
                    playerCarIndex = data[22],          // Index of player's car in the array
                    secondaryPlayerCarIndex = data[23] // Index of secondary player's car in the array (splitscreen)
                                                       // 255 if no second player
                };
                return packet;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: Failed to get header data: " + e.Message);
                return null;
            }
        }
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketEventArgs : EventArgs
    {
        public Packet Packet { get; set; }

        public PacketEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }
}


