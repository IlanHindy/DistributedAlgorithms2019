////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\EchoImproved\EchoImprovedMessage.cs
///
/// \brief Implements the template message class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms.Algorithms.Waves.EchoImproved
{



    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EchoImprovedMessage
    ///
    /// \brief A EchoImproved message.
    ///
    /// \par Usage Notes. 
    ///      The use of this class is in the following way :
    ///      -#   Decide what are the types of the messages the algorithm needs
    ///      -#   See if you can use the already defined message types in bm.MessageTypes
    ///      -#   Fill the enum m.MessageTypes with the messages types 
    ///      -#   Decide what are the fields needed for all the messages 
    ///      -#   Fill the m.FieldKeys enum with the fields 
    ///      -#   In order to create a message use one of the constructors
    ///      -#   Note that some of the constructors are for the system to be used and you should
    ///           not use them (because they create a message that does not have the obligatory fields) 
    ///      -#   Add fields to the message with the method AddField()
    ///      -#   Get header fields of the method (fields common to all the messages) using the GetHeaderField()
    ///           (The header fields enum is bm.HeaderFieldKeys)
    ///      -#   Get private fields of the message using the method GetField() 
    ///           (The private fields enum is m.FieldKeys)
    ///
    /// \par Usage Notes.
    ///      It is possible to declare other messages classes inheritted from the algorithm's main message type
    ///      (EchoImprovedMessage)
    ///      The type of the message that you handle in the receive has to be the type of the message
    ///      that was sent (even though the parameter to the process's ReceiveHandeling method gets
    ///      a EchoImprovedMessage as a parameter)
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

    public class EchoImprovedMessage : BaseMessage
    {
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoImprovedMessage() : base()
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

        public EchoImprovedMessage() : base() { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoImprovedMessage(EchoImprovedNetwork network):base(network)
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
        /// \param network  (EchoImprovedNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(EchoImprovedNetwork network) : base(network)
        { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoImprovedMessage(EchoImprovedNetwork network, EchoImprovedMessage sourceMessage): base(network, sourceMessage)
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
        /// \param network       (EchoImprovedNetwork) - The network.
        /// \param sourceMessage (EchoImprovedMessage) - Message describing the source.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(EchoImprovedNetwork network, EchoImprovedMessage sourceMessage) : base(network, sourceMessage)
        {
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoImprovedMessage(EchoImprovedMessage sourceMessage, EchoImprovedChannel sendingChannel): base(sourceMessage, sendingChannel)
        ///
        /// \brief Constructor.
        ///
        /// \par Description. 
        ///      A message that is duplicatted to the source except for the channel parameters.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      This constructor is usfull when you want to forwared a massege to other channels
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sourceMessage  (EchoImprovedMessage) - Message describing the source.
        /// \param sendingChannel (EchoImprovedChannel) - The sending channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(EchoImprovedNetwork network, EchoImprovedMessage sourceMessage, EchoImprovedChannel sendingChannel) :
            base(network, sourceMessage, sendingChannel)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoImprovedMessage(EchoImprovedNetwork network, object messageType, AttributeDictionary fields, EchoImprovedChannel channel, string messageName, int round, int logicalClock):base(network, messageType, fields, channel, messageName, round, logicalClock)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Create a message from :
        ///      -#   The messageType
        ///      -#   Fields for the algorithm specific fields in attribute dictionary.
        ///      -#   The sending parameters from the channel
        ///      -#   The message name
        ///      -#   The parameters for the header round, logical clock
        ///      
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param network       (EchoImprovedNetwork) - The network.
        /// \param messageType  (dynamic) - Type of the message.
        /// \param fields       (AttributeDictionary) - The fields.
        /// \param channel      (EchoImprovedChannel) - The channel.
        /// \param messageName   (string) - Name of the message.
        /// \param round        (int) - The round.
        /// \param logicalClock (int) - The logical clock.
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(BaseNetwork network,
            object messageType,
            AttributeDictionary fields,
            BaseChannel channel,
            string messageName,
            int round, int logicalClock) : base(network, messageType, fields, channel, messageName, round, logicalClock)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoImprovedMessage(EchoImprovedNetwork network, string messageString):base(network, messageString)
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
        /// \param network        (EchoImprovedNetwork) - The network.
        /// \param messageString (string) - The message string.
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(EchoImprovedNetwork network, string messageString) : base(network, messageString)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoImprovedMessage(EchoImprovedNetwork network, object messageType, int sourceProcess, int sourcePort, int destProcess, int destPort, string messageName, int round, int logicalClock) : base(network, messageType, sourceProcess, sourcePort, destProcess, destPort, messageName, round, logicalClock)
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
        /// \param network        (EchoImprovedNetwork) - The network.
        /// \param messageType   (dynamic) - Type of the message.
        /// \param sourceProcess (int) - Source process.
        /// \param sourcePort    (int) - Source port.
        /// \param destProcess   (int) - Destination process.
        /// \param destPort      (int) - Destination port.
        /// \param messageName    (string) - Name of the message.
        /// \param round         (int) - The round.
        /// \param logicalClock  (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(EchoImprovedNetwork network,
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
        /// \fn public EchoImprovedMessage(EchoImprovedNetwork network, object messageType, EchoImprovedChannel channel, string messageName, int round = 0, int logicalClock = 0): base(network, messageType, channel, messageName, round, logicalClock)
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
        /// \param network       (EchoImprovedNetwork) - The network.
        /// \param messageType  (dynamic) - Type of the message.
        /// \param channel      (EchoImprovedChannel) - The channel.
        /// \param messageName   (string) - Name of the message.
        /// \param round        (Optional)  (int) - The round.
        /// \param logicalClock (Optional)  (int) - The logical clock.
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(EchoImprovedNetwork network, object messageType, EchoImprovedChannel channel, string messageName, int round = 0, int logicalClock = 0) :
            base(network, messageType, channel, messageName, round, logicalClock)
        {

        }
        #endregion
    }
}
