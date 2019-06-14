using DistributedAlgorithms.Algorithms.Base.Base;
using System.Collections.Generic;


namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle
{

	#region /// \name Enums for ChandyLamport_NewStyleMessage
	public class m
	{
 
		public enum baseMessage
		{
	
		}
 
		public enum marker
		{
			MarkerWeight
		}
 
		public enum report
		{
			Id, Snapshot, ReportWeight
		}
 
		public enum MessageTypes
		{
			BaseMessage, Marker, Report
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_NewStyleMessage
	public partial class ChandyLamport_NewStyleMessage: BaseMessage
	{

        //public System.Int32 MarkerWeight
        //{
        //	 get { return marke[m.ork_marker.MarkerWeight]; }
        //	 set { marke[m.ork_marker.MarkerWeight] = value; }
        //}

        public System.Int32 MarkerWeight
        {
            get { return or[m.marker.MarkerWeight]; }
            set { or[m.marker.MarkerWeight] = value; }
        }

  //      public System.Int32 Id
		//{
		//	 get { return repor[m.ork_report.Id]; }
		//	 set { repor[m.ork_report.Id] = value; }
		//}

        public System.Int32 Id
        {
            get { return or[m.report.Id]; }
            set { or[m.report.Id] = value; }
        }

        public System.String Snapshot
		{
			 get { return or[m.report.Snapshot]; }
			 set { or[m.report.Snapshot] = value; }
		}

        public System.Int32 ReportWeight
        {
            get { return or[m.report.Snapshot]; }
            set { or[m.report.Snapshot] = value; }
        }

        public new m.MessageTypes MessageType
        {
			 get { return pa[bm.pak.MessageType]; }
			 set { pa[bm.pak.MessageType] = value; }
		}
	}
	#endregion

	#region /// \name Enums for ChandyLamport_NewStyleNetwork
	public class n
	{
 
		public enum pak
		{
			Version
		}
 
		public enum ork
		{
	
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_NewStyleNetwork
	public partial class ChandyLamport_NewStyleNetwork: BaseNetwork
	{
		const string markerWeight = "MarkerWeight";
		const string id = "Id";
		const string snapshot = "Snapshot";
		const string reportWeight = "ReportWeight";
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes Report = m.MessageTypes.Report;
		const string type = "Type";
		const string edited = "Edited";
		const string algorithm = "Algorithm";
		const string centrilized = "Centrilized";
		const string directedNetwork = "DirectedNetwork";
		const string version = "Version";
		const string singleStepStatus = "SingleStepStatus";
		const string name = "Name";
		const string initiator = "Initiator";
		const string maxRounds = "MaxRounds";
		const string statusColor = "StatusColor";
		const string recordered = "Recordered";
		const string receivedMessageFrom = "ReceivedMessageFrom";
		const string results = "Results";
		const string weight = "Weight";
		const string round = "Round";
		const string terminationStatus = "TerminationStatus";
		const string messageQ = "MessageQ";
		const string receivePort = "ReceivePort";
		const string frameColor = "FrameColor";
		const string frameWidth = "FrameWidth";
		const string frameHeight = "FrameHeight";
		const string frameLeft = "FrameLeft";
		const string frameTop = "FrameTop";
		const string frameLineWidth = "FrameLineWidth";
		const string background = "Background";
		const string foreground = "Foreground";
		const string text = "Text";
		const string breakpointsFrameColor = "BreakpointsFrameColor";
		const string breakpointsFrameWidth = "BreakpointsFrameWidth";
		const string breakpointsBackground = "BreakpointsBackground";
		const string breakpointsForeground = "BreakpointsForeground";
		const string sourceProcess = "SourceProcess";
		const string destProcess = "DestProcess";
		const string state = "State";
		const string recorderd = "Recorderd";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";



		public override int GetVersion()
		{
			try
			{
				return pa[n.pak.Version];
			}
			catch
			{
				return 0;
			}
		}



		protected override void CodeGenerationAdditionalInit()
		{
			ea[bn.eak.Centrilized] = true;
			ea[bn.eak.DirectedNetwork] = false;
		}
	}
	#endregion

	#region /// \name Enums for ChandyLamport_NewStyleProcess
	public class p
	{
 
		public enum pak
		{
			MaxRounds
		}
 
		public enum ork
		{
			StatusColor, Recordered, Snapshot, ReceivedMessageFrom, Results, Weight
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_NewStyleProcess
	public partial class ChandyLamport_NewStyleProcess: BaseProcess
	{
		const string markerWeight = "MarkerWeight";
		const string id = "Id";
		const string snapshot = "Snapshot";
		const string reportWeight = "ReportWeight";
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes Report = m.MessageTypes.Report;
		const string type = "Type";
		const string edited = "Edited";
		const string algorithm = "Algorithm";
		const string centrilized = "Centrilized";
		const string directedNetwork = "DirectedNetwork";
		const string version = "Version";
		const string singleStepStatus = "SingleStepStatus";
		const string name = "Name";
		const string initiator = "Initiator";
		const string maxRounds = "MaxRounds";
		const string statusColor = "StatusColor";
		const string recordered = "Recordered";
		const string receivedMessageFrom = "ReceivedMessageFrom";
		const string results = "Results";
		const string weight = "Weight";
		const string round = "Round";
		const string terminationStatus = "TerminationStatus";
		const string messageQ = "MessageQ";
		const string receivePort = "ReceivePort";
		const string frameColor = "FrameColor";
		const string frameWidth = "FrameWidth";
		const string frameHeight = "FrameHeight";
		const string frameLeft = "FrameLeft";
		const string frameTop = "FrameTop";
		const string frameLineWidth = "FrameLineWidth";
		const string background = "Background";
		const string foreground = "Foreground";
		const string text = "Text";
		const string breakpointsFrameColor = "BreakpointsFrameColor";
		const string breakpointsFrameWidth = "BreakpointsFrameWidth";
		const string breakpointsBackground = "BreakpointsBackground";
		const string breakpointsForeground = "BreakpointsForeground";
		const string sourceProcess = "SourceProcess";
		const string destProcess = "DestProcess";
		const string state = "State";
		const string recorderd = "Recorderd";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
	}
	#endregion

	#region /// \name Enums for ChandyLamport_NewStyleChannel
	public class c
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
			State, Recorderd
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_NewStyleChannel
	public partial class ChandyLamport_NewStyleChannel: BaseChannel
	{
		const string markerWeight = "MarkerWeight";
		const string id = "Id";
		const string snapshot = "Snapshot";
		const string reportWeight = "ReportWeight";
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes Report = m.MessageTypes.Report;
		const string type = "Type";
		const string edited = "Edited";
		const string algorithm = "Algorithm";
		const string centrilized = "Centrilized";
		const string directedNetwork = "DirectedNetwork";
		const string version = "Version";
		const string singleStepStatus = "SingleStepStatus";
		const string name = "Name";
		const string initiator = "Initiator";
		const string maxRounds = "MaxRounds";
		const string statusColor = "StatusColor";
		const string recordered = "Recordered";
		const string receivedMessageFrom = "ReceivedMessageFrom";
		const string results = "Results";
		const string weight = "Weight";
		const string round = "Round";
		const string terminationStatus = "TerminationStatus";
		const string messageQ = "MessageQ";
		const string receivePort = "ReceivePort";
		const string frameColor = "FrameColor";
		const string frameWidth = "FrameWidth";
		const string frameHeight = "FrameHeight";
		const string frameLeft = "FrameLeft";
		const string frameTop = "FrameTop";
		const string frameLineWidth = "FrameLineWidth";
		const string background = "Background";
		const string foreground = "Foreground";
		const string text = "Text";
		const string breakpointsFrameColor = "BreakpointsFrameColor";
		const string breakpointsFrameWidth = "BreakpointsFrameWidth";
		const string breakpointsBackground = "BreakpointsBackground";
		const string breakpointsForeground = "BreakpointsForeground";
		const string sourceProcess = "SourceProcess";
		const string destProcess = "DestProcess";
		const string state = "State";
		const string recorderd = "Recorderd";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
	}
	#endregion

	#region /// \name partial class for ChandyLamport_NewStyleProcess
	public partial class ChandyLamport_NewStyleProcess: BaseProcess
	{
		public void SendBaseMessage(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null,
			int round = -1,
			int clock = -1)
		{
			if(fields is null)
			{
				fields = MessageDataFor_BaseMessage();
			}
			Send(m.MessageTypes.BaseMessage, fields, selectingMethod, ids, round, clock);
		}

        public AttributeDictionary MessageDataFor_BaseMessage()
        {
            return null;
        }

        public void SendMarker(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null,
			int round = -1,
			int clock = -1)
		{
			if(fields is null)
			{
				fields = MessageDataFor_Marker();
			}
			Send(m.MessageTypes.Marker, fields, selectingMethod, ids, round, clock);
		}

        public AttributeDictionary MessageDataFor_Marker() { return null; }

        public void SendReport(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null,
			int round = -1,
			int clock = -1)
		{
			if(fields is null)
			{
				fields = MessageDataFor_Report();
			}
			Send(m.MessageTypes.Report, fields, selectingMethod, ids, round, clock);
		}
        public AttributeDictionary MessageDataFor_Report() { return null; }
    }
	#endregion
}