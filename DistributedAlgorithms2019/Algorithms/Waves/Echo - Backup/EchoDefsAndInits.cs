using DistributedAlgorithms.Algorithms.Base.Base;
using System.Collections.Generic;


namespace DistributedAlgorithms.Algorithms.Waves.Echo
{

	#region /// \name Enums for EchoMessage
	public class m
	{
 
		public enum wave
		{
			Parent
		}
 
		public enum MessageTypes
		{
			Wave
		}
	}
	# endregion

	#region /// \name partial class for EchoProcess
	public partial class EchoProcess: BaseProcess
	{
 
		public AttributeDictionary MessageDataFor_Wave( bm.PrmSource prmSource,
			AttributeDictionary dictionary = null,
			System.Int32 parent = 0)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
 
			if ( prmSource == bm.PrmSource.Default)
			{
				dictionary.Add( m.wave.Parent, new Attribute { Value = 0 } );
				return dictionary;
			} 

			dictionary.Add( m.wave.Parent, new Attribute { Value = parent } );
			return dictionary;
		}
 
		public void SendWave(AttributeDictionary  fields, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null)
		{
			Send(m.MessageTypes.Wave, fields, selectingMethod, ids, 0, 0);
		}
	}
	#endregion

	#region /// \name Enums for EchoNetwork
	public class n
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
	
		}
	}
	# endregion

	#region /// \name partial class for EchoNetwork
	public partial class EchoNetwork: BaseNetwork
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

	#region /// \name Enums for EchoProcess
	public class p
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
			Parent, Received
		}
	}
	# endregion

	#region /// \name partial class for EchoProcess
	public partial class EchoProcess: BaseProcess
	{
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			base.InitPrivateAttributes();
		}
		protected AttributeList Init_ork_Received()
		{
			AttributeList list = new AttributeList();
			return list;
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			dictionary.Add(p.ork.Parent, new Attribute { Value = -1 } );
			dictionary.Add(p.ork.Received, new Attribute { Value = Init_ork_Received() } );
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for EchoChannel
	public class c
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
	
		}
	}
	# endregion

	#region /// \name partial class for EchoChannel
	public partial class EchoChannel: BaseChannel
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
}