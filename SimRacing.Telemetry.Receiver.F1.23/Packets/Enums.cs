using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRacing.Telemetry.Receiver.F1_23.Packets
{
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

    public enum PacketType
    {
        Motion = 0,
        Session = 1,
        LapData = 2,
        Event = 3,
        Participants = 4,
        CarSetups = 5,
        CarTelemetery = 6,
        CarStatus = 7,
        FinalClassification = 8,
        LobbyInfo = 9,
        CarDamage = 10,
        SessionHistory = 11,
        TyreSets = 12,
        MotionExtra = 13
    }

    public enum Tyre
    {
        RearLeft,
        RearRight,
        FrontLeft,
        FrontRight
    }

    public enum FaultStatus
    {
        Ok,
        Fault
    }

    public enum TractionControlStatus
    {
        Off,
        Medium,
        Full
    }

    public enum AntiLockBrakeStatus
    {
        Off,
        On
    }

    public enum FuelMixType
    {
        Lean,
        Standard,
        Rich,
        Max
    }

    public enum PitLimiterStatus
    {
        Off,
        On
    }

    public enum DRSStatus
    {
        Disabled,
        Enabled
    }

    public enum ActualTyreType
    {
        //This is the actual tyre compund used by the car.
        //The actual tyre compound is different than the tyre compound visually displayed on the car.

        //F1 Cars
        F1_C5 = 16,
        F1_C4 = 17,
        F1_C3 = 18,
        F1_C2 = 19,
        F1_C1 = 20,
        F1_C0 = 21,
        F1_Inter = 7,
        F1_Wet = 8,

        //F1 Classic Cars
        F1_Classic_Dry = 9,
        F1_Classic_Wet = 10,

        //F2 Cars
        F2_SuperSoft = 11,
        F2_Soft = 12,
        F2_Medium = 13,
        F2_Hard = 14,
        F2_Wet = 15
    }

    public enum VisualTyreType
    {
        //This is the tyre compound displayed on the car in game.
        //This is different than the tyre type actually used in game.

        //F1 and F1 Classic Cars
        F1_Soft = 16,
        F1_Medium = 17,
        F1_Hard = 18,
        F1_Inter = 7,
        F1_Wet = 8,

        //F2 Cars
        F2_Wet = 15,
        F2_SuperSoft = 19,
        F2_Soft = 20,
        F2_Medium = 21,
        F2_Hard = 22
    }

    ////Event String Codes
    public enum EventCode
    {
        ////Event	Code	Description
        SSTA,   //Session Started	“SSTA”	Sent when the session starts
        SEND,   //Session Ended	“SEND”	Sent when the session ends
        FTLP,   //Fastest Lap	“FTLP”	When a driver achieves the fastest lap
        RTMT,   //Retirement	“RTMT”	When a driver retires
        DRSE,   //DRS enabled	“DRSE”	Race control have enabled DRS
        DRSD,   //DRS disabled	“DRSD”	Race control have disabled DRS
        TMPT,   //Team mate in pits	“TMPT”	Your team mate has entered the pits
        CHQF,   //Chequered flag	“CHQF”	The chequered flag has been waved
        RCWN,   //Race Winner	“RCWN”	The race winner is announced
        PENA,   //Penalty Issued	“PENA”	A penalty has been issued – details in event
        SPTP,   //Speed Trap Triggered	“SPTP”	Speed trap has been triggered by fastest speed
        STLG,   //Start lights	“STLG”	Start lights – number shown
        LGOT,   //Lights out	“LGOT”	Lights out
        DTSV,   //Drive through served	“DTSV”	Drive through penalty served
        SGSV,   //Stop go served	“SGSV”	Stop go penalty served
        FLBK,   //Flashback	“FLBK”	Flashback activated
        BUTN,    //Button status	“BUTN”	Button status changed
        OVTK    //Overtake event
    };

    public enum EventName
    {
        SessionStart,
        SessionEnd,
        FastestLap,
        Retirement,
        DRSEnabled,
        DRSDisabled,
        TeammateInPits,
        ChequeredFlag,
        RaceWinner,
        Penalty,
        SpeedTrapTriggered,
        StartLights,
        LightsOut,
        DriveThroughPenaltyServed,
        StopGoPenaltyServed,
        Flashback,
        Button,
        Overtake
    }

    public enum Platform { 
        Steam = 1,
        Playstation = 3,
        Xbox = 4,
        Origin = 6,
        Unknown = 255
    }

    public enum ResultStatus
    {
        // Result status - 0 = invalid, 1 = inactive, 2 = active
        // 3 = finished, 4 = didnotfinish, 5 = disqualified
        // 6 = not classified, 7 = retired

        Invalid,
        Inactive,
        Active,
        Finished,
        DNF,
        DSQ,
        NotClassified,
        Retired
    }

    public enum PitStatus
    {
        None,
        Pitting,
        InPitArea
    }

    public enum Sector
    {
        Sector1,
        Sector2,
        Sector3
    }

    public enum LapStatus
    {
        Valid,
        Invalid
    }

    public enum DriverStatus
    {
        InGarage,
        FlyingLap,
        InLap,
        OutLap,
        OnTrack
    }

    public enum PitLaneTimerStatus
    {
        Inactive,
        Active
    }

    public enum HumanNPCStatus
    {
        Human,
        NPC
    }

    public enum ReadyStatus
    {
        NotReady,
        Ready,
        Spectating
    }

    public enum TeamId
    {
        Mercedes = 0,
        Ferrari = 1,
        RedBullRacing = 2,
        Williams = 3,
        AstonMartin = 4,
        Alpine = 5,
        AlphaTauri = 6,
        Haas = 7,
        McLaren = 8,
        AlfaRomeo = 9,
        Mercedes2020 = 85,
        Ferrari2020 = 86,
        RedBull2020 = 87,
        Williams2020 = 88,
        RacingPoint2020 = 89,
        Renault2020 = 90,
        AlphaTauri2020 = 91,
        Haas2020 = 92,
        McLaren2020 = 93,
        AlfaRomeo2020 = 94,
        AstonMartinDB11V12 = 95,
        AstonMartinVantageF1Edition = 96,
        AstonMartinVantageSafetyCar = 97,
        FerrariF8Tributo = 98,
        FerrariRoma = 99,
        McLaren720S = 100,
        McLarenArtura = 101,
        MercedesAMGGTBlackSeriesSafetyCar = 102,
        MercedesAMGGTRPro = 103,
        F1CustomTeam = 104,
        Prema21 = 106,
        UniVirtuosi21 = 107,
        Carlin21 = 108,
        Hitech21 = 109,
        ArtGP21 = 110,
        MPMotorsport21 = 111,
        Charouz21 = 112,
        Dams21 = 113,
        Campos21 = 114,
        BWT21 = 115,
        Trident21 = 116,
        MercedesAMGGTBlackSeries = 117,
        Prema22 = 118,
        Virtuosi22 = 119,
        Carlin22 = 120,
        Hitech22 = 121,
        ArtGP22 = 122,
        MPMotorsport22 = 123,
        Charouz22 = 124,
        Dams22 = 125,
        Campos22 = 126,
        VanAmersfoortRacing22 = 127,
        Trident22 = 128
    }

    public enum DriverID
    {
        CarlosSainz = 0,
        DaniilKvyat = 1,
        DanielRicciardo = 2,
        FernandoAlonso = 3,
        FelipeMassa = 4,
        KimiRaikkonen = 6,
        LewisHamilton = 7,
        MaxVerstappen = 9,
        NicoHulkenburg = 10,
        KevinMagnussen = 11,
        RomainGrosjean = 12,
        SebastianVettel = 13,
        SergioPerez = 14,
        ValtteriBottas = 15,
        EstebanOcon = 17,
        LanceStroll = 19,
        ArronBarnes = 20,
        MartinGiles = 21,
        AlexMurray = 22,
        LucasRoth = 23,
        IgorCorreia = 24,
        SophieLevasseur = 25,
        JonasSchiffer = 26,
        AlainForest = 27,
        JayLetourneau = 28,
        EstoSaari = 29,
        YasarAtiyeh = 30,
        CallistoCalabresi = 31,
        NaotaIzum = 32,
        HowardClarke = 33,
        WilheimKaufmann = 34,
        MarieLaursen = 35,
        FlavioNieves = 36,
        PeterBelousov = 37,
        KlimekMichalski = 38,
        SantiagoMoreno = 39,
        BenjaminCoppens = 40,
        NoahVisser = 41,
        GertWaldmuller = 42,
        JulianQuesada = 43,
        DanielJones = 44,
        ArtemMarkelov = 45,
        TadasukeMakino = 46,
        SeanGelael = 47,
        NyckDeVries = 48,
        JackAitken = 49,
        GeorgeRussell = 50,
        MaximilianGunther = 51,
        NireiFukuzumi = 52,
        LucaGhiotto = 53,
        LandoNorris = 54,
        SergioSetteCamara = 55,
        LouisDeletraz = 56,
        AntonioFuoco = 57,
        CharlesLeclerc = 58,
        PierreGasly = 59,
        AlexanderAlbon = 62,
        NicholasLatifi = 63,
        DorianBoccolacci = 64,
        NikoKari = 65,
        RobertoMerhi = 66,
        ArjunMaini = 67,
        AlessioLorandi = 68,
        RubenMeijer = 69,
        RashidNair = 70,
        JackTremblay = 71,
        DevonButler = 72,
        LukasWeber = 73,
        AntonioGiovinazzi = 74,
        RobertKubica = 75,
        AlainProst = 76,
        AyrtonSenna = 77,
        NobuharuMatsushita = 78,
        NikitaMazepin = 79,
        GuanyaZhou = 80,
        MickSchumacher = 81,
        CallumIlott = 82,
        JuanManuelCorrea = 83,
        JordanKing = 84,
        MahaveerRaghunathan = 85,
        TatianaCalderon = 86,
        AnthoineHubert = 87,
        GuilianoAlesi = 88,
        RalphBoschung = 89,
        MichaelSchumacher = 90,
        DanTicktum = 91,
        MarcusArmstrong = 92,
        ChristianLundgaard = 93,
        YukiTsunoda = 94,
        JehanDaruvala = 95,
        GulhermeSamaia = 96,
        PedroPiquet = 97,
        FelipeDrugovich = 98,
        RobertSchwartzman = 99,
        RoyNissany = 100,
        MarinoSato = 101,
        AidanJackson = 102,
        CasperAkkerman = 103,
        JensonButton = 109,
        DavidCoulthard = 110,
        NicoRosberg = 111,
        OscarPiastri = 112,
        LiamLawson = 113,
        JuriVips = 114,
        TheoPourchaire = 115,
        RichardVerschoor = 116,
        LirimZendeli = 117,
        DavidBeckmann = 118,
        AlessioDeledda = 121,
        BentViscaal = 122,
        EnzoFittipaldi = 123,
        MarkWebber = 125,
        JacquesVilleneuve = 126,
        JakeHughes = 127,
        FrederikVesti = 128,
        OlliCaldwell = 129,
        LoganSargeant = 130,
        CemBolukbasi = 131,
        AyumaIwasa = 132,
        ClementNovolak = 133,
        DennisHauger = 134,
        CalanWilliams = 135,
        JackDoohan = 136,
        AmauryCordeel = 137,
        MikaHakkinen = 138
    }
    public enum TrackID
    {
        Melbourne = 0,
        PaulRicard = 1,
        Shanghai = 2,
        Sakhir = 3,
        Catalunya = 4,
        Monaco = 5,
        Montreal = 6,
        Silverstone = 7,
        Hockenheim = 8,
        Hungaroring = 9,
        Spa = 10,
        Monza = 11,
        Singapore = 12,
        Suzuka = 13,
        AbuDhabi = 14,
        Texas = 15,
        Brazil = 16,
        Austria = 17,
        Sochi = 18,
        Mexico = 19,
        Baku = 20,
        SakhirShort = 21,
        SilverstoneShort = 22,
        TexasShort = 23,
        SuzukaShort = 24,
        Hanoi = 25,
        Zandvoort = 26,
        Imola = 27,
        Portimao = 28,
        Jeddah = 29,
        Miami = 30
    }
    public enum NationalityID
    {
        American = 1,
        Argentinean = 2,
        Australian = 3,
        Austrian = 4,
        Azerbaijani = 5,
        Bahraini = 6,
        Belgian = 7,
        Bolivian = 8,
        Brazilian = 9,
        British = 10,
        Bulgarian = 11,
        Cameroonian = 12,
        Canadian = 13,
        Chilean = 14,
        Chinese = 15,
        Colombian = 16,
        CostaRican = 17,
        Croatian = 18,
        Cypriot = 19,
        Czech = 20,
        Danish = 21,
        Dutch = 22,
        Ecuadorian = 23,
        English = 24,
        Emirian = 25,
        Estonian = 26,
        Finnish = 27,
        French = 28,
        German = 29,
        Ghanaian = 30,
        Greek = 31,
        Guatemalan = 32,
        Honduran = 33,
        HongKonger = 34,
        Hungarian = 35,
        Icelander = 36,
        Indian = 37,
        Indonesian = 38,
        Irish = 39,
        Israeli = 40,
        Italian = 41,
        Jamaican = 42,
        Japanese = 43,
        Jordanian = 44,
        Kuwaiti = 45,
        Latvian = 46,
        Lebanese = 47,
        Lithuanian = 48,
        Luxembourger = 49,
        Malaysian = 50,
        Maltese = 51,
        Mexican = 52,
        Monegasque = 53,
        NewZealander = 54,
        Nicaraguan = 55,
        NorthernIrish = 56,
        Norwegian = 57,
        Omani = 58,
        Pakistani = 59,
        Panamanian = 60,
        Paraguayan = 61,
        Peruvian = 62,
        Polish = 63,
        Portuguese = 64,
        Qatari = 65,
        Romanian = 66,
        Russian = 67,
        Salvadoran = 68,
        Saudi = 69,
        Scottish = 70,
        Serbian = 71,
        Singaporean = 72,
        Slovakian = 73,
        Slovenian = 74,
        SouthKorean = 75,
        SouthAfrican = 76,
        Spanish = 77,
        Swedish = 78,
        Swiss = 79,
        Thai = 80,
        Turkish = 81,
        Uruguayan = 82,
        Ukrainian = 83,
        Venezuelan = 84,
        Barbadian = 85,
        Welsh = 86,
        Vietnamese = 87
    }
    public enum GameModeID
    {
        EventMode = 0,
        GrandPrix = 3,
        TimeTrial = 5,
        Splitscreen = 6,
        OnlineCustom = 7,
        OnlineLeague = 8,
        CareerInvitational = 11,
        ChampionshipInvitational = 12,
        Championship = 13,
        OnlineChampionship = 14,
        OnlineWeeklyEvent = 15,
        Career22 = 19,
        Career22Online = 20,
        Benchmark = 127
    }
    public enum RulesetID
    {
        PracticeAndQualifying = 0,
        Race = 1,
        TimeTrial = 2,
        TimeAttack = 4,
        CheckpointChallenge = 6,
        Autocross = 8,
        Drift = 9,
        AverageSpeedZone = 10,
        RivalDuel = 11
    }
    public enum SurfaceTypeID
    {
        Tarmac = 0,
        RumbleStrip = 1,
        Concrete = 2,
        Rock = 3,
        Gravel = 4,
        Mud = 5,
        Sand = 6,
        Grass = 7,
        Water = 8,
        Cobblestone = 9,
        Metal = 10,
        Ridged = 11
    }

    public enum PenaltyType
    {
        DriveThrough = 0,
        StopGo = 1,
        GridPenalty = 2,
        PenaltyReminder = 3,
        TimePenalty = 4,
        Warning = 5,
        Disqualified = 6,
        RemovedFromFormationLap = 7,
        ParkedTooLongTimer = 8,
        TyreRegulations = 9,
        LapInvalidated = 10,
        LapAndNextInvalidated = 11,
        LapInvalidatedWithoutReason = 12,
        LapAndNextInvalidatedWithoutReason = 13,
        PreviousLapInvalidated = 14,
        PreviousLapInvalidatedWithoutReason = 15,
        Retired = 16,
        BlackFlagTimer = 17
    }

    public enum InfringementType
    {
        BlockingBySlowDriving = 0,
        BlockingByWrongWayDriving = 1,
        ReversingOffStartLine = 2,
        BigCollision = 3,
        SmallCollision = 4,
        CollisionFailedToHandBackPositionSingle = 5,
        CollisionFailedToHandBackPositionMultiple = 6,
        CornerCuttingGainedTime = 7,
        CornerCuttingOvertakeSingle = 8,
        CornerCuttingOvertakeMultiple = 9,
        CrossedPitExitLane = 10,
        IgnoringBlueFlags = 11,
        IgnoringYellowFlags = 12,
        IgnoringDriveThrough = 13,
        TooManyDriveThroughs = 14,
        DriveThroughReminderServeWithinNLaps = 15,
        DriveThroughReminderServeThisLap = 16,
        PitLaneSpeeding = 17,
        ParkedForTooLong = 18,
        IgnoringTyreRegulations = 19,
        TooManyPenalties = 20,
        MultipleWarnings = 21,
        ApproachingDisqualification = 22,
        TyreRegulationsSelectSingle = 23,
        TyreRegulationsSelectMultiple = 24,
        LapInvalidatedCornerCutting = 25,
        LapInvalidatedRunningWide = 26,
        CornerCuttingRanWideGainedTimeMinor = 27,
        CornerCuttingRanWideGainedTimeSignificant = 28,
        CornerCuttingRanWideGainedTimeExtreme = 29,
        LapInvalidatedWallRiding = 30,
        LapInvalidatedFlashbackUsed = 31,
        LapInvalidatedResetToTrack = 32,
        BlockingThePitlane = 33,
        JumpStart = 34,
        SafetyCarToCarCollision = 35,
        SafetyCarIllegalOvertake = 36,
        SafetyCarExceedingAllowedPace = 37,
        VirtualSafetyCarExceedingAllowedPace = 38,
        FormationLapBelowAllowedSpeed = 39,
        FormationLapParking = 40,
        RetiredMechanicalFailure = 41,
        RetiredTerminallyDamaged = 42,
        SafetyCarFallingTooFarBack = 43,
        BlackFlagTimer = 44,
        UnservedStopGoPenalty = 45,
        UnservedDriveThroughPenalty = 46,
        EngineComponentChange = 47,
        GearboxChange = 48,
        ParcFermeChange = 49,
        LeagueGridPenalty = 50,
        RetryPenalty = 51,
        IllegalTimeGain = 52,
        MandatoryPitstop = 53,
        AttributeAssigned = 54
    }

    public enum MyTeamStatus
    {
        NotMyTeamMode,
        MyTeamMode
    }

    public enum PlayerUDPStatus
    {
        Restricted,
        Public
    }

    public enum SessionType : byte
    {
        Unknown = 0,
        P1 = 1,
        P2 = 2,
        P3 = 3,
        ShortP = 4,
        Q1 = 5,
        Q2 = 6,
        Q3 = 7,
        ShortQ = 8,
        OSQ = 9,
        R = 10,
        R2 = 11,
        R3 = 12,
        TimeTrial = 13
    }

    public enum Weather : byte
    {
        Clear = 0,
        LightCloud = 1,
        Overcast = 2,
        LightRain = 3,
        HeavyRain = 4,
        Storm = 5
    }

    public enum TrackTemperatureChange : sbyte
    {
        Up = 0,
        Down = 1,
        NoChange = 2
    }

    public enum AirTemperatureChange : sbyte
    {
        Up = 0,
        Down = 1,
        NoChange = 2
    }

}
