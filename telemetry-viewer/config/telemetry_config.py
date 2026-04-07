TIME_COLUMN = -2

TELEMETRY_PARAMETERS = {

"CONTROL":[
"ADC0","ADC1","Roll","Pitch","Yaw",
"RollRate","PitchRate","YawRate",
"Latitude","Longitude","Altitude","Depth","Temperature",
"SysStat1","SysStat2","AlgoStat1","AlgoStat2",
"SenStat1","SenStat2","PageNum","BlockNum","Footer",
"Timer","Time",
"Nvel","Evel","Dvel","RollCmd","PitchCmd","YawCmd",
"dLat","dLong","PosX","PosY","PosZ","Uvel",
"PrelaunchFlag","DynamicCmdSharing","VelocityControl",
"AltitudeControl","DVL_Altitude","INS_Resultant_Velocity"
],

"TSU":[
"TSU1","TSU2","TSU3","TSU4","TSU5","TSU6","TSU7","TSU8",
"Timer","Time"
],

"BLDC":[
"BLDC1","BLDC2","BLDC3","BLDC4","BLDC5","BLDC6","BLDC7",
"BLDC8","BLDC9","BLDC10","BLDC11","BLDC12","BLDC13",
"BLDC14","BLDC15","BLDC16","BLDC17",
"OBC_BLDC1","OBC_BLDC2","OBC_BLDC3","OBC_BLDC4",
"OBC_BLDC5","OBC_BLDC6",
"Timer","Time"
],

"EASSC":[
"EASSC1","EASSC2","EASSC3","EASSC4","EASSC5",
"EASSC6","EASSC7","EASSC8","EASSC9","EASSC10",
"EASSC11","EASSC12","EASSC13","EASSC14","EASSC15",
"EASSC16","EASSC17","EASSC18","EASSC19","EASSC20",
"EASSC21","EASSC22","EASSC23","EASSC24","EASSC25",
"Timer","Time"
],

"SMC":[
"SMC1","SMC2","SMC3","SMC4","SMC5","SMC6","SMC7","SMC8","SMC9",
"SMC10","SMC11","SMC12","SMC13","SMC14","SMC15","SMC16","SMC17",
"SMC18","SMC19","SMC20","SMC21","SMC22","SMC23","SMC24",
"Timer","Time"
],

"BMS":[
"BMS1","BMS2","BMS3","BMS4","BMS5","BMS6","BMS7","BMS8",
"BMS9","BMS10","BMS11","BMS12","BMS13","BMS14","BMS15",
"BMS16","BMS17","BMS18","BMS19","BMS20",
"Timer","Time"
],

"DVL":[
"Beam1","Beam2","Beam3","Beam4","Beam5","Beam6","Beam7",
"Beam8","Beam9","Beam10","Beam11",
"Timer","Time","BtRes","WtRes","RunNumber","AvgBeamRange"
],

"REMOTE":[
"RefDepth","RefYaw","Speed","PCL","DEL"
],

"HEALTH":[
"BMS","PED","SMC","EASSC","ALT","CTD","TSU","PHINS","IPS",
"MCS","LPRS","ELINT","IPS2","SSS","SECSS","MBE","FLS",
"FDS","AM","BLS","AAS","FAS","CSS","SCS","MSS"
],

"CONTROLGAINS":[
"ROLL_INTG_ON_TIME","ROLL_GAIN","ROLL_RATE_GAIN_3KN",
"ROLL_RATE_GAIN_6KN","ROLL_CMD_LIM","YAW_GAIN",
"YAW_RATE_GAIN_3KN","YAW_RATE_GAIN_6KN","YAW_INTG_GAIN",
"YAW_CMD_LIM","DEPTH_GAIN","DERR_MAX","PITCH_GAIN",
"PITCH_RATE_GAIN_3KN","PITCH_RATE_GAIN_6KN","PITCH_CMD_LIM",
"FIN_CMD_MAX","SONAR_TRANSMIT_DEPTH_LIM","YAW_RATE_LOOP_PROP_GAIN"
],

"SEQLEGS":[
"LegDuration","Speed","Depth","YawRef","TurnRate",
"DCO","CCO","SearchSide","SearchPattern","RCL",
"PCL","YCL","DECL"
],

"PRESETPARAMETERS":[
"PresetFlo","PresetDco","PresetRuntime","StaticTestFlag",
"StaticTestStart","StaticTestStartTime","StaticTestStopTime",
"StaticTestRPM","NoOfLegs","PingerMissionFlag"
],

"IPS":[
"WaypointID","HeadingRate","Speed","Status0","Status1",
"Status2","Status3","Status4","Status5","Status6",
"Status7","Depth","ImageNumber","NoOfPings",
"NoOfSamples","ImageTimestamp","TimestampIncrement",
"TotalDetections","PacketNumber","DetectionsInPacket"
],

"RUNTERMINATION":[
"MotorRT","PEDCommFail","AltCommFail","MPIDSCommFail",
"RunTimeRT","LCSIndication","EASSCCommFail",
"MissionStop","MissionAbort","DCORT",
"BoundaryCondition","MPIDS_SA3","MPIDS_SA4",
"MPIDS_SA5","MPIDS","Time"
],

"MCSRTC":[
"Timer","Day","Month","Year","Hour","Minute","Second"
],

"SONAR":[
"TargetCount","Latitude","Longitude","Day","Month",
"Year","Hour","Minute","Second","Millisecond",
"ActiveInterval","NoOfPings","PRI","FreqStart",
"FreqEnd","PassiveGain","ActiveGainMin",
"ActiveGainMax","SourceLevel"
],

"CTD":[
"Conductivity","Temperature","Depth"
],

"SECSS":[
"DistressAck","EraseStart","EraseStop","Shutdown"
],

"IPSDETECTION":[
"Class","X1","Y1","X2","Y2","Lat1","Long1","Lat2","Long2"
],

"RECOVERY":[
"MPSConfigError","MPSRange","PHINSCommFail","PEDCommFail",
"MPIDSCommFail","MASTCommFail","LPRSCommFail",
"LCSWirelessFail","LCSWiredFail","ELINTFail",
"IPSFail","CTDFail","CSSFail","BMSFail",
"AltimeterFail","SONARFail","SCSFail"
],

"MAST":[
"MCS2MAST1","MCS2MAST2","MCS2MAST3","MCS2MAST4","MCS2MAST5",
"MAST2MCS1","MAST2MCS2","MAST2MCS3","MAST2MCS4","MAST2MCS5",
"Timer","Time"
]

}