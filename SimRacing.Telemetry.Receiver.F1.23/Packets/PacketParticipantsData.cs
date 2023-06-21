using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
    /// <summary>
    /// Represents participant data for a car.
    /// </summary>
    public class ParticipantData
    {
        /// <summary>
        /// Whether the vehicle is AI (1) or Human (0) controlled.
        /// </summary>
        public byte aiControlled;

        /// <summary>
        /// Driver id - see appendix, 255 if network human.
        /// </summary>
        public byte driverId;

        /// <summary>
        /// Network id - unique identifier for network players.
        /// </summary>
        public byte networkId;

        /// <summary>
        /// Team id - see appendix.
        /// </summary>
        public byte teamId;

        /// <summary>
        /// My team flag - 1 = My Team, 0 = otherwise.
        /// </summary>
        public byte myTeam;

        /// <summary>
        /// Race number of the car.
        /// </summary>
        public byte raceNumber;

        /// <summary>
        /// Nationality of the driver.
        /// </summary>
        public byte nationality;

        /// <summary>
        /// Name of participant in UTF-8 format - null terminated.
        /// Will be truncated with ... (U+2026) if too long.
        /// </summary>
        public char[] name;

        /// <summary>
        /// The player's UDP setting - 0 = restricted, 1 = public.
        /// </summary>
        public byte yourTelemetry;

        /// <summary>
        /// Represents the player's setting for displaying online names. 0 indicates the feature is off and 1 indicates it is on.
        /// </summary>
        public byte showOnlineNames;

        /// <summary>
        /// Represents the platform on which the game is being played. 
        /// 1 indicates Steam, 3 indicates PlayStation, 4 indicates Xbox, 6 indicates Origin, and 255 indicates an unknown platform.
        /// </summary>
        public byte platform;

    };



    /// <summary>
    /// Participants Packet
    /// This is a list of participants in the race. If the vehicle is controlled by AI, then the name will be the driver name. If this is a multiplayer game, the names will be the Steam Id on PC, or the LAN name if appropriate.
    /// N.B. on Xbox One, the names will always be the driver name, on PS4 the name will be the LAN name if playing a LAN game, otherwise it will be the driver name. 
    /// The array should be indexed by vehicle index.
    /// Frequency: Every 5 seconds
    /// Size: 1257 bytes
    /// Version: 1
    /// </summary>
    public class PacketParticipantsData : Packet
    {
        public PacketParticipantsData(byte[] data) : base(data)
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

            numActiveCars = data[byteIndex++];
            participants = new ParticipantData[MAX_CARS_ON_TRACK];
            for (int i = 0; i < MAX_CARS_ON_TRACK; i++)
            {
                ParticipantData temp = new ParticipantData();
                temp.aiControlled = data[byteIndex];           // Whether the vehicle is AI (1) or Human (0) controlled
                byteIndex++;
                temp.driverId = data[byteIndex];      // Driver id - see appendix, 255 if network human
                byteIndex++;
                temp.networkId = data[byteIndex];     // Network id – unique identifier for network players
                byteIndex++;
                temp.teamId = data[byteIndex];                 // Team id - see appendix
                byteIndex++;
                temp.myTeam = data[byteIndex];                 // My team flag – 1 = My Team, 0 = otherwise
                byteIndex++;
                temp.raceNumber = data[byteIndex];             // Race number of the car
                byteIndex++;
                temp.nationality = data[byteIndex];            // Nationality of the driver
                byteIndex++;
                temp.name = new char[48];// [48];               // Name of participant in UTF-8 format – null terminated
                                         // Will be truncated with … (U+2026) if too long
                for (int x = 0; x < temp.name.Length; x++)
                {
                    char temp2 = new char();
                    temp2 = Convert.ToChar(byteIndex);
                    byteIndex++;
                    temp.name[x] = temp2;
                }
                temp.yourTelemetry = data[byteIndex];          // The player's UDP setting, 0 = restricted, 1 = public
                byteIndex++;
                temp.showOnlineNames = data[byteIndex];
                byteIndex++;
                temp.platform = data[byteIndex];
                byteIndex++;

                participants[i] = temp;
            }

        }
        public byte numActiveCars;  // Number of active cars in the data – should match number of
                                    // cars on HUD
        public ParticipantData[] participants;// [22];
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketParticipantsDataEventArgs : EventArgs
    {
        public PacketParticipantsData Packet { get; set; }

        public PacketParticipantsDataEventArgs(PacketParticipantsData packet)
        {
            Packet = packet;
        }
    }

}
