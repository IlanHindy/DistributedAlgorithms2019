////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Algorithms\Base\Message.cs
///
///\brief   Implements the message class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DistributedAlgorithms.Algorithms.Base.Base
{
    #region /// \name Enums

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class bm
    ///
    /// \brief A bm.
    ///
    /// \par Description.
    ///      This class holds the enums decelerations of BaseMessage
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 27/06/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class bm
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum headerork
        ///
        /// \brief Keys for the header of the messages which are common to all the messages
        ///        The fields are in the PrivateAttributes dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum pak { MessageType, SourceProcess, SourcePort, DestProcess, DestPort, Round, LogicalClock}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum messageTypes
        ///
        /// \brief Keys for the message types. The following message types are used by the Base
        ///        algorithm and message types used by the BaseProcess for it's internal processing 
        ///        (Terminate, EmptyMessage). The fields are in the OperationResults dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum MessageTypes { NullMessageType, EmptyMessage, Terminate, Forewared, Backwared, LastMessage, BaseMessage }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum opk
        ///
        /// \brief Keys to be used in the Operation Parameters .
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum opk { Breakpoints }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum fieldKeys
        ///
        /// \brief Keys for the other fields that are not part of the header. The keys are common to all the 
        ///        algorithms
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ork { Name, PositionInProcessQ }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum PrmSource
        ///
        /// \brief Values that represent prm sources.
        ///        Tesls the message building methods which set of parameters to use
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum PrmSource { MainPrm, Default, Prms}
    }
#endregion
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class BaseMessage
    ///
    /// \brief A base message.
    ///
    /// \par Description. 
    ///      This class represents a message The message holds a header in the
    ///      PrivateAttributes dictionary If additional fields are requiered it can be added by the
    ///      message creator to the OperationResults dictionary.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 14/03/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class BaseMessage : NetworkElement
    {
        #region /// \name Member Variables
        #endregion
        
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage() : base(true)
        ///
        /// \brief Default constructor.       
        ///
        /// \par Description.
        ///      See explanation about constructors in NetworkElement
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage() : base(true) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(BaseNetwork network):base(network)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      See explanation about constructors in NetworkElement
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param network  (BaseNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage(BaseNetwork network):base(network)
        {
            this.network = network;            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(BaseNetwork network, BaseMessage sourceMessage) : base(sourceMessage)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      See explanation about constructors in NetworkElement
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param network        (BaseNetwork) - The network.
        /// \param sourceMessage  (BaseMessage) - Message describing the source.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage(BaseNetwork network, BaseMessage sourceMessage) : base(sourceMessage)
        {
            this.network = network;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(BaseNetwork network, BaseMessage sourceMessage, BaseChannel sendingChannel): base(true)
        ///
        /// \brief Constructor.
        ///
        /// \par Description. 
        ///      A message that is duplicatted to the source except for the channel parameters.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network         (BaseNetwork) - The network.
        /// \param sourceMessage  (BaseMessage) - Message describing the source.
        /// \param sendingChannel (BaseChannel) - The sending channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage(BaseNetwork network,
            BaseMessage sourceMessage, 
            BaseChannel sendingChannel):
            base(true)
        {
            this.network = network;
            pa.Add(bm.pak.MessageType, new Attribute { Value = sourceMessage.pa[bm.pak.MessageType] });
            pa.Add(bm.pak.Round, new Attribute { Value = sourceMessage.pa[bm.pak.Round] });
            pa.Add(bm.pak.LogicalClock, new Attribute { Value = sourceMessage.pa[bm.pak.LogicalClock] });
            pa.Add(bm.pak.SourceProcess, new Attribute { Value = sendingChannel.ea[bc.eak.SourceProcess] });
            pa.Add(bm.pak.SourcePort, new Attribute { Value = sendingChannel.or[bc.ork.SourcePort], IncludedInShortDescription=false, IncludeInEqualsTo = false });
            pa.Add(bm.pak.DestProcess, new Attribute { Value = sendingChannel.ea[bc.eak.DestProcess] });
            pa.Add(bm.pak.DestPort, new Attribute { Value = sendingChannel.or[bc.ork.DestPort], IncludedInShortDescription = false, IncludeInEqualsTo = false });
            op.DeepCopy(sourceMessage.op);
            or.DeepCopy(sourceMessage.or);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(BaseNetwork network, dynamic messageType, AttributeDictionary fields, BaseChannel channel, string messageName = "" , int round, int logicalClock):base(true)
        ///
        /// \brief Constructor.
        ///
        /// \par Description. 
        ///      Create a message from :
        ///      -# The messageType
        ///      -# Fields for the algorithm specific fields in attribute dictionary.
        ///      -# The sending parameters from the channel
        ///      -# The message name
        ///      -# The parameters for the header : round, logical clock
        ///      
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network       (BaseNetwork) - The network.
        /// \param messageType  (dynamic) - Type of the message.
        /// \param fields       (AttributeDictionary) - The fields.
        /// \param channel      (BaseChannel) - The channel.
        /// \param messageName  (Optional)  (string) - Name of the message.
        /// \param round        (Optional) (int) - The round.
        /// \param logicalClock (Optional) (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage(BaseNetwork network,
            dynamic messageType, 
            AttributeDictionary fields, 
            BaseChannel channel,
            string messageName = "" ,
            int round = 0, int logicalClock = 0):base(true)
        {
            this.network = network;
            pa.Add(bm.pak.MessageType, new Attribute { Value = messageType});
            try
            {
                pa.Add(bm.pak.SourceProcess, new Attribute { Value = channel.ea[bc.eak.SourceProcess] });
                pa.Add(bm.pak.SourcePort, new Attribute { Value = channel.or[bc.ork.SourcePort], IncludedInShortDescription = false, IncludeInEqualsTo=false });
                pa.Add(bm.pak.DestProcess, new Attribute { Value = channel.ea[bc.eak.DestProcess] });
                pa.Add(bm.pak.DestPort, new Attribute { Value = channel.or[bc.ork.DestPort], IncludedInShortDescription = false, IncludeInEqualsTo=false });
            }
            catch
            {
                pa.Add(bm.pak.SourceProcess, new Attribute { Value = 0 });
                pa.Add(bm.pak.SourcePort, new Attribute { Value = 0, IncludedInShortDescription = false, IncludeInEqualsTo=false });
                pa.Add(bm.pak.DestProcess, new Attribute { Value = 0 });
                pa.Add(bm.pak.DestPort, new Attribute { Value = 0, IncludedInShortDescription = false, IncludeInEqualsTo=false });
            }
            pa.Add(bm.pak.Round, new Attribute { Value = round });
            pa.Add(bm.pak.LogicalClock, new Attribute { Value = logicalClock });
            if (messageName == "")
            {
                messageName = TypesUtility.GetKeyToString(messageType);
            }
            or.Add(bm.ork.Name, new Attribute { Value = messageName });
            op.Add(bm.opk.Breakpoints, new Attribute { Value = new Breakpoint(Breakpoint.HostingElementTypes.Message), Editable = false, IncludedInShortDescription = false });
            or.Add(bm.ork.PositionInProcessQ, new Attribute { Value = 0, IncludedInShortDescription = false });

            if (fields != null)
            {
                foreach (var entry in fields)
                {
                    AddField(entry.Key, entry.Value);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(BaseNetwork network, string messageString) :base(true)
        ///
        /// \brief Constructor.
        ///
        /// \par Description. 
        ///      Create a message from string which holds the message xml.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network        (BaseNetwork) - The network.
        /// \param messageString (string) - A xml message string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage(BaseNetwork network,
            string messageString)
            :base(true) 
        {
            this.network = network;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(messageString);
            Load(xmlDoc.GetElementsByTagName("Message")[0]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(BaseNetwork network, dynamic messageType, int sourceProcess, int sourcePort, int destProcess, int destPort, string messageName, int round, int logicalClock):base(true)
        ///
        /// \brief Constructor.
        ///
        /// \par Description. 
        ///      Create a message when all the header fields are given.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network        (BaseNetwork) - The network.
        /// \param messageType   (dynamic) - Type of the message.
        /// \param sourceProcess (int) - Source process.
        /// \param sourcePort    (int) - Source port.
        /// \param destProcess   (int) - Destination process.
        /// \param destPort      (int) - Destination port.
        /// \param messageName    (string) - Name of the message.
        /// \param round         (int) - The round.
        /// \param logicalClock  (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage(BaseNetwork network,
            dynamic messageType,
            int sourceProcess,
            int sourcePort,
            int destProcess,
            int destPort,
            string messageName,
            int round,
            int logicalClock):base(true)
        {
            this.network = network;
            pa.Add(bm.pak.MessageType, new Attribute { Value = messageType });
            pa.Add(bm.pak.SourceProcess, new Attribute { Value = sourceProcess });
            pa.Add(bm.pak.SourcePort, new Attribute { Value = sourcePort, IncludedInShortDescription = false, IncludeInEqualsTo=false });
            pa.Add(bm.pak.DestProcess, new Attribute { Value = destProcess });
            pa.Add(bm.pak.DestPort, new Attribute { Value = destPort, IncludedInShortDescription = false, IncludeInEqualsTo=false });
            pa.Add(bm.pak.Round, new Attribute { Value = round });
            pa.Add(bm.pak.LogicalClock, new Attribute { Value = logicalClock });
            op.Add(bm.opk.Breakpoints, new Attribute { Value = new Breakpoint(Breakpoint.HostingElementTypes.Message), Editable = false, IncludedInShortDescription = false });
            or.Add(bm.ork.Name, new Attribute { Value = messageName});
            or.Add(bm.ork.PositionInProcessQ, new Attribute { Value = 0, IncludedInShortDescription = false });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(BaseNetwork network, dynamic messageType, BaseChannel channel, string messageName = "" , int round = 0, int logicalClock = 0):base(true)
        ///
        /// \brief Constructor.
        ///
        /// \par Description. 
        ///      Construct a message from header parameters.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network       (BaseNetwork) - The network.
        /// \param messageType  (dynamic) - Type of the message.
        /// \param channel      (BaseChannel) - The channel.
        /// \param messageName  (Optional)  (string) - Name of the message.
        /// \param round        (Optional)  (int) - The round.
        /// \param logicalClock (Optional)  (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage(BaseNetwork network,
            dynamic messageType, 
            BaseChannel channel,
            string messageName = "" ,
            int round = 0, int logicalClock = 0):base(true)
        {
            this.network = network;
            pa.Add(bm.pak.MessageType, new Attribute { Value = messageType });
            pa.Add(bm.pak.SourceProcess, new Attribute { Value = channel.ea[bc.eak.SourceProcess] });
            pa.Add(bm.pak.SourcePort, new Attribute { Value = channel.or[bc.ork.SourcePort], IncludedInShortDescription = false, IncludeInEqualsTo=false });
            pa.Add(bm.pak.DestProcess, new Attribute { Value = channel.ea[bc.eak.DestProcess] });
            pa.Add(bm.pak.DestPort, new Attribute { Value = channel.or[bc.ork.DestPort], IncludedInShortDescription = false, IncludeInEqualsTo=false });
            pa.Add(bm.pak.Round, new Attribute { Value = round });
            pa.Add(bm.pak.LogicalClock, new Attribute { Value = logicalClock });
            if (messageName == "")
            {
                messageName = TypesUtility.GetKeyToString(messageType);
            }
            or.Add(bm.ork.Name, new Attribute { Value = messageName });
            op.Add(bm.opk.Breakpoints, new Attribute { Value = new Breakpoint(Breakpoint.HostingElementTypes.Message), Editable = false, IncludedInShortDescription = false });
            or.Add(bm.ork.PositionInProcessQ, new Attribute { Value = 0, IncludedInShortDescription = false });
        }
        #endregion
        #region /// \name Init (for code generation purposes

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected sealed override void InitPrivateAttributes()
        ///
        /// \brief Init private attributes.
        ///        Init private attributes for automatic code generation purposes
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/03/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected sealed override void InitPrivateAttributes()
        {
            pa.Add(bm.pak.MessageType, new Attribute { Value = bm.MessageTypes.NullMessageType });
            pa.Add(bm.pak.SourceProcess, new Attribute { Value = 0 });
            pa.Add(bm.pak.SourcePort, new Attribute { Value = 0, IncludedInShortDescription=false, IncludeInEqualsTo=false });
            pa.Add(bm.pak.DestProcess, new Attribute { Value = 0 });
            pa.Add(bm.pak.DestPort, new Attribute { Value = 0, IncludedInShortDescription=false, IncludeInEqualsTo=false});
            pa.Add(bm.pak.Round, new Attribute { Value = 0});
            pa.Add(bm.pak.LogicalClock, new Attribute { Value = 0});
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected sealed override void InitOperationResults()
        ///
        /// \brief Init operation results.
        ///
        /// \par Description.
        ///      Init operation results for automatic code generation purposes
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/03/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected sealed override void InitOperationResults()
        {
            or.Add(bm.ork.Name, new Attribute { Value = "" });
            op.Add(bm.opk.Breakpoints, new Attribute { Value = new Breakpoint()});
            or.Add(bm.ork.PositionInProcessQ, new Attribute { Value = 0, IncludedInShortDescription=false});
        }

        #endregion
        #region /// \name Methods used in send and receive processes

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseMessage CreateMessage(string messageString)
        ///
        /// \brief Creates a message.
        ///
        /// \par Description.
        ///      - This method is used by the AsynchronousReader to generate all types of the messages
        ///      - The messages are sent in Xml.
        ///      - The root name is the type of the message
        ///
        /// \par Algorithm.
        ///        -#   Load the string to XmlDocument
        ///        -#   Get the root node
        ///        -#   Generate a message from the type which is the name of the root node
        ///        -#   Load all the message
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param messageString  (string) - The message string.
        ///
        /// \return The new message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseMessage CreateMessage(BaseNetwork network, string messageString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(messageString);
            XmlElement root = xmlDoc.DocumentElement;

            // Create a message with no parameters constructor so that the permissions will not interfeer with the 
            // loading
            BaseMessage message = (BaseMessage)TypesUtility.CreateObjectFromTypeString(root.Name);
            message.Load(root);
            return message;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string MessageString()
        ///
        /// \brief Message string.
        ///
        /// \par Description. 
        ///        Convert the message to xml string.
        ///        This method is used by the AsynchronousSender to generate the string that will be sent
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string MessageString()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode xmlNode = xmlDoc.CreateElement(GetType().ToString());
            Save(xmlDoc, xmlNode);
            xmlDoc.AppendChild(xmlNode);
            string[] messageLines = Logger.formatXml(xmlDoc);
            string messageString = "";
            for (int idx = 0; idx < messageLines.Length; idx++)
            {
                messageString += messageLines[idx];
            }
            return messageString;
        }
        #endregion
        #region /// \name Fields access

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GetMessageType()
        ///
        /// \brief Gets message type.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \return The message type.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GetMessageType()
        {
            return  TypesUtility.GetKeyToString(pa[bm.pak.MessageType]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void AddField(dynamic key, Attribute attribute)
        ///
        /// \brief Adds a field to the message
        ///
        /// \par Description. 
        ///        Add a field to the algorithm specific dictionary (OperationResults).
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param key       (dynamic) - The key.
        /// \param attribute (Attribute) - The attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddField(dynamic key, dynamic value)
        {
            Attribute attribute;
            if (!(value is Attribute))
            {
                attribute = new Attribute { Value = value };
            }
            else
            {
                attribute = value;
            }


            if (or.ContainsKey(key))
            {
                or[key] = attribute.Value;
            }
            else
            {
                or.Add(key, attribute);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public AttributeDictionary GetAlgorithmFields()
        ///
        /// \brief Gets algorithm fields.
        ///
        /// \par Description. 
        ///        Gets a dictionary of the algorithm fields.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \return The algorithm fields.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeDictionary GetAlgorithmFields()
        {
            return or;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute GetField(dynamic key)
        ///
        /// \brief Gets a field.
        ///
        /// \par Description. 
        ///        Get an algorithm specific field from the message
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param key (dynamic) - The key.
        ///
        /// \return The field.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public dynamic GetField(dynamic key)
        {
            try
            {
                return or[key];
            }
            catch
            {
                return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetField(dynamic key, dynamic value)
        ///
        /// \brief Sets a field.
        ///
        /// \par Description.
        ///      Set a value to an algorithm specific field in the message
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param key    (dynamic) - The key.
        /// \param value  (dynamic) - The value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetField(dynamic key, dynamic value)
        {
            try
            {
                or[key] = value;
            }
            catch
            {

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute GetHeaderField(dynamic key)
        ///
        /// \brief Gets header field.
        ///
        /// \par Description. 
        ///        Get header field.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param key (dynamic) - The key.
        ///
        /// \return The header field.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public dynamic GetHeaderField(dynamic key)
        {
            try
            {
                return pa[key];
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region /// \name Utilities

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool IsEmpty()
        ///
        /// \brief Query if this message is empty.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \return True if empty, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsEmpty()
        {
            if (pa.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }        
       }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string ToString()
        ///
        /// \brief Convert this object into a string representation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \return A string that represents this object.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            string result;
            if (!IsEmpty())
            {
                result = TypesUtility.GetKeyToString(GetHeaderField(bm.pak.MessageType));
                result += " From:" + GetHeaderField(bm.pak.SourceProcess).ToString();
                result += " To:" + GetHeaderField(bm.pak.DestProcess).ToString();
            }
            else
            {
                result = "Empty Message";
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string Description()
        ///
        /// \brief Gets the description.
        ///
        /// \par Description.
        ///      The description is composed from :
        ///      -# The message type 
        ///      -# The round
        ///      -# All the fields that has IncludedInShortDescription = true in the or dictionary
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string Description()
        {
            string result = "";
            if (!IsEmpty())
            {
                result += "[" + GetField(bm.ork.PositionInProcessQ).ToString() + "]";
                result += " Round=" + GetHeaderField(bm.pak.Round).ToString();
                result += ";" + pa[bm.pak.SourceProcess].ToString() + "->" + pa[bm.pak.DestProcess].ToString();
                result += ";" + TypesUtility.GetKeyToString(GetHeaderField(bm.pak.MessageType));
                result += " : " + or.ShortDescription();
            }
            return result;
        }
        #endregion
    }
}
