using DistributedAlgorithms.Algorithms.Base.Base;


namespace DistributedAlgorithms.Algorithms.Waves.EchoImproved
{

	#region /// \name Enums for EchoImprovedNetwork
	public class n
	{
		public enum pak
		{
		
		}
		public enum ork
		{
		
		}
	}
	#endregion

	#region /// \name partial class for EchoImprovedNetwork
	public partial class EchoImprovedNetwork: BaseNetwork
	{
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			base.InitPrivateAttributes();
		}
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for EchoImprovedProcess
	public class p
	{
		public enum pak
		{
		
		}
		public enum ork
		{
			Parent,Received
		}
	}
	#endregion

	#region /// \name partial class for EchoImprovedProcess
	public partial class EchoImprovedProcess: BaseProcess
	{
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			base.InitPrivateAttributes();
		}
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.Add(p.ork.Parent,new Attribute { Value = -1});
			dictionary.Add(p.ork.Received,new Attribute { Value = 0});
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for EchoImprovedChannel
	public class c
	{
		public enum pak
		{
		
		}
		public enum ork
		{
		
		}
	}
	#endregion

	#region /// \name partial class for EchoImprovedChannel
	public partial class EchoImprovedChannel: BaseChannel
	{
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			base.InitPrivateAttributes();
		}
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for EchoImprovedMessage
	public class m
	{
		public enum messageTypes
		{
			Wave
		}
		public enum fieldKeys 
		{
			Sender
		}
	}
	#endregion
}