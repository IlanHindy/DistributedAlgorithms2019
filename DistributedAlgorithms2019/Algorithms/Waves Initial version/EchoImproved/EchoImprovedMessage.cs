////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\EchoImproved\EchoImprovedMessage.cs
///
/// \brief Implements the EchoImproved message class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.System.Base;

namespace DistributedAlgorithms.Algorithms.Waves.EchoImproved
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EchoImprovedMessage
    ///
    /// \brief A EchoImproved message.
    ///
    /// \brief #### Usage Notes.
    /// There is not actual message type for an algorithm
    /// In order to create a message use the DistributedAlgorithms.Algorithms.System.Base.BaseMessage class. 
    /// The instance of a message is done in the following way:
    /// 1. Decide what are the types of the messages the algorithm needs
    /// 2. Fill the enum MessageTypes with the messages types 
    /// 3. Decide what are the fields needed for all the messages 
    /// 4. Fill the FieldKeys enum with the fields 
    /// 5. In order to create a message use one of the DistributedAlgorithms.Algorithms.System.Base.BaseMessage constructors 
    /// 6. Add fields to the message with the method DistributedAlgorithms.Algorithms.System.Base.BaseMessage.AddField()
    ///
    /// \author Ilan Hindy
    /// \date 24/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class EchoImprovedMessage : BaseMessage
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum MessageTypes
        ///
        /// \brief Values that represent message types.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new enum MessageTypes { Wave }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum FieldKeys
        ///
        /// \brief Values that represent field types.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new enum FieldKeys { }
        #region Constructors
        public EchoImprovedMessage() { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(BaseMessage sourceMessage, BaseChannel sendingChannel):base(sourceMessage)
        ///
        /// \brief Constructor.
        ///
        /// \brief #### Description.
        ///        A message that is duplicatted to the source except for the channel parameters
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sourceMessage   (BaseMessage) - Message describing the source.
        /// \param sendingChannel  (BaseChannel) - The sending channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(BaseMessage sourceMessage, BaseChannel sendingChannel):
            base(sourceMessage, sendingChannel)
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(dynamic messageType, AttributeDictionary fields, BaseChannel channel, int round, int logicalClock)
        ///
        /// \brief Constructor.
        ///
        /// \brief #### Description.
        ///        Create a message from :
        ///        -# The messageType
        ///        -# The sending parameters from the channel
        ///        -# The parameters for the header round, logical clock
        ///        -# Fields for the algorithm specific fields in attribute dictionary
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param messageType   (dynamic) - Type of the message.
        /// \param fields        (AttributeDictionary) - The fields.
        /// \param channel       (BaseChannel) - The channel.
        /// \param round         (int) - The round.
        /// \param logicalClock  (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(object messageType, 
            AttributeDictionary fields, 
            BaseChannel channel, 
            int round, int logicalClock):base(messageType, fields, channel, round, logicalClock)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(string messageString)
        ///
        /// \brief Constructor.
        ///
        /// \brief #### Description.
        ///        Create a message from string which holds the message xml
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param messageString  (string) - The message string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(string messageString):base(messageString)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(dynamic messageType, int sourceProcess, int sourcePort, int destProcess, int destPort, int round, int logicalClock)
        ///
        /// \brief Constructor.
        ///
        /// \brief #### Description.
        ///        Create a message when all the header are givven
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param messageType    (dynamic) - Type of the message.
        /// \param sourceProcess  (int) - Source process.
        /// \param sourcePort     (int) - Source port.
        /// \param destProcess    (int) - Destination process.
        /// \param destPort       (int) - Destination port.
        /// \param round          (int) - The round.
        /// \param logicalClock   (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(object messageType,
            int sourceProcess,
            int sourcePort,
            int destProcess,
            int destPort,
            int round,
            int logicalClock) : base(messageType,
            sourceProcess,
            sourcePort,
            destProcess,
            destPort,
            round,
            logicalClock)
        {

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage(dynamic messageType, BaseChannel channel, int round = 0, int logicalClock = 0)
        ///
        /// \brief Constructor.
        ///
        /// \brief #### Description.
        ///        Construct a message from header parameters
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param messageType   (dynamic) - Type of the message.
        /// \param channel       (BaseChannel) - The channel.
        /// \param round        (Optional)  (int) - The round.
        /// \param logicalClock (Optional)  (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoImprovedMessage(object messageType, BaseChannel channel, int round = 0, int logicalClock = 0):
            base(messageType, channel, round, logicalClock)
        {

        }
#endregion
    }
}
