////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\LayYoung\LayYoungNetwork.cs
///
/// \brief Implements the template network class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.System.Base;

namespace DistributedAlgorithms.Algorithms.Snapshots.LayYoung
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class LayYoungNetwork
    ///
    /// \brief A template network.
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
    /// \brief #### Usage Notes.
    ///
    /// \author Ilan Hindy
    /// \date 26/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class LayYoungNetwork : BaseNetwork
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum PrivateAttributeKeys
        ///
        /// \brief Values that represent private attribute keys. Private attributes are attributes of
        /// the network that do not change during operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum PrivateAttributeKeys { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum OperationResultKeys
        ///
        /// \brief Values that represent operation result keys.Operation results are all the variables
        /// used by the program to implement the algorithm
        /// 
        /// \remark Even though it is possible to use regular variables the regular variables cannot be
        /// reconstructed when saving and loading debug statuses and cannot be changed by the user during
        /// running of the algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new enum OperationResultKeys { }

        #region constructors
        #endregion
        #region Init (methods that are activated while in Init phase)
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void Init(int idx, bool clearPresentation = true)
        ///
        /// \brief This method is activated when the init button is pressed to init the network
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -   Do not deleate the call to bese.Init
        /// -   Set the network configuration parameters so it can be applsyed when the network
        ///     is been builded
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
            // Set network configuration
            ElementAttributes[BaseNetwork.ElementAttributeKeys.DirectedNetwork].Value = true;
            ElementAttributes[BaseNetwork.ElementAttributeKeys.Centrilized].Value = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CreateInitNetwork()
        ///
        /// \brief Creates init network.
        ///
        /// \brief #### Description.
        ///        This method creates the initial network use this method if you want that the init 
        ///        operation (from the gui) will create a network with some processes and channels
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///        The following is an example for how to create an init network (It is the method
        ///        of the BaseNetwork)
        /// ~~~{.cs}
        /// protected virtual void CreateInitNetwork()
        /// {
        ///     //Init the processes
        ///     int numberOfProcesses = 2;
        ///     int channelIdx = 0;
        ///     for (int idx = 0; idx<numberOfProcesses; idx++)
        ///     {
        ///         //Create and init a process
        ///         BaseProcess process = NetworkElementClassFactory.GetProcess(this);
        ///         Processes.Add(process);
        ///         process.Init(idx);
        ///
        ///         //Create a Channel from the process to itself - used to terminate the sockets
        ///         //Used by the process
        ///         BaseChannel channel = NetworkElementClassFactory.GetChannel(channelIdx, idx, idx, this);
        ///         Channels.Add(channel);
        ///         channelIdx++;
        ///      }
        ///
        ///      //Init the other channels
        ///      int numberOfChannels = 4;
        ///      for (; channelIdx<numberOfChannels; channelIdx++)
        ///      {
        ///         BaseChannel channel = NetworkElementClassFactory.GetChannel(channelIdx, channelIdx % 2, (channelIdx + 1) % 2, this);
        ///         Channels.Add(channel);
        ///      }
        /// }
        ///~~~
        /// 
        ///
        /// \author Ilan Hindy
        /// \date 26/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CreateInitNetwork()
        {
            // Call the base class method - remove if you want to build another init network
            base.CreateInitNetwork();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitPrivateAttributes()
        ///
        /// \brief Init private attributes.
        ///
        /// \brief #### Description.
        /// -#  This method is activated when a new network is initiated This method is used in order to
        /// add new private attributes to the network
        /// -#  Private attributes are attributes describing the network that do not change during
        /// execution.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Insert all the new attributes after the statement : `base.InitPrivateAttributes;`
        /// -#  The following is the format of adding to a dictionary `PrivateAttributes.Add(key, new
        /// Attribute() { Value = ...})`.
        /// -#  This is the place to set the network configuration parameters :
        ///     -#  Centrilized
        ///     -#  Directed
        ///
        /// \author Ilan Hindy
        /// \date 26/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitPrivateAttributes()
        {
            base.InitPrivateAttributes();

            // Add initialization of the private attributes here
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitOperationResults()
        ///
        /// \brief Init operation results.
        ///
        /// \brief #### Description.
        /// -#  This method is activated when a new network is initiated This method is used in order to
        /// add new operation results to the network
        /// -#  Operation results are attributes tha holds the execution parameters.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Insert all the new attributes after the statement : `base.InitOperationResults();`
        /// -#  The following is the format of adding to a dictionary `OperationResults.Add(key, new
        /// Attribute() { Value = ...})`.
        ///
        /// \author Ilan Hindy
        /// \date 26/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitOperationResults()
        {
            base.InitOperationResults();

            // Add initialization of OperationResults here
        }

        #endregion
        #region Design (methods that are activated while in design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool LayYoungNetworkDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        ///
        /// \brief A prototypr for attribute checking 
        ///        
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Each Attribute can be assigned a value check that will be performed when changing the
        /// attribute from the network edit window while in design mode
        /// -#  for each attribute that you want to add a check to one method with the prototype of the
        /// following method have to be designed
        /// -#  In order to add the check to the attribute you have to set the EndInputOperationMethod
        /// variable of the Attribute with the method you generated for example :
        ///     -# `attribute.EndInputOperationMethod =  LayYoungnetworkDefaultAttributeCheck`
        ///     -# `PrivateAttributes.Add(PrivateAttributeKeys.SomeKey, new Attribute {Value = SomeValue,
        ///         EndInputOperationMethod =  LayYoungnetworkDefaultAttributeCheck})`
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
        public static bool LayYoungNetworkDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        /// \brief #### Description.
        /// This method performs checks on the network at the check network phase 
        /// (before running and after designing)
        ///
        /// \brief #### checking network process.
        /// The check and correct of a network element is composed from :
        /// -#  A CheckBuild method that does the checks and for each error it finds it fills a
        ///     BuildCorrectionParameters data structure and insert it to a list given as 
        ///     parameter to the method
        /// -#  The BuildCorrectionParameters data structure is composed from :
        ///     -#  A message that will be presented
        ///     -#  A method that correctes the error
        ///     -#  An object for passing the parameters from the check method to the correction method
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  In order to access tne element of the network start with the private `network` member of this class
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param buildCorrectionsParameters Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            base.CheckBuild(buildCorrectionsParameters);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CorrectionMethod(object correctionParameters)
        ///
        /// \brief Correct an error detected during check build
        ///        
        /// \brief #### Description.
        /// - This method is an example for the methods that needs to be created for correcting build errors.
        /// - For each error that has to be handled a method from this prototype has to be created
        /// 
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Create a correction method for the error with this prototype
        /// -#  In the CheckBuild when encountering an error set the correction method name in the 
        ///     BuildCorrectionParameters data structurs
        /// -#  The methods gets its parameters from the object parameter. The structure of the 
        ///     parameters has to be the same in the CheckBuild method and the correction method
        /// -#  In order to access tne element of the network start with the private `network` member of this class
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param buildCorrectionsParameters Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CorrectionMethod(object correctionParameters)
        {

        }

        #endregion
        #region Create (methods that are activated while in create phase)
        #endregion
        #region #region Presentation (Methods that are activated while in running phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string PresentationText()
        ///
        /// \brief Presentation text.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 26/01/2017
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
