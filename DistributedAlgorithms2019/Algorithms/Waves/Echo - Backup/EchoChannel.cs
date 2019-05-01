////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\Echo\EchoChannel.cs
///
/// \brief Implements the template channel class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.Drawing;

namespace DistributedAlgorithms.Algorithms.Waves.Echo
{


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EchoChannel
    ///
    /// \brief A template channel.
    ///
    /// \par Description.
    ///      when working there are the following phases
    ///      -#  Init    - create a new network
    ///      -#  Design  - Design the network and fill it's parameters
    ///      -#  Check   - Check if the network design is legal according to the algorithm specifications
    ///      -#  Create  - Create the network from the software point of view
    ///      -#  Run     - Run the algorithm
    ///      -#  Presentation - update the presentation while running 
    ///
    /// \par Usage Notes.
    ///      -#  The channel represents a directed channel between 2 processors
    ///      -#  The channel can be accessed from the source process's OutChannels() list
    ///      -#  The channel can be accessed from the destination processor's InChannels() list
    ///      -#  The channel's or dictionary can hold data specific to the channel (which the processes put in)
    ///      -#  After each running phase the channel is drawn on the screen. In order to change
    ///          the presentation change the attributes of the pp dictionary using the bc.ppk keys
    ///      -#  Note that removing attributes from the pp dictionary is dangerous
    ///     because the presentation of the channel Use them.  
    ///
    /// \author Ilan Hindy
    /// \date 24/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class EchoChannel : BaseChannel
    {

        #region /// \name constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoChannel() : base()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      It is not recommended to change the constructor. All the attributes has to be inserted
        ///      to the pa or dictionaries in the Init... methods 
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoChannel() : base() { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoChannel(EchoNetwork network) : base(network)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      It is not recommended to change the constructor. All the attributes has to be inserted
        ///      to the pa or dictionaries in the Init... methods         ///      
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param network  (EchoNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoChannel(EchoNetwork network) : base(network) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoChannel(bool PermissionsValue) : base(PermissionsValue)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      It is not recommended to change the constructor. All the attributes has to be inserted
        ///      to the pa or dictionaries in the Init... methods         ///      
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param PermissionsValue  (bool) - true to permissions value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoChannel(bool PermissionsValue) : base(PermissionsValue) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoChannel(EchoNetwork network, int id, int sourceProcessId, int destProcessId) : base(network, id, sourceProcessId, destProcessId)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param network          (EchoNetwork) - The network.
        /// \param id               (int) - The identifier.
        /// \param sourceProcessId  (int) - Identifier for the source process.
        /// \param destProcessId    (int) - Identifier for the destination process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoChannel(EchoNetwork network, int id, int sourceProcessId, int destProcessId) 
            : base(network, id, sourceProcessId, destProcessId)
        { }

        #endregion
        #region /// \name Init (methods that are activated while in Init phase)
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void AdditionalInitiations()
        ///
        /// \brief Additional initiations.
        ///
        /// \par Description.
        ///      This method is activated at the end of the init phase to handle all the actions needed
        ///      that are not addition of attributes
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void AdditionalInitiations()
        {
            base.AdditionalInitiations();
        }

        #endregion
        #region /// \name Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool EchoChannelDefaultAttributeCheck(EchoNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        ///
        /// \brief A prototype for attribute checking.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      -#  Each Attribute can be assigned a value check that will be performed when changing the
        ///          attribute from the channel edit window while in design mode
        ///      -#  For each attribute that you want to add a check to one method with the prototype of the
        ///          following method have to be designed
        ///      -#  In order to add the check to the attribute you have to set the EndInputOperationMethod
        ///          member of the Attribute with the method you generated for example :      
        /// ~~~{.cs}
        ///       // Setting the method to an existing attribute
        ///       attribute.EndInputOperationMethod =  EchoChannelDefaultAttributeCheck
        ///       
        ///       // Setting the method to an attribute while creation
        ///       pa.Add(c.pak.SomeKey, new Attribute {Value = SomeValue, EndInputOperationMethod =  EchoChannelDefaultAttributeCheck})
        /// ~~~ 
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param       network          (EchoNetwork) - The network.
        /// \param       networkElement   (NetworkElement) - The network element.
        /// \param       parentAttribute  (Attribute) - The parent attribute.
        /// \param       attribute        (Attribute) - The attribute.
        /// \param       newValue         (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        ///
        /// \return True if it's OK to change the attribute's value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool EchoChannelDefaultAttributeCheck(EchoNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        #endregion
        #region /// \name Check (methods that are activated while in check phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check build.
        ///
        /// \par Description.
        ///      This method performs checks on the channel at the check network
        ///      phase (before running and after designing)
        ///
        /// \par checking network process.
        ///      The check and correct of a network element is composed from :
        ///      -#  A CheckBuild method that does the checks and for each error it finds it fills a
        ///          BuildCorrectionParameters data structure and insert it to a list given as parameter to the
        ///          method
        ///      -#  The BuildCorrectionParameters data structure is composed from :
        ///         -#  A message that will be presented
        ///         -#  A method that corrects the error (a method with the same prototype as the CorrectionMethod of this class)
        ///         -#  An object for passing the parameters from the check method to the correction method.
        ///
        /// \par Algorithm.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \par Usage Notes.
        ///      -  In order to access element of the network, Start with the private `network` member of
        ///         this class.
        ///      -  The processes of the network are in `network.Processes`  
        ///      -  The channels of the network are in `network.Channels`
        ///
        /// \param buildCorrectionsParameters  (List&lt;BuildCorrectionParameters&gt;) - Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            base.CheckBuild(buildCorrectionsParameters);

            // Insert your checks here
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CorrectionMethod(object correctionParameters)
        ///
        /// \brief Correct an error detected during check build.
        ///
        /// \par Description.
        ///      - This method is an example for the methods that needs to be created for correcting build
        ///        errors.
        ///      - For each error there has to be a method from this prototype that corrects the error.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      -#  Create a correction method for each error with a method with this prototype
        ///      -#  In the CheckBuild when encountering an error set the correction method name in the
        ///          BuildCorrectionParameters data structures
        ///      -#  The methods gets its parameters from the object parameter. The structure of the
        ///          parameters has to be the same in the CheckBuild method and the correction method
        ///      -#  In order to access element of the network, Start with the private `network` member of
        ///         this class.
        ///      -#  The processes of the network are in `network.Processes`  
        ///      -#  The channels of the network are in `network.Channels`
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param correctionParameters  (object) - Options for controlling the correction.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CorrectionMethod(object correctionParameters)
        {

        }

        #endregion
        #region /// \name Presentation (Methods that are activated while in running phase)

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
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \return A string that represents this object.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            return base.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string PresentationText()
        ///
        /// \brief Generates the text that will be presented on the screen.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string PresentationText()
        {
            return base.PresentationText();
        }

        #endregion
        #region /// \name algorithm utility methods
        #endregion
    }
}
