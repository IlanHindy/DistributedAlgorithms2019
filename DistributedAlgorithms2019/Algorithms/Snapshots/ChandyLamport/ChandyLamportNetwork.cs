////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\ChandyLamport\ChandyLamportNetwork.cs
///
/// \brief Implements the template network class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport
{

    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ChandyLamportNetwork
    ///
    /// \brief A template network.
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
    ///
    /// \author Ilan Hindy
    /// \date 26/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class ChandyLamportNetwork : BaseNetwork
    {

        #region /// \name constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamportNetwork():base()
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

        public ChandyLamportNetwork():base() { }

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
        ///      Set the attribute of the network:
        ///      -# Centralized
        ///      -# Directed
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void AdditionalInitiations()
        {
            base.AdditionalInitiations();
            ea[bn.eak.DirectedNetwork] = false;
            ea[bn.eak.Centrilized] = true;
        }

        #endregion
        #region /// \name Design (methods that are activated while in design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool ChandyLamportNetworkDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        ///
        /// \brief A prototype for attribute checking.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      -#  Each Attribute can be assigned a value check that will be performed when changing the
        ///          attribute from the network edit window while in design mode
        ///      -#  For each attribute that you want to add a check to, one method with the prototype of the
        ///          following method have to be designed
        ///      -#  In order to add the check to the attribute you have to set the EndInputOperationMethod
        ///          variable of the Attribute with the method you generated for example :
        /// ~~~{.cs}
        ///      // Setting the method to an existing attribute
        ///      attribute.EndInputOperationMethod =  ChandyLamportnetworkDefaultAttributeCheck
        ///      
        ///      // Setting the method to an attribute while creation
        ///      pa.Add(n.pak.SomeKey, new Attribute {Value = SomeValue, EndInputOperationMethod =  ChandyLamportnetworkDefaultAttributeCheck})
        /// ~~~
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param       network          (BaseNetwork) - The network.
        /// \param       networkElement   (NetworkElement) - The network element.
        /// \param       parentAttribute  (Attribute) - The parent attribute.
        /// \param       attribute        (Attribute) - The attribute.
        /// \param       newValue         (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool ChandyLamportNetworkDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        ///      This method performs checks on the network at the check network phase
        ///      (before running and after designing)
        ///
        /// \par checking network process.
        ///      The check and correct of a network element is composed from :
        ///      -#  A CheckBuild method that does the checks and for each error it finds it fills a
        ///          BuildCorrectionParameters data structure and insert it to a list that is given as parameter to the
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
        ///      -  The processes of the network are in `network.Processes`  
        ///      -  The channels of the network are in `network.Channels`.
        ///
        /// \param buildCorrectionsParameters  (List&lt;BuildCorrectionParameters&gt;) - Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            base.CheckBuild(buildCorrectionsParameters);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CorrectionMethod(object correctionParameters)
        ///
        /// \brief Correct an error detected during check build.
        ///
        /// \par Description.
        ///      - This method is an example for the methods that needs to be created for correcting build
        ///         errors.
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
        ///      -#  The processes of the network are in `network.Processes`  
        ///      -#  The channels of the network are in `network.Channels`.
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
        #region /// \name Create (methods that are activated while in create phase)
        #endregion
        #region /// \name #region /// \name Presentation (Methods that are activated while in running phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string PresentationText()
        ///
        /// \brief Presentation text.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
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
        #region /// \name algorithm utility methods
        #endregion 
    }
}
