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
using DistributedAlgorithms.Algorithms.System.Base;
using System.Drawing;

namespace DistributedAlgorithms.Algorithms.Waves.Echo
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EchoChannel
    ///
    /// \brief A template channel.
    ///
    /// \brief #### Description.
    /// when working there are the following phases
    /// -#  Init    - create a new networ
    /// -#  Design  - Designe the network and fill it's parameters
    /// -#  Check   - Check if the network design is leagal according to the algorithm specifications
    /// -#  Create  - Create the network fro the software point of view
    /// -#  Run     - Run the algorithm
    /// -#  Presentation - update the presentation while running 
    ///
    /// \brief
    /// #### Usage Notes.
    /// -#  The channel represents a directed channel between 2 processors
    /// -#  The channel can be accessed from the source processir's OutGoingChannels list
    /// -#  The channel can be accessed from the destination processor's InCommingChannels
    /// list
    /// -#  The channel's dictionaries can hold data specific to the channel (which the
    ///     processes put
    /// in)
    /// -#  After each running phase the channel is drawn on the screen in order to change
    ///     the presentation change the attributes of the PresentationParameters using the
    ///     PresentationParametersKeys
    /// -#  Note that there is no effect to add attributes from the PresentationParameter
    /// dictionary
    /// -#  Note that removing attributes from the PresentationParameters is dangerouse
    ///     because thepresentation Use them.  
    ///
    /// \author Ilan Hindy
    /// \date 24/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class EchoChannel : BaseChannel
    {
        #region Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum PrivateAttributeKeys
        ///
        /// \brief Values that represent private attribute keys. Private attributes are attributes of
        /// the channel that do not change during operation Insert.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum PrivateAttributeKeys { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum OperationResultKeys
        ///
        /// \brief Values that represent operation result keys. Operation results are all the variables
        /// used by the program to implement the algorithm
        /// 
        /// \remark Even though it is possible to use regular variables the regular variables cannot be
        /// reconstructed when saving and loading debug statuses and cannot be changed by the user during
        /// running of the algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new enum OperationResultKeys { Status }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Status
        ///
        /// \brief Values that represent status.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum Status { Father, Null, SonSent }
        #endregion
        #region constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoChannel()
        ///
        /// \brief Default constructor.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoChannel()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoChannel(int id, int sourceProcessId, int destProcessId, BaseNetwork network) : base(id, sourceProcessId, destProcessId, network)
        ///
        /// \brief Constructor.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param id              The identifier.
        /// \param sourceProcessId Identifier for the source process.
        /// \param destProcessId   Identifier for the destination process.
        /// \param network         The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoChannel(int id, int sourceProcessId, int destProcessId, BaseNetwork network) :
            base(id, sourceProcessId, destProcessId, network)
        { }

        #endregion
        #region Init (methods that are activated while in Init phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void Init(int idx, bool clearPresentation = true)
        ///
        /// \brief This method is activated when the init button is pressed to init the channel
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// - Do not deleate the call to bese.Init
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Init(int idx, bool clearPresentation = true)
        {
            base.Init(idx, clearPresentation);

            // Insert your modifications here
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitPrivateAttributes.Value()
        ///
        /// \brief Init private attributes.
        ///
        /// \brief #### Description.
        /// -#  This method is activated when a new channel is initiated This method is used in order to
        /// add new private attributes to the channel
        /// -#  Private attributes are attributes describing the channel that do not change during
        /// execution.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Insert all the new attributes after the statement : `base.InitPrivateAttributes.Value;`
        /// -#  The following is the format of adding to a dictionary `PrivateAttributes.Value.Add(key, new
        /// Attribute() { Value = ...})`.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \brief #### Usage Notes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitPrivateAttributes.Value()
        {
            base.InitPrivateAttributes.Value();

            // Add initialization of the private attributes here
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitOperationResults.Value()
        ///
        /// \brief Init operation results.
        ///
        /// \brief #### Description.
        /// -#  This method is activated when a new channel is initiated This method is used in order to
        /// add new operation results to the channel
        /// -#  Operation results are attributes tha holds the execution parameters.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Insert all the new attributes after the statement : `base.InitOperationResults.Value();`
        /// -#  The following is the format of adding to a dictionary `OperationResults.Value.Add(key, new
        /// Attribute() { Value = ...})`.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \brief #### Usage Notes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitOperationResults.Value()
        {
            base.InitOperationResults.Value();

            // Add initialization of OperationResults.Value here
            OperationResults.Value.Add(OperationResultKeys.Status, new Attribute { Value = Status.Null });
        }

        #endregion
        #region Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool EchoChannelDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        ///
        /// \brief A prototypr for attribute checking.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Each Attribute can be assigned a value check that will be performed when changing the
        /// attribute from the channel edit window while in design mode
        /// -#  for each attribute that you want to add a check to one method with the prototype of the
        /// following method have to be designed
        /// -#  In order to add the check to the attribute you have to set the EndInputOperationMethod
        /// variable of the Attribute with the method you generated for example :
        /// -# `attribute.EndInputOperationMethod =  EchoChannelDefaultAttributeCheck`
        /// -# `PrivateAttributes.Value.Add(PrivateAttributeKeys.SomeKey, new Attribute {Value = SomeValue,
        ///  EndInputOperationMethod =  EchoChannelDefaultAttributeCheck})`.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param network         The network.
        /// \param       networkElement  The network element.   (in this case the process that is been editted)
        /// \param       parentAttribute The parent attribute.  (The parent attribute in the editting window tree)
        /// \param       attribute       The attribute.         (The attribute that is been changed
        /// \param       newValue        The new value.         (The new value
        /// \param [out] errorMessage    Message describing the error.  (A string for a message that will be presented if an error was detected by this method)
        ///
        /// \return True if it's OK to change the attribute's value
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool EchoChannelDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        #endregion
        #region Check (methods that are activated while in check phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check build.
        ///
        /// \brief #### Description. This method performs checks on the channel at the check network
        /// phase (before running and after designing)
        ///
        /// \brief #### checking network process. The check and correct of a network element is composed
        /// from :
        /// -#  A CheckBuild method that does the checks and for each error it finds it fills a
        /// BuildCorrectionParameters data structure and insert it to a list given as parameter to the
        /// method
        /// -#  The BuildCorrectionParameters data structure is composed from :
        /// -#  A message that will be presented
        /// -#  A method that correctes the error
        /// -#  An object for passing the parameters from the check method to the correction method.
        ///
        /// \brief #### Algorithm.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \brief #### Usage Notes.
        /// -#  In order to access tne element of the network start with the private `network` member of
        /// this class.
        ///
        /// \param buildCorrectionsParameters Options for controlling the build corrections.
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
        /// \brief #### Description.
        /// - This method is an example for the methods that needs to be created for correcting build
        /// errors.
        /// - For each error that has to be handled a method from this prototype has to be created.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Create a correction method for the error with this prototype
        /// -#  In the CheckBuild when encountering an error set the correction method name in the
        /// BuildCorrectionParameters data structurs
        /// -#  The methods gets its parameters from the object parameter. The structure of the
        /// parameters has to be the same in the CheckBuild method and the correction method
        /// -#  In order to access tne element of the network start with the private `network` member of
        /// this class.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param correctionParameters Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CorrectionMethod(object correctionParameters)
        {

        }

        #endregion
        #region Presentation (Methods that are activated while in running phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string ToString()
        ///
        /// \brief Convert this object into a string representation.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
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
        /// \brief Genarates the text that will be presented on the screen.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
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
        #region algorithm utility methods
        #endregion
    }
}
