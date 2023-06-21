using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{


    /// <summary>
    /// Represents a marshal zone on the track.
    /// </summary>
    public class MarshalZone
    {
        /// <summary>
        /// Fraction (0..1) of the way through the lap the marshal zone starts.
        /// </summary>
        public float zoneStart;
        /// <summary>
        /// Marshal zone flag: -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red.
        /// </summary>
        public sbyte zoneFlag;
    }

    /// <summary>
    /// Weather forecast sample.
    /// </summary>
    public class WeatherForecastSample
    {
        /// <summary>
        /// Session type. 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P, 5 = Q1,
        /// 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ, 10 = R, 11 = R2, 12 = R3, 13 = Time Trial.
        /// </summary>
        public byte sessionType;

        /// <summary>
        /// Time offset in minutes.
        /// </summary>
        public byte timeOffset;

        /// <summary>
        /// Weather condition. 0 = clear, 1 = light cloud, 2 = overcast,
        /// 3 = light rain, 4 = heavy rain, 5 = storm.
        /// </summary>
        public byte weather;

        /// <summary>
        /// Track temperature in degrees Celsius.
        /// </summary>
        public sbyte trackTemperature;

        /// <summary>
        /// Track temperature change. 0 = up, 1 = down, 2 = no change.
        /// </summary>
        public sbyte trackTemperatureChange;

        /// <summary>
        /// Air temperature in degrees Celsius.
        /// </summary>
        public sbyte airTemperature;

        /// <summary>
        /// Air temperature change. 0 = up, 1 = down, 2 = no change.
        /// </summary>
        public sbyte airTemperatureChange;

        /// <summary>
        /// Rain percentage (0-100).
        /// </summary>
        public byte rainPercentage;
    }


    /// <summary>
    /// Session Packet
    /// The session packet includes details about the current session in progress.
    /// Frequency: 2 per second
    /// Size: 632 bytes
    /// Version: 1    
    /// </summary>
    public class PacketSessionData : Packet
    {
        public PacketSessionData(byte[] data) : base(data)
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

            weather = data[byteIndex];                // Weather - 0 = clear, 1 = light cloud, 2 = overcast
            byteIndex++;                                        // 3 = light rain, 4 = heavy rain, 5 = storm
            trackTemperature = Convert.ToSByte(data[byteIndex]);        // Track temp. in degrees celsius
            byteIndex++;
            airTemperature = Convert.ToSByte(data[byteIndex]);          // Air temp. in degrees celsius
            byteIndex++;
            totalLaps = data[byteIndex];              // Total number of laps in this race
            byteIndex++;
            trackLength = BitConverter.ToUInt16(data, byteIndex);               // Track length in metres
            byteIndex += 2;
            sessionType = data[byteIndex];            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
            byteIndex++;                                     // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
                                                             // 10 = R, 11 = R2, 12 = R3, 13 = Time Trial
            trackId = Convert.ToSByte(data[byteIndex]);                 // -1 for unknown, see appendix
            byteIndex++;
            formula = data[byteIndex++];                    // Formula, 0 = F1 Modern, 1 = F1 Classic, 2 = F2,
                                                            // 3 = F1 Generic, 4 = Beta, 5 = Supercars
                                                            // 6 = Esports, 7 = F2 2021
            sessionTimeLeft = BitConverter.ToUInt16(data, byteIndex);       // Time left in session in seconds
            byteIndex += 2;
            sessionDuration = BitConverter.ToUInt16(data, byteIndex);       // Session duration in seconds
            byteIndex += 2;
            pitSpeedLimit = data[byteIndex];          // Pit speed limit in kilometres per hour
            byteIndex++;
            gamePaused = data[byteIndex];                // Whether the game is paused – network game only
            byteIndex++;
            isSpectating = data[byteIndex];           // Whether the player is spectating
            byteIndex++;
            spectatorCarIndex = data[byteIndex];      // Index of the car being spectated
            byteIndex++;
            sliProNativeSupport = data[byteIndex];    // SLI Pro support, 0 = inactive, 1 = active
            byteIndex++;
            numMarshalZones = data[byteIndex];            // Number of marshal zones to follow
            byteIndex++;

            marshalZones = new MarshalZone[21]; // List of marshal zones – max 21
            for (int i = 0; i < marshalZones.Length; i++)
            {
                MarshalZone temp = new MarshalZone();
                temp.zoneStart = BitConverter.ToSingle(data, byteIndex);   // Fraction (0..1) of way through the lap the marshal zone starts
                byteIndex += 4;
                temp.zoneFlag = Convert.ToSByte(data[byteIndex]);    // -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red
                byteIndex++;
                marshalZones[i] = temp;
            }
            safetyCarStatus = data[byteIndex];           // 0 = no safety car, 1 = full
            byteIndex++;                                            // 2 = virtual, 3 = formation lap
            networkGame = data[byteIndex];               // 0 = offline, 1 = online
            byteIndex++;
            numWeatherForecastSamples = data[byteIndex]; // Number of weather samples to follow
            byteIndex++;
            weatherForecastSamples = new WeatherForecastSample[56];// [56];   // Array of weather forecast samples
            for (int i = 0; i < weatherForecastSamples.Length; i++)
            {
                WeatherForecastSample temp = new WeatherForecastSample();
                temp.sessionType = data[byteIndex];              // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P, 5 = Q1
                byteIndex++;                                                 // 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ, 10 = R, 11 = R2
                                                                             // 12 = R3, 13 = Time Trial
                temp.timeOffset = data[byteIndex];               // Time in minutes the forecast is for
                byteIndex++;
                temp.weather = data[byteIndex];                  // Weather - 0 = clear, 1 = light cloud, 2 = overcast
                byteIndex++;                                       // 3 = light rain, 4 = heavy rain, 5 = storm
                temp.trackTemperature = Convert.ToSByte(data[byteIndex]);         // Track temp. in degrees Celsius
                byteIndex++;
                temp.trackTemperatureChange = Convert.ToSByte(data[byteIndex]);   // Track temp. change – 0 = up, 1 = down, 2 = no change
                byteIndex++;
                temp.airTemperature = Convert.ToSByte(data[byteIndex]);           // Air temp. in degrees celsius
                byteIndex++;
                temp.airTemperatureChange = Convert.ToSByte(data[byteIndex]);     // Air temp. change – 0 = up, 1 = down, 2 = no change
                byteIndex++;
                temp.rainPercentage = data[byteIndex];           // Rain percentage (0-100)
                byteIndex++;
                weatherForecastSamples[i] = temp;
            }
            forecastAccuracy = data[byteIndex];          // 0 = Perfect, 1 = Approximate
            byteIndex++;
            aiDifficulty = data[byteIndex];              // AI Difficulty rating – 0-110
            byteIndex++;

            seasonLinkIdentifier = BitConverter.ToUInt32(data, byteIndex);      // Identifier for season - persists across saves
            byteIndex += 4;
            weekendLinkIdentifier = BitConverter.ToUInt32(data, byteIndex);     // Identifier for weekend - persists across saves
            byteIndex += 4;
            sessionLinkIdentifier = BitConverter.ToUInt32(data, byteIndex);     // Identifier for session - persists across saves
            byteIndex += 4;
            pitStopWindowIdealLap = data[byteIndex];     // Ideal lap to pit on for current strategy (player)
            byteIndex++;
            pitStopWindowLatestLap = data[byteIndex];    // Latest lap to pit on for current strategy (player)
            byteIndex++;
            pitStopRejoinPosition = data[byteIndex];     // Predicted position to rejoin at (player)
            byteIndex++;
            steeringAssist = data[byteIndex];            // 0 = off, 1 = on
            byteIndex++;
            brakingAssist = data[byteIndex];             // 0 = off, 1 = low, 2 = medium, 3 = high
            byteIndex++;
            gearboxAssist = data[byteIndex];             // 1 = manual, 2 = manual & suggested gear, 3 = auto
            byteIndex++;
            pitAssist = data[byteIndex];                 // 0 = off, 1 = on
            byteIndex++;
            pitReleaseAssist = data[byteIndex];          // 0 = off, 1 = on
            byteIndex++;
            ERSAssist = data[byteIndex];                 // 0 = off, 1 = on
            byteIndex++;
            DRSAssist = data[byteIndex];                 // 0 = off, 1 = on
            byteIndex++;
            dynamicRacingLine = data[byteIndex];         // 0 = off, 1 = corners only, 2 = full
            byteIndex++;
            dynamicRacingLineType = data[byteIndex];     // 0 = 2D, 1 = 3D
            byteIndex++;
            gameMode = data[byteIndex];                  // Game mode id - see appendix
            byteIndex++;
            ruleSet = data[byteIndex];                   // Ruleset - see appendix
            byteIndex++;
            timeOfDay = BitConverter.ToUInt32(data, byteIndex);                 // Local time of day - minutes since midnight
            byteIndex += 4;
            sessionLength = data[byteIndex];             // 0 = None, 2 = Very Short, 3 = Short, 4 = Medium
            byteIndex++;                                           // 5 = Medium Long, 6 = Long, 7 = Full

            speedUnitsLeadPlayer = data[byteIndex];             // 0 = MPH, 1 = KPH
            byteIndex++;
            temperatureUnitsLeadPlayer = data[byteIndex];       // 0 = Celsius, 1 = Fahrenheit
            byteIndex++;
            speedUnitsSecondaryPlayer = data[byteIndex];        // 0 = MPH, 1 = KPH
            byteIndex++;
            temperatureUnitsSecondaryPlayer = data[byteIndex];  // 0 = Celsius, 1 = Fahrenheit
            byteIndex++;
            numSafetyCarPeriods = data[byteIndex];              // Number of safety cars called during session
            byteIndex++;
            numVirtualSafetyCarPeriods = data[byteIndex];       // Number of virtual safety cars called
            byteIndex++;
            numRedFlagPeriods = data[byteIndex];                // Number of red flags called during session
        }


        /// <summary>
        /// Weather - 0 = clear, 1 = light cloud, 2 = overcast
        ///           3 = light rain, 4 = heavy rain, 5 = storm
        /// </summary>
        public byte weather;

        /// <summary>
        /// Track temp. in degrees Celsius
        /// </summary>
        public sbyte trackTemperature;

        /// <summary>
        /// Air temp. in degrees Celsius
        /// </summary>
        public sbyte airTemperature;

        /// <summary>
        /// Total number of laps in this race
        /// </summary>
        public byte totalLaps;

        /// <summary>
        /// Track length in metres
        /// </summary>
        public ushort trackLength;

        /// <summary>
        /// 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
        /// 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
        /// 10 = R, 11 = R2, 12 = R3, 13 = Time Trial
        /// </summary>
        public byte sessionType;

        /// <summary>
        /// -1 for unknown, see appendix
        /// </summary>
        public sbyte trackId;

        /// <summary>
        /// Formula, 0 = F1 Modern, 1 = F1 Classic, 2 = F2,
        /// 3 = F1 Generic, 4 = Beta, 5 = Supercars
        /// 6 = Esports, 7 = F2 2021
        /// </summary>
        public byte formula;

        /// <summary>
        /// Time left in session in seconds
        /// </summary>
        public ushort sessionTimeLeft;

        /// <summary>
        /// Session duration in seconds
        /// </summary>
        public ushort sessionDuration;

        /// <summary>
        /// Pit speed limit in kilometres per hour
        /// </summary>
        public byte pitSpeedLimit;

        /// <summary>
        /// Whether the game is paused – network game only
        /// </summary>
        public byte gamePaused;

        /// <summary>
        /// Whether the player is spectating
        /// </summary>
        public byte isSpectating;

        /// <summary>
        /// Index of the car being spectated
        /// </summary>
        public byte spectatorCarIndex;

        /// <summary>
        /// SLI Pro support, 0 = inactive, 1 = active
        /// </summary>
        public byte sliProNativeSupport;

        /// <summary>
        /// Number of marshal zones to follow
        /// </summary>
        public byte numMarshalZones;

        /// <summary>
        /// List of marshal zones – max 21
        /// </summary>
        public MarshalZone[] marshalZones;

        /// <summary>
        /// 0 = no safety car, 1 = full, 2 = virtual, 3 = formation lap
        /// </summary>
        public byte safetyCarStatus;

        /// <summary>
        /// 0 = offline, 1 = online
        /// </summary>
        public byte networkGame;

        /// <summary>
        /// Number of weather samples to follow
        /// </summary>
        public byte numWeatherForecastSamples;

        /// <summary>
        /// Array of weather forecast samples
        /// </summary>
        public WeatherForecastSample[] weatherForecastSamples;

        /// <summary>
        /// 0 = Perfect, 1 = Approximate
        /// </summary>
        public byte forecastAccuracy;

        /// <summary>
        /// AI Difficulty rating – 0-110
        /// </summary>
        public byte aiDifficulty;

        /// <summary>
        /// Identifier for season - persists across saves
        /// </summary>
        public uint seasonLinkIdentifier;

        /// <summary>
        /// Identifier for weekend - persists across saves
        /// </summary>
        public uint weekendLinkIdentifier;

        /// <summary>
        /// Identifier for session - persists across saves
        /// </summary>
        public uint sessionLinkIdentifier;

        /// <summary>
        /// Ideal lap to pit on for current strategy (player)
        /// </summary>
        public byte pitStopWindowIdealLap;

        /// <summary>
        /// Latest lap to pit on for current strategy (player)
        /// </summary>
        public byte pitStopWindowLatestLap;

        /// <summary>
        /// Predicted position to rejoin at (player)
        /// </summary>
        public byte pitStopRejoinPosition;

        /// <summary>
        /// Steering assist status: 0 = off, 1 = on
        /// </summary>
        public byte steeringAssist;

        /// <summary>
        /// Braking assist level: 0 = off, 1 = low, 2 = medium, 3 = high
        /// </summary>
        public byte brakingAssist;

        /// <summary>
        /// Gearbox assist type: 1 = manual, 2 = manual & suggested gear, 3 = auto
        /// </summary>
        public byte gearboxAssist;

        /// <summary>
        /// Pit assist status: 0 = off, 1 = on
        /// </summary>
        public byte pitAssist;

        /// <summary>
        /// Pit release assist status: 0 = off, 1 = on
        /// </summary>
        public byte pitReleaseAssist;

        /// <summary>
        /// ERS assist status: 0 = off, 1 = on
        /// </summary>
        public byte ERSAssist;

        /// <summary>
        /// DRS assist status: 0 = off, 1 = on
        /// </summary>
        public byte DRSAssist;

        /// <summary>
        /// Dynamic racing line setting: 0 = off, 1 = corners only, 2 = full
        /// </summary>
        public byte dynamicRacingLine;

        /// <summary>
        /// Dynamic racing line type: 0 = 2D, 1 = 3D
        /// </summary>
        public byte dynamicRacingLineType;

        /// <summary>
        /// Game mode id - see appendix
        /// </summary>
        public byte gameMode;

        /// <summary>
        /// Ruleset - see appendix
        /// </summary>
        public byte ruleSet;

        /// <summary>
        /// Local time of day - minutes since midnight
        /// </summary>
        public uint timeOfDay;

        /// <summary>
        /// Session length type: 0 = None, 2 = Very Short, 3 = Short, 4 = Medium
        /// 5 = Medium Long, 6 = Long, 7 = Full
        /// </summary>
        public byte sessionLength;

        /// <summary>
        /// Speed units used by the lead player where 0 indicates miles per hour (MPH) and 1 indicates kilometers per hour (KPH).
        /// </summary>
        public byte speedUnitsLeadPlayer;

        /// <summary>
        /// Temperature units used by the lead player where 0 indicates Celsius and 1 indicates Fahrenheit.
        /// </summary>
        public byte temperatureUnitsLeadPlayer;

        /// <summary>
        /// Speed units used by the secondary player where 0 indicates miles per hour (MPH) and 1 indicates kilometers per hour (KPH).
        /// </summary>
        public byte speedUnitsSecondaryPlayer;

        /// <summary>
        /// Temperature units used by the secondary player where 0 indicates Celsius and 1 indicates Fahrenheit.
        /// </summary>
        public byte temperatureUnitsSecondaryPlayer;

        /// <summary>
        /// Represents the number of safety cars that were called during the session.
        /// </summary>
        public byte numSafetyCarPeriods;

        /// <summary>
        /// Represents the number of virtual safety cars that were called during the session.
        /// </summary>
        public byte numVirtualSafetyCarPeriods;

        /// <summary>
        /// Represents the number of red flags that were called during the session.
        /// </summary>
        public byte numRedFlagPeriods;


    };

    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketSessionDataEventArgs : EventArgs
    {
        public PacketSessionData Packet { get; set; }

        public PacketSessionDataEventArgs(PacketSessionData packet)
        {
            Packet = packet;
        }
    }
}
