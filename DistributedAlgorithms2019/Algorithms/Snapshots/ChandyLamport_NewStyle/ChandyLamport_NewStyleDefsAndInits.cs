using DistributedAlgorithms.Algorithms.Base.Base;
using System.Collections.Generic;


namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle
{

	#region /// \name Enums for ChandyLamport_NewStyleMessage
	public class m
	{
 
		public enum report
		{
			ReporterId, Snapshot, ReportWeight
		}
 
		public enum marker
		{
			MarkerWeight
		}
 
		public enum baseMessage
		{
	
		}
 
		public enum MessageTypes
		{
			Report, Marker, BaseMessage
		}
	}
	#endregion

    public class DerivedTest:BaseMessage
    {
        //public System.Double ReporterId
        //{
        //    get { return or[m.report.ReporterId]; }
        //    set { or[m.report.ReporterId] = value; }
        //}

        //public System.String Snapshot
        //{
        //    get { return or[m.report.Snapshot]; }
        //    set { or[m.report.Snapshot] = value; }
        //}

        //public System.Double ReportWeight
        //{
        //    get { return or[m.report.ReportWeight]; }
        //    set { or[m.report.ReportWeight] = value; }
        //}

        //public System.Double MarkerWeight
        //{
        //    get { return or[m.marker.MarkerWeight]; }
        //    set { or[m.marker.MarkerWeight] = value; }
        //}

        //public new m.MessageTypes MessageType
        //{
        //    get { return pa[bm.pak.MessageType]; }
        //    set { pa[bm.pak.MessageType] = value; }
        //}
    }

	#region /// \name partial class for ChandyLamport_NewStyleMessage
	public partial class ChandyLamport_NewStyleMessage: BaseMessage
	{
 
		public System.Double ReporterId
		{
			 get { return or[m.report.ReporterId]; }
			 set { or[m.report.ReporterId] = value; }
		}
 
		public System.String Snapshot
		{
			 get { return or[m.report.Snapshot]; }
			 set { or[m.report.Snapshot] = value; }
		}
 
		public System.Double ReportWeight
		{
			 get { return or[m.report.ReportWeight]; }
			 set { or[m.report.ReportWeight] = value; }
		}
 
		public System.Double MarkerWeight
		{
			 get { return or[m.marker.MarkerWeight]; }
			 set { or[m.marker.MarkerWeight] = value; }
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
		const string id = "Id";
		const string snapshot = "Snapshot";
		const string reportWeight = "ReportWeight";
		const string markerWeight = "MarkerWeight";
		const m.MessageTypes Report = m.MessageTypes.Report;
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
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
		const string weight = "Weight";
		const string results = "Results";
		const string receivedMessageFrom = "ReceivedMessageFrom";
		const string statusColor = "StatusColor";
		const string recorderd = "Recorderd";
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
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.Double Version
		{
			 get { return pa[n.pak.Version]; }
			 set { pa[n.pak.Version] = value; }
		}



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
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.n+pak";
			dictionary.Add(n.pak.Version, new Attribute { Value = 9 ,Editable = false ,Changed = false } );
			base.InitPrivateAttributes();
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.n+ork";
			base.InitOperationResults();
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
			Weight, Results, ReceivedMessageFrom, Snapshot, StatusColor, Recorderd
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_NewStyleProcess
	public partial class ChandyLamport_NewStyleProcess: BaseProcess
	{
		const string id = "Id";
		const string snapshot = "Snapshot";
		const string reportWeight = "ReportWeight";
		const string markerWeight = "MarkerWeight";
		const m.MessageTypes Report = m.MessageTypes.Report;
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
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
		const string weight = "Weight";
		const string results = "Results";
		const string receivedMessageFrom = "ReceivedMessageFrom";
		const string statusColor = "StatusColor";
		const string recorderd = "Recorderd";
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
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.Double MaxRounds
		{
			 get { return pa[p.pak.MaxRounds]; }
			 set { pa[p.pak.MaxRounds] = value; }
		}
 
		public System.Double Weight
		{
			 get { return or[p.ork.Weight]; }
			 set { or[p.ork.Weight] = value; }
		}
 
		public DistributedAlgorithms.AttributeList Results
		{
			 get { return or[p.ork.Results]; }
			 set { or[p.ork.Results] = value; }
		}
 
		public DistributedAlgorithms.AttributeList ReceivedMessageFrom
		{
			 get { return or[p.ork.ReceivedMessageFrom]; }
			 set { or[p.ork.ReceivedMessageFrom] = value; }
		}
 
		public System.String Snapshot
		{
			 get { return or[p.ork.Snapshot]; }
			 set { or[p.ork.Snapshot] = value; }
		}
 
		public System.String StatusColor
		{
			 get { return or[p.ork.StatusColor]; }
			 set { or[p.ork.StatusColor] = value; }
		}
 
		public System.Boolean Recorderd
		{
			 get { return or[p.ork.Recorderd]; }
			 set { or[p.ork.Recorderd] = value; }
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.p+pak";
			dictionary.Add(p.pak.MaxRounds, new Attribute { Value = 5 ,Changed = false } );
			base.InitPrivateAttributes();
		}
		protected AttributeList Init_ork_Results()
		{
			AttributeList list = new AttributeList();
			list.codeAttributeCounter = 0;
			list.designAttributeCounter = 0;
			return list;
		}
		protected AttributeList Init_ork_ReceivedMessageFrom()
		{
			AttributeList list = new AttributeList();
			list.codeAttributeCounter = 0;
			list.designAttributeCounter = 0;
			return list;
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.p+ork";
			dictionary.Add(p.ork.Weight, new Attribute { Value = 0 ,Changed = false } );
			dictionary.Add(p.ork.Results, new Attribute { Value = Init_ork_Results() ,Changed = false } );
			dictionary.Add(p.ork.ReceivedMessageFrom, new Attribute { Value = Init_ork_ReceivedMessageFrom() ,Changed = false } );
			dictionary.Add(p.ork.Snapshot, new Attribute { Value = "" ,Changed = false } );
			dictionary.Add(p.ork.StatusColor, new Attribute { Value = "Not Set" ,Changed = false } );
			dictionary.Add(p.ork.Recorderd, new Attribute { Value = false ,Changed = false } );
			base.InitOperationResults();
		}
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
			Recorderd, State
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_NewStyleChannel
	public partial class ChandyLamport_NewStyleChannel: BaseChannel
	{
		const string id = "Id";
		const string snapshot = "Snapshot";
		const string reportWeight = "ReportWeight";
		const string markerWeight = "MarkerWeight";
		const m.MessageTypes Report = m.MessageTypes.Report;
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
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
		const string weight = "Weight";
		const string results = "Results";
		const string receivedMessageFrom = "ReceivedMessageFrom";
		const string statusColor = "StatusColor";
		const string recorderd = "Recorderd";
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
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.Boolean Recorderd
		{
			 get { return or[c.ork.Recorderd]; }
			 set { or[c.ork.Recorderd] = value; }
		}
 
		public DistributedAlgorithms.AttributeList State
		{
			 get { return or[c.ork.State]; }
			 set { or[c.ork.State] = value; }
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.c+pak";
			base.InitPrivateAttributes();
		}
		protected AttributeList Init_ork_State()
		{
			AttributeList list = new AttributeList();
			list.codeAttributeCounter = 0;
			list.designAttributeCounter = 0;
			return list;
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.c+ork";
			dictionary.Add(c.ork.Recorderd, new Attribute { Value = false ,Changed = false } );
			dictionary.Add(c.ork.State, new Attribute { Value = Init_ork_State() ,Changed = false } );
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_NewStyleProcess
	public partial class ChandyLamport_NewStyleProcess: BaseProcess
	{
 
		public AttributeDictionary MessageDataFor_Report( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null,
			System.Double id = 0,
			System.String snapshot = "",
			System.Double reportWeight = 0)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.m+ork_report";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				dictionary.Add( m.report.ReporterId, new Attribute { Value = 0 ,Changed = false } );
				dictionary.Add( m.report.Snapshot, new Attribute { Value = "" ,Changed = false } );
				dictionary.Add( m.report.ReportWeight, new Attribute { Value = 0 ,Changed = false } );
				return dictionary;
			} 

			dictionary.Add( m.report.ReporterId, new Attribute { Value = id ,Changed = false } );
			dictionary.Add( m.report.Snapshot, new Attribute { Value = snapshot ,Changed = false } );
			dictionary.Add( m.report.ReportWeight, new Attribute { Value = reportWeight ,Changed = false } );
			return dictionary;
		}
 
		public AttributeDictionary MessageDataFor_Marker( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null,
			System.Double markerWeight = 0)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.m+ork_marker";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				dictionary.Add( m.marker.MarkerWeight, new Attribute { Value = 0 ,Changed = false } );
				return dictionary;
			} 

			dictionary.Add( m.marker.MarkerWeight, new Attribute { Value = markerWeight ,Changed = false } );
			return dictionary;
		}
 
		public AttributeDictionary MessageDataFor_BaseMessage( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle.m+ork_baseMessage";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				return dictionary;
			} 

			return dictionary;
		}
 
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
	}
	#endregion
}