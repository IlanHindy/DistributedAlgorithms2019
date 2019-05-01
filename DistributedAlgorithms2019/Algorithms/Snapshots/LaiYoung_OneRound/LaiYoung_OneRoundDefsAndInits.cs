using DistributedAlgorithms.Algorithms.Base.Base;
using System.Collections.Generic;


namespace DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound
{

	#region /// \name Enums for LaiYoung_OneRoundProcess
	public class m
	{
 
		public enum presnp
		{
			NumMsg
		}
 
		public enum baseMessage
		{
			Flag, Message
		}
 
		public enum MessageTypes
		{
			Presnp, BaseMessage
		}
	}
	#endregion

	#region /// \name partial class for LaiYoung_OneRoundProcess
	public partial class LaiYoung_OneRoundProcess: BaseProcess
	{
 
		public AttributeDictionary MessageDataFor_Presnp( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null,
			System.Double numMsg = 0)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound.m+ork_presnp";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				dictionary.Add( m.presnp.NumMsg, new Attribute { Value = 0 ,Changed = false } );
				return dictionary;
			} 

			dictionary.Add( m.presnp.NumMsg, new Attribute { Value = numMsg ,Changed = false } );
			return dictionary;
		}
 
		public AttributeDictionary MessageDataFor_BaseMessage( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null,
			System.Boolean flag = false,
			DistributedAlgorithms.Algorithms.Base.Base.BaseMessage message = null)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound.m+ork_baseMessage";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				dictionary.Add( m.baseMessage.Flag, new Attribute { Value = false ,Changed = false } );
				dictionary.Add( m.baseMessage.Message, new Attribute { Value = new DistributedAlgorithms.Algorithms.Base.Base.BaseMessage() ,Changed = false } );
				return dictionary;
			} 

			dictionary.Add( m.baseMessage.Flag, new Attribute { Value = flag ,Changed = false } );
			dictionary.Add( m.baseMessage.Message, new Attribute { Value = message ,Changed = false } );
			return dictionary;
		}
 
		public void SendPresnp(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null,
			int round = -1,
			int clock = -1)
		{
			if(fields is null)
			{
				MessageDataFor_Presnp();
			}
			Send(m.MessageTypes.Presnp, fields, selectingMethod, ids, round, clock);
		}
 
		public void SendBaseMessage(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null,
			int round = -1,
			int clock = -1)
		{
			if(fields is null)
			{
				MessageDataFor_BaseMessage();
			}
			Send(m.MessageTypes.BaseMessage, fields, selectingMethod, ids, round, clock);
		}
	}
	#endregion

	#region /// \name Enums for LaiYoung_OneRoundNetwork
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

	#region /// \name partial class for LaiYoung_OneRoundNetwork
	public partial class LaiYoung_OneRoundNetwork: BaseNetwork
	{
		const string numMsg = "NumMsg";
		const string flag = "Flag";
		const string message = "Message";
		const m.MessageTypes Presnp = m.MessageTypes.Presnp;
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
		const string snapshot = "Snapshot";
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
		const string sent = "Sent";
		const string state = "State";
		const string expected = "Expected";
		const string arrived = "Arrived";
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
			ea[bn.eak.DirectedNetwork] = true;
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound.n+pak";
			dictionary.Add(n.pak.Version, new Attribute { Value = 9 ,Editable = false ,Changed = false } );
			base.InitPrivateAttributes();
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound.n+ork";
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for LaiYoung_OneRoundProcess
	public class p
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
			Snapshot, Recordered
		}
	}
	#endregion

	#region /// \name partial class for LaiYoung_OneRoundProcess
	public partial class LaiYoung_OneRoundProcess: BaseProcess
	{
		const string numMsg = "NumMsg";
		const string flag = "Flag";
		const string message = "Message";
		const m.MessageTypes Presnp = m.MessageTypes.Presnp;
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
		const string snapshot = "Snapshot";
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
		const string sent = "Sent";
		const string state = "State";
		const string expected = "Expected";
		const string arrived = "Arrived";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.String Snapshot
		{
			 get { return or[p.ork.Snapshot]; }
			 set { or[p.ork.Snapshot] = value; }
		}
 
		public System.Boolean Recordered
		{
			 get { return or[p.ork.Recordered]; }
			 set { or[p.ork.Recordered] = value; }
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound.p+pak";
			base.InitPrivateAttributes();
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound.p+ork";
			dictionary.Add(p.ork.Snapshot, new Attribute { Value = "" ,Changed = false } );
			dictionary.Add(p.ork.Recordered, new Attribute { Value = false ,Changed = false } );
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for LaiYoung_OneRoundChannel
	public class c
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
			Sent, State, Expected, Arrived
		}
	}
	#endregion

	#region /// \name partial class for LaiYoung_OneRoundChannel
	public partial class LaiYoung_OneRoundChannel: BaseChannel
	{
		const string numMsg = "NumMsg";
		const string flag = "Flag";
		const string message = "Message";
		const m.MessageTypes Presnp = m.MessageTypes.Presnp;
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
		const string snapshot = "Snapshot";
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
		const string sent = "Sent";
		const string state = "State";
		const string expected = "Expected";
		const string arrived = "Arrived";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.Int32 Sent
		{
			 get { return or[c.ork.Sent]; }
			 set { or[c.ork.Sent] = value; }
		}
 
		public DistributedAlgorithms.AttributeList State
		{
			 get { return or[c.ork.State]; }
			 set { or[c.ork.State] = value; }
		}
 
		public System.Int32 Expected
		{
			 get { return or[c.ork.Expected]; }
			 set { or[c.ork.Expected] = value; }
		}
 
		public System.Int32 Arrived
		{
			 get { return or[c.ork.Arrived]; }
			 set { or[c.ork.Arrived] = value; }
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound.c+pak";
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
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Snapshots.LaiYoung_OneRound.c+ork";
			dictionary.Add(c.ork.Sent, new Attribute { Value = 0 ,Changed = false } );
			dictionary.Add(c.ork.State, new Attribute { Value = Init_ork_State() ,Changed = false } );
			dictionary.Add(c.ork.Expected, new Attribute { Value = -1 ,Changed = false } );
			dictionary.Add(c.ork.Arrived, new Attribute { Value = 0 ,Changed = false } );
			base.InitOperationResults();
		}
	}
	#endregion
}