using DistributedAlgorithms.Algorithms.Base.Base;
using System.Collections.Generic;


namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound
{

	#region /// \name Enums for ChandyLamport_OneRoundProcess
	public class m
	{
 
		public enum marker
		{
	
		}
 
		public enum baseMessage
		{
	
		}
 
		public enum MessageTypes
		{
			Marker, BaseMessage
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_OneRoundProcess
	public partial class ChandyLamport_OneRoundProcess: BaseProcess
	{
 
		public AttributeDictionary MessageDataFor_Marker( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound.m+ork_marker";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				return dictionary;
			} 

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
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound.m+ork_baseMessage";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				return dictionary;
			} 

			return dictionary;
		}
 
		public void SendMarker(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null)
		{
			if(fields is null)
			{
				MessageDataFor_Marker();
			}
			Send(m.MessageTypes.Marker, fields, selectingMethod, ids, 0, 0);
		}
 
		public void SendBaseMessage(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null)
		{
			if(fields is null)
			{
				MessageDataFor_BaseMessage();
			}
			Send(m.MessageTypes.BaseMessage, fields, selectingMethod, ids, 0, 0);
		}
	}
	#endregion

	#region /// \name Enums for ChandyLamport_OneRoundNetwork
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

	#region /// \name partial class for ChandyLamport_OneRoundNetwork
	public partial class ChandyLamport_OneRoundNetwork: BaseNetwork
	{
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const string type = "Type";
		const string edited = "Edited";
		const string id = "Id";
		const string algorithm = "Algorithm";
		const string centrilized = "Centrilized";
		const string directedNetwork = "DirectedNetwork";
		const string version = "Version";
		const string singleStepStatus = "SingleStepStatus";
		const string name = "Name";
		const string initiator = "Initiator";
		const string status = "Status";
		const string recordered = "Recordered";
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
		const string marked = "Marked";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.Int32 Version
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
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound.n+pak";
			dictionary.Add(n.pak.Version, new Attribute { Value = 2 ,Editable = false ,Changed = false } );
			base.InitPrivateAttributes();
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound.n+ork";
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for ChandyLamport_OneRoundProcess
	public class p
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
			Status, Recordered
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_OneRoundProcess
	public partial class ChandyLamport_OneRoundProcess: BaseProcess
	{
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const string type = "Type";
		const string edited = "Edited";
		const string id = "Id";
		const string algorithm = "Algorithm";
		const string centrilized = "Centrilized";
		const string directedNetwork = "DirectedNetwork";
		const string version = "Version";
		const string singleStepStatus = "SingleStepStatus";
		const string name = "Name";
		const string initiator = "Initiator";
		const string status = "Status";
		const string recordered = "Recordered";
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
		const string marked = "Marked";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.String Status
		{
			 get { return or[p.ork.Status]; }
			 set { or[p.ork.Status] = value; }
		}
 
		public System.Boolean Recordered
		{
			 get { return or[p.ork.Recordered]; }
			 set { or[p.ork.Recordered] = value; }
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound.p+pak";
			base.InitPrivateAttributes();
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound.p+ork";
			dictionary.Add(p.ork.Status, new Attribute { Value = "" ,Changed = true } );
			dictionary.Add(p.ork.Recordered, new Attribute { Value = false ,Changed = true } );
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for ChandyLamport_OneRoundChannel
	public class c
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
			State, Marked
		}
	}
	#endregion

	#region /// \name partial class for ChandyLamport_OneRoundChannel
	public partial class ChandyLamport_OneRoundChannel: BaseChannel
	{
		const m.MessageTypes Marker = m.MessageTypes.Marker;
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const string type = "Type";
		const string edited = "Edited";
		const string id = "Id";
		const string algorithm = "Algorithm";
		const string centrilized = "Centrilized";
		const string directedNetwork = "DirectedNetwork";
		const string version = "Version";
		const string singleStepStatus = "SingleStepStatus";
		const string name = "Name";
		const string initiator = "Initiator";
		const string status = "Status";
		const string recordered = "Recordered";
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
		const string marked = "Marked";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public DistributedAlgorithms.AttributeList State
		{
			 get { return or[c.ork.State]; }
			 set { or[c.ork.State] = value; }
		}
 
		public System.Boolean Marked
		{
			 get { return or[c.ork.Marked]; }
			 set { or[c.ork.Marked] = value; }
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound.c+pak";
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
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound.c+ork";
			dictionary.Add(c.ork.State, new Attribute { Value = Init_ork_State() ,Changed = true } );
			dictionary.Add(c.ork.Marked, new Attribute { Value = false ,Changed = true } );
			base.InitOperationResults();
		}
	}
	#endregion
}