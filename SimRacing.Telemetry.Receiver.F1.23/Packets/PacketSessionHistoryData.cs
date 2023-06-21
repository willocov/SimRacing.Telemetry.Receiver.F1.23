using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
    /// <summary>
    /// Data history for a single lap
    /// </summary>
    public class LapHistoryData
    {
        /// <summary>
        /// Lap time in milliseconds
        /// </summary>
        public uint lapTimeInMS;

        /// <summary>
        /// Sector 1 time in milliseconds
        /// </summary>
        public ushort sector1TimeInMS;

        /// <summary>
        /// Sector 1 whole minute part
        /// </summary>
        public byte sector1TimeInMinutes;

        /// <summary>
        /// Sector 2 time in milliseconds
        /// </summary>
        public ushort sector2TimeInMS;

        /// <summary>
        /// Sector 2 whole minute part
        /// </summary>
        public byte sector2TimeInMinutes;

        /// <summary>
        /// Sector 3 time in milliseconds
        /// </summary>
        public ushort sector3TimeInMS;

        /// <summary>
        /// Sector 3 whole minute part
        /// </summary>
        public byte sector3TimeInMinutes;

        /// <summary>
        /// Lap and sector validity flags: 
        /// 0x01 bit set-lap valid,
        /// 0x02 bit set-sector 1 valid,
        /// 0x04 bit set-sector 2 valid,
        /// 0x08 bit set-sector 3 valid
        /// </summary>
        public byte lapValidBitFlags;
    };

    /// <summary>
    /// Data for tyre usage history
    /// </summary>
    public class TyreStintHistoryData
    {
        /// <summary>
        /// Lap the tyre usage ends on (255 of current tyre)
        /// </summary>
        public byte endLap;

        /// <summary>
        /// Actual tyres used by this driver
        /// </summary>
        public byte tyreActualCompound;

        /// <summary>
        /// Visual tyres used by this driver
        /// </summary>
        public byte tyreVisualCompound;
    };

    /// <summary>
    /// Session History Packet.
    /// This packet contains lap times and tyre usage for the session. This packet works slightly differently to other packets.
    /// To reduce CPU and bandwidth, each packet relates to a specific vehicle and is sent every 1/20 s, and the vehicle being sent is cycled through.
    /// Therefore, in a 20 car race you should receive an update for each vehicle at least once per second.
    /// Note that at the end of the race, after the final classification packet has been sent, a final bulk update of all the session histories for the vehicles in that session will be sent.
    /// Frequency: 20 per second but cycling through cars.
    /// Size: 1155 bytes.
    /// Version: 1.
    /// </summary>

    public class PacketSessionHistoryData : Packet
    {
        public PacketSessionHistoryData(byte[] data) : base(data)
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

            carIdxl = data[byteIndex];                   // Index of the car this lap data relates to
            byteIndex++;
            numLapsl = data[byteIndex];                  // Num laps in the data (including current partial lap)
            byteIndex++;
            numTyreStintsl = data[byteIndex];            // Number of tyre stints in the data
            byteIndex++;
            bestLapTimeLapNuml = data[byteIndex];        // Lap the best lap time was achieved on
            byteIndex++;
            bestSector1LapNuml = data[byteIndex];        // Lap the best Sector 1 time was achieved on
            byteIndex++;
            bestSector2LapNuml = data[byteIndex];        // Lap the best Sector 2 time was achieved on
            byteIndex++;
            bestSector3LapNuml = data[byteIndex];        // Lap the best Sector 3 time was achieved on
            byteIndex++;

            lapHistoryData = new LapHistoryData[100];
            for (int i = 0; i < lapHistoryData.Length; i++)
            {
                LapHistoryData temp = new LapHistoryData();
                temp.lapTimeInMS = BitConverter.ToUInt32(data, byteIndex);           // Lap time in milliseconds
                byteIndex += 4;
                temp.sector1TimeInMS = BitConverter.ToUInt16(data, byteIndex);       // Sector 1 time in milliseconds
                byteIndex += 2;
                temp.sector1TimeInMinutes = data[byteIndex];
                byteIndex++;
                temp.sector2TimeInMS = BitConverter.ToUInt16(data, byteIndex);       // Sector 2 time in milliseconds
                byteIndex += 2;
                temp.sector2TimeInMinutes = data[byteIndex];
                byteIndex++;
                temp.sector3TimeInMS = BitConverter.ToUInt16(data, byteIndex);       // Sector 3 time in milliseconds
                byteIndex += 2;
                temp.sector3TimeInMinutes = data[byteIndex];
                byteIndex++;
                temp.lapValidBitFlags = data[byteIndex++];      // 0x01 bit set-lap valid,      0x02 bit set-sector 1 valid
                                                                // 0x04 bit set-sector 2 valid, 0x08 bit set-sector 3 valid
                lapHistoryData[i] = temp;
            }

            tyreStintsHistoryData = new TyreStintHistoryData[8];
            for (int i = 0; i < tyreStintsHistoryData.Length; i++)
            {
                TyreStintHistoryData temp = new TyreStintHistoryData();
                temp.endLap = data[byteIndex];                // Lap the tyre usage ends on (255 of current tyre)
                byteIndex++;
                temp.tyreActualCompound = data[byteIndex];    // Actual tyres used by this driver
                byteIndex++;
                temp.tyreVisualCompound = data[byteIndex];    // Visual tyres used by this driver
                byteIndex++;
                tyreStintsHistoryData[i] = temp;
            }

        }

        public byte carIdxl;                   // Index of the car this lap data relates to
        public byte numLapsl;                  // Num laps in the data (including current partial lap)
        public byte numTyreStintsl;            // Number of tyre stints in the data

        public byte bestLapTimeLapNuml;        // Lap the best lap time was achieved on
        public byte bestSector1LapNuml;        // Lap the best Sector 1 time was achieved on
        public byte bestSector2LapNuml;        // Lap the best Sector 2 time was achieved on
        public byte bestSector3LapNuml;        // Lap the best Sector 3 time was achieved on

        public LapHistoryData[] lapHistoryData;// [100];	// 100 laps of data max
        public TyreStintHistoryData[] tyreStintsHistoryData;// [8];
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketSessionHistoryDataEventArgs : EventArgs
    {
        public PacketSessionHistoryData Packet { get; set; }

        public PacketSessionHistoryDataEventArgs(PacketSessionHistoryData packet)
        {
            Packet = packet;
        }
    }
}
