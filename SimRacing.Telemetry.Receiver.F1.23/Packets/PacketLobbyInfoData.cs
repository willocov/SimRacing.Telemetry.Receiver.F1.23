using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
    /// <summary>
    /// Represents the lobby information data for a participant.
    /// </summary>
    public class LobbyInfoData
    {
        /// <summary>
        /// Indicates whether the vehicle is AI controlled (1) or human controlled (0).
        /// </summary>
        public byte aiControlled;

        /// <summary>
        /// Team ID of the participant - see appendix. If no team is currently selected, the value is 255.
        /// </summary>
        public byte teamId;

        /// <summary>
        /// Nationality of the driver.
        /// </summary>
        public byte nationality;

        /// <summary>
        /// 1 = Steam, 3 = Playstation, 4 = Xbox, 6 = Origin, 255 = Unknown
        /// </summary>
        public byte platform;

        /// <summary>
        /// Name of the participant in UTF-8 format. It is null-terminated and will be truncated with ellipsis (U+2026) if too long.
        /// </summary>
        public char[] name; // [48]

        /// <summary>
        /// Car number of the player.
        /// </summary>
        public byte carNumber;

        /// <summary>
        /// Ready status of the participant - 0 = not ready, 1 = ready, 2 = spectating.
        /// </summary>
        public byte readyStatus;

    };



    /// <summary>
    /// Lobby Info Packet
    /// This packet details the players currently in a multiplayer lobby. It details each player’s selected car, any AI involved in the game and also the ready status of each of the participants.
    /// Frequency: Two every second when in the lobby
    /// Size: 1191 bytes
    /// Version: 1    
    /// </summary>
    public class PacketLobbyInfoData : Packet
    {
        public PacketLobbyInfoData(byte[] data) : base(data)
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
            numPlayers = data[byteIndex];
            byteIndex++;
            lobbyPlayers = new LobbyInfoData[MAX_CARS_ON_TRACK];
            for (int i = 0; i < HEADER_BYTE_SIZE; i++)
            {
                LobbyInfoData temp = new LobbyInfoData();
                temp.aiControlled = data[byteIndex];            // Whether the vehicle is AI (1) or Human (0) controlled
                byteIndex++;
                temp.teamId = data[byteIndex];                  // Team id - see appendix (255 if no team currently selected)
                byteIndex++;
                temp.nationality = data[byteIndex];             // Nationality of the driver
                byteIndex++;
                temp.platform = data[byteIndex];
                byteIndex++;
                temp.name = new char[48];// [48];        // Name of participant in UTF-8 format – null terminated
                                         // Will be truncated with ... (U+2026) if too long
                for (int x = 0; x < temp.name.Length; x++)
                {
                    char temp2 = new char();
                    temp2 = Convert.ToChar(byteIndex);
                    byteIndex++;
                    temp.name[x] = temp2;
                }
                temp.carNumber = data[byteIndex];               // Car number of the player
                byteIndex++;
                temp.readyStatus = data[byteIndex];             // 0 = not ready, 1 = ready, 2 = spectating
                byteIndex++;
                lobbyPlayers[i] = temp;
            }



        }

        // Packet specific data
        byte numPlayers;               // Number of players in the lobby data
        LobbyInfoData[] lobbyPlayers;// [22];
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketLobbyInfoDataEventArgs : EventArgs
    {
        public PacketLobbyInfoData Packet { get; set; }

        public PacketLobbyInfoDataEventArgs(PacketLobbyInfoData packet)
        {
            Packet = packet;
        }
    }
}
