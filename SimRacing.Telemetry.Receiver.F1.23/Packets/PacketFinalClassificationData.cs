using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1_22_UDP_Telemetry_Receiver.Packets
{
    /// <summary>
    /// Data representing the final classification of a race.
    /// </summary>
    public class FinalClassificationData
    {
        /// <summary>
        /// Finishing position of the car.
        /// </summary>
        public byte finishingPosition;

        /// <summary>
        /// Number of laps completed by the car.
        /// </summary>
        public byte numberOfLapsCompleted;

        /// <summary>
        /// Grid position of the car.
        /// </summary>
        public byte gridPosition;

        /// <summary>
        /// Number of points scored by the car.
        /// </summary>
        public byte pointsScored;

        /// <summary>
        /// Number of pit stops made by the car.
        /// </summary>
        public byte numberOFPitStops;

        /// <summary>
        /// Result status of the car.
        /// </summary>
        public byte resultStatus;

        /// <summary>
        /// Best lap time of the session in milliseconds.
        /// </summary>
        public uint bestLapTimeInMS;

        /// <summary>
        /// Total race time in seconds without penalties.
        /// </summary>
        public double totalRaceTime;

        /// <summary>
        /// Total penalties accumulated in seconds.
        /// </summary>
        public byte penaltiesTime;

        /// <summary>
        /// Number of penalties applied to this driver.
        /// </summary>
        public byte numberOfPenalties;

        /// <summary>
        /// Number of tyre stints up to the maximum.
        /// </summary>
        public byte numberOfTyreStints;

        /// <summary>
        /// Actual tyres used by this driver.
        /// </summary>
        public byte[] tyreStintsActual;

        /// <summary>
        /// Visual tyres used by this driver.
        /// </summary>
        public byte[] tyreStintsVisual;

        /// <summary>
        /// The lap number stints end on.
        /// </summary>
        public byte[] tyreStintsEndLaps;
    }
    /// <summary>
    /// Final Classification Packet
    /// This packet details the final classification at the end of the race, and the data will match with the post race results screen. This is especially useful for multiplayer games where it is not always possible to send lap times on the final frame because of network delay.
    /// Frequency: Once at the end of a race
    /// Size: 1015 bytes
    /// Version: 1    
    /// </summary>
    public class PacketFinalClassificationData : Packet
    {
        public PacketFinalClassificationData(byte[] data) : base(data)
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
            numCars = data[byteIndex++];

            classificationData = new FinalClassificationData[MAX_CARS_ON_TRACK];
            for (int i = 0; i < classificationData.Length; i++)
            {
                FinalClassificationData temp = new FinalClassificationData();


                temp.finishingPosition = data[byteIndex];              // Finishing position
                byteIndex++;
                temp.numberOfLapsCompleted = data[byteIndex];               // Number of laps completed
                byteIndex++;
                temp.gridPosition = data[byteIndex];          // Grid position of the car
                byteIndex++;
                temp.pointsScored = data[byteIndex];                // Number of points scored
                byteIndex++;
                temp.numberOFPitStops = data[byteIndex];           // Number of pit stops made
                byteIndex++;
                temp.resultStatus = data[byteIndex];          // Result status - 0 = invalid, 1 = inactive, 2 = active
                byteIndex++;                                                // 3 = finished, 4 = didnotfinish, 5 = disqualified
                                                                            // 6 = not classified, 7 = retired
                temp.bestLapTimeInMS = BitConverter.ToUInt32(data, byteIndex); ;       // Best lap time of the session in milliseconds
                byteIndex += 4;
                temp.totalRaceTime = BitConverter.ToDouble(data, byteIndex); ;         // Total race time in seconds without penalties
                byteIndex += 8;
                temp.penaltiesTime = data[byteIndex];         // Total penalties accumulated in seconds
                byteIndex++;
                temp.numberOfPenalties = data[byteIndex];          // Number of penalties applied to this driver
                byteIndex++;
                temp.numberOfTyreStints = data[byteIndex];         // Number of tyres stints up to maximum
                byteIndex++;
                temp.tyreStintsActual = new byte[TYRE_STINT_TOTAL];// [8];   // Actual tyres used by this driver
                for (int x = 0; x < TYRE_STINT_TOTAL; x++)
                {
                    temp.tyreStintsActual[x] = data[byteIndex];
                    byteIndex++;
                }

                temp.tyreStintsVisual = new byte[TYRE_STINT_TOTAL];// [8];   // Visual tyres used by this driver
                for (int x = 0; x < TYRE_STINT_TOTAL; x++)
                {
                    temp.tyreStintsVisual[x] = data[byteIndex];
                    byteIndex++;
                }

                temp.tyreStintsEndLaps = new byte[TYRE_STINT_TOTAL];// [8];  // The lap number stints end on
                for (int x = 0; x < TYRE_STINT_TOTAL; x++)
                {
                    temp.tyreStintsEndLaps[x] = data[byteIndex];
                    byteIndex++;
                }
            }
        }

        public byte numCars;          // Number of cars in the final classification
        public FinalClassificationData[] classificationData;// [22];
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketFinalClassificationDataEventArgs : EventArgs
    {
        public PacketFinalClassificationData Packet { get; set; }

        public PacketFinalClassificationDataEventArgs(PacketFinalClassificationData packet)
        {
            Packet = packet;
        }
    }

}
