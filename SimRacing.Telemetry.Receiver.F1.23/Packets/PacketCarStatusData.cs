using Sim_Racing_UDP_Receiver.Games.F1_2022;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{



    /// <summary>
    /// Car status data for each vehicle in the session.
    /// </summary>
    public class CarStatusData
    {
        /// <summary>
        /// Traction control - 0 = off, 1 = medium, 2 = full.
        /// </summary>
        public byte tractionControl;

        /// <summary>
        /// Anti-lock brakes - 0 (off), 1 (on).
        /// </summary>
        public byte antiLockBrakes;

        /// <summary>
        /// Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max.
        /// </summary>
        public byte fuelMix;

        /// <summary>
        /// Front brake bias (percentage).
        /// </summary>
        public byte frontBrakeBias;

        /// <summary>
        /// Pit limiter status - 0 = off, 1 = on.
        /// </summary>
        public byte pitLimiterStatus;

        /// <summary>
        /// Current fuel mass.
        /// </summary>
        public float fuelInTank;

        /// <summary>
        /// Fuel capacity.
        /// </summary>
        public float fuelCapacity;

        /// <summary>
        /// Fuel remaining in terms of laps (value on MFD).
        /// </summary>
        public float fuelRemainingLaps;

        /// <summary>
        /// Cars max RPM, point of rev limiter.
        /// </summary>
        public ushort maxRPM;

        /// <summary>
        /// Cars idle RPM.
        /// </summary>
        public ushort idleRPM;

        /// <summary>
        /// Maximum number of gears.
        /// </summary>
        public byte maxGears;

        /// <summary>
        /// DRS allowed - 0 = not allowed, 1 = allowed.
        /// </summary>
        public byte drsAllowed;

        /// <summary>
        /// DRS activation distance - 0 = DRS not available,
        /// non-zero = DRS will be available in [X] metres.
        /// </summary>
        public ushort drsActivationDistance;

        /// <summary>
        /// Actual tyre compound.
        /// </summary>
        public byte actualTyreCompound;

        /// <summary>
        /// Visual tyre compound (can be different from actual compound).
        /// </summary>
        public byte visualTyreCompound;

        /// <summary>
        /// Age in laps of the current set of tyres.
        /// </summary>
        public byte tyresAgeLaps;

        /// <summary>
        /// Vehicle flags from the FIA - -1 = invalid/unknown,
        /// 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red.
        /// </summary>
        public sbyte vehicleFiaFlags;

        /// <summary>
        /// Engine power output of ICE (W)
        /// </summary>
        public float engingPowerICE;

        /// <summary>
        /// Engine power output of MGU-K (W)
        /// </summary>
        public float enginePowerMGUK;

        /// <summary>
        /// ERS energy store in Joules.
        /// </summary>
        public float ersStoreEnergy;

        /// <summary>
        /// ERS deployment mode - 0 = none, 1 = medium,
        /// 2 = hotlap, 3 = overtake.
        /// </summary>
        public byte ersDeployMode;

        /// <summary>
        /// ERS energy harvested this lap by MGU-K.
        /// </summary>
        public float ersHarvestedThisLapMGUK;

        /// <summary>
        /// ERS energy harvested this lap by MGU-H.
        /// </summary>
        public float ersHarvestedThisLapMGUH;

        /// <summary>
        /// ERS energy deployed this lap.
        /// </summary>
        public float ersDeployedThisLap;

        /// <summary>
        /// Whether the car is paused in a network game        
        /// </summary>
        public byte networkPaused;
    };

    /// <summary>
    /// Car Status Packet
    /// This packet details car statuses for all the cars in the race.
    /// Frequency: Rate as specified in menus
    /// Size: 1058 bytes
    /// Version: 1    
    /// </summary>
    public class PacketCarStatusData : Packet
    {
        public PacketCarStatusData(byte[] data) : base(data)
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
            carStatusData = new CarStatusData[MAX_CARS_ON_TRACK];
            int byteIndex = HEADER_BYTE_SIZE;
            for (int i = 0; i < MAX_CARS_ON_TRACK; i++)
            {
                CarStatusData temp = new CarStatusData();


                temp.tractionControl = data[byteIndex];           // Traction control - 0 = off, 1 = medium, 2 = full
                byteIndex++;
                temp.antiLockBrakes = data[byteIndex];            // 0 (off) - 1 (on)
                byteIndex++;
                temp.fuelMix = data[byteIndex];                   // Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
                byteIndex++;
                temp.frontBrakeBias = data[byteIndex];            // Front brake bias (percentage)
                byteIndex++;
                temp.pitLimiterStatus = data[byteIndex];          // Pit limiter status - 0 = off, 1 = on
                byteIndex++;
                temp.fuelInTank = BitConverter.ToSingle(data, byteIndex); ;               // Current fuel mass
                byteIndex += 4;
                temp.fuelCapacity = BitConverter.ToSingle(data, byteIndex); ;             // Fuel capacity
                byteIndex += 4;
                temp.fuelRemainingLaps = BitConverter.ToSingle(data, byteIndex); ;        // Fuel remaining in terms of laps (value on MFD)
                byteIndex += 4;
                temp.maxRPM = BitConverter.ToUInt16(data, byteIndex);                   // Cars max RPM, point of rev limiter
                byteIndex += 2;
                temp.idleRPM = BitConverter.ToUInt16(data, byteIndex);                  // Cars idle RPM
                byteIndex += 2;
                temp.maxGears = data[byteIndex];                  // Maximum number of gears
                byteIndex++;
                temp.drsAllowed = data[byteIndex];                // 0 = not allowed, 1 = allowed
                byteIndex++;
                temp.drsActivationDistance = BitConverter.ToUInt16(data, byteIndex);    // 0 = DRS not available, non-zero - DRS will be available
                byteIndex += 2;                                                             // in [X] metres
                temp.actualTyreCompound = data[byteIndex];     // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1
                byteIndex++;                                      // 7 = inter, 8 = wet
                                                                  // F1 Classic - 9 = dry, 10 = wet
                                                                  // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard
                                                                  // 15 = wet
                temp.visualTyreCompound = data[byteIndex];        // F1 visual (can be different from actual compound)
                byteIndex++;                                                // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
                                                                            // F1 Classic – same as above
                                                                            // F2 ‘19, 15 = wet, 19 – super soft, 20 = soft
                                                                            // 21 = medium , 22 = hard
                temp.tyresAgeLaps = data[byteIndex];              // Age in laps of the current set of tyres
                byteIndex++;
                temp.vehicleFiaFlags = Convert.ToSByte(data[byteIndex]);     // -1 = invalid/unknown, 0 = none, 1 = green
                byteIndex++;                 // 2 = blue, 3 = yellow, 4 = red
                temp.engingPowerICE = BitConverter.ToSingle(data, byteIndex); ;             // Fuel capacity
                byteIndex += 4;
               temp.enginePowerMGUK = BitConverter.ToSingle(data, byteIndex); ;             // Fuel capacity
                byteIndex += 4;
                temp.ersStoreEnergy = BitConverter.ToSingle(data, byteIndex); ;           // ERS energy store in Joules
                byteIndex += 4;
                temp.ersDeployMode = data[byteIndex];             // ERS deployment mode, 0 = none, 1 = medium
                byteIndex++;                 // 2 = hotlap, 3 = overtake
                temp.ersHarvestedThisLapMGUK = BitConverter.ToSingle(data, byteIndex); ;  // ERS energy harvested this lap by MGU-K
                byteIndex += 4;
                temp.ersHarvestedThisLapMGUH = BitConverter.ToSingle(data, byteIndex); ;  // ERS energy harvested this lap by MGU-H
                byteIndex += 4;
                temp.ersDeployedThisLap = BitConverter.ToSingle(data, byteIndex); ;       // ERS energy deployed this lap
                byteIndex += 4;
                temp.networkPaused = data[byteIndex];             // Whether the car is paused in a network game
                byteIndex++;
                carStatusData[i] = temp;
            }
        }
        public CarStatusData[] carStatusData;// [22];
    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketCarStatusDataEventArgs : EventArgs
    {
        public PacketCarStatusData Packet { get; set; }

        public PacketCarStatusDataEventArgs(PacketCarStatusData packet)
        {
            Packet = packet;
        }
    }
}
