using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
    ////Event String Codes
    //public enum EventCode
    //{
    //    ////Event	Code	Description
    //    SSTA,   //Session Started	“SSTA”	Sent when the session starts
    //    SEND,   //Session Ended	“SEND”	Sent when the session ends
    //    FTLP,   //Fastest Lap	“FTLP”	When a driver achieves the fastest lap
    //    RTMT,   //Retirement	“RTMT”	When a driver retires
    //    DRSE,   //DRS enabled	“DRSE”	Race control have enabled DRS
    //    DRSD,   //DRS disabled	“DRSD”	Race control have disabled DRS
    //    TMPT,   //Team mate in pits	“TMPT”	Your team mate has entered the pits
    //    CHQF,   //Chequered flag	“CHQF”	The chequered flag has been waved
    //    RCWN,   //Race Winner	“RCWN”	The race winner is announced
    //    PENA,   //Penalty Issued	“PENA”	A penalty has been issued – details in event
    //    SPTP,   //Speed Trap Triggered	“SPTP”	Speed trap has been triggered by fastest speed
    //    STLG,   //Start lights	“STLG”	Start lights – number shown
    //    LGOT,   //Lights out	“LGOT”	Lights out
    //    DTSV,   //Drive through served	“DTSV”	Drive through penalty served
    //    SGSV,   //Stop go served	“SGSV”	Stop go penalty served
    //    FLBK,   //Flashback	“FLBK”	Flashback activated
    //    BUTN    //Button status	“BUTN”	Button status changed
    //};

    //public enum EventName {
    //    SessionStart,
    //    SessionEnd,
    //    FastestLap,
    //    Retirement,
    //    DRSEnabled,
    //    DRSDisabled,
    //    TeammateInPits,
    //    ChequeredFlag,
    //    RaceWinner,
    //    Penalty,
    //    SpeedTrapTriggered,
    //    StartLights,
    //    LightsOut,
    //    DriveThroughPenaltyServed,
    //    StopGoPenaltyServed,
    //    Flashback,
    //    Button    
    //}

    //    //Event Packet
    //    //This packet gives details of events that happen during the course of a session.
    //    //Frequency: When the event occurs
    //    //Size: 40 bytes
    //    //Version: 1

    public class PacketEventData : Packet
    {
        /// <summary>
        /// Event type as parsed from the packet.
        /// </summary>
        private byte[] eventStringCode;

        /// <summary>
        /// Event type accessible to the public.
        /// </summary>
        public EventCode eventType;

        /// <summary>
        /// Event name for easier readability.
        /// </summary>
        public EventName eventName;

        /// <summary>
        /// Event details - should be interpreted differently for each type.
        /// </summary>
        public EventDataDetails eventDetails;
        public PacketEventData(byte[] data) : base(data)
        {
            try
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

                //Parse the rest of the packet here
                int byteIndex = HEADER_BYTE_SIZE;

                //Get the event code
                eventStringCode = new byte[4];

                eventStringCode[0] = data[byteIndex];
                byteIndex++;
                eventStringCode[1] = data[byteIndex];
                byteIndex++;
                eventStringCode[2] = data[byteIndex];
                byteIndex++;
                eventStringCode[3] = data[byteIndex];
                byteIndex++;
                //Convert the byte array to an enum
                eventType = ByteArrayToEventCode(eventStringCode);

                //Switch statement to parse the rest of the packet according to event type
                switch (eventType)
                {
                    case EventCode.SSTA:
                        SessionStartEvent ssta = new SessionStartEvent();
                        eventName = EventName.SessionStart;
                        eventDetails = ssta;
                        break;
                    case EventCode.SEND:
                        SessionEndEvent send = new SessionEndEvent();
                        eventName = EventName.SessionEnd;
                        eventDetails = send;
                        break;
                    case EventCode.FTLP:
                        FastestLapEvent ftlp = new FastestLapEvent();
                        ftlp.vehicleIndex = data[byteIndex];
                        byteIndex++;
                        ftlp.lapTime = BitConverter.ToSingle(data, byteIndex); ;                 // Amount of throttle applied (0.0 to 1.0)
                        eventName = EventName.FastestLap;                                                                         //byteIndex += 4;//No need to increment because we are at the end of the packet
                        eventDetails = ftlp;
                        break;
                    case EventCode.RTMT:
                        RetirementEvent rtmt = new RetirementEvent();
                        rtmt.vehicleIndex = data[byteIndex];
                        eventDetails = rtmt;
                        eventName = EventName.Retirement;
                        break;
                    case EventCode.DRSE:
                        DRSEnabledEvent drse = new DRSEnabledEvent();
                        eventDetails = drse;
                        eventName = EventName.DRSEnabled;
                        break;
                    case EventCode.DRSD:
                        DRSDisabledEvent drsd = new DRSDisabledEvent();
                        eventDetails = drsd;
                        eventName = EventName.DRSDisabled;
                        break;
                    case EventCode.TMPT:
                        TeamMateInPitsEvent tmpt = new TeamMateInPitsEvent();
                        tmpt.vehicleIndex = data[byteIndex];
                        eventDetails = tmpt;
                        eventName = EventName.TeammateInPits;
                        break;
                    case EventCode.CHQF:
                        ChequeredFlagEvent chqf = new ChequeredFlagEvent();
                        eventDetails = chqf;
                        eventName = EventName.ChequeredFlag;
                        break;
                    case EventCode.RCWN:
                        RaceWinnerEvent rcwn = new RaceWinnerEvent();
                        rcwn.vehicleIndex = data[byteIndex];
                        eventDetails = rcwn;
                        eventName = EventName.RaceWinner;
                        break;
                    case EventCode.PENA:
                        PenaltyEvent pena = new PenaltyEvent();
                        pena.penaltyType = data[byteIndex];    //Penalty Type, see enum for definitons
                        byteIndex++;
                        pena.infringementType = data[byteIndex];   //Infringement Type, see enum for definitions
                        byteIndex++;
                        pena.vehicleIndex = data[byteIndex];   //Index of the car receiving the penalty
                        byteIndex++;
                        pena.otherVehicleIndex = data[byteIndex];  //Index of the other vehicle involved in the incident
                        byteIndex++;
                        pena.time = data[byteIndex];   //Time gained, or time spent doing actions in seconds
                        byteIndex++;
                        pena.lapNum = data[byteIndex]; //Lap the penalty occured
                        byteIndex++;
                        pena.placesGained = data[byteIndex];   //Number of places gained.
                        eventDetails = pena;
                        eventName = EventName.Penalty;
                        break;
                    case EventCode.SPTP:
                        SpeedTrapEvent sptp = new SpeedTrapEvent();
                        sptp.vehicleIndex = data[byteIndex];   //Index of vehicle going through speed trap
                        byteIndex++;
                        sptp.speed = data[byteIndex]; //Top speed logged in the speed trap (kph)
                        byteIndex++;
                        sptp.isOverallFastestInSession = data[byteIndex];  //1 if fastest, otherwise not
                        byteIndex++;
                        sptp.isDriverFastestInSession = data[byteIndex];   //Fastest speed for driver in session. 1 = true, otherwise false
                        byteIndex++;
                        sptp.fastestVehicleIndexInSession = data[byteIndex];   //Index of the fastest vehicle in the session
                        eventDetails = sptp;
                        eventName = EventName.SpeedTrapTriggered;
                        break;
                    case EventCode.STLG:
                        StartLightsEvent stlg = new StartLightsEvent();
                        stlg.numLights = data[byteIndex];
                        eventDetails = stlg;
                        eventName = EventName.StartLights;
                        break;
                    case EventCode.LGOT:
                        LightsOutEvent lgot = new LightsOutEvent();
                        eventDetails = lgot;
                        eventName = EventName.LightsOut;
                        break;
                    case EventCode.DTSV:
                        DriveThroughPenaltyServedEvent dtsv = new DriveThroughPenaltyServedEvent();
                        dtsv.vehicleIndex = data[byteIndex];
                        eventDetails = dtsv;
                        eventName = EventName.DriveThroughPenaltyServed;
                        break;
                    case EventCode.SGSV:
                        StopGoPenaltyServedEvent sgsv = new StopGoPenaltyServedEvent();
                        sgsv.vehicleIndex = data[byteIndex];
                        eventDetails = sgsv;
                        eventName = EventName.StopGoPenaltyServed;
                        break;
                    case EventCode.FLBK:
                        FlashbackEvent flbk = new FlashbackEvent();
                        flbk.flashbackFrameIdentifier = BitConverter.ToUInt32(data, byteIndex);
                        byteIndex += 4;
                        flbk.flashbackSessionTime = BitConverter.ToSingle(data, byteIndex);
                        eventDetails = flbk;
                        eventName = EventName.Flashback;
                        break;
                    case EventCode.BUTN:
                        ButtonsEvent butn = new ButtonsEvent();
                        butn.buttonStatus = BitConverter.ToUInt32(data, byteIndex);
                        eventDetails = butn;
                        eventName = EventName.Button;
                        break;
                    case EventCode.OVTK:
                        OvertakeEvent ovtk = new OvertakeEvent();
                        ovtk.overtakingVehicleIndex = data[byteIndex];
                        byteIndex++;
                        ovtk.beingOvertakenVehicleIndex = data[byteIndex];
                        byteIndex++;
                        break;
                    default:
                        throw new Exception("Event Packet Type Not Defined: " + eventType);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        private EventCode ByteArrayToEventCode(byte[] array)
        {
            if (array.Length != 4)
                throw new ArgumentException("Array must have a length of 4.", nameof(array));

            string eventCodeStr = Encoding.ASCII.GetString(array);

            if (Enum.TryParse(eventCodeStr, out EventCode eventCode))
                return eventCode;

            throw new ArgumentException($"No EventCode exists with the value '{eventCodeStr}'.");
        }
    }

    public abstract class EventDataDetails { }
    /// <summary>
    /// Event details for the SessionStartEvent.
    /// </summary>
    public class SessionStartEvent : EventDataDetails { }

    /// <summary>
    /// Event details for the SessionEndEvent.
    /// </summary>
    public class SessionEndEvent : EventDataDetails { }

    /// <summary>
    /// Event details for the FastestLapEvent.
    /// </summary>
    public class FastestLapEvent : EventDataDetails
    {
        /// <summary>
        /// Vehicle index of the car achieving the fastest lap.
        /// </summary>
        public byte vehicleIndex;

        /// <summary>
        /// Lap time in seconds.
        /// </summary>
        public float lapTime;
    }

    /// <summary>
    /// Event details for the RetirementEvent.
    /// </summary>
    public class RetirementEvent : EventDataDetails
    {
        /// <summary>
        /// Index of the retiring car.
        /// </summary>
        public byte vehicleIndex;
    }

    /// <summary>
    /// Event details for the DRSEnabledEvent.
    /// </summary>
    public class DRSEnabledEvent : EventDataDetails { }

    /// <summary>
    /// Event details for the DRSDisabledEvent.
    /// </summary>
    public class DRSDisabledEvent : EventDataDetails { }

    /// <summary>
    /// Event details for the TeamMateInPitsEvent.
    /// </summary>
    public class TeamMateInPitsEvent : EventDataDetails
    {
        /// <summary>
        /// Vehicle index of the teammate.
        /// </summary>
        public byte vehicleIndex;
    }

    /// <summary>
    /// Event details for the ChequeredFlagEvent.
    /// </summary>
    public class ChequeredFlagEvent : EventDataDetails { }

    /// <summary>
    /// Event details for the RaceWinnerEvent.
    /// </summary>
    public class RaceWinnerEvent : EventDataDetails
    {
        /// <summary>
        /// Index of the winning vehicle.
        /// </summary>
        public byte vehicleIndex;
    }

    /// <summary>
    /// Event details for the PenaltyEvent.
    /// </summary>
    public class PenaltyEvent : EventDataDetails
    {
        /// <summary>
        /// Penalty type.
        /// </summary>
        public byte penaltyType;

        /// <summary>
        /// Infringement type.
        /// </summary>
        public byte infringementType;

        /// <summary>
        /// Index of the car receiving the penalty.
        /// </summary>
        public byte vehicleIndex;

        /// <summary>
        /// Index of the other vehicle involved in the incident.
        /// </summary>
        public byte otherVehicleIndex;

        /// <summary>
        /// Time gained or time spent doing actions in seconds.
        /// </summary>
        public byte time;

        /// <summary>
        /// Lap the penalty occurred.
        /// </summary>
        public byte lapNum;

        /// <summary>
        /// Number of places gained.
        /// </summary>
        public byte placesGained;
    }
    /// <summary>
    /// Event details for the SpeedTrapEvent.
    /// </summary>
    public class SpeedTrapEvent : EventDataDetails
    {
        /// <summary>
        /// Index of the vehicle going through the speed trap.
        /// </summary>
        public byte vehicleIndex;

        /// <summary>
        /// Top speed logged in the speed trap (kph).
        /// </summary>
        public float speed;

        /// <summary>
        /// 1 if overall fastest in the session, otherwise 0.
        /// </summary>
        public byte isOverallFastestInSession;

        /// <summary>
        /// Fastest speed for the driver in the session.
        /// 1 if true, otherwise 0.
        /// </summary>
        public byte isDriverFastestInSession;

        /// <summary>
        /// Index of the fastest vehicle in the session.
        /// </summary>
        public byte fastestVehicleIndexInSession;
    }

    /// <summary>
    /// Event details for the StartLightsEvent.
    /// </summary>
    public class StartLightsEvent : EventDataDetails
    {
        /// <summary>
        /// Number of lights showing.
        /// </summary>
        public byte numLights;
    }

    /// <summary>
    /// Event details for the LightsOutEvent.
    /// </summary>
    public class LightsOutEvent : EventDataDetails { }

    /// <summary>
    /// Event details for the DriveThroughPenaltyServedEvent.
    /// </summary>
    public class DriveThroughPenaltyServedEvent : EventDataDetails
    {
        /// <summary>
        /// Index of the vehicle serving the drive-through penalty.
        /// </summary>
        public byte vehicleIndex;
    }

    /// <summary>
    /// Event details for the StopGoPenaltyServedEvent.
    /// </summary>
    public class StopGoPenaltyServedEvent : EventDataDetails
    {
        /// <summary>
        /// Index of the vehicle serving the penalty.
        /// </summary>
        public byte vehicleIndex;
    }

    /// <summary>
    /// Event details for the FlashbackEvent.
    /// </summary>
    public class FlashbackEvent : EventDataDetails
    {
        /// <summary>
        /// Frame ID that the game was flashed back to.
        /// </summary>
        public uint flashbackFrameIdentifier;

        /// <summary>
        /// Session time flashed back to.
        /// </summary>
        public float flashbackSessionTime;
    }

    /// <summary>
    /// Event details for the ButtonsEvent.
    /// </summary>
    public class ButtonsEvent : EventDataDetails
    {
        /// <summary>
        /// Bit flags specifying which buttons are being pressed.
        /// </summary>
        public uint buttonStatus;
    }

    /// <summary>
    /// Event details for an overtaking event.
    /// </summary>
    public class OvertakeEvent : EventDataDetails {
        /// <summary>
        /// Index of the vehicle doing the overtaking
        /// </summary>
        public byte overtakingVehicleIndex;

        /// <summary>
        /// Index of the vehicle being overtaken
        /// </summary>
        public byte beingOvertakenVehicleIndex;
    }


    //This class is used to wrap and pass the packet to subscribing classes when raising an event
    public class PacketEventDataEventArgs : EventArgs
    {
        public PacketEventData Packet { get; set; }

        public PacketEventDataEventArgs(PacketEventData packet)
        {
            Packet = packet;
        }
    }
}
