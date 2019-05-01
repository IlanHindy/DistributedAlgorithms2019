
using System.Collections.Generic;
using DistributedAlgorithms;
using DistributedAlgorithms.Algorithms.Base.Base;
namespace DistributedAlgorithms
{

	#region /// \name partial class for NetworkElement
	public partial class NetworkElement: IValueHolder
	{
 
		public System.String Type
		{
			 get { return ea[ne.eak.Type]; }
			 set { ea[ne.eak.Type] = value; }
		}
 
		public System.Boolean Edited
		{
			 get { return ea[ne.eak.Edited]; }
			 set { ea[ne.eak.Edited] = value; }
		}
 
		public System.Int32 Id
		{
			 get { return ea[ne.eak.Id]; }
			 set { ea[ne.eak.Id] = value; }
		}
	}
	#endregion
}
namespace DistributedAlgorithms.Algorithms.Base.Base
{
	#region /// \name partial class for BaseMessage
	public partial class BaseMessage: NetworkElement
	{
 
		public DistributedAlgorithms.Algorithms.Base.Base.bm.MessageTypes MessageType
		{
			 get { return pa[bm.pak.MessageType]; }
			 set { pa[bm.pak.MessageType] = value; }
		}
 
		public System.Int32 SourceProcess
		{
			 get { return pa[bm.pak.SourceProcess]; }
			 set { pa[bm.pak.SourceProcess] = value; }
		}
 
		public System.Int32 SourcePort
		{
			 get { return pa[bm.pak.SourcePort]; }
			 set { pa[bm.pak.SourcePort] = value; }
		}
 
		public System.Int32 DestProcess
		{
			 get { return pa[bm.pak.DestProcess]; }
			 set { pa[bm.pak.DestProcess] = value; }
		}
 
		public System.Int32 DestPort
		{
			 get { return pa[bm.pak.DestPort]; }
			 set { pa[bm.pak.DestPort] = value; }
		}
 
		public System.Int32 Round
		{
			 get { return pa[bm.pak.Round]; }
			 set { pa[bm.pak.Round] = value; }
		}
 
		public System.Int32 LogicalClock
		{
			 get { return pa[bm.pak.LogicalClock]; }
			 set { pa[bm.pak.LogicalClock] = value; }
		}
 
		public DistributedAlgorithms.Breakpoint Breakpoints
		{
			 get { return op[bm.opk.Breakpoints]; }
			 set { op[bm.opk.Breakpoints] = value; }
		}
 
		public System.String Name
		{
			 get { return or[bm.ork.Name]; }
			 set { or[bm.ork.Name] = value; }
		}
 
		public System.Int32 PositionInProcessQ
		{
			 get { return or[bm.ork.PositionInProcessQ]; }
			 set { or[bm.ork.PositionInProcessQ] = value; }
		}
	}
	#endregion
	#region /// \name partial class for BaseNetwork
	public partial class BaseNetwork: NetworkElement
	{
 
		public System.String Algorithm
		{
			 get { return ea[bn.eak.Algorithm]; }
			 set { ea[bn.eak.Algorithm] = value; }
		}
 
		public System.Boolean Centrilized
		{
			 get { return ea[bn.eak.Centrilized]; }
			 set { ea[bn.eak.Centrilized] = value; }
		}
 
		public System.Boolean DirectedNetwork
		{
			 get { return ea[bn.eak.DirectedNetwork]; }
			 set { ea[bn.eak.DirectedNetwork] = value; }
		}
 
		public DistributedAlgorithms.Breakpoint Breakpoints
		{
			 get { return op[bn.opk.Breakpoints]; }
			 set { op[bn.opk.Breakpoints] = value; }
		}
 
		public System.Boolean SingleStepStatus
		{
			 get { return or[bn.ork.SingleStepStatus]; }
			 set { or[bn.ork.SingleStepStatus] = value; }
		}
	}
	#endregion
	#region /// \name partial class for BaseProcess
	public partial class BaseProcess: NetworkElement
	{
 
		public System.String Name
		{
			 get { return ea[bp.eak.Name]; }
			 set { ea[bp.eak.Name] = value; }
		}
 
		public System.Boolean Initiator
		{
			 get { return ea[bp.eak.Initiator]; }
			 set { ea[bp.eak.Initiator] = value; }
		}
 
		public DistributedAlgorithms.Breakpoint Breakpoints
		{
			 get { return op[bp.opk.Breakpoints]; }
			 set { op[bp.opk.Breakpoints] = value; }
		}
 
		public DistributedAlgorithms.BaseAlgorithmHandler BaseAlgorithm
		{
			 get { return op[bp.opk.BaseAlgorithm]; }
			 set { op[bp.opk.BaseAlgorithm] = value; }
		}
 
		public DistributedAlgorithms.InternalEventsHandler InternalEvents
		{
			 get { return op[bp.opk.InternalEvents]; }
			 set { op[bp.opk.InternalEvents] = value; }
		}
 
		public DistributedAlgorithms.ChangeMessageOrder ChangeOrderEvents
		{
			 get { return op[bp.opk.ChangeOrderEvents]; }
			 set { op[bp.opk.ChangeOrderEvents] = value; }
		}
 
		public System.Int32 Round
		{
			 get { return or[bp.ork.Round]; }
			 set { or[bp.ork.Round] = value; }
		}
 
		public DistributedAlgorithms.Algorithms.Base.Base.BaseProcess.TerminationStatuses TerminationStatus
		{
			 get { return or[bp.ork.TerminationStatus]; }
			 set { or[bp.ork.TerminationStatus] = value; }
		}
 
		public DistributedAlgorithms.MessageQ MessageQ
		{
			 get { return or[bp.ork.MessageQ]; }
			 set { or[bp.ork.MessageQ] = value; }
		}
 
		public System.Int32 ReceivePort
		{
			 get { return or[bp.ork.ReceivePort]; }
			 set { or[bp.ork.ReceivePort] = value; }
		}
 
		public System.Drawing.KnownColor FrameColor
		{
			 get { return pp[bp.ppk.FrameColor]; }
			 set { pp[bp.ppk.FrameColor] = value; }
		}
 
		public System.Int32 FrameWidth
		{
			 get { return pp[bp.ppk.FrameWidth]; }
			 set { pp[bp.ppk.FrameWidth] = value; }
		}
 
		public System.Int32 FrameHeight
		{
			 get { return pp[bp.ppk.FrameHeight]; }
			 set { pp[bp.ppk.FrameHeight] = value; }
		}
 
		public System.Int32 FrameLeft
		{
			 get { return pp[bp.ppk.FrameLeft]; }
			 set { pp[bp.ppk.FrameLeft] = value; }
		}
 
		public System.Int32 FrameTop
		{
			 get { return pp[bp.ppk.FrameTop]; }
			 set { pp[bp.ppk.FrameTop] = value; }
		}
 
		public System.Int32 FrameLineWidth
		{
			 get { return pp[bp.ppk.FrameLineWidth]; }
			 set { pp[bp.ppk.FrameLineWidth] = value; }
		}
 
		public System.Drawing.KnownColor Background
		{
			 get { return pp[bp.ppk.Background]; }
			 set { pp[bp.ppk.Background] = value; }
		}
 
		public System.Drawing.KnownColor Foreground
		{
			 get { return pp[bp.ppk.Foreground]; }
			 set { pp[bp.ppk.Foreground] = value; }
		}
 
		public System.String Text
		{
			 get { return pp[bp.ppk.Text]; }
			 set { pp[bp.ppk.Text] = value; }
		}
 
		public System.Drawing.KnownColor BreakpointsFrameColor
		{
			 get { return pp[bp.ppk.BreakpointsFrameColor]; }
			 set { pp[bp.ppk.BreakpointsFrameColor] = value; }
		}
 
		public System.Int32 BreakpointsFrameWidth
		{
			 get { return pp[bp.ppk.BreakpointsFrameWidth]; }
			 set { pp[bp.ppk.BreakpointsFrameWidth] = value; }
		}
 
		public System.Drawing.KnownColor BreakpointsBackground
		{
			 get { return pp[bp.ppk.BreakpointsBackground]; }
			 set { pp[bp.ppk.BreakpointsBackground] = value; }
		}
 
		public System.Drawing.KnownColor BreakpointsForeground
		{
			 get { return pp[bp.ppk.BreakpointsForeground]; }
			 set { pp[bp.ppk.BreakpointsForeground] = value; }
		}
	}
	#endregion
	#region /// \name partial class for BaseChannel
	public partial class BaseChannel: NetworkElement
	{
 
		public System.Int32 SourceProcess
		{
			 get { return ea[bc.eak.SourceProcess]; }
			 set { ea[bc.eak.SourceProcess] = value; }
		}
 
		public System.Int32 DestProcess
		{
			 get { return ea[bc.eak.DestProcess]; }
			 set { ea[bc.eak.DestProcess] = value; }
		}
 
		public DistributedAlgorithms.Breakpoint Breakpoints
		{
			 get { return op[bc.opk.Breakpoints]; }
			 set { op[bc.opk.Breakpoints] = value; }
		}
 
		public System.Int32 SourcePort
		{
			 get { return or[bc.ork.SourcePort]; }
			 set { or[bc.ork.SourcePort] = value; }
		}
 
		public System.Int32 DestPort
		{
			 get { return or[bc.ork.DestPort]; }
			 set { or[bc.ork.DestPort] = value; }
		}
 
		public DistributedAlgorithms.Algorithms.Base.Base.BaseChannel.TerminationStatuses TerminationStatus
		{
			 get { return or[bc.ork.TerminationStatus]; }
			 set { or[bc.ork.TerminationStatus] = value; }
		}
 
		public System.Drawing.KnownColor LineColor
		{
			 get { return pp[bc.ppk.LineColor]; }
			 set { pp[bc.ppk.LineColor] = value; }
		}
 
		public System.Drawing.KnownColor HeadColor
		{
			 get { return pp[bc.ppk.HeadColor]; }
			 set { pp[bc.ppk.HeadColor] = value; }
		}
 
		public System.Drawing.KnownColor FrameColor
		{
			 get { return pp[bc.ppk.FrameColor]; }
			 set { pp[bc.ppk.FrameColor] = value; }
		}
 
		public System.Int32 FrameWidth
		{
			 get { return pp[bc.ppk.FrameWidth]; }
			 set { pp[bc.ppk.FrameWidth] = value; }
		}
 
		public System.Drawing.KnownColor Background
		{
			 get { return pp[bc.ppk.Background]; }
			 set { pp[bc.ppk.Background] = value; }
		}
 
		public System.Drawing.KnownColor Foreground
		{
			 get { return pp[bc.ppk.Foreground]; }
			 set { pp[bc.ppk.Foreground] = value; }
		}
 
		public System.String PresentationTxt
		{
			 get { return pp[bc.ppk.PresentationTxt]; }
			 set { pp[bc.ppk.PresentationTxt] = value; }
		}
 
		public System.Drawing.KnownColor MessagesFrameColor
		{
			 get { return pp[bc.ppk.MessagesFrameColor]; }
			 set { pp[bc.ppk.MessagesFrameColor] = value; }
		}
 
		public System.Int32 MessagesFrameWidth
		{
			 get { return pp[bc.ppk.MessagesFrameWidth]; }
			 set { pp[bc.ppk.MessagesFrameWidth] = value; }
		}
 
		public System.Drawing.KnownColor MessagesBackground
		{
			 get { return pp[bc.ppk.MessagesBackground]; }
			 set { pp[bc.ppk.MessagesBackground] = value; }
		}
 
		public System.Drawing.KnownColor MessagesForeground
		{
			 get { return pp[bc.ppk.MessagesForeground]; }
			 set { pp[bc.ppk.MessagesForeground] = value; }
		}
	}
	#endregion
}