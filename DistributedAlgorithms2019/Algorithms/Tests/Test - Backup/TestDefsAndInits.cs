using DistributedAlgorithms.Algorithms.Base.Base;
using System.Collections.Generic;


namespace DistributedAlgorithms.Algorithms.Tests.Test
{

	#region /// \name Enums for TestProcess
	public class m
	{
 
		public enum baseMessage
		{
	
		}
 
		public enum algorithmMessage
		{
	
		}
 
		public enum message1
		{
			PrevAttr, S1
		}
 
		public enum MessageTypes
		{
			BaseMessage, AlgorithmMessage, Message1
		}
	}
	#endregion

	#region /// \name partial class for TestProcess
	public partial class TestProcess: BaseProcess
	{
 
		public AttributeDictionary MessageDataFor_BaseMessage( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.m+ork_baseMessage";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				return dictionary;
			} 

			return dictionary;
		}
 
		public AttributeDictionary MessageDataFor_AlgorithmMessage( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.m+ork_algorithmMessage";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				return dictionary;
			} 

			return dictionary;
		}
 
		public AttributeDictionary MessageDataFor_Message1( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null,
			System.Boolean prevAttr = false,
			System.String s1 = "")
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.m+ork_message1";
 
			if ( prmSource == bm.PrmSource.Default)
			{
				dictionary.Add( m.message1.PrevAttr, new Attribute { Value = false ,Changed = false } );
				dictionary.Add( m.message1.S1, new Attribute { Value = "" ,Changed = false } );
				return dictionary;
			} 

			dictionary.Add( m.message1.PrevAttr, new Attribute { Value = prevAttr ,Changed = false } );
			dictionary.Add( m.message1.S1, new Attribute { Value = s1 ,Changed = false } );
			return dictionary;
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
 
		public void SendAlgorithmMessage(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null,
			int round = -1,
			int clock = -1)
		{
			if(fields is null)
			{
				MessageDataFor_AlgorithmMessage();
			}
			Send(m.MessageTypes.AlgorithmMessage, fields, selectingMethod, ids, round, clock);
		}
 
		public void SendMessage1(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null,
			int round = -1,
			int clock = -1)
		{
			if(fields is null)
			{
				MessageDataFor_Message1();
			}
			Send(m.MessageTypes.Message1, fields, selectingMethod, ids, round, clock);
		}
	}
	#endregion

	#region /// \name Enums for TestNetwork
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

	#region /// \name partial class for TestNetwork
	public partial class TestNetwork: BaseNetwork
	{
		const string prevAttr = "PrevAttr";
		const string s1 = "S1";
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const m.MessageTypes AlgorithmMessage = m.MessageTypes.AlgorithmMessage;
		const m.MessageTypes Message1 = m.MessageTypes.Message1;
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
		const string maxRound = "MaxRound";
		const string runGetSetTest = "RunGetSetTest";
		const string testList = "TestList";
		const string testDictionary = "TestDictionary";
		const string testNetworkElement = "TestNetworkElement";
		const string testListNotEdittable = "TestListNotEdittable";
		const string testDictionaryNotEdittable = "TestDictionaryNotEdittable";
		const string testNetworkElementNotEdittable = "TestNetworkElementNotEdittable";
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
		const string cs2 = "Cs2";
		const string cs1 = "Cs1";
		const string cs3 = "Cs3";
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
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.n+pak";
			dictionary.Add(n.pak.Version, new Attribute { Value = 12 ,Editable = false ,Changed = false } );
			base.InitPrivateAttributes();
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.n+ork";
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for TestProcess
	public class p
	{
 
		public enum pak
		{
			MaxRound, RunGetSetTest
		}
 
		public enum ork_testDictionary
		{
	
		}
 
		public enum ork_testDictionaryNotEdittable
		{
	
		}
 
		public enum ork
		{
			S1, TestList, TestDictionary, TestNetworkElement, TestListNotEdittable, TestDictionaryNotEdittable, TestNetworkElementNotEdittable
		}
	}
	#endregion

	#region /// \name partial class for TestProcess
	public partial class TestProcess: BaseProcess
	{
		const string prevAttr = "PrevAttr";
		const string s1 = "S1";
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const m.MessageTypes AlgorithmMessage = m.MessageTypes.AlgorithmMessage;
		const m.MessageTypes Message1 = m.MessageTypes.Message1;
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
		const string maxRound = "MaxRound";
		const string runGetSetTest = "RunGetSetTest";
		const string testList = "TestList";
		const string testDictionary = "TestDictionary";
		const string testNetworkElement = "TestNetworkElement";
		const string testListNotEdittable = "TestListNotEdittable";
		const string testDictionaryNotEdittable = "TestDictionaryNotEdittable";
		const string testNetworkElementNotEdittable = "TestNetworkElementNotEdittable";
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
		const string cs2 = "Cs2";
		const string cs1 = "Cs1";
		const string cs3 = "Cs3";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.Int32 MaxRound
		{
			 get { return pa[p.pak.MaxRound]; }
			 set { pa[p.pak.MaxRound] = value; }
		}
 
		public System.Boolean RunGetSetTest
		{
			 get { return pa[p.pak.RunGetSetTest]; }
			 set { pa[p.pak.RunGetSetTest] = value; }
		}
 
		public System.String S1
		{
			 get { return or[p.ork.S1]; }
			 set { or[p.ork.S1] = value; }
		}
 
		public DistributedAlgorithms.AttributeList TestList
		{
			 get { return or[p.ork.TestList]; }
			 set { or[p.ork.TestList] = value; }
		}
 
		public DistributedAlgorithms.AttributeDictionary TestDictionary
		{
			 get { return or[p.ork.TestDictionary]; }
			 set { or[p.ork.TestDictionary] = value; }
		}
 
		public DistributedAlgorithms.NetworkElement TestNetworkElement
		{
			 get { return or[p.ork.TestNetworkElement]; }
			 set { or[p.ork.TestNetworkElement] = value; }
		}
 
		public DistributedAlgorithms.AttributeList TestListNotEdittable
		{
			 get { return or[p.ork.TestListNotEdittable]; }
			 set { or[p.ork.TestListNotEdittable] = value; }
		}
 
		public DistributedAlgorithms.AttributeDictionary TestDictionaryNotEdittable
		{
			 get { return or[p.ork.TestDictionaryNotEdittable]; }
			 set { or[p.ork.TestDictionaryNotEdittable] = value; }
		}
 
		public DistributedAlgorithms.NetworkElement TestNetworkElementNotEdittable
		{
			 get { return or[p.ork.TestNetworkElementNotEdittable]; }
			 set { or[p.ork.TestNetworkElementNotEdittable] = value; }
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.p+pak";
			dictionary.Add(p.pak.MaxRound, new Attribute { Value = 5 ,Changed = false } );
			dictionary.Add(p.pak.RunGetSetTest, new Attribute { Value = false ,Changed = false } );
			base.InitPrivateAttributes();
		}
		protected AttributeList Init_ork_TestList()
		{
			AttributeList list = new AttributeList();
			list.codeAttributeCounter = 0;
			list.designAttributeCounter = 0;
			return list;
		}
 
		protected AttributeDictionary Init_ork_TestDictionary()
		{
			AttributeDictionary dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.p+ork_testDictionary";
			return dictionary;
		}
		protected AttributeList Init_ork_TestListNotEdittable()
		{
			AttributeList list = new AttributeList();
			list.codeAttributeCounter = 0;
			list.designAttributeCounter = 0;
			return list;
		}
 
		protected AttributeDictionary Init_ork_TestDictionaryNotEdittable()
		{
			AttributeDictionary dictionary = new AttributeDictionary();
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.p+ork_testDictionaryNotEdittable";
			return dictionary;
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.p+ork";
			dictionary.Add(p.ork.S1, new Attribute { Value = "" ,Changed = false } );
			dictionary.Add(p.ork.TestList, new Attribute { Value = Init_ork_TestList() ,Changed = false } );
			dictionary.Add(p.ork.TestDictionary, new Attribute { Value = Init_ork_TestDictionary() ,Changed = false } );
			dictionary.Add(p.ork.TestNetworkElement, new Attribute { Value = new DistributedAlgorithms.NetworkElement() ,Changed = false } );
			dictionary.Add(p.ork.TestListNotEdittable, new Attribute { Value = Init_ork_TestListNotEdittable() ,Editable = false ,Changed = false } );
			dictionary.Add(p.ork.TestDictionaryNotEdittable, new Attribute { Value = Init_ork_TestDictionaryNotEdittable() ,Editable = false ,Changed = false } );
			dictionary.Add(p.ork.TestNetworkElementNotEdittable, new Attribute { Value = new DistributedAlgorithms.NetworkElement() ,Editable = false ,Changed = false } );
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for TestChannel
	public class c
	{
 
		public enum pak
		{
			Cs2, Cs1
		}
 
		public enum ork
		{
			Cs3
		}
	}
	#endregion

	#region /// \name partial class for TestChannel
	public partial class TestChannel: BaseChannel
	{
		const string prevAttr = "PrevAttr";
		const string s1 = "S1";
		const m.MessageTypes BaseMessage = m.MessageTypes.BaseMessage;
		const m.MessageTypes AlgorithmMessage = m.MessageTypes.AlgorithmMessage;
		const m.MessageTypes Message1 = m.MessageTypes.Message1;
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
		const string maxRound = "MaxRound";
		const string runGetSetTest = "RunGetSetTest";
		const string testList = "TestList";
		const string testDictionary = "TestDictionary";
		const string testNetworkElement = "TestNetworkElement";
		const string testListNotEdittable = "TestListNotEdittable";
		const string testDictionaryNotEdittable = "TestDictionaryNotEdittable";
		const string testNetworkElementNotEdittable = "TestNetworkElementNotEdittable";
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
		const string cs2 = "Cs2";
		const string cs1 = "Cs1";
		const string cs3 = "Cs3";
		const string sourcePort = "SourcePort";
		const string destPort = "DestPort";
		const string lineColor = "LineColor";
		const string headColor = "HeadColor";
		const string presentationTxt = "PresentationTxt";
		const string messagesFrameColor = "MessagesFrameColor";
		const string messagesFrameWidth = "MessagesFrameWidth";
		const string messagesBackground = "MessagesBackground";
		const string messagesForeground = "MessagesForeground";
 
		public System.String Cs2
		{
			 get { return pa[c.pak.Cs2]; }
			 set { pa[c.pak.Cs2] = value; }
		}
 
		public System.String Cs1
		{
			 get { return pa[c.pak.Cs1]; }
			 set { pa[c.pak.Cs1] = value; }
		}
 
		public System.String Cs3
		{
			 get { return or[c.ork.Cs3]; }
			 set { or[c.ork.Cs3] = value; }
		}
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.c+pak";
			dictionary.Add(c.pak.Cs2, new Attribute { Value = "" ,Changed = false } );
			dictionary.Add(c.pak.Cs1, new Attribute { Value = "" ,Changed = false } );
			base.InitPrivateAttributes();
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.selfEnumName = "DistributedAlgorithms.Algorithms.Tests.Test.c+ork";
			dictionary.Add(c.ork.Cs3, new Attribute { Value = "" ,Changed = false } );
			base.InitOperationResults();
		}
	}
	#endregion
}