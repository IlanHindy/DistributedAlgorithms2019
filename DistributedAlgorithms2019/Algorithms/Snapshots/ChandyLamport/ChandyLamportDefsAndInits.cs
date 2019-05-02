using DistributedAlgorithms.Algorithms.Base.Base;
using System.Collections.Generic;


namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport
{

	#region /// \name Enums for ChandyLamportMessage
	public class m
	{
 
		public enum report
		{
			Id, Snapshot, Weight
		}
 
		public enum marker
		{
			Weight
		}
 
		public enum baseMessage
		{
	
		}
 
		public enum MessageTypes
		{
			Report, Marker, BaseMessage
		}
	}
	# endregion

	#region /// \name partial class for ChandyLamportProcess
	public partial class ChandyLamportProcess: BaseProcess
	{
 
		public AttributeDictionary MessageDataFor_Report( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null,
			System.Double id = 0,
			System.String snapshot = "",
			System.Double weight = 0)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
 
			if ( prmSource == bm.PrmSource.Default)
			{
				dictionary.Add( m.report.Id, new Attribute { Value = 0 ,Changed = false } );
				dictionary.Add( m.report.Snapshot, new Attribute { Value = "" ,Changed = false } );
				dictionary.Add( m.report.Weight, new Attribute { Value = 0 ,Changed = false } );
				return dictionary;
			} 

			dictionary.Add( m.report.Id, new Attribute { Value = id ,Changed = false } );
			dictionary.Add( m.report.Snapshot, new Attribute { Value = snapshot ,Changed = false } );
			dictionary.Add( m.report.Weight, new Attribute { Value = weight ,Changed = false } );
			return dictionary;
		}
 
		public AttributeDictionary MessageDataFor_Marker( bm.PrmSource prmSource = bm.PrmSource.Default,
			AttributeDictionary dictionary = null,
			System.Double weight = 0)
		{
 
			if ( prmSource == bm.PrmSource.MainPrm)
			{
				return dictionary;
			}
			dictionary = new AttributeDictionary();
 
			if ( prmSource == bm.PrmSource.Default)
			{
				dictionary.Add( m.marker.Weight, new Attribute { Value = 0 ,Changed = false } );
				return dictionary;
			} 

			dictionary.Add( m.marker.Weight, new Attribute { Value = weight ,Changed = false } );
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
 
			if ( prmSource == bm.PrmSource.Default)
			{
				return dictionary;
			} 

			return dictionary;
		}
 
		public void SendReport(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null)
		{
			if(fields is null)
			{
				MessageDataFor_Report();
			}
			Send(m.MessageTypes.Report, fields, selectingMethod, ids, -1, 0);
		}
 
		public void SendMarker(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null)
		{
			if(fields is null)
			{
				MessageDataFor_Marker();
			}
			Send(m.MessageTypes.Marker, fields, selectingMethod, ids, -1, 0);
		}
 
		public void SendBaseMessage(AttributeDictionary  fields = null, 
			SelectingMethod selectingMethod = SelectingMethod.All,
			List<int> ids = null)
		{
			if(fields is null)
			{
				MessageDataFor_BaseMessage();
			}
			Send(m.MessageTypes.BaseMessage, fields, selectingMethod, ids, -1, 0);
		}
	}
	#endregion

	#region /// \name Enums for ChandyLamportNetwork
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
	# endregion

	#region /// \name partial class for ChandyLamportNetwork
	public partial class ChandyLamportNetwork: BaseNetwork
	{
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
			dictionary.Add(n.pak.Version, new Attribute { Value = 6 ,Editable = false ,Changed = false } );
			base.InitPrivateAttributes();
		}
 
		protected override void InitOperationResults()
		{
			AttributeDictionary dictionary = or;
			base.InitOperationResults();
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
	}
	#endregion

	#region /// \name Enums for ChandyLamportProcess
	public class p
	{
 
		public enum pak
		{
			MaxRounds
		}
 
		public enum ork
		{
			Weight, Results, ReceivedMessageFrom, Snapshot, Recordered, StatusColor
		}
	}
	# endregion

	#region /// \name partial class for ChandyLamportProcess
	public partial class ChandyLamportProcess: BaseProcess
	{
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
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
			dictionary.Add(p.ork.Weight, new Attribute { Value = 0 ,Changed = false } );
			dictionary.Add(p.ork.Results, new Attribute { Value = Init_ork_Results() ,Changed = false } );
			dictionary.Add(p.ork.ReceivedMessageFrom, new Attribute { Value = Init_ork_ReceivedMessageFrom() ,Changed = false } );
			dictionary.Add(p.ork.Snapshot, new Attribute { Value = "" ,Changed = false } );
            dictionary.Add(p.ork.StatusColor, new Attribute { Value = "Not Set", Changed = false });
            dictionary.Add(p.ork.Recordered, new Attribute { Value = false ,Changed = false } );
			base.InitOperationResults();
		}
	}
	#endregion

	#region /// \name Enums for ChandyLamportChannel
	public class c
	{
 
		public enum pak
		{
	
		}
 
		public enum ork
		{
			Marker, State
		}
	}
	# endregion

	#region /// \name partial class for ChandyLamportChannel
	public partial class ChandyLamportChannel: BaseChannel
	{
 
		protected override void InitPrivateAttributes()
		{
			AttributeDictionary dictionary = pa;
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
			dictionary.Add(c.ork.Marker, new Attribute { Value = false ,Changed = false } );
			dictionary.Add(c.ork.State, new Attribute { Value = Init_ork_State() ,Changed = false } );
			base.InitOperationResults();
		}
	}
	#endregion
}