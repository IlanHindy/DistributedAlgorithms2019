////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\ChandyLamport_OneRound\ChandyLamport_OneRoundMessage.cs
///
/// \brief Implements the template message class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_OneRound
{



    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ChandyLamport_OneRoundMessage
    ///
    /// \brief A ChandyLamport_OneRound message.
    ///
    /// \par Usage Notes. 
    ///      The use of this class is in the following way :
    ///      -#   Decide what are the types of the messages the algorithm needs
    ///      -#   See if you can use the already defined message types in bm.MessageTypes
    ///      -#   Fill the enum m.MessageTypes with the messages types 
    ///      -#   Decide what are the fields needed for all the messages 
    ///      -#   Fill the m.ork enum with the fields 
    ///      -#   In order to create a message use one of the constructors
    ///      -#   Note that some of the constructors are for the system to be used and you should
    ///           not use them (because they create a message that does not have the obligatory fields) 
    ///      -#   Add fields to the message with the method AddField()
    ///      -#   Get header fields of the method (fields common to all the messages) using the GetHeaderField()
    ///           (The header fields enum is bm.pak)
    ///      -#   Get private fields of the message using the method GetField() 
    ///           (The private fields enum is m.ork)
    ///
    /// \par Usage Notes.
    ///      It is possible to declare other messages classes inherited from the algorithm's main message type
    ///      (ChandyLamport_OneRoundMessage)
    ///      The type of the message that you handle in the receive has to be the type of the message
    ///      that was sent (even though the parameter to the process's ReceiveHandeling method gets
    ///      a ChandyLamport_OneRoundMessage as a parameter)
    ///      
    /// \par Utility Messages      
    /// ~~~{.cs}
    ///     // Add field to the message  
    ///     public void AddField(dynamic key, Attribute attribute)
    ///
    ///     // Get the value of a header field
    ///     public dynamic GetHeaderField(dynamic key)
    ///
    ///     // Get the value of an algorithm specific field
    ///     public dynamic GetField(dynamic key)
    ///
    ///     // Set the value of an algorithm specific field
    ///     public void SetField(dynamic key, dynamic value)
    ///
    ///     // Get the message type
    ///     public string GetMessageType()
    /// ~~~
    ///
    /// \author Ilan Hindy
    /// \date 24/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ChandyLamport_OneRoundMessage : BaseMessage
    {
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_OneRoundMessage() : base()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      Constructor used by the system. You should not use it
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_OneRoundMessage() : base() { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network):base(network)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      Constructor used by the system. You should not use it
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param network  (ChandyLamport_OneRoundNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network):base(network)
        { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network, ChandyLamport_OneRoundMessage sourceMessage): base(network, sourceMessage)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Constructor that generates a duplication of a source message.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param network       (ChandyLamport_OneRoundNetwork) - The network.
        /// \param sourceMessage (ChandyLamport_OneRoundMessage) - Message describing the source.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network, ChandyLamport_OneRoundMessage sourceMessage): base(network, sourceMessage)
        {
        }

        

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundMessage sourceMessage, ChandyLamport_OneRoundChannel sendingChannel): base(sourceMessage, sendingChannel)
        ///
        /// \brief Constructor.
        ///
        /// \par Description. 
        ///      A message that is duplicated to the source except for the channel parameters.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      This constructor is useful when you want to forward a message to other channels
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sourceMessage  (ChandyLamport_OneRoundMessage) - Message describing the source.
        /// \param sendingChannel (ChandyLamport_OneRoundChannel) - The sending channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network, ChandyLamport_OneRoundMessage sourceMessage, ChandyLamport_OneRoundChannel sendingChannel):
            base(network, sourceMessage, sendingChannel)
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_OneRoundMessage(BaseNetwork network, object messageType, AttributeDictionary fields, BaseChannel channel, string messageName, int round, int logicalClock):base(network, messageType, fields, channel, messageName, round, logicalClock)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Create a message from :
        ///      -#   The messageType
        ///      -#   Fields for the algorithm specific fields in attribute dictionary.
        ///      -#   The sending parameters from the channel
        ///      -#   The message name
        ///      -#   The parameters for the header round, logical clock.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network      (BaseNetwork) - The network.
        /// \param messageType  (dynamic) - Type of the message.
        /// \param fields       (AttributeDictionary) - The fields.
        /// \param channel      (BaseChannel) - The channel.
        /// \param messageName  (string) - Name of the message.
        /// \param round        (int) - The round.
        /// \param logicalClock (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_OneRoundMessage(BaseNetwork network,
            object messageType, 
            AttributeDictionary fields, 
            BaseChannel channel,
            string messageName, 
            int round, int logicalClock):base(network, messageType, fields, channel, messageName, round, logicalClock)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network, string messageString):base(network, messageString)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Create a message from string which holds the message XML.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network        (ChandyLamport_OneRoundNetwork) - The network.
        /// \param messageString (string) - The message string.
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network, string messageString):base(network, messageString)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network, object messageType, int sourceProcess, int sourcePort, int destProcess, int destPort, string messageName, int round, int logicalClock) : base(network, messageType, sourceProcess, sourcePort, destProcess, destPort, messageName, round, logicalClock)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Create a message when all the header are given.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network        (ChandyLamport_OneRoundNetwork) - The network.
        /// \param messageType   (dynamic) - Type of the message.
        /// \param sourceProcess (int) - Source process.
        /// \param sourcePort    (int) - Source port.
        /// \param destProcess   (int) - Destination process.
        /// \param destPort      (int) - Destination port.
        /// \param messageName    (string) - Name of the message.
        /// \param round         (int) - The round.
        /// \param logicalClock  (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network,
            object messageType,
            int sourceProcess,
            int sourcePort,
            int destProcess,
            int destPort,
            string messageName,
            int round,
            int logicalClock) : base(network,
                messageType,
            sourceProcess,
            sourcePort,
            destProcess,
            destPort,
            messageName,
            round,
            logicalClock)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network, object messageType, ChandyLamport_OneRoundChannel channel, string messageName, int round = 0, int logicalClock = 0): base(network, messageType, channel, messageName, round, logicalClock)
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
        /// \param network       (ChandyLamport_OneRoundNetwork) - The network.
        /// \param messageType  (dynamic) - Type of the message.
        /// \param channel      (ChandyLamport_OneRoundChannel) - The channel.
        /// \param messageName   (string) - Name of the message.
        /// \param round        (Optional)  (int) - The round.
        /// \param logicalClock (Optional)  (int) - The logical clock.
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_OneRoundMessage(ChandyLamport_OneRoundNetwork network, object messageType, ChandyLamport_OneRoundChannel channel, string messageName, int round = 0, int logicalClock = 0):
            base(network, messageType, channel, messageName, round, logicalClock)
        {

        }
        #endregion
    }
}
