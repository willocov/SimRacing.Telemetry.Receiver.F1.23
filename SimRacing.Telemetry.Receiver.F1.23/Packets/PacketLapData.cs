using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
    /// <summary>
    /// Data representing the lap information of a car during a race.
    /// </summary>
    public class LapData
    {
        /// <summary>
        /// Last lap time in milliseconds.
        /// </summary>
        public uint lastLapTimeInMS;

        /// <summary>
        /// Current time around the lap in milliseconds.
        /// </summary>
        public uint currentLapTimeInMS;

        /// <summary>
        /// Sector 1 time in milliseconds.
        /// </summary>
        public ushort sector1TimeInMS;

        /// <summary>
        /// Sector 1 whole minute part.
        /// </summary>
        public byte sector1TimeInMinutes;

        /// <summary>
        /// Sector 2 time in milliseconds.
        /// </summary>
        public ushort sector2TimeInMS;

        /// <summary>
        /// Sector 2 whole minute part.
        /// </summary>
        public byte sector2TimeInMinutes;

        /// <summary>
        /// Delta to the car in front in ms.
        /// </summary>
        public ushort deltaToCarInFrontInMS;

        /// <summary>
        /// Delta to the race leader in ms.
        /// </summary>
        public ushort deltaToRaceLeaderInMS;

        /// <summary>
        /// Distance the vehicle has traveled around the current lap in meters.
        /// </summary>
        public float lapDistanceInMeters;

        /// <summary>
        /// Total distance traveled in the session in meters.
        /// </summary>
        public float totalDistanceInMeters;

        /// <summary>
        /// Delta in seconds for the safety car.
        /// </summary>
        public float safetyCarDelta;

        /// <summary>
        /// Race position of the car.
        /// </summary>
        public byte carPosition;

        /// <summary>
        /// Current lap number.
        /// </summary>
        public byte currentLapNum;

        /// <summary>
        /// Pit status of the car - 0 = none, 1 = pitting, 2 = in pit area.
        /// </summary>
        public byte pitStatus;

        /// <summary>
        /// Number of pit stops taken in this race.
        /// </summary>
        public byte numPitStops;

        /// <summary>
        /// Current sector - 0 = sector 1, 1 = sector 2, 2 = sector 3.
        /// </summary>
        public byte sector;

        /// <summary>
        /// Indicates if the current lap is invalid - 0 = valid, 1 = invalid.
        /// </summary>
        public byte currentLapInvalid;

        /// <summary>
        /// Accumulated time penalties in seconds to be added.
        /// </summary>
        public byte penalties;

        /// <summary>
        /// Accumulated number of warnings issued.
        /// </summary>
        public byte totalWarnings;

        /// <summary>
        /// Accumulated number of corner cutting warnings issued.
        /// </summary>
        public byte cornerCuttingWarnings;

        /// <summary>
        /// Number of unserved drive-through penalties left to serve.
        /// </summary>
        public byte numUnservedDriveThroughPens;

        /// <summary>
        /// Number of unserved stop-go penalties left to serve.
        /// </summary>
        public byte numUnservedStopGoPens;

        /// <summary>
        /// Grid position the vehicle started the race in.
        /// </summary>
        public byte gridPosition;

        /// <summary>
        /// Status of the driver - 0 = in garage, 1 = flying lap, 2 = in lap, 3 = out lap, 4 = on track.
        /// </summary>
        public byte driverStatus;

        /// <summary>
        /// Result status - 0 = invalid, 1 = inactive, 2 = active, 3 = finished, 4 = did not finish, 
        /// 5 = disqualified, 6 = not classified, 7 = retired.
        /// </summary>
        public byte resultStatus;

        /// <summary>
        /// Pit lane timing - 0 = inactive, 1 = active.
        /// </summary>
        public byte pitLaneTimerActive;

        /// <summary>
        /// If the pit lane timer is active, represents the current time spent in the pit lane in milliseconds.
        /// </summary>
        public ushort pitLaneTimeInLaneInMS;

        /// <summary>
        /// Time of the actual pit stop in milliseconds.
        /// </summary>
        public ushort pitStopTimerInMS;

        /// <summary>
        /// Indicates whether the car should serve a penalty at this pit stop - 0 = no, 1 = yes.
        /// </summary>
        public byte pitStopShouldServePen;
    };



    /// <summary>
    /// Lap Data Packet
    /// The lap data packet gives details of all the cars in the session.
    /// Frequency: Rate as specified in menus
    /// Size: 972 bytes
    /// Version: 1    
    /// </summary>
    public class PacketLapData : Packet
    {
        public PacketLapData(byte[] data) : base(data)
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
            lapData = new LapData[MAX_CARS_ON_TRACK];
            for (int i = 0; i < MAX_CARS_ON_TRACK; i++)
            {
                LapData temp = new LapData();
                temp.lastLapTimeInMS = BitConverter.ToUInt32(data, byteIndex);            // Last lap time in milliseconds
                byteIndex += 4;
                temp.currentLapTimeInMS = BitConverter.ToUInt32(data, byteIndex);     // Current time around the lap in milliseconds
                byteIndex += 4;
                temp.sector1TimeInMS = BitConverter.ToUInt16(data, byteIndex);           // Sector 1 time in milliseconds
                byteIndex += 2;
                temp.sector1TimeInMinutes = data[byteIndex];
                byteIndex++;
                temp.sector2TimeInMS = BitConverter.ToUInt16(data, byteIndex);           // Sector 2 time in milliseconds
                byteIndex += 2;
                temp.sector2TimeInMinutes = data[byteIndex];
                byteIndex++;
                temp.deltaToCarInFrontInMS = BitConverter.ToUInt16(data, byteIndex);           // Sector 1 time in milliseconds
                byteIndex += 2;
                temp.deltaToRaceLeaderInMS = BitConverter.ToUInt16(data, byteIndex);           // Sector 1 time in milliseconds
                byteIndex += 2;
                temp.lapDistanceInMeters = BitConverter.ToSingle(data, byteIndex);         // Distance vehicle is around current lap in metres – could
                byteIndex += 4;                                             // be negative if line hasn’t been crossed yet
                temp.totalDistanceInMeters = BitConverter.ToSingle(data, byteIndex);       // Total distance travelled in session in metres – could
                byteIndex += 4;                                             // be negative if line hasn’t been crossed yet
                temp.safetyCarDelta = BitConverter.ToSingle(data, byteIndex);            // Delta in seconds for safety car
                byteIndex += 4;
                temp.carPosition = data[byteIndex];             // Car race position
                byteIndex++;
                temp.currentLapNum = data[byteIndex];       // Current lap number
                byteIndex++;
                temp.pitStatus = data[byteIndex];               // 0 = none, 1 = pitting, 2 = in pit area
                byteIndex++;
                temp.numPitStops = data[byteIndex];                 // Number of pit stops taken in this race
                byteIndex++;
                temp.sector = data[byteIndex];                  // 0 = sector1, 1 = sector2, 2 = sector3
                byteIndex++;
                temp.currentLapInvalid = data[byteIndex];       // Current lap invalid - 0 = valid, 1 = invalid
                byteIndex++;
                temp.penalties = data[byteIndex];               // Accumulated time penalties in seconds to be added
                byteIndex++;
                temp.totalWarnings = data[byteIndex];                  // Accumulated number of warnings issued
                byteIndex++;
                temp.cornerCuttingWarnings = data[byteIndex];                  // Accumulated number of corner cutting warnings issued
                byteIndex++;
                temp.numUnservedDriveThroughPens = data[byteIndex];  // Num drive through pens left to serve
                byteIndex++;
                temp.numUnservedStopGoPens = data[byteIndex];        // Num stop go pens left to serve
                byteIndex++;
                temp.gridPosition = data[byteIndex];            // Grid position the vehicle started the race in
                byteIndex++;
                temp.driverStatus = data[byteIndex];             // Status of driver - 0 = in garage, 1 = flying lap
                byteIndex++;                                                // 2 = in lap, 3 = out lap, 4 = on track
                temp.resultStatus = data[byteIndex];              // Result status - 0 = invalid, 1 = inactive, 2 = active
                byteIndex++;                                                  // 3 = finished, 4 = didnotfinish, 5 = disqualified
                                                                              // 6 = not classified, 7 = retired
                temp.pitLaneTimerActive = data[byteIndex];          // Pit lane timing, 0 = inactive, 1 = active
                byteIndex++;
                temp.pitLaneTimeInLaneInMS = BitConverter.ToUInt16(data, byteIndex);      // If active, the current time spent in the pit lane in ms
                byteIndex += 2;
                temp.pitStopTimerInMS = BitConverter.ToUInt16(data, byteIndex);           // Time of the actual pit stop in ms
                byteIndex += 2;
                temp.pitStopShouldServePen = data[byteIndex];       // Whether the car should serve a penalty at this stop
                byteIndex++;
                lapData[i] = temp;
            }
            //Parse time trail data here
            timeTrialPBCarIdx = data[byteIndex];  // Index of Personal Best car in time trial (255 if invalid)
            byteIndex++;
            timeTrialRivalCarIdx = data[byteIndex];   // Index of Rival car in time trial (255 if invalid)
            byteIndex++;
        }

        public LapData[] lapData;// [22];         // Lap data for all cars on track

        public byte timeTrialPBCarIdx;  // Index of Personal Best car in time trial (255 if invalid)
        public byte timeTrialRivalCarIdx;   // Index of Rival car in time trial (255 if invalid)
    }

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketLapDataEventArgs : EventArgs
    {
        public PacketLapData Packet { get; set; }

        public PacketLapDataEventArgs(PacketLapData packet)
        {
            Packet = packet;
        }
    }

};

