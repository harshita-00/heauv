using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Net.Http;

namespace prjNAND_Data_Ext
{
    class Program
    {

        #region Variable Declarations

        public static byte TimerSeqOc;
        public static byte Pdu150OcCh1, Pdu150OcCh2, Pdu150OcCh3, Pdu150OcCh4;
        public static byte Pcu28Oc, Pcu28Ov, Pcu48Oc, Pcu48Ov;
        public static byte Pdu28_1OcCh1, Pdu28_1OcCh2, Pdu28_1OcCh3, Pdu28_1OcCh4, Pdu28_1OcCh5, Pdu28_1OcCh6, Pdu28_1OcCh7, Pdu28_1OcCh8;
        public static byte Pdu28_2OcCh1, Pdu28_2OcCh2, Pdu28_2OcCh3, Pdu28_2OcCh4, Pdu28_2OcCh5, Pdu28_2OcCh6, Pdu28_2OcCh7, Pdu28_2OcCh8;
        public static byte Pdu28_3OcCh1, Pdu28_3OcCh2, Pdu28_3OcCh3, Pdu28_3OcCh4, Pdu28_3OcCh5, Pdu28_3OcCh6, Pdu28_3OcCh7, Pdu28_3OcCh8;
        public static byte Pdu28_4OcCh1, Pdu28_4OcCh2, Pdu28_4OcCh3, Pdu28_4OcCh4, Pdu28_4OcCh5, Pdu28_4OcCh6, Pdu28_4OcCh7, Pdu28_4OcCh8;
        public static byte Pdu28_5OcCh1, Pdu28_5OcCh2, Pdu28_5OcCh3, Pdu28_5OcCh4, Pdu28_5OcCh5, Pdu28_5OcCh6, Pdu28_5OcCh7, Pdu28_5OcCh8;
        public static byte Pdu48_1OcCh1, Pdu48_1OcCh2, Pdu48_1OcCh3, Pdu48_1OcCh4, Pdu48_1OcCh5, Pdu48_1OcCh6, Pdu48_1OcCh7, Pdu48_1OcCh8;
        public static byte Pdu48_2OcCh1, Pdu48_2OcCh2, Pdu48_2OcCh3, Pdu48_2OcCh4, Pdu48_2OcCh5, Pdu48_2OcCh6, Pdu48_2OcCh7, Pdu48_2OcCh8;
        public static byte Pdu48_3OcCh1, Pdu48_3OcCh2, Pdu48_3OcCh3, Pdu48_3OcCh4, Pdu48_3OcCh5, Pdu48_3OcCh6, Pdu48_3OcCh7, Pdu48_3OcCh8;

        public const bool NAND_EXTRACT = false;
        public const float RADTODEG = (float)(180.0 / Math.PI);
        public static string fileName = string.Empty;
        public static string SubFileName = string.Empty;
        public static string CONTROLFileName = string.Empty;
        public static string TSUFileName = string.Empty;
        public static string BLDCFileName = string.Empty;
        public static string EASSCFileName = string.Empty;
        public static string SMCFileName = string.Empty;
        public static string BMSFileName = string.Empty;
        public static string DVLFileName = string.Empty;
        public static string RTFileName = string.Empty;
        public static string RunTerminationFileName = string.Empty;
        public static string MCS_RTCFileName = string.Empty;
        public static string SONARFileName = string.Empty;
        public static string RemoteFileName = string.Empty;
        public static string HealthFileName = string.Empty;
        public static string ControlGainsFileName = string.Empty;
        public static string SeqLegsFileName = string.Empty;
        public static string PresetParametersFileName = string.Empty;
        public static string IPSParametersFileName = string.Empty;
        public static string CTDFileName = string.Empty;
        public static string SECSSFileName = string.Empty;
        public static string IPS_DetectionFileName = string.Empty;
        public static string Recovery_ConditionsFileName = string.Empty;
        public static string MastFileName = string.Empty;
        public static StreamWriter swPhins;
        public static StreamWriter swPresetParameters;
        public static StreamWriter swIPS;
        public static StreamWriter swCtd;
        public static StreamWriter swTsu;
        public static StreamWriter swBldc;
        public static StreamWriter swEassc;
        public static StreamWriter swSmc;
        public static StreamWriter swBms;
        public static StreamWriter swDvl;
        public static StreamWriter swRt;
        public static StreamWriter swRunTermination;
        public static StreamWriter swMCS_RTC_Clock;
        public static StreamWriter swControlGains;
        public static StreamWriter swSeqLegs;
        public static StreamWriter swRemote;
        public static StreamWriter swHealth;
        public static StreamWriter swSONAR;
        public static StreamWriter swCTD;
        public static StreamWriter swSecSS;
        public static StreamWriter swIPS_Detection;
        public static StreamWriter swRecovery_Conditions;
        public static StreamWriter swMAST;
        public static double g_dTime = 0.0, g_dTime1 = 0.0, g_dTime2 = 0.0;
        public static double flADC0, flADC1;
        public static byte StatusCode0, StatusCode1, StatusCode2, StatusCode3, StatusCode4, StatusCode5, StatusCode6, StatusCode7;

        public static ushort Counter = 0;
        public static ushort usADC0 = 0, usADC1 = 0, usHeader = 0, usFooter = 0;
        public static ushort usNPageNum = 0, usNBlkNum = 0, RunNumber = 0;

        public static ushort PresetFlo = 0, presetDco = 0, PresetMotorSampleRate = 0;

        public static ushort WpDepth, usPrevNum = 0, usCurrNum = 0;

        public static ushort RTFlags_1 = 0, RTFlags_2 = 0;

        public static float Nvel = 0.0f, Evel = 0.0f, Dvel = 0.0f, PosX = 0.0f, PosY = 0.0f, PosZ = 0.0f, Uvel = 0.0f;

        public static byte PMode, MMode, RMode, SMode, AMode, Ps, Leak, Remote;

        public static byte MotorRt, PedCf, PhinsCf, AltCf, MpidsCf, RuntimeRt, EasscCf, Stop, Abort, Dco;


        public static byte RT4m_MPIDS, RT4m_MPIDS_SA5, RT4m_MPIDS_SA4, RT4m_MPIDS_SA3, g_ucBoundaryConditionRT, g_ucRT4m_DCO, g_ucMission_Abort, g_ucMission_Stop, 
g_ucCommFail4mEASSC, g_ucLCS_Indication, g_ucRunTimeRT,
            g_ucCommFail4mMPIDS, g_ucCommFail4mALTI, g_ucCommFail4mPHINS, g_ucCommFail4mPED, g_ucRT4mMotor;

        public static byte g_Motor1_Over_Current_Flag, g_ucMotor1Over_Voltage_flag, g_ucMotor1Under_Voltage_flag, g_ucFault4mMotor_flag, g_ucRT4mIPS, 
g_ucIPSError_FLSAvailFlag, g_ucIPS_Error_SCMFlag, g_ucIPSError_FirstWpRangeFlag, g_ucIPSError_WpAvailFlag, g_ucIPSError_VehicleStateAvailFlag,
            g_ucIPSError_ConfigCmdFlag, g_ucIPSError_MPSSWFlag, g_ucCommFail4mIPS, g_ucCommFail4mBMS, g_ucMPSNoGuidanceRT;

        public static byte g_ucCommFail4mLCS, g_ucCommFail4mCSS, g_ucCommFail4mNCS, g_ucCommFail4mFLSC, g_ucCommFail4mSSSC, g_ucCommFail4mMBE, g_ucCommFail4mBLSC, 
g_ucCommFail4mAASC, g_ucCommFail4mFASC, g_ucCommFail4mAMC, g_ucCommFail4mLPRS, g_ucCommFail4mELINT, g_ucCommFail4mMSS, g_ucCommFail4mSCS, g_ucCommFail4mSECSS, 
g_ucAMTransmitEnableFlag;

        public static ushort psusFasTargetCount, g_usiWaypointId4mIps;

        public static byte pinger_st_mission_flag;

        public static byte Rc_ubfMPSConfigCmdError, Rc_ubfMPSBeyondRange, Rc_ubfPHINSCommFail, Rc_ubfPEDCommFail, Rc_ubfMPIDSCommFail, Rc_ubfMASTCommFail;
        public static byte Rc_ubfLPRSCommFail, Rc_ubfLCSWirelessCommFail, Rc_ubfLCSWiredCommFail, Rc_ubfELINTCommFail, Rc_ubfEASSCCommFail;
        public static byte Rc_ubfIPSCommFail, Rc_ubfCTDCommFail, Rc_ubfCSSCommFailNA, Rc_ubfBMSCommFail, Rc_ubfAltimeterCommFail;

        public static byte Rc_ubfSONARCommFail, Rc_ubfSCSCommFail, Rc_ubfMPSWaypointNA, Rc_ubfMPSWaypointComplete, Rc_ubfPSonIndication;
        public static byte Rc_ubfMotorRT, Rc_ubfStop, Rc_ubfAbort, Rc_ubfRunTimeRT, Rc_ubfLCS_Indication, Rc_ubfDCO, Rc_ubfIpsRT;
        public static byte Rc_ubfMPSVehicleStateNA, Rc_ubfMPSSWError, Rc_ubfMPSSafetyCorridorMonitor, Rc_ubfMPSFLSNA;

        public static byte Rc_ubfSecSSCommFail, Rc_ubfTSUCommFail, Rc_ubfMPSNoGuidance;
        public static byte RecordPrelaunchDataFlag, DynamicCmdSharingFlag, VelocityControlSystem, AltitudeControlSystem;
        public static float DVL_Altitude = 0.0f, INS_Resultant_Velocity = 0.0f;

        public static ushort[] MCS2MAST = new ushort[5];
        public static ushort[] MAST2MCS = new ushort[5];

        public static ushort Waypoint_ID_4m_IPS;
        public static float Headingrate_4m_IPS, Speed_4m_IPS, SONAR_TRANSMIT_DEPTH_LIM, YAW_RATE_LOOP_PROP_GAIN;
        public static byte FLSC_Power_Status;
        public static byte FASC_Power_Status;
        public static byte AASC_Power_Status;
        public static byte AMC_Power_Status;
        public static byte BLSC_Power_Status;
        public static byte FLSC_Transmit_Status;
        public static byte FASC_Transmit_Status;
        public static byte AMC_Transmit_Status;

        public static uint Day;
        public static uint Month;
        public static uint Year;
        public static uint Hours;
        public static uint Minutes;
        public static uint Seconds;
        public static byte FLSCCurrent = 0, FASCCurrent = 0;
        public static ushort lcs_static_test_rpm;

        public static float RefDepthScs = 0.0f, RefYawScs = 0.0f, PCLScs = 0.0f, DelScs = 0.0f;

        public static uint SpeedScs, PresetRuntime = 0;



        public static float flRoll = 0.0f, flPitch = 0.0f, flYaw = 0.0f, flRollRate = 0.0f, flPitchRate = 0.0f, flYawRate = 0.0f, flLat = 0.0f, flLong = 0.0f, flAlt = 
0.0f, flAltDepth = 0.0f, flAltTemp = 0.0f, flPitchCmd = 0.0f, flRollCmd = 0.0f, flYawCmd = 0.0f;

        public static int iIndex = 0;

        public static double dLat, dLong;

        public static uint ulTime = 0, ulTime1 = 0, ulTime2 = 0;
        public static uint SysStat1 = 0, SysStat2 = 0, AlgoStat1 = 0, AlgoStat2 = 0, SenStat1 = 0, SenStat2 = 0;

        public static BinaryReader bin_reader = null;
        public static String FileName;

        public static byte HBms, HPed, HSmc, HEassc, HAlt, HCtd, HTsu, HPhins, IPS_MDS_HEALTH;
        public static byte HMcs, HLPRS, HELINT, HIps, HSSS, HSECSS, HMBE, HFls, HFds, HAM, HBLS, HAAS, HFAS, HCSS, HSCS, HMSS, Hswitch;

        public static byte MsgCntSmc, MsgCntEassc, MsgCntPed, MsgCntBms, MsgCntAlt, MsgCntTsu, MsgCntPhins, MsgCntDvl;

        public static double BtRes, WtRes,dAvgBeamRange;
        public static float ROLL_INTG_ON_TIME;
        public static float ROLL_GAIN;
        public static float ROLL_RATE_GAIN_3KN;
        public static float ROLL_RATE_GAIN_6KN;
        public static float ROLL_CMD_LIM;
        public static float YAW_GAIN;
        public static float YAW_RATE_GAIN_3KN;
        public static float YAW_RATE_GAIN_6KN;
        public static float YAW_INTG_GAIN;
        public static float YAW_CMD_LIM;
        public static float DEPTH_GAIN;
        public static float DERR_MAX;
        public static float PITCH_GAIN;
        public static float PITCH_RATE_GAIN_3KN;
        public static float PITCH_RATE_GAIN_6KN;
        public static float PITCH_CMD_LIM;
        public static float FIN_CMD_MAX;

        public const byte MAX_BUFFER_SIZE = 32;
        public const byte MSQ_PKT_LEGS = 21;

        public static ushort[] usBLDC2OBC = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usOBC2BLDC = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usOBC2FASC_SA1 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usOBC2FASC_SA2 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usFASC2OBC_SA3 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usOBC2SMC_SA1 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usSMC2OBC_SA2 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usSMC2OBC_SA3 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usSMC2OBC_SA4 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usSMC2OBC_SA5 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usSMC2OBC_SA6 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usSMC2OBC_SA7 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usTsuData = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usBMS2OBC_SA1 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usBMS2OBC_SA2 = new ushort[MAX_BUFFER_SIZE];
        public static ushort[] usOBC2BMS_SA3 = new ushort[MAX_BUFFER_SIZE];

        public static ushort[] usCTD = new ushort[MAX_BUFFER_SIZE];
        public static byte psbNooflegs, psbNoofTargets;
        public static uint[] psuiLegDuration = new uint[MSQ_PKT_LEGS];
        public static byte[] psbSpeed = new byte[MSQ_PKT_LEGS];
        public static float[] psusDepth = new float[MSQ_PKT_LEGS];
        public static short[] pssYawRef = new short[MSQ_PKT_LEGS];
        public static float[] pfTurnRate = new float[MSQ_PKT_LEGS];
        public static ushort[] psusDco = new ushort[MSQ_PKT_LEGS];
        public static ushort[] psusCco = new ushort[MSQ_PKT_LEGS];
        public static byte[] psbSearchPattern = new byte[MSQ_PKT_LEGS];
        public static byte[] psbSearchSide = new byte[MSQ_PKT_LEGS];
        public static byte[] psbRCL = new byte[MSQ_PKT_LEGS];
        public static byte[] psbPCL = new byte[MSQ_PKT_LEGS];
        public static byte[] psbYCL = new byte[MSQ_PKT_LEGS];
        public static byte[] psbDECL = new byte[MSQ_PKT_LEGS];

        public static byte static_test_flag, static_test_start, distress_ack, erasure_start, erasure_stop, shutdown;
        //public static ushort lcs_static_test_rpm;
        public static uint static_test_start_time, static_test_stop_time;
        public static ushort image_number, no_of_pings, no_of_sample;
        public static byte total_no_of_detections, detect_pkt_no_in_cycle, no_of_det_in_pkt;
        public static float image_st_timestamp = 0.0f, timestamp_incr = 0.0f;
        public static byte[] detclass = new byte[10];
        public static ushort[] detx1 = new ushort[10];
        public static ushort[] dety1 = new ushort[10];
        public static ushort[] detx2 = new ushort[10];
        public static ushort[] dety2 = new ushort[10];
        public static double[] dlat1 = new double[10];
        public static double[] dlong1 = new double[10];
        public static double[] dlat2 = new double[10];
        public static double[] dlong2 = new double[10];

        public static uint AmMcsMissionAbort;
        public static byte AmMcsMissionChange;
        public static byte AmMcsRecvSurf;
        public static float AmMcsLat = 0.0f, AmMcsLong = 0.0f, AmMcsAlt = 0.0f;


        public static byte bRead, bRead1, bRead2, bRead3, bRead4;
        public const int NAND_PACKET_SIZE = 4096;
        public static int read_size;
        public static byte[] bArray = new byte[NAND_PACKET_SIZE];
        public static byte[] bMsgErrCnt = new byte[50];
        public static double Timer_100ms_Count;

        public static short[] usDvlData = new short[11];
        public static bool flag = true;

        public static double fasc_lat, fasc_long;
        public static byte fasc_day, fasc_month, fasc_hour, fasc_minute, fasc_second;
        public static ushort fasc_year, fasc_milliseconds;

        public static byte[] targetid = new byte[8];
        public static float[] trackquality = new float[8];
        public static float[] bearing = new float[8];
        public static float[] bearingrate = new float[8];
        public static float[] snr = new float[8];
        public static byte[] targetrangeflag = new byte[8];
        public static float[] range = new float[8];
        public static float[] rangerate = new float[8];

        public static byte ActiveTransmissionInterval, noofpings, PRI, PassiveGain, ActiveGainMin, ActiveGainMax, scSourceLevel;
        public static ushort FrequencyRangeStart, FrequencyRangeEnd;

        public static uint SSS_SystemMode, SSS_Altitude, SSS_SpeedOfSurvey, SSS_MaxSwathToRecord, SSS_SpeedOfSound, SSS_RawDataRecord;
        public static uint MBE_SystemMode, MBE_Altitude, MBE_SpeedOfSurvey, MBE_MaxScanLinesToRecord, MBE_SpeedOfSound, MBE_RawDataRecord;

        public static float TxTriggerDepth = 0.0f, CFARThreshold = 0.0f;
        public static byte FlsPRI, FlsPulseLength, FlsSourceLevel;

        public static ushort FlsFaultSummary, FasFaultSummary, AasFaultSummary, AmFaultSummary, BlsFaultSummary;

        public static byte FlsDay, FlsMonth, FlsHour, FlsMin, FlsSec;
        public static ushort FlsYear, FlsMsec;

        public static byte FasDay, FasMonth, FasHour, FasMin, FasSec;
        public static ushort FasYear, FasMsec;

        public static byte AasDay, AasMonth, AasHour, AasMin, AasSec;
        public static ushort AasYear, AasMsec;

        public static byte AmDay, AmMonth, AmHour, AmMin, AmSec;
        public static ushort AmYear, AmMsec;

        public static byte BlsDay, BlsMonth, BlsHour, BlsMin, BlsSec;
        public static ushort BlsYear, BlsMsec;

        #endregion
        static void Main(string[] args)
        {
          Console.WriteLine("Args count: " + args.Length);

        if (args.Length == 0)
            {
                Console.WriteLine("❌ Please provide folder path.");
                return;
            }

        string folderPath = args[0];   // ✅ now safe

        Console.WriteLine("Processing folder: " + folderPath);

        string[] files = Directory.GetFiles(folderPath, "*.bin");

        foreach (string file in files)
            {
                Console.WriteLine("Processing: " + file);
                NAND_data_extraction(file);
            }
		}

        public static void NAND_data_extraction(string FileName)
        {
           string dir = Path.GetDirectoryName(FileName) ?? "";

        string baseFolder = Path.Combine(
        dir,
        Path.GetFileNameWithoutExtension(FileName)
        );

        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }
           //open bin file
        bin_reader = new BinaryReader(
        new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        );
            int runNum=0;
            string runFolder = Path.Combine(baseFolder, "Run-" + runNum);
            try
            {
                if (Directory.Exists(runFolder))
               {
                Directory.Delete(runFolder, true);
               }
             Directory.CreateDirectory(runFolder);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Folder creation error: " + e.Message);
            }
            // Use runFolder instead of FileName
            SubFileName = runFolder + Path.DirectorySeparatorChar;

    CONTROLFileName = SubFileName + "RUN-" + runNum + "_Control_" +
                      DateTime.Now.ToString("d-M-yyyy_HH-mm-ss") + ".dat";

    swPhins = new StreamWriter(CONTROLFileName)
    {
        AutoFlush = true
    };
           
            TSUFileName = SubFileName + "RUN-" + runNum + "_Tsu_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString()
 + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swTsu = new StreamWriter(TSUFileName)
            {
                AutoFlush = true
            };
            BLDCFileName = SubFileName + "RUN-" + runNum + "_BLDC_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString
() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swBldc = new StreamWriter(BLDCFileName)
            {
                AutoFlush = true
            };
            EASSCFileName = SubFileName + "RUN-" + runNum + "_EASSC_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.
ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swEassc = new StreamWriter(EASSCFileName)
            {
                AutoFlush = true
            };
            SMCFileName = SubFileName + "RUN-" + runNum + "_SMC_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString()
 + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swSmc = new StreamWriter(SMCFileName)
            {
                AutoFlush = true
            };
            BMSFileName = SubFileName + "RUN-" + runNum + "_BMS_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString()
 + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swBms = new StreamWriter(BMSFileName)
            {
                AutoFlush = true
            };
            DVLFileName = SubFileName + "RUN-" + runNum + "_DVL_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString()
 + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swDvl = new StreamWriter(DVLFileName)
            {
                AutoFlush = true
            };

            RemoteFileName = SubFileName + "RUN-" + runNum + "_Remote_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year
.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swRemote = new StreamWriter(RemoteFileName)
            {
                AutoFlush = true
            };

            HealthFileName = SubFileName + "RUN-" + runNum + "_Health_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year
.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swHealth = new StreamWriter(HealthFileName)
            {
                AutoFlush = true
            };
            ControlGainsFileName = SubFileName + "RUN-" + runNum + "_Gains_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now
.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swControlGains = new StreamWriter(ControlGainsFileName)
            {
                AutoFlush = true
            };
            SeqLegsFileName = SubFileName + "RUN-" + runNum + "_Seq_Legs_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.
Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swSeqLegs = new StreamWriter(SeqLegsFileName)
            {
                AutoFlush = true
            };

            PresetParametersFileName = SubFileName + "RUN-" + runNum + "_Preset_Parameters_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + 
"-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat"
;
            swPresetParameters = new StreamWriter(PresetParametersFileName)
            {
                AutoFlush = true
            };

            IPSParametersFileName = SubFileName + "RUN-" + runNum + "_IPS_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime
.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swIPS = new StreamWriter(IPSParametersFileName)
            {
                AutoFlush = true
            };

            RunTerminationFileName = SubFileName + "RUN-" + runNum + "_RunTermination_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + 
"-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat"
;
            swRunTermination = new StreamWriter(RunTerminationFileName)
            {
                AutoFlush = true
            };

            MCS_RTCFileName = SubFileName + "RUN-" + runNum + "_MCS_RTC_Clock_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swMCS_RTC_Clock = new StreamWriter(MCS_RTCFileName)
            {
                AutoFlush = true
            };

            SONARFileName = SubFileName + "RUN-" + runNum + "_SONAR_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.
Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swSONAR = new StreamWriter(SONARFileName)
            {
                AutoFlush = true
            };

            CTDFileName = SubFileName + "RUN-" + runNum + "_CTD_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.
ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swCTD = new StreamWriter(CTDFileName)
            {
                AutoFlush = true
            };
            SECSSFileName = SubFileName + "RUN-" + runNum + "_SECSS_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.
Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
            swSecSS = new StreamWriter(SECSSFileName)
            {
                AutoFlush = true
            };

            IPS_DetectionFileName = SubFileName + "RUN-" + runNum + "_Detection_Packet_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + 
"-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat"
;
            swIPS_Detection = new StreamWriter(IPS_DetectionFileName)
            {
                AutoFlush = true
            };

            Recovery_ConditionsFileName = SubFileName + "RUN-" + runNum + "_Recovery_Conditions_Packet_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.
Month.ToString() + "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.
ToString() + ".dat";
            swRecovery_Conditions = new StreamWriter(Recovery_ConditionsFileName)
            {
                AutoFlush = true
            };

            MastFileName = SubFileName + "RUN-" + runNum + "_MAST_Packet_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.
Month.ToString() + "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.
ToString() + ".dat";
            swMAST = new StreamWriter(MastFileName)
            {
                AutoFlush = true
            };

            Console.WriteLine("created--=");

            try
            {
                while ((bin_reader.BaseStream.Position != bin_reader.BaseStream.Length))
                {
                    bRead1 = bin_reader.ReadByte();
                    bRead2 = bin_reader.ReadByte();

                    iIndex = 0;
                    //if((bArray[iIndex] == 0xA2 && bArray[iIndex + 1] == 0xA2) || (bArray[iIndex] == 0xA3 && bArray[iIndex + 1] == 0xA3))
                    if (/*(bRead1 == 0xA1 && bRead2 == 0xA1) || */(bRead1 == 0xA2 && bRead2 == 0xA2) || (bRead1 == 0xA3 && bRead2 == 0xA3) /*|| (bRead1 == 0xA5 && bRead2 == 0xA5)*/)
                    {
                        read_size = NAND_PACKET_SIZE - 2;
                        //  bArray[0] = bRead1;
                        //  bArray[1] = bRead2;
                        bin_reader.BaseStream.Position = bin_reader.BaseStream.Position - 2;
                        bArray = bin_reader.ReadBytes(NAND_PACKET_SIZE);
                        #region Header


                        Timer_100ms_Count += 0.1F;
                        // Array.Reverse(bArray, iIndex, 2);
                        //usHeader = BitConverter.ToUInt16(bArray, iIndex);
                        usHeader = (UInt16)((bRead1 << 8) | bRead2);
                        iIndex += 2; /* iIndex = 2 */


                        #endregion

                        #region 10msec Time

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        ulTime1 = BitConverter.ToUInt32(bArray, iIndex);
                        g_dTime1 = ulTime1 * 0.01;
                        iIndex += 4; /* iIndex = 6 */

                        #endregion

                        #region 100msec Time
                        UInt32 temp11;
                        temp11 = BitConverter.ToUInt32(bArray, iIndex);
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        ulTime = BitConverter.ToUInt32(bArray, iIndex);
                        g_dTime = ulTime * 0.1;
                        iIndex += 4; /* iIndex = 10 */

                        #endregion

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        RunNumber = BitConverter.ToUInt16(bArray, 1418);
                        //usCurrNum = RunNumber;


                        #region test 
#if false
                        if ((g_dTime == 0.2)  || (g_dTime == 0.1) /*usPrevNum < usCurrNum*/)
                        {
                            //flag = true;
                            usPrevNum = usCurrNum;
                            runNum++;
                            if (runNum != 0)
                            {
                                swPhins.Flush();
                                swPhins.Close();
                                swBldc.Flush();
                                swBldc.Close();
                                swEassc.Flush();
                                swEassc.Close();
                                swSmc.Flush();
                                swSmc.Close();
                                swTsu.Flush();
                                swTsu.Close();
                                swBms.Flush();
                                swBms.Close();
                                swDvl.Flush();
                                swDvl.Close();
                                swRemote.Flush();
                                swRemote.Close();
                                swHealth.Flush();
                                swHealth.Close();
                                swControlGains.Flush();
                                swControlGains.Close();
                                swSeqLegs.Flush();
                                swSeqLegs.Close();
                                swPresetParameters.Flush();
                                swPresetParameters.Close();
                                swIPS.Flush();
                                swIPS.Close();
                                swRunTermination.Flush();
                                swRunTermination.Close();
                                swMCS_RTC_Clock.Flush();
                                swMCS_RTC_Clock.Close();
                                swSONAR.Flush();
                                swSONAR.Close();
                                swCTD.Flush();
                                swCTD.Close();
                                swIPS_Detection.Flush();
                                swIPS_Detection.Close();
                                swSecSS.Flush();
                                swSecSS.Close();
                                swRecovery_Conditions.Flush();
                                swRecovery_Conditions.Close();
                            }

                            try
                            {
                                string runFolder = Path.Combine(baseFolder, "Run-" + runNum);

    if (Directory.Exists(runFolder))
        Directory.Delete(runFolder, true);

    Directory.CreateDirectory(runFolder);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }

                            SubFileName = FileName + "\\Run-" + runNum + "\\";
                            CONTROLFileName = SubFileName + "_RUN-" + runNum + "_Control_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swPhins = new StreamWriter(CONTROLFileName)
                            {
                                AutoFlush = true
                            };

                            TSUFileName = SubFileName + "RUN-" + runNum + "_Tsu_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swTsu = new StreamWriter(TSUFileName)
                            {
                                AutoFlush = true
                            };
                            BLDCFileName = SubFileName + "RUN-" + runNum + "_BLDC_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swBldc = new StreamWriter(BLDCFileName)
                            {
                                AutoFlush = true
                            };
                            EASSCFileName = SubFileName + "RUN-" + runNum + "_EASSC_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swEassc = new StreamWriter(EASSCFileName)
                            {
                                AutoFlush = true
                            };
                            SMCFileName = SubFileName + "RUN-" + runNum + "_SMC_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swSmc = new StreamWriter(SMCFileName)
                            {
                                AutoFlush = true
                            };
                            BMSFileName = SubFileName + "RUN-" + runNum + "_BMS_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swBms = new StreamWriter(BMSFileName)
                            {
                                AutoFlush = true
                            };
                            DVLFileName = SubFileName + "RUN-" + runNum + "_DVL_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swDvl = new StreamWriter(DVLFileName)
                            {
                                AutoFlush = true
                            };

                            RemoteFileName = SubFileName + "RUN-" + runNum + "_Remote_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swRemote = new StreamWriter(RemoteFileName)
                            {
                                AutoFlush = true
                            };

                            HealthFileName = SubFileName + "RUN-" + runNum + "_Health_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swHealth = new StreamWriter(HealthFileName)
                            {
                                AutoFlush = true
                            };
                            ControlGainsFileName = SubFileName + "RUN-" + runNum + "_Gains_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + 
"-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swControlGains = new StreamWriter(ControlGainsFileName)
                            {
                                AutoFlush = true
                            };
                            SeqLegsFileName = SubFileName + "RUN-" + runNum + "_Seq_Legs_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" 
+ DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swSeqLegs = new StreamWriter(SeqLegsFileName)
                            {
                                AutoFlush = true
                            };

                            PresetParametersFileName = SubFileName + "RUN-" + runNum + "_Preset_Parameters_File_" + DateTime.Now.Day.ToString() + "-" + 
DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + 
DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
                            swPresetParameters = new StreamWriter(PresetParametersFileName)
                            {
                                AutoFlush = true
                            };

                            IPSParametersFileName = SubFileName + "RUN-" + runNum + "_IPS_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() 
+ "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swIPS = new StreamWriter(IPSParametersFileName)
                            {
                                AutoFlush = true
                            };

                            RunTerminationFileName = SubFileName + "RUN-" + runNum + "_RunTermination_Data_File_" + DateTime.Now.Day.ToString() + "-" + 
DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + 
DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
                            swRunTermination = new StreamWriter(RunTerminationFileName)
                            {
                                AutoFlush = true
                            };
                            MCS_RTCFileName = SubFileName + "RUN-" + runNum + "_MCS_RTC_Clock_Data_File_" + DateTime.Now.Day.ToString() + "-" + 
DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + 
DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
                            swMCS_RTC_Clock = new StreamWriter(MCS_RTCFileName)
                            {
                                AutoFlush = true
                            };

                            SONARFileName = SubFileName + "RUN-" + runNum + "_SONAR_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" 
+ DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swSONAR = new StreamWriter(SONARFileName)
                            {
                                AutoFlush = true
                            };

                            CTDFileName = SubFileName + "RUN-" + runNum + "_CTD_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + 
DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swCTD = new StreamWriter(CTDFileName)
                            {
                                AutoFlush = true
                            };
                            SECSSFileName = SubFileName + "RUN-" + runNum + "_SECSS_Data_File_" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" 
+ DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + 
DateTime.Now.Second.ToString() + ".dat";
                            swSecSS = new StreamWriter(SECSSFileName)
                            {
                                AutoFlush = true
                            };

                            IPS_DetectionFileName = SubFileName + "RUN-" + runNum + "_Detection_Packet_Data_File_" + DateTime.Now.Day.ToString() + "-" + 
DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + 
DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
                            swIPS_Detection = new StreamWriter(IPS_DetectionFileName)
                            {
                                AutoFlush = true
                            };

                            Recovery_ConditionsFileName = SubFileName + "RUN-" + runNum + "_Recovery_Conditions_Packet_Data_File_" + DateTime.Now.Day.ToString() + "-" + 
DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + 
DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".dat";
                            swRecovery_Conditions = new StreamWriter(Recovery_ConditionsFileName)
                            {
                                AutoFlush = true
                            };
                        }
#endif
#endregion

                        #region 1sec Time

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        ulTime2 = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* iIndex = 14 */

                        #endregion

                        #region Motor Parameters

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 16 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 18 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 20 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 22 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 24 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 26 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 28 */

                        iIndex += 2;    /* for 0x0D0D */ /* iIndex = 30 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 32 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 34 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[9] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 36 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[10] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 38 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 40 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[12] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 42 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[13] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 44 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[14] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 46 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[15] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 48 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBLDC2OBC[16] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 50 */


                        #endregion

                        #region OBC2MOTOR

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BLDC[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 52 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BLDC[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 54 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BLDC[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 56 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BLDC[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 58 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BLDC[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 60 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BLDC[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 62 */

                        #endregion

                        #region Actuation Parameters

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2FASC_SA1[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 64 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2FASC_SA1[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 66 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2FASC_SA1[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 68 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2FASC_SA1[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 70 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2FASC_SA2[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 72 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 74 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 76 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 78 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 80 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 82 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 84 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 86 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 88 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 90 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[9] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 92 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[10] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 94 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 96 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[12] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 98 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[13] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 100 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[14] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 102 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[15] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 104 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[16] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 106 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[17] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 108 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[18] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 110 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usFASC2OBC_SA3[19] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 112 */

                        #endregion

                        #region Control Parameters

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flRoll = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 116 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flPitch = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 120 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flYaw = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 124 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flRollRate = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 128 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flPitchRate = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 132 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flYawRate = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 136 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flLat = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 140 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flLong = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 144 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flAlt = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 148 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Nvel = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 152 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Evel = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 156 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Dvel = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 160 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flAltDepth = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 164 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flAltTemp = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 168 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usADC0 = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 170 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usADC1 = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 172 */

                        #endregion

                        #region Phins Status
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        SysStat1 = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* iIndex = 176 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        SysStat2 = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* iIndex = 180 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        AlgoStat1 = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* iIndex = 184 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        AlgoStat2 = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* iIndex = 188 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        SenStat1 = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* iIndex = 192 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        SenStat2 = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* iIndex = 196 */

                        #endregion

                        #region TSU Data

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usTsuData[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 198 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usTsuData[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 200 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usTsuData[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 202 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usTsuData[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 204 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usTsuData[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 206 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usTsuData[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 208 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usTsuData[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 210 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usTsuData[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 212 */

                        #endregion

                        #region Health Data

                        // iIndex = 212;

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            HBms = 1;
                        }
                        else
                        {
                            HBms = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            HPed = 1;
                        }
                        else
                        {
                            HPed = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            HSmc = 1;
                        }
                        else
                        {
                            HSmc = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            HEassc = 1;
                        }
                        else
                        {
                            HEassc = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            HAlt = 1;
                        }
                        else
                        {
                            HAlt = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            HCtd = 1;
                        }
                        else
                        {
                            HCtd = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            HTsu = 1;
                        }
                        else
                        {
                            HTsu = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            HPhins = 1;
                        }
                        else
                        {
                            HPhins = 0;
                        }
                        iIndex += 1; /* iIndex = 213 */

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            IPS_MDS_HEALTH = 1;
                        }
                        else
                        {
                            IPS_MDS_HEALTH = 0;
                        }
                        iIndex += 1; /* iIndex = 214 */

                        // iIndex = 214;

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            HMcs = 1;
                        }
                        else
                        {
                            HMcs = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            HLPRS = 1;
                        }
                        else
                        {
                            HLPRS = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            HELINT = 1;
                        }
                        else
                        {
                            HELINT = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            HIps = 1;
                        }
                        else
                        {
                            HIps = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            HSSS = 1;
                        }
                        else
                        {
                            HSSS = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            HSECSS = 1;
                        }
                        else
                        {
                            HSECSS = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            HMBE = 1;
                        }
                        else
                        {
                            HMBE = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            HFls = 1;
                        }
                        else
                        {
                            HFls = 0;
                        }
                        iIndex += 1; /* iIndex = 215 */

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            HFds = 1;
                        }
                        else
                        {
                            HFds = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            HAM = 1;
                        }
                        else
                        {
                            HAM = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            HBLS = 1;
                        }
                        else
                        {
                            HBLS = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            HAAS = 1;
                        }
                        else
                        {
                            HAAS = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            HFAS = 1;
                        }
                        else
                        {
                            HFAS = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            HCSS = 1;
                        }
                        else
                        {
                            HCSS = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            HSCS = 1;
                        }
                        else
                        {
                            HSCS = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            HMSS = 1;
                        }
                        else
                        {
                            HMSS = 0;
                        }
                        iIndex += 1; /* iIndex = 216 */

                        #endregion

                        #region SMC Data

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 218 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 220 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 222 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 224 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 226 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 228 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 230 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 232 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2SMC_SA1[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 234 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 236 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 238 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 240 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 242 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 244 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 246 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 248 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 250 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA2[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 252 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA3[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 254 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA3[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 256 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA3[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 258 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA3[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 260 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA3[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 262 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA3[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 264 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA3[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 266 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA3[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 268 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 270 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 272 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 274 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 276 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 278 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 280 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 282 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 284 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 286 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[9] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 288 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[10] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 290 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 292 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[12] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 294 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[13] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 296 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[14] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 298 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[15] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 300 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[16] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 302 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[17] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 304 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[18] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 306 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA4[19] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 308 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 310 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 312 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 314 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 316 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 318 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 320 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 322 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 324 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 326 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[9] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 328 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[10] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 330 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 332 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[12] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 334 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[13] = BitConverter.ToUInt16(bArray, iIndex);
                        TimerSeqOc = (byte)((usSMC2OBC_SA5[13] & 0xFF) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 336 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA5[14] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 338 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 340 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[1] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu150OcCh1 = (byte)((usSMC2OBC_SA6[1] & 0xFF) * 80 / 255.0F);
                        Pdu150OcCh2 = (byte)(((usSMC2OBC_SA6[1] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 342 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[2] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu150OcCh3 = (byte)((usSMC2OBC_SA6[2] & 0xFF) * 30 / 255.0F);
                        Pdu150OcCh4 = (byte)(((usSMC2OBC_SA6[2] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 344 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 346 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 348 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 350 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[6] = BitConverter.ToUInt16(bArray, iIndex);
                        Pcu28Oc = (byte)(usSMC2OBC_SA6[6] * 150 / 255.0F);
                        iIndex += 2; /* iIndex = 352 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 354 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[8] = BitConverter.ToUInt16(bArray, iIndex);
                        Pcu28Ov = (byte)(((usSMC2OBC_SA6[8] & 0xFF) << 8) * 55 / 255.0F);
                        iIndex += 2; /* iIndex = 356 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[9] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 358 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[10] = BitConverter.ToUInt16(bArray, iIndex);
                        Pcu48Oc = (byte)(usSMC2OBC_SA6[10] * 150 / 255.0F);
                        iIndex += 2; /* iIndex = 360 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 362 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[12] = BitConverter.ToUInt16(bArray, iIndex);
                        Pcu48Ov = (byte)(((usSMC2OBC_SA6[12] & 0xFF) << 8) * 55 / 255.0F);
                        iIndex += 2; /* iIndex = 364 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[13] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 366 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[14] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_1OcCh1 = (byte)((usSMC2OBC_SA6[14] & 0xFF) * 30 / 255.0F);
                        Pdu28_1OcCh2 = (byte)(((usSMC2OBC_SA6[14] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 368 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[15] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_1OcCh3 = (byte)((usSMC2OBC_SA6[15] & 0xFF) * 30 / 255.0F);
                        Pdu28_1OcCh4 = (byte)(((usSMC2OBC_SA6[15] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 370 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[16] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_1OcCh5 = (byte)((usSMC2OBC_SA6[16] & 0xFF) * 30 / 255.0F);
                        Pdu28_1OcCh6 = (byte)(((usSMC2OBC_SA6[16] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 372 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[17] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_1OcCh7 = (byte)((usSMC2OBC_SA6[17] & 0xFF) * 30 / 255.0F);
                        Pdu28_1OcCh8 = (byte)(((usSMC2OBC_SA6[17] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 374 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[18] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 376 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[19] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 378 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[20] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_2OcCh1 = (byte)((usSMC2OBC_SA6[20] & 0xFF) * 30 / 255.0F);
                        Pdu28_2OcCh2 = (byte)(((usSMC2OBC_SA6[20] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 380 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[21] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_2OcCh3 = (byte)((usSMC2OBC_SA6[21] & 0xFF) * 30 / 255.0F);
                        Pdu28_2OcCh4 = (byte)(((usSMC2OBC_SA6[21] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 382 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[22] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_2OcCh5 = (byte)((usSMC2OBC_SA6[22] & 0xFF) * 30 / 255.0F);
                        Pdu28_2OcCh6 = (byte)(((usSMC2OBC_SA6[22] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 384 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[23] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_2OcCh7 = (byte)((usSMC2OBC_SA6[23] & 0xFF) * 30 / 255.0F);
                        Pdu28_2OcCh8 = (byte)(((usSMC2OBC_SA6[23] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 386 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[24] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 388 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[25] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 390 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[26] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_3OcCh1 = (byte)((usSMC2OBC_SA6[26] & 0xFF) * 30 / 255.0F);
                        Pdu28_3OcCh2 = (byte)(((usSMC2OBC_SA6[26] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 392 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[27] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_3OcCh3 = (byte)((usSMC2OBC_SA6[27] & 0xFF) * 30 / 255.0F);
                        Pdu28_3OcCh4 = (byte)(((usSMC2OBC_SA6[27] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 394 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[28] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_3OcCh5 = (byte)((usSMC2OBC_SA6[28] & 0xFF) * 30 / 255.0F);
                        Pdu28_3OcCh6 = (byte)(((usSMC2OBC_SA6[28] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 396 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[29] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_3OcCh7 = (byte)((usSMC2OBC_SA6[29] & 0xFF) * 30 / 255.0F);
                        Pdu28_3OcCh8 = (byte)(((usSMC2OBC_SA6[29] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 398 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA6[30] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 400 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 402 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[1] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_4OcCh1 = (byte)((usSMC2OBC_SA7[1] & 0xFF) * 30 / 255.0F);
                        Pdu28_4OcCh2 = (byte)(((usSMC2OBC_SA7[1] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 404 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[2] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_4OcCh3 = (byte)((usSMC2OBC_SA7[2] & 0xFF) * 30 / 255.0F);
                        Pdu28_4OcCh4 = (byte)(((usSMC2OBC_SA7[2] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 406 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[3] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_4OcCh5 = (byte)((usSMC2OBC_SA7[3] & 0xFF) * 30 / 255.0F);
                        Pdu28_4OcCh6 = (byte)(((usSMC2OBC_SA7[3] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 408 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[4] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_4OcCh7 = (byte)((usSMC2OBC_SA7[4] & 0xFF) * 30 / 255.0F);
                        Pdu28_4OcCh8 = (byte)(((usSMC2OBC_SA7[4] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 410 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 412 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 414 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[7] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_5OcCh1 = (byte)((usSMC2OBC_SA7[7] & 0xFF) * 30 / 255.0F);
                        Pdu28_5OcCh2 = (byte)(((usSMC2OBC_SA7[7] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 416 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[8] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_5OcCh3 = (byte)((usSMC2OBC_SA7[8] & 0xFF) * 30 / 255.0F);
                        Pdu28_5OcCh4 = (byte)(((usSMC2OBC_SA7[8] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 418 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[9] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_5OcCh5 = (byte)((usSMC2OBC_SA7[9] & 0xFF) * 30 / 255.0F);
                        Pdu28_5OcCh6 = (byte)(((usSMC2OBC_SA7[9] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 420 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[10] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu28_5OcCh7 = (byte)((usSMC2OBC_SA7[10] & 0xFF) * 30 / 255.0F);
                        Pdu28_5OcCh8 = (byte)(((usSMC2OBC_SA7[10] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 422 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 424 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[12] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 426 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[13] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_1OcCh1 = (byte)((usSMC2OBC_SA7[13] & 0xFF) * 30 / 255.0F);
                        Pdu48_1OcCh2 = (byte)(((usSMC2OBC_SA7[13] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 428 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[14] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_1OcCh3 = (byte)((usSMC2OBC_SA7[14] & 0xFF) * 30 / 255.0F);
                        Pdu48_1OcCh4 = (byte)(((usSMC2OBC_SA7[14] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 430 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[15] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_1OcCh5 = (byte)((usSMC2OBC_SA7[15] & 0xFF) * 30 / 255.0F);
                        Pdu48_1OcCh6 = (byte)(((usSMC2OBC_SA7[15] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 432 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[16] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_1OcCh7 = (byte)((usSMC2OBC_SA7[16] & 0xFF) * 30 / 255.0F);
                        Pdu48_1OcCh8 = (byte)(((usSMC2OBC_SA7[16] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 434 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[17] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 436 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[18] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 438 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[19] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_2OcCh1 = (byte)((usSMC2OBC_SA7[19] & 0xFF) * 30 / 255.0F);
                        Pdu48_2OcCh2 = (byte)(((usSMC2OBC_SA7[19] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 440 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[20] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_2OcCh3 = (byte)((usSMC2OBC_SA7[20] & 0xFF) * 30 / 255.0F);
                        Pdu48_2OcCh4 = (byte)(((usSMC2OBC_SA7[20] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 442 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[21] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_2OcCh5 = (byte)((usSMC2OBC_SA7[21] & 0xFF) * 30 / 255.0F);
                        Pdu48_2OcCh6 = (byte)(((usSMC2OBC_SA7[21] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 444 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[22] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_2OcCh7 = (byte)((usSMC2OBC_SA7[22] & 0xFF) * 30 / 255.0F);
                        Pdu48_2OcCh8 = (byte)(((usSMC2OBC_SA7[22] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 446 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[23] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 448 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[24] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 450 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[25] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_3OcCh1 = (byte)((usSMC2OBC_SA7[25] & 0xFF) * 30 / 255.0F);
                        Pdu48_3OcCh2 = (byte)(((usSMC2OBC_SA7[25] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 452 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[26] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_3OcCh3 = (byte)((usSMC2OBC_SA7[26] & 0xFF) * 30 / 255.0F);
                        Pdu48_3OcCh4 = (byte)(((usSMC2OBC_SA7[26] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 454 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[27] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_3OcCh5 = (byte)((usSMC2OBC_SA7[27] & 0xFF) * 30 / 255.0F);
                        Pdu48_3OcCh6 = (byte)(((usSMC2OBC_SA7[27] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 456 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[28] = BitConverter.ToUInt16(bArray, iIndex);
                        Pdu48_3OcCh7 = (byte)((usSMC2OBC_SA7[28] & 0xFF) * 30 / 255.0F);
                        Pdu48_3OcCh8 = (byte)(((usSMC2OBC_SA7[28] & 0xFF) << 8) * 30 / 255.0F);
                        iIndex += 2; /* iIndex = 458 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usSMC2OBC_SA7[29] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 460 */


                        #endregion

                        #region BMS Data

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 462 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 464 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 466 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 468 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 470 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 472 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 474 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 476 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 478 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[9] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 480 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[10] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 482 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 484 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[12] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 486 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[13] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 488 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[14] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 490 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[15] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 492 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[16] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 494 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[17] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 496 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[18] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 498 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[19] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 500 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[20] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 502 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[21] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 504 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[22] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 506 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[23] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 508 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[24] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 510 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[25] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 512 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[26] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 514 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[27] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 516 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[28] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 518 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA1[29] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 520 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 522 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 524 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 526 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 528 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 530 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 532 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 534 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 536 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 538 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[9] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        iIndex += 2;    /* for 0x0D0D */  /* iIndex = 542 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[10] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 544 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 546 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[12] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 548 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[13] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 550 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[14] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 552 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[15] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 554 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[16] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 556 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[17] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 558 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[18] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 560 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[19] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 562 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[20] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 564 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[21] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 566 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[22] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 568 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[23] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 570 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[24] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 572 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[25] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 574 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[26] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 576 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[27] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 578 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[28] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 580 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usBMS2OBC_SA2[29] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 582 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 584 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 586 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 588 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 590 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 592 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[5] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 594 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[6] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 596 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[7] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 598 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[8] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 600 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[9] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 602 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[10] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 604 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[11] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 606 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[12] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 608 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[13] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 610 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[14] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 612 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[15] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 614 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[16] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 616 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[17] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 618 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[18] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 620 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[19] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 622 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[20] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 624 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[21] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 626 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[22] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 628 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usOBC2BMS_SA3[23] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 630 */

                        #endregion

                        #region Message Errors & Message Counts

                        MsgCntSmc = (byte)(bArray[iIndex] & 0xFF);
                        MsgCntEassc = (byte)(bArray[iIndex + 1] & 0xFF);
                        iIndex += 2; /* iIndex = 632 */

                        MsgCntPed = (byte)(bArray[iIndex] & 0xFF);
                        MsgCntBms = (byte)(bArray[iIndex + 1] & 0xFF);
                        iIndex += 2; /* iIndex = 634 */

                        MsgCntAlt = (byte)(bArray[iIndex] & 0xFF);
                        MsgCntPhins = (byte)(bArray[iIndex + 1] & 0xFF);
                        iIndex += 2; /* iIndex = 636 */

                        MsgCntTsu = (byte)(bArray[iIndex] & 0xFF);
                        MsgCntDvl = (byte)(bArray[iIndex + 1] & 0xFF);
                        iIndex += 2; /* iIndex = 638 */

                        #endregion

                        #region NAND Page, Block Number & Flags

                        iIndex = 674;

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usNPageNum = (ushort)(bArray[iIndex] & 0x7F);
                        iIndex += 2; /* iIndex = 676 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usNBlkNum = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 678 */

                        #region DVL Data

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[0] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 680 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[1] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 682 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[2] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 684 */

                        BtRes = Math.Sqrt(Math.Pow(usDvlData[0] / 1000.0, 2) + Math.Pow(usDvlData[1] / 1000.0, 2) + Math.Pow(usDvlData[2] / 1000.0, 2));

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[3] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 686 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[4] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 688 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[5] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 690 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[6] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 692 */

                        dAvgBeamRange = (usDvlData[3] + usDvlData[4] + usDvlData[5] + usDvlData[6]) / 4.0f;

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[7] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 694 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[8] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 696 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[9] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2;/* iIndex = 698 */

                        WtRes = Math.Sqrt(Math.Pow(usDvlData[7] / 1000.0, 2) + Math.Pow(usDvlData[8] / 1000.0, 2) + Math.Pow(usDvlData[9] / 1000.0, 2));

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        usDvlData[10] = BitConverter.ToInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 700 */

                        #endregion



                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            MMode = 1;
                        }
                        else
                        {
                            //PMode = 1;
                            MMode = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            AMode = 1;
                        }
                        else
                        {
                            AMode = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            SMode = 1;
                        }
                        else
                        {
                            SMode = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            RMode = 1;
                        }
                        else
                        {
                            RMode = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            Ps = 1;
                        }
                        else
                        {
                            Ps = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            Leak = 1;
                        }
                        else
                        {
                            Leak = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            Remote = 1;
                        }
                        else
                        {
                            Remote = 0;
                        }
                        iIndex += 2; /* iIndex = 702 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flPitchCmd = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 706 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flRollCmd = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 710 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        flYawCmd = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 714 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
                        Array.Reverse(bArray, iIndex + 4, 2);
                        Array.Reverse(bArray, iIndex + 6, 2);
#endif
                        dLat = BitConverter.ToUInt64(bArray, iIndex);
                        dLat = ((dLat * 180) / 0x7FFFFFFFFFFFFFFF);
                        if (dLat > 180)
                        {
                            dLat = 180 - dLat;
                        }
                        else
                        {
                            ;
                        }
                        iIndex += 8; /* iIndex = 722 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
                        Array.Reverse(bArray, iIndex + 4, 2);
                        Array.Reverse(bArray, iIndex + 6, 2);
#endif
                        dLong = BitConverter.ToUInt64(bArray, iIndex);
                        dLong = ((dLong * 180) / 0x7FFFFFFFFFFFFFFF);
                        if (dLong > 180)
                        {
                            dLong = 180 - dLong;
                        }
                        else
                        {
                            ;
                        }
                        iIndex += 8; /* iIndex = 730 */







                        iIndex += 2; /*spare*/  /* iIndex = 732 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        RefDepthScs = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 736 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        RefYawScs = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 740 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        SpeedScs = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* iIndex = 744 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PCLScs = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* iIndex = 748 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        DelScs = BitConverter.ToSingle(bArray, iIndex);

                        iIndex += 4; /* iIndex = 752 */

                        // iIndex = 752; 


                        /* CTD Parameters  Can be added here */


                        iIndex = 822;   /* iIndex = 822 */

                        psbNooflegs = bArray[iIndex + 1];


                        iIndex += 2;   /* iIndex = 824 */

                        for (int i = 0; i < psbNooflegs; i++)
                        {
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                            psuiLegDuration[i] = BitConverter.ToUInt32(bArray, iIndex);
                            iIndex += 4;
                            psbSpeed[i] = bArray[iIndex + 1];
                            iIndex += 2;
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                            psusDepth[i] = BitConverter.ToSingle(bArray, iIndex);
                            iIndex += 4;
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                            pssYawRef[i] = BitConverter.ToInt16(bArray, iIndex);
                            iIndex += 2;
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                            pfTurnRate[i] = BitConverter.ToSingle(bArray, iIndex);
                            iIndex += 4;
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                            psusDco[i] = BitConverter.ToUInt16(bArray, iIndex);
                            iIndex += 2;
#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                            psusCco[i] = BitConverter.ToUInt16(bArray, iIndex);
                            iIndex += 2;
                            psbSearchSide[i] = bArray[iIndex + 1];
                            iIndex += 1;
                            psbSearchPattern[i] = bArray[iIndex];
                            iIndex += 1;
                            psbRCL[i] = bArray[iIndex + 1];
                            iIndex += 1;
                            psbPCL[i] = bArray[iIndex];
                            iIndex += 1;
                            psbYCL[i] = bArray[iIndex + 1];
                            iIndex += 1;
                            psbDECL[i] = bArray[iIndex];
                            iIndex += 1;
                        }

                        iIndex = 1350;  /* iIndex = 1350 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        ROLL_INTG_ON_TIME = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1354 */
                        ROLL_INTG_ON_TIME = ROLL_INTG_ON_TIME * 10.0F;
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        ROLL_GAIN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1358 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        ROLL_RATE_GAIN_3KN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1362 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        ROLL_RATE_GAIN_6KN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1366 */
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        ROLL_CMD_LIM = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1370 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        YAW_GAIN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1374 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        YAW_RATE_GAIN_3KN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1378 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        YAW_RATE_GAIN_6KN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1382 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        YAW_INTG_GAIN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1386 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        YAW_CMD_LIM = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1390 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        DEPTH_GAIN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1394 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        DERR_MAX = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1398 */


#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PITCH_GAIN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1402 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PITCH_RATE_GAIN_3KN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1406 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PITCH_RATE_GAIN_6KN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1410 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PITCH_CMD_LIM = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* index = 1414 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        FIN_CMD_MAX = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;  /* index = 1418 */


                        // Array.Reverse(bArray, iIndex, 2);
                        // RunNumber = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* index = 1420 */

                        iIndex += 2; /* index = 1422 */

                        iIndex += 2; /* index = 1424 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        PresetFlo = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* index = 1426 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        presetDco = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* index = 1428 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PresetRuntime = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /* index = 1432 */

                        /*   Array.Reverse(bArray, iIndex, 2);
                           presetDco = BitConverter.ToUInt16(bArray, iIndex); 
                           iIndex += 2; */


#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        PresetMotorSampleRate = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* index = 1434 */

                        iIndex = 1434;

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            g_ucRT4mMotor = 1;
                        }
                        else
                        {
                            g_ucRT4mMotor = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            g_ucCommFail4mPED = 1;
                        }
                        else
                        {
                            g_ucCommFail4mPED = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            g_ucCommFail4mALTI = 1;
                        }
                        else
                        {
                            g_ucCommFail4mALTI = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            g_ucCommFail4mPHINS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mPHINS = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            g_ucCommFail4mMPIDS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mMPIDS = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            g_ucRunTimeRT = 1;
                        }
                        else
                        {
                            g_ucRunTimeRT = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            g_ucLCS_Indication = 1;
                        }
                        else
                        {
                            g_ucLCS_Indication = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            g_ucCommFail4mEASSC = 1;
                        }
                        else
                        {
                            g_ucCommFail4mEASSC = 0;
                        }
                        iIndex += 1; /* index = 1435*/

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            g_ucMission_Stop = 1;
                        }
                        else
                        {
                            g_ucMission_Stop = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            g_ucMission_Abort = 1;
                        }
                        else
                        {
                            g_ucMission_Abort = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            g_ucRT4m_DCO = 1;
                        }
                        else
                        {
                            g_ucRT4m_DCO = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            g_ucBoundaryConditionRT = 1;
                        }
                        else
                        {
                            g_ucBoundaryConditionRT = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            RT4m_MPIDS_SA3 = 1;
                        }
                        else
                        {
                            RT4m_MPIDS_SA3 = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            RT4m_MPIDS_SA4 = 1;
                        }
                        else
                        {
                            RT4m_MPIDS_SA4 = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            RT4m_MPIDS_SA5 = 1;
                        }
                        else
                        {
                            RT4m_MPIDS_SA5 = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            RT4m_MPIDS = 1;
                        }
                        else
                        {
                            RT4m_MPIDS = 0;
                        }




                        iIndex += 1; /* index = 1436 */

                        if ((bArray[iIndex] & 0x0001) == 0x0001)
                        {
                            g_ucCommFail4mBMS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mBMS = 0;
                        }

                        if ((bArray[iIndex] & 0x0002) == 0x0002)
                        {
                            g_ucCommFail4mIPS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mIPS = 0;
                        }

                        if ((bArray[iIndex] & 0x0004) == 0x0004)
                        {
                            g_ucIPSError_MPSSWFlag = 1;
                        }
                        else
                        {
                            g_ucIPSError_MPSSWFlag = 0;
                        }

                        if ((bArray[iIndex] & 0x0008) == 0x0008)
                        {
                            g_ucIPSError_ConfigCmdFlag = 1;
                        }
                        else
                        {
                            g_ucIPSError_ConfigCmdFlag = 0;
                        }

                        if ((bArray[iIndex] & 0x0010) == 0x0010)
                        {
                            g_ucIPSError_VehicleStateAvailFlag = 1;
                        }
                        else
                        {
                            g_ucIPSError_VehicleStateAvailFlag = 0;
                        }

                        if ((bArray[iIndex] & 0x0020) == 0x0020)
                        {
                            g_ucIPSError_WpAvailFlag = 1;
                        }
                        else
                        {
                            g_ucIPSError_WpAvailFlag = 0;
                        }

                        if ((bArray[iIndex] & 0x0040) == 0x0040)
                        {
                            g_ucIPSError_FirstWpRangeFlag = 1;
                        }
                        else
                        {
                            g_ucIPSError_FirstWpRangeFlag = 0;
                        }

                        if ((bArray[iIndex] & 0x0080) == 0x0080)
                        {
                            g_ucIPS_Error_SCMFlag = 1;
                        }
                        else
                        {
                            g_ucIPS_Error_SCMFlag = 0;
                        }

                        iIndex += 1; /*index = 1437*/

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            g_ucIPSError_FLSAvailFlag = 1;
                        }
                        else
                        {
                            g_ucIPSError_FLSAvailFlag = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            g_ucRT4mIPS = 1;
                        }
                        else
                        {
                            g_ucRT4mIPS = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            g_ucFault4mMotor_flag = 1;
                        }
                        else
                        {
                            g_ucFault4mMotor_flag = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            g_ucMotor1Under_Voltage_flag = 1;
                        }
                        else
                        {
                            g_ucMotor1Under_Voltage_flag = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            g_ucMotor1Over_Voltage_flag = 1;
                        }
                        else
                        {
                            g_ucMotor1Over_Voltage_flag = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            g_Motor1_Over_Current_Flag = 1;
                        }
                        else
                        {
                            g_Motor1_Over_Current_Flag = 0;
                        }

                        iIndex += 1; /*index = 1438*/
                        g_ucMPSNoGuidanceRT = bArray[iIndex];
                        iIndex += 2;     /*index = 1440*/


#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        Waypoint_ID_4m_IPS = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;        /*index = 1442*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Headingrate_4m_IPS = BitConverter.ToSingle(bArray, iIndex);
                        Headingrate_4m_IPS = Headingrate_4m_IPS * RADTODEG;
                        iIndex += 4;    /*index = 1446*/
#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Speed_4m_IPS = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;    /*index = 1450*/


#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        SONAR_TRANSMIT_DEPTH_LIM = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;/*index = 1454*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        YAW_RATE_LOOP_PROP_GAIN = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;    /*index = 1458*/

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            FLSC_Power_Status = 1;
                        }
                        else
                        {
                            FLSC_Power_Status = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            FASC_Power_Status = 1;
                        }
                        else
                        {
                            FASC_Power_Status = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            AASC_Power_Status = 1;
                        }
                        else
                        {
                            AASC_Power_Status = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            AMC_Power_Status = 1;
                        }
                        else
                        {
                            AMC_Power_Status = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            BLSC_Power_Status = 1;
                        }
                        else
                        {
                            BLSC_Power_Status = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            FLSC_Transmit_Status = 1;
                        }
                        else
                        {
                            FLSC_Transmit_Status = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            FASC_Transmit_Status = 1;
                        }
                        else
                        {
                            FASC_Transmit_Status = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            AMC_Transmit_Status = 1;
                        }
                        else
                        {
                            AMC_Transmit_Status = 0;
                        }
                        iIndex += 2;    /*index = 1460*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Day = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;    /*index = 1464*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Month = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;    /*index = 1468*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Year = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;    /*index = 1472*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Hours = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;    /*index = 1476*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Minutes = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;    /*index = 1480*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Seconds = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4; /*index = 1484*/

                        iIndex = 438;
                        ushort temp = BitConverter.ToUInt16(bArray, iIndex);

                        FLSCCurrent = (byte)(temp & 0x00FF);

                        iIndex = 442;
                        ushort temp1 = BitConverter.ToUInt16(bArray, iIndex);

                        FASCCurrent = (byte)(temp1 & 0x00FF);

                        iIndex = 1484;

                        StatusCode0 = bArray[iIndex + 1];
                        iIndex += 1;    /*index = 1485*/

                        StatusCode1 = bArray[iIndex];
                        iIndex += 1;    /*index = 1486*/

                        StatusCode2 = bArray[iIndex + 1];
                        iIndex += 1;    /*index = 1487*/

                        StatusCode3 = bArray[iIndex];
                        iIndex += 1;/*index = 1488*/

                        StatusCode4 = bArray[iIndex + 1];
                        iIndex += 1;/*index = 1489*/

                        StatusCode5 = bArray[iIndex];
                        iIndex += 1;/*index = 1490*/

                        StatusCode6 = bArray[iIndex + 1];
                        iIndex += 1;    /*index = 1491*/

                        StatusCode7 = bArray[iIndex];
                        iIndex += 1;/*index = 1492*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PosX = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;    /*index = 1496*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PosY = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;/*index = 1500*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        PosZ = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;/*index = 1504*/

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        Uvel = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* 1508 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        WpDepth = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* index = 1510 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        lcs_static_test_rpm = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* index = 1512 */

                        psbNoofTargets = bArray[iIndex + 1];
                        iIndex += 1;/* index = 1514 */


                        for (int i = 0; i < 8; i++)
                        {
                            targetid[i] = bArray[iIndex + 1];
                            iIndex += 1;


                            trackquality[i] = BitConverter.ToSingle(bArray, iIndex);
                            iIndex += 4;

                            bearing[i] = BitConverter.ToSingle(bArray, iIndex);
                            iIndex += 4;

                            bearingrate[i] = BitConverter.ToSingle(bArray, iIndex);
                            iIndex += 4;

                            snr[i] = BitConverter.ToSingle(bArray, iIndex);
                            iIndex += 4;

                            targetrangeflag[i] = bArray[iIndex + 1];
                            iIndex += 1;

                            range[i] = BitConverter.ToSingle(bArray, iIndex);
                            iIndex += 4;

                            rangerate[i] = BitConverter.ToSingle(bArray, iIndex);
                            iIndex += 4;

                            iIndex = iIndex + 28;
                        }


                        fasc_lat = BitConverter.ToDouble(bArray, iIndex);
                        iIndex += 8;

                        fasc_long = BitConverter.ToDouble(bArray, iIndex);
                        iIndex += 8;

                        fasc_day = bArray[iIndex];
                        iIndex += 1;

                        fasc_month = bArray[iIndex + 1];
                        iIndex += 1;


                        fasc_year = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        fasc_hour = bArray[iIndex];
                        iIndex += 1;

                        fasc_minute = bArray[iIndex + 1];
                        iIndex += 1;

                        fasc_second = bArray[iIndex];
                        iIndex += 1;


                        fasc_milliseconds = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        #endregion

                        #region FAS Configuration Packet

                        iIndex = 1970;

                        ActiveTransmissionInterval = bArray[iIndex];
                        iIndex += 1;

                        noofpings = bArray[iIndex + 1];
                        iIndex += 1;

                        PRI = bArray[iIndex];
                        iIndex += 1;

                        FrequencyRangeStart = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        FrequencyRangeEnd = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        PassiveGain = bArray[iIndex];
                        iIndex += 1;

                        ActiveGainMin = bArray[iIndex + 1];
                        iIndex += 1;

                        ActiveGainMax = bArray[iIndex];
                        iIndex += 1;

                        scSourceLevel = bArray[iIndex + 1];
                        iIndex += 1;

                        #endregion

                        #region SSS Configuration Packet

                        iIndex = 1982;

                        SSS_SystemMode = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;

                        SSS_Altitude = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;


                        SSS_SpeedOfSurvey = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;


                        SSS_MaxSwathToRecord = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;

                        SSS_SpeedOfSound = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;


                        SSS_RawDataRecord = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;

                        #endregion

                        #region MBE Configuration Packet

                        iIndex = 2006;

                        MBE_SystemMode = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;


                        MBE_Altitude = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;


                        MBE_SpeedOfSurvey = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;


                        MBE_MaxScanLinesToRecord = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;


                        MBE_SpeedOfSound = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;


                        MBE_RawDataRecord = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;

                        #endregion

                        #region FLS Configuration Packet
                        iIndex = 2030;

                        TxTriggerDepth = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;

                        FlsPRI = bArray[iIndex];
                        iIndex += 1;

                        FlsPulseLength = bArray[iIndex];
                        iIndex += 1;

                        CFARThreshold = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;

                        iIndex += 3;

                        FlsSourceLevel = bArray[iIndex];
                        iIndex += 1;

                        #endregion

                        #region FDS Fault Summary
                        iIndex = 2044;
                        FlsFaultSummary = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        FlsDay = bArray[iIndex + 1];
                        iIndex += 1;

                        FlsMonth = bArray[iIndex];
                        iIndex += 1;

                        FlsYear = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        FlsHour = bArray[iIndex + 1];
                        iIndex += 1;

                        FlsMin = bArray[iIndex];
                        iIndex += 1;

                        FlsSec = bArray[iIndex + 1];
                        iIndex += 1;

                        FlsMsec = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        iIndex += 9;

                        FasFaultSummary = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        FasDay = bArray[iIndex + 1];
                        iIndex += 1;

                        FasMonth = bArray[iIndex];
                        iIndex += 1;

                        FasYear = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        FasHour = bArray[iIndex + 1];
                        iIndex += 1;

                        FasMin = bArray[iIndex];
                        iIndex += 1;

                        FasSec = bArray[iIndex + 1];
                        iIndex += 1;

                        FasMsec = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        iIndex += 9;

                        AasFaultSummary = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        AasDay = bArray[iIndex + 1];
                        iIndex += 1;

                        AasMonth = bArray[iIndex];
                        iIndex += 1;

                        AasYear = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        AasHour = bArray[iIndex + 1];
                        iIndex += 1;

                        AasMin = bArray[iIndex];
                        iIndex += 1;

                        AasSec = bArray[iIndex + 1];
                        iIndex += 1;

                        AasMsec = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        iIndex += 9;

                        AmFaultSummary = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        AmDay = bArray[iIndex + 1];
                        iIndex += 1;

                        AmMonth = bArray[iIndex];
                        iIndex += 1;

                        AmYear = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        AmHour = bArray[iIndex + 1];
                        iIndex += 1;

                        AmMin = bArray[iIndex];
                        iIndex += 1;

                        AmSec = bArray[iIndex + 1];
                        iIndex += 1;

                        AmMsec = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        iIndex += 9;

                        BlsFaultSummary = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        BlsDay = bArray[iIndex + 1];
                        iIndex += 1;

                        BlsMonth = bArray[iIndex];
                        iIndex += 1;

                        BlsYear = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        BlsHour = bArray[iIndex + 1];
                        iIndex += 1;

                        BlsMin = bArray[iIndex];
                        iIndex += 1;

                        BlsSec = bArray[iIndex + 1];
                        iIndex += 1;

                        BlsMsec = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        iIndex += 9;

                        #endregion

                        #region Static_Test_Parameters & secss parameters

                        iIndex = 2144;

                        if ((bArray[iIndex] & 0x0001) == 0x0001)
                        {
                            static_test_flag = 1;
                        }
                        else
                        {
                            static_test_flag = 0;
                        }

                        if ((bArray[iIndex] & 0x0002) == 0x0002)
                        {
                            static_test_start = 1;
                        }
                        else
                        {
                            static_test_start = 0;
                        }

                        if ((bArray[iIndex] & 0x0004) == 0x0004)
                        {
                            distress_ack = 1;
                        }
                        else
                        {
                            distress_ack = 0;
                        }

                        if ((bArray[iIndex] & 0x0008) == 0x0008)
                        {
                            erasure_start = 1;
                        }
                        else
                        {
                            erasure_start = 0;
                        }

                        if ((bArray[iIndex] & 0x0010) == 0x0010)
                        {
                            erasure_stop = 1;
                        }
                        else
                        {
                            erasure_stop = 0;
                        }

                        if ((bArray[iIndex] & 0x0020) == 0x0020)
                        {
                            shutdown = 1;
                        }
                        else
                        {
                            shutdown = 0;
                        }
                        iIndex += 2; /*iIndex = 2146; */

                        iIndex += 2; //lcs_static_test_rpm added second time  iIndex = 2148;

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        static_test_start_time = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;    /*iIndex = 2152 */

#if NAND_EXTRACT
                        Array.Reverse(bArray, iIndex, 2);
                        Array.Reverse(bArray, iIndex + 2, 2);
#endif
                        static_test_stop_time = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;    /*iIndex = 2156 */

                        #endregion

                        #region IPS Detection Packet

                        image_number = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        no_of_pings = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        no_of_sample = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;

                        image_st_timestamp = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;

                        timestamp_incr = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;

                        total_no_of_detections = bArray[iIndex + 1];
                        iIndex += 1;

                        detect_pkt_no_in_cycle = bArray[iIndex + 1];
                        iIndex += 1;

                        no_of_det_in_pkt = bArray[iIndex + 1];
                        iIndex += 1;

                        for (int i = 0; i < 10; i++)
                        {
                            detclass[i] = bArray[iIndex + 1];
                            iIndex += 1;

                            detx1[i] = BitConverter.ToUInt16(bArray, iIndex);
                            iIndex += 2;

                            dety1[i] = BitConverter.ToUInt16(bArray, iIndex);
                            iIndex += 2;

                            detx2[i] = BitConverter.ToUInt16(bArray, iIndex);
                            iIndex += 2;

                            dety2[i] = BitConverter.ToUInt16(bArray, iIndex);
                            iIndex += 2;

                            dlat1[i] = BitConverter.ToDouble(bArray, iIndex);
                            iIndex += 8;

                            dlong1[i] = BitConverter.ToDouble(bArray, iIndex);
                            iIndex += 8;

                            dlat2[i] = BitConverter.ToDouble(bArray, iIndex);
                            iIndex += 8;

                            dlong2[i] = BitConverter.ToDouble(bArray, iIndex);
                            iIndex += 8;
                        }

                        #endregion

                        #region DataPacket2Am

                        iIndex = 2370;

                        AmMcsMissionAbort = BitConverter.ToUInt32(bArray, iIndex);
                        iIndex += 4;

                        AmMcsMissionChange = bArray[iIndex + 1];
                        iIndex += 1;

                        AmMcsRecvSurf = bArray[iIndex + 1];
                        iIndex += 1;

                        AmMcsLat = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;

                        AmMcsLong = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;

                        AmMcsAlt = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4;  /* index = 2388*/

                        #endregion

                        #region ETHERNET COMM FAIL FLAGS AND AM DATA TRANSMIT ENABLE FLAG

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            g_ucCommFail4mLCS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mLCS = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            g_ucCommFail4mCSS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mCSS = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            g_ucCommFail4mNCS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mNCS = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            g_ucCommFail4mFLSC = 1;
                        }
                        else
                        {
                            g_ucCommFail4mFLSC = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            g_ucCommFail4mSSSC = 1;
                        }
                        else
                        {
                            g_ucCommFail4mSSSC = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            g_ucCommFail4mMBE = 1;
                        }
                        else
                        {
                            g_ucCommFail4mMBE = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            g_ucCommFail4mBLSC = 1;
                        }
                        else
                        {
                            g_ucCommFail4mBLSC = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            g_ucCommFail4mAASC = 1;
                        }
                        else
                        {
                            g_ucCommFail4mAASC = 0;
                        }
                        iIndex += 1;    /* index = 2389*/

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            g_ucCommFail4mFASC = 1;
                        }
                        else
                        {
                            g_ucCommFail4mFASC = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            g_ucCommFail4mAMC = 1;
                        }
                        else
                        {
                            g_ucCommFail4mAMC = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            g_ucCommFail4mLPRS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mLPRS = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            g_ucCommFail4mIPS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mIPS = 0;
                        }
                        iIndex += 1; /* index = 2390*/

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            g_ucCommFail4mELINT = 1;
                        }
                        else
                        {
                            g_ucCommFail4mELINT = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            g_ucCommFail4mMSS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mMSS = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            g_ucCommFail4mSCS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mSCS = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            g_ucCommFail4mSECSS = 1;
                        }
                        else
                        {
                            g_ucCommFail4mSECSS = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            g_ucAMTransmitEnableFlag = 1;
                        }
                        else
                        {
                            g_ucAMTransmitEnableFlag = 0;
                        }
                        iIndex += 2;        /* index = 2392*/

                        #endregion

                        #region FAS TARGET COUNT

#if NAND_EXTRACT
                        Array.Reverse(bArray, 1418, 2);
#endif
                        psusFasTargetCount = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2;    /* index = 2394*/

                        #endregion

                        #region PINGER START MISSION FLAG

                        pinger_st_mission_flag = bArray[iIndex + 1];
                        iIndex += 2;

                        #endregion

                        #region Recovery Conditions Packet Flags , New Flags , MAST, DVL Altitude, INS Resultant Velocity

                        iIndex = 2396;

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            Rc_ubfMPSConfigCmdError = 1;
                        }
                        else
                        {
                            Rc_ubfMPSConfigCmdError = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            Rc_ubfMPSBeyondRange = 1;
                        }
                        else
                        {
                            Rc_ubfMPSBeyondRange = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            Rc_ubfPHINSCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfPHINSCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            Rc_ubfPEDCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfPEDCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            Rc_ubfMPIDSCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfMPIDSCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            Rc_ubfMASTCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfMASTCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            Rc_ubfLPRSCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfLPRSCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            Rc_ubfLCSWirelessCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfLCSWirelessCommFail = 0;
                        }
                        iIndex += 1;

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            Rc_ubfLCSWiredCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfLCSWiredCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            Rc_ubfELINTCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfELINTCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            Rc_ubfEASSCCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfEASSCCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            Rc_ubfIPSCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfIPSCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            Rc_ubfCTDCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfCTDCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            Rc_ubfCSSCommFailNA = 1;
                        }
                        else
                        {
                            Rc_ubfCSSCommFailNA = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            Rc_ubfBMSCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfBMSCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            Rc_ubfAltimeterCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfAltimeterCommFail = 0;
                        }
                        iIndex += 1;

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            Rc_ubfSONARCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfSONARCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            Rc_ubfSCSCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfSCSCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            Rc_ubfMPSWaypointNA = 1;
                        }
                        else
                        {
                            Rc_ubfMPSWaypointNA = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            Rc_ubfMPSWaypointComplete = 1;
                        }
                        else
                        {
                            Rc_ubfMPSWaypointComplete = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            Rc_ubfPSonIndication = 1;
                        }
                        else
                        {
                            Rc_ubfPSonIndication = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            Rc_ubfMotorRT = 1;
                        }
                        else
                        {
                            Rc_ubfMotorRT = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            Rc_ubfStop = 1;
                        }
                        else
                        {
                            Rc_ubfStop = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            Rc_ubfAbort = 1;
                        }
                        else
                        {
                            Rc_ubfAbort = 0;
                        }
                        iIndex += 1;

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            Rc_ubfRunTimeRT = 1;
                        }
                        else
                        {
                            Rc_ubfRunTimeRT = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            Rc_ubfLCS_Indication = 1;
                        }
                        else
                        {
                            Rc_ubfLCS_Indication = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            Rc_ubfDCO = 1;
                        }
                        else
                        {
                            Rc_ubfDCO = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            Rc_ubfIpsRT = 1;
                        }
                        else
                        {
                            Rc_ubfIpsRT = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            Rc_ubfMPSVehicleStateNA = 1;
                        }
                        else
                        {
                            Rc_ubfMPSVehicleStateNA = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            Rc_ubfMPSSWError = 1;
                        }
                        else
                        {
                            Rc_ubfMPSSWError = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            Rc_ubfMPSSafetyCorridorMonitor = 1;
                        }
                        else
                        {
                            Rc_ubfMPSSafetyCorridorMonitor = 0;
                        }

                        if ((bArray[iIndex] & 0x80) == 0x80)
                        {
                            Rc_ubfMPSFLSNA = 1;
                        }
                        else
                        {
                            Rc_ubfMPSFLSNA = 0;
                        }
                        iIndex += 1;

                        if ((bArray[iIndex] & 0x01) == 0x01)
                        {
                            Rc_ubfSecSSCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfSecSSCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x02) == 0x02)
                        {
                            Rc_ubfTSUCommFail = 1;
                        }
                        else
                        {
                            Rc_ubfTSUCommFail = 0;
                        }

                        if ((bArray[iIndex] & 0x04) == 0x04)
                        {
                            Rc_ubfMPSNoGuidance = 1;
                        }
                        else
                        {
                            Rc_ubfMPSNoGuidance = 0;
                        }

                        if ((bArray[iIndex] & 0x08) == 0x08)
                        {
                            RecordPrelaunchDataFlag = 1;
                        }
                        else
                        {
                            RecordPrelaunchDataFlag = 0;
                        }

                        if ((bArray[iIndex] & 0x10) == 0x10)
                        {
                            DynamicCmdSharingFlag = 1;
                        }
                        else
                        {
                            DynamicCmdSharingFlag = 0;
                        }

                        if ((bArray[iIndex] & 0x20) == 0x20)
                        {
                            VelocityControlSystem = 1;
                        }
                        else
                        {
                            VelocityControlSystem = 0;
                        }

                        if ((bArray[iIndex] & 0x40) == 0x40)
                        {
                            AltitudeControlSystem = 1;
                        }
                        else
                        {
                            AltitudeControlSystem = 0;
                        }
                        iIndex += 1;

                        DVL_Altitude = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* 1508 */

                        MCS2MAST[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MCS2MAST[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MCS2MAST[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MCS2MAST[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MCS2MAST[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MAST2MCS[0] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MAST2MCS[1] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MAST2MCS[2] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MAST2MCS[3] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        MAST2MCS[4] = BitConverter.ToUInt16(bArray, iIndex);
                        iIndex += 2; /* iIndex = 540 */

                        INS_Resultant_Velocity = BitConverter.ToSingle(bArray, iIndex);
                        iIndex += 4; /* 1508 */

                        #endregion

                        //if ((bArray[NAND_PACKET_SIZE - 2] == 0xE2 && bArray[NAND_PACKET_SIZE - 1] == 0xE2) || (bArray[NAND_PACKET_SIZE - 2] == 0xE3 && bArray[NAND_PACKET_SIZE - 1] == 0xE3))
                        if (/*(bArray[NAND_PACKET_SIZE - 2] == 0xE1 && bArray[NAND_PACKET_SIZE - 1] == 0xE1) ||*/ (bArray[NAND_PACKET_SIZE - 2] == 0xE2 && bArray[NAND_PACKET_SIZE - 1] == 0xE2) || (bArray[NAND_PACKET_SIZE - 2] == 0xE3 && bArray[NAND_PACKET_SIZE - 1] == 0xE3) /*|| (bArray[NAND_PACKET_SIZE - 2] == 0xE5 && bArray[NAND_PACKET_SIZE - 1] == 0xE5)*/)
                        {
                            Array.Reverse(bArray, NAND_PACKET_SIZE - 2, 2);
                            usFooter = BitConverter.ToUInt16(bArray, NAND_PACKET_SIZE - 2);
                            flADC0 = Convert_ADC_volt(usADC0);
                            flADC0 *= 2.5;
                            flADC1 = Convert_ADC_volt(usADC1);
                            flADC1 *= 10.0;

                            for (int i = 0; i < 5; i++)
                            {
                                swMAST.Write("{0} ", MCS2MAST[i]);
                            }

                            for (int j = 0; j < 5; j++)
                            {
                                swMAST.Write("{0} ", MAST2MCS[j]);
                            }
                            swMAST.Write("{0} {1}", Timer_100ms_Count, g_dTime);

                            swMAST.WriteLine();
                            swMAST.Flush();

                            swSecSS.WriteLine("{0} {1} {2} {3}", distress_ack, erasure_start, erasure_stop, shutdown);
                            swSecSS.Flush();

                            swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} ", Timer_100ms_Count, psbNoofTargets, fasc_lat, fasc_long, fasc_day, fasc_month,
fasc_year, fasc_hour, fasc_minute, fasc_second, fasc_milliseconds);
                            swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} ", ActiveTransmissionInterval, noofpings, PRI, FrequencyRangeStart, FrequencyRangeEnd,
PassiveGain, ActiveGainMin, ActiveGainMax, scSourceLevel);
                            swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} ", SSS_SystemMode, SSS_Altitude, SSS_SpeedOfSurvey, SSS_MaxSwathToRecord,
SSS_SpeedOfSound, SSS_RawDataRecord, MBE_SystemMode, MBE_Altitude, MBE_SpeedOfSurvey, MBE_MaxScanLinesToRecord, MBE_SpeedOfSound,
MBE_RawDataRecord);
                            swSONAR.Write("{0} {1} {2} {3} {4} ", TxTriggerDepth, FlsPRI, FlsPulseLength, CFARThreshold, FlsSourceLevel);
                            swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", FlsFaultSummary, FlsDay, FlsMonth, FlsYear, FlsHour, FlsMin, FlsSec, FlsMsec);
                            swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", FasFaultSummary, FasDay, FasMonth, FasYear, FasHour, FasMin, FasSec, FasMsec);
                            swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", AasFaultSummary, AasDay, AasMonth, AasYear, AasHour, AasMin, AasSec, AasMsec);
                            swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", AmFaultSummary, AmDay, AmMonth, AmYear, AmHour, AmMin, AmSec, AmMsec);
                            swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", BlsFaultSummary, BlsDay, BlsMonth, BlsYear, BlsHour, BlsMin, BlsSec, BlsMsec);

                            for (int i = 0; i < 0/*psbNoofTargets*/; i++)
                            {
                                swSONAR.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", targetid[i], trackquality[i], bearing[i], bearingrate[i], snr[i], targetrangeflag[i],
range[i], rangerate[i]);
                            }

                            swSONAR.WriteLine("{0} {1} {2} {3} {4} {5} {6}", AmMcsMissionAbort, AmMcsMissionChange, AmMcsRecvSurf, AmMcsLat, AmMcsLong, AmMcsAlt,
psusFasTargetCount);
                            swSONAR.Flush();

                            swMCS_RTC_Clock.WriteLine("{0} {1} {2} {3} {4} {5} {6}", Timer_100ms_Count, Day, Month, Year, Hours, Minutes, Seconds);
                            swMCS_RTC_Clock.Flush();

                            swPhins.Write("{0} {1} {2} {3} ", usHeader, flADC0, flADC1, flRoll);
                            swPhins.Write("{0} {1} {2} {3} {4} ", flPitch, flYaw, flRollRate, flPitchRate, flYawRate);
                            swPhins.Write("{0} {1} {2} {3} {4} ", flLat, flLong, flAlt, flAltDepth, flAltTemp);
                            swPhins.Write("{0} {1} {2} ", SysStat1, SysStat2, AlgoStat1);
                            swPhins.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16} {17} {18} {19} {20} {21} {22} {23} {24} {25} {26} {27}", AlgoStat2, SenStat1,
SenStat2, usNPageNum, usNBlkNum, usFooter, Timer_100ms_Count, g_dTime, g_dTime1, ulTime2, Nvel, Evel, Dvel, flRollCmd, flPitchCmd, flYawCmd,
dLat, dLong, PosX, PosY, PosZ, Uvel, RecordPrelaunchDataFlag, DynamicCmdSharingFlag, VelocityControlSystem, AltitudeControlSystem, DVL_Altitude, INS_Resultant_Velocity);
                            swPhins.Flush();

                            swRemote.WriteLine("{0} {1} {2} {3} {4}", RefDepthScs, RefYawScs, SpeedScs, PCLScs, DelScs);
                            swRemote.Flush();

                            swHealth.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16} {17} {18} {19} {20} {21} {22} {23} {24}",
HBms, HPed, HSmc, HEassc, HAlt, HCtd, HTsu, HPhins, IPS_MDS_HEALTH, HMcs, HLPRS, HELINT, HIps, HSSS, HSECSS, HMBE, HFls, HFds, HAM, HBLS,
HAAS, HFAS, HCSS, HSCS, HMSS);
                            swHealth.Flush();

                            for (int i = 0; i < 11; i++)
                            {
                                swDvl.Write("{0} ", usDvlData[i]);
                            }
                            swDvl.WriteLine("{0} {1} {2} {3} {4} {5}", Timer_100ms_Count, g_dTime, BtRes, WtRes, RunNumber, dAvgBeamRange);

                            for (int z = 0; z < 8; z++)
                            {
                                swTsu.Write("{0} ", usTsuData[z]);
                            }
                            swTsu.Write("{0} {1}", Timer_100ms_Count, g_dTime);

                            swTsu.WriteLine();
                            swTsu.Flush();

                            for (int i = 0; i < 17; i++)
                            {
                                swBldc.Write("{0} ", usBLDC2OBC[i]);
                            }

                            for (int j = 0; j < 6; j++)
                            {
                                swBldc.Write("{0} ", usOBC2BLDC[j]);
                            }
                            swBldc.Write("{0} {1}", Timer_100ms_Count, g_dTime);

                            swBldc.WriteLine();
                            swBldc.Flush();

                            for (int k = 0; k < 4; k++)
                            {
                                swEassc.Write("{0} ", usOBC2FASC_SA1[k]);
                            }

                            for (int y = 0; y < 1; y++)
                            {
                                swEassc.Write("{0} ", usOBC2FASC_SA2[y]);
                            }

                            for (int x = 0; x < 20; x++)
                            {
                                swEassc.Write("{0} ", usFASC2OBC_SA3[x]);
                            }

                            swEassc.Write("{0} {1}", Timer_100ms_Count, g_dTime);

                            swEassc.WriteLine();
                            swEassc.Flush();

                            for (int s = 0; s < 9; s++)
                            {
                                swSmc.Write("{0} ", usOBC2SMC_SA1[s]);
                            }

                            for (int s = 0; s < 9; s++)
                            {
                                swSmc.Write("{0} ", usSMC2OBC_SA2[s]);
                            }

                            for (int p = 0; p < 8; p++)
                            {
                                swSmc.Write("{0} ", usSMC2OBC_SA3[p]);
                            }

                            for (int q = 0; q < 20; q++)
                            {
                                swSmc.Write("{0} ", usSMC2OBC_SA4[q]);
                            }

                            for (int r = 0; r < 15; r++)
                            {
                                swSmc.Write("{0} ", usSMC2OBC_SA5[r]);
                            }

                            for (int r = 0; r < 31; r++)
                            {
                                swSmc.Write("{0} ", usSMC2OBC_SA6[r]);
                            }

                            for (int r = 0; r < 30; r++)
                            {
                                swSmc.Write("{0} ", usSMC2OBC_SA7[r]);
                            }

                            swSmc.Write("{0} {1} ", Timer_100ms_Count, g_dTime);

                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", FLSCCurrent, FASCCurrent, FLSC_Power_Status, FASC_Power_Status, AASC_Power_Status,
AMC_Power_Status, BLSC_Power_Status, FLSC_Transmit_Status, FASC_Transmit_Status, AMC_Transmit_Status);

                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} ", TimerSeqOc, Pdu150OcCh1, Pdu150OcCh2, Pdu150OcCh3, Pdu150OcCh4, Pcu28Oc, Pcu28Ov, Pcu48Oc, Pcu48Ov);
                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", Pdu28_1OcCh1, Pdu28_1OcCh2, Pdu28_1OcCh3, Pdu28_1OcCh4, Pdu28_1OcCh5, Pdu28_1OcCh6, Pdu28_1OcCh7, Pdu28_1OcCh8);
                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", Pdu28_2OcCh1, Pdu28_2OcCh2, Pdu28_2OcCh3, Pdu28_2OcCh4, Pdu28_2OcCh5, Pdu28_2OcCh6, Pdu28_2OcCh7, Pdu28_2OcCh8);
                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", Pdu28_3OcCh1, Pdu28_3OcCh2, Pdu28_3OcCh3, Pdu28_3OcCh4, Pdu28_3OcCh5, Pdu28_3OcCh6, Pdu28_3OcCh7, Pdu28_3OcCh8);
                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", Pdu28_4OcCh1, Pdu28_4OcCh2, Pdu28_4OcCh3, Pdu28_4OcCh4, Pdu28_4OcCh5, Pdu28_4OcCh6, Pdu28_4OcCh7, Pdu28_4OcCh8);
                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", Pdu28_5OcCh1, Pdu28_5OcCh2, Pdu28_5OcCh3, Pdu28_5OcCh4, Pdu28_5OcCh5, Pdu28_5OcCh6, Pdu28_5OcCh7, Pdu28_5OcCh8);
                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", Pdu48_1OcCh1, Pdu48_1OcCh2, Pdu48_1OcCh3, Pdu48_1OcCh4, Pdu48_1OcCh5, Pdu48_1OcCh6, Pdu48_1OcCh7, Pdu48_1OcCh8);
                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", Pdu48_2OcCh1, Pdu48_2OcCh2, Pdu48_2OcCh3, Pdu48_2OcCh4, Pdu48_2OcCh5, Pdu48_2OcCh6, Pdu48_2OcCh7, Pdu48_2OcCh8);
                            swSmc.Write("{0} {1} {2} {3} {4} {5} {6} {7} ", Pdu48_3OcCh1, Pdu48_3OcCh2, Pdu48_3OcCh3, Pdu48_3OcCh4, Pdu48_3OcCh5, Pdu48_3OcCh6, Pdu48_3OcCh7, Pdu48_3OcCh8);

                            swSmc.WriteLine();
                            swSmc.Flush();

                            for (int i = 0; i < 30; i++)
                            {
                                swBms.Write("{0} ", usBMS2OBC_SA1[i]);
                            }
                            for (int i = 0; i < 30; i++)
                            {
                                swBms.Write("{0} ", usBMS2OBC_SA2[i]);
                            }
                            for (int i = 0; i < 24; i++)
                            {
                                swBms.Write("{0} ", usOBC2BMS_SA3[i]);
                            }

                            swBms.Write("{0} {1}", Timer_100ms_Count, g_dTime);

                            swBms.WriteLine();
                            swBms.Flush();

                            swControlGains.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16} {17} {18}", ROLL_INTG_ON_TIME,
ROLL_GAIN, ROLL_RATE_GAIN_3KN, ROLL_RATE_GAIN_6KN, ROLL_CMD_LIM, YAW_GAIN, YAW_RATE_GAIN_3KN, YAW_RATE_GAIN_6KN, YAW_INTG_GAIN, YAW_CMD_LIM,
DEPTH_GAIN, DERR_MAX, PITCH_GAIN, PITCH_RATE_GAIN_3KN, PITCH_RATE_GAIN_6KN, PITCH_CMD_LIM, FIN_CMD_MAX, SONAR_TRANSMIT_DEPTH_LIM,
YAW_RATE_LOOP_PROP_GAIN);
                            swControlGains.Flush();

                            for (int i = 0; i < psbNooflegs; i++)
                            {
                                swSeqLegs.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12}", psuiLegDuration[i], psbSpeed[i], psusDepth[i],
pssYawRef[i], pfTurnRate[i], psusDco[i], psusCco[i], psbSearchSide[i], psbSearchPattern[i], psbRCL[i], psbPCL[i], psbYCL[i], psbDECL[i]);
                            }
                            swSeqLegs.Flush();

                            swPresetParameters.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", PresetFlo, presetDco, PresetRuntime, static_test_flag,
static_test_start, static_test_start_time, static_test_stop_time, lcs_static_test_rpm, psbNooflegs, pinger_st_mission_flag);
                            swPresetParameters.Flush();

                            swIPS.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16} {17} {18} {19}", Waypoint_ID_4m_IPS,
Headingrate_4m_IPS, Speed_4m_IPS, StatusCode0, StatusCode1, StatusCode2, StatusCode3, StatusCode4, StatusCode5, StatusCode6, StatusCode7,
WpDepth, image_number, no_of_pings, no_of_sample, image_st_timestamp, timestamp_incr, total_no_of_detections, detect_pkt_no_in_cycle,
no_of_det_in_pkt);
                            swIPS.Flush();

                            for (int i = 0; i < 10; i++)
                            {
                                swIPS_Detection.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8}", detclass[i], detx1[i], dety1[i], detx2[i], dety2[i], dlat1[i],
dlong1[i], dlat2[i], dlong2[i]);
                            }
                            swIPS_Detection.Flush();

                            swRunTermination.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} ", g_ucRT4mMotor, g_ucCommFail4mPED,
g_ucCommFail4mALTI, g_ucCommFail4mMPIDS, g_ucRunTimeRT, g_ucLCS_Indication, g_ucCommFail4mEASSC, g_ucMission_Stop, g_ucMission_Abort,
g_ucRT4m_DCO, g_ucBoundaryConditionRT, RT4m_MPIDS_SA3, RT4m_MPIDS_SA4, RT4m_MPIDS_SA5, RT4m_MPIDS);
                            swRunTermination.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} ", g_ucCommFail4mBMS, g_ucCommFail4mIPS,
g_ucIPSError_MPSSWFlag, g_ucIPSError_ConfigCmdFlag, g_ucIPSError_VehicleStateAvailFlag, g_ucIPSError_WpAvailFlag,
g_ucIPSError_FirstWpRangeFlag, g_ucIPS_Error_SCMFlag, g_ucIPSError_FLSAvailFlag, g_ucRT4mIPS, g_ucFault4mMotor_flag,
g_ucMotor1Under_Voltage_flag, g_ucMotor1Over_Voltage_flag, g_Motor1_Over_Current_Flag, g_dTime, g_ucCommFail4mPHINS);
                            swRunTermination.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}", g_ucCommFail4mLCS, g_ucCommFail4mCSS,
g_ucCommFail4mNCS, g_ucCommFail4mFLSC, g_ucCommFail4mSSSC, g_ucCommFail4mMBE, g_ucCommFail4mBLSC, g_ucCommFail4mAASC, g_ucCommFail4mFASC,
g_ucCommFail4mAMC, g_ucCommFail4mLPRS, g_ucCommFail4mELINT, g_ucCommFail4mMSS, g_ucCommFail4mSCS, g_ucCommFail4mSECSS,
g_ucAMTransmitEnableFlag);
                            swRunTermination.Flush();

                            swRecovery_Conditions.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} ", Rc_ubfMPSConfigCmdError, Rc_ubfMPSBeyondRange,
Rc_ubfPHINSCommFail, Rc_ubfPEDCommFail, Rc_ubfMPIDSCommFail, Rc_ubfMASTCommFail, Rc_ubfLPRSCommFail, Rc_ubfLCSWirelessCommFail,
Rc_ubfLCSWiredCommFail, Rc_ubfELINTCommFail, Rc_ubfEASSCCommFail);
                            swRecovery_Conditions.Write("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} ", Rc_ubfIPSCommFail, Rc_ubfCTDCommFail, Rc_ubfCSSCommFailNA,
Rc_ubfBMSCommFail, Rc_ubfAltimeterCommFail, Rc_ubfSONARCommFail, Rc_ubfSCSCommFail, Rc_ubfMPSWaypointNA, Rc_ubfMPSWaypointComplete,
Rc_ubfPSonIndication, Rc_ubfMotorRT);
                            swRecovery_Conditions.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12}", Rc_ubfStop, Rc_ubfAbort, Rc_ubfRunTimeRT,
Rc_ubfLCS_Indication, Rc_ubfDCO, Rc_ubfIpsRT, Rc_ubfMPSVehicleStateNA, Rc_ubfMPSSWError, Rc_ubfMPSSafetyCorridorMonitor, Rc_ubfMPSFLSNA,
Rc_ubfSecSSCommFail, Rc_ubfTSUCommFail, Rc_ubfMPSNoGuidance);
                            swRecovery_Conditions.Flush();
                        }
                    }
                }
            }
            catch (Exception e) // for detecting end of binary stream reader
            {
                Console.Write(e);
                swPhins.Flush();
                swPhins.Close();
                swBldc.Flush();
                swBldc.Close();
                swEassc.Flush();
                swEassc.Close();
                swSmc.Flush();
                swSmc.Close();
                swTsu.Flush();
                swTsu.Close();
                swBms.Flush();
                swBms.Close();
                swDvl.Flush();
                swDvl.Close();
                swRemote.Flush();
                swRemote.Close();
                swHealth.Flush();
                swHealth.Close();
                swControlGains.Flush();
                swControlGains.Close();
                swSeqLegs.Flush();
                swSeqLegs.Close();
                swPresetParameters.Flush();
                swPresetParameters.Close();
                swIPS.Flush();
                swIPS.Close();
                swRunTermination.Flush();
                swRunTermination.Close();
                swMCS_RTC_Clock.Flush();
                swMCS_RTC_Clock.Close();
                swSONAR.Flush();
                swSONAR.Close();
                swCTD.Flush();
                swCTD.Close();
                swIPS_Detection.Flush();
                swIPS_Detection.Close();
                swSecSS.Flush();
                swSecSS.Close();
                swRecovery_Conditions.Flush();
                swRecovery_Conditions.Close();
                swMAST.Flush();
                swMAST.Close();
                Console.Write(e.Message);
                Console.ReadKey();
            }
        }

        public static double Convert_ADC_volt(ushort usADC_val)
        {
            double flADC;
            if (usADC_val > 0x7FFF)
            {
                flADC = (double)((usADC_val * 20.0 / 65535.0) - 20.0);
            }
            else
            {
                flADC = (double)(usADC_val * 20.0 / 65535.0);
            }

            return (flADC);
        }

        static float Sign(float fADC)
        {
            if (fADC >= 0)
                return 1;
            else
                return -1;
        }
    }
}

