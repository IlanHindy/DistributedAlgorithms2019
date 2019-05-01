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
using DistributedAlgorithms.Algorithms.System.Base;

namespace DistributedAlgorithms.Algorithms.Waves.EchoImproved
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EchoImprovedMessage
    ///
    /// \brief A template message.
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
    }
}
