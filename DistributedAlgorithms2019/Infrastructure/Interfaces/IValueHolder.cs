////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file infrastructure\ivalueholder.cs
///
/// \brief Implements the ivalueholder class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \interface IValueHolder
    ///
    /// \brief Interface for value holder.
    ///        This interface contains all the methods for classes that holds values
    ///        of the algorithm implementation.
    ///
    /// \author Ilanh
    /// \date 25/04/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public interface IValueHolder
    {
        #region /// \name Getters
        BaseNetwork Network { get; }
        NetworkElement Element { get; }
        Permissions Permissions { get; }
        IValueHolder Parent { get; }

        #endregion
        #region /// \name Load and Save

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn bool Load(XmlNode xmlNode, Type hostType = null);
        ///
        /// \brief Loads.
        ///        Load the object from xml doc (or file)
        ///
        /// \param xmlNode  (XmlNode) - The XML node.
        /// \param hostType (Optional)  (Type) - Type of the host.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool Load(XmlNode xmlNode, Type hostType = null);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn XmlNode Save(XmlDocument xmlDoc, XmlNode xmlNode);
        ///
        /// \brief Saves.
        ///        Saves the object to xml doc (or file)
        ///
        /// \param xmlDoc  (XmlDocument) - The XML document.
        /// \param xmlNode (XmlNode) - The XML node.
        ///
        /// \return An XmlNode.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        XmlNode Save(XmlDocument xmlDoc, XmlNode xmlNode);
        #endregion
        #region /// \name Scanning operations

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void ScanAndReport(IScanConsumer scanConsumer, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, dynamic key = null);
        ///
        /// \brief Scans and activate.
        ///        This method implements a deep scanning of all the attribute tree of the IValueHolder.  
        ///
        /// \param scanConsumer   (IScanConsumer) - The scan consumer.
        /// \param mainDictionary (ElementDictionaries) - Dictionary key of the dictionary in the scanned
        ///                       NetworkElement (The network element which is the root of the scanning)
        /// \param dictionary     (ElementDictionaries) - Dictionary key of the dictionary of the
        ///                       NetworkElement which is currently scanned.
        /// \param key            (Optional)  (dynamic) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void ScanAndReport(IScanConsumer scanConsumer,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary, dynamic key = null);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn bool EqualsTo(int nestingLevel, ref string error, IValueHolder other, bool checkNotSameObject = false);
        ///
        /// \brief Equals to the other element (deep compare)
        ///
        /// \param          nestingLevel       (int) - The nesting level.
        /// \param [in,out] error              (ref string) - The error.
        /// \param          other              (IValueHolder) - The value holder.
        /// \param          checkNotSameObject (Optional)  (bool) - true to check not same object.
        ///
        /// \return True if equals to, false if not.
        ///
        /// \par Usage
        ///      - If you want to start a compare you should call : `ValueHolder.EqualsTo()`  
        ///      - The method does the common checks and call this method to propagate the compare to the childrens.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool EqualsTo(int nestingLevel, ref string error, IValueHolder other, bool print, bool checkNotSameObject = false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void DeepCopy(IValueHolder source, bool isInitiator = false);
        ///
        /// \brief Deep copy.
        ///        Copy all the attribute tree under the source.
        ///
        /// \param source      (IValueHolder) - Source for the.
        /// \param isInitiator (Optional)  (bool) - true if this object is initiator of the copy.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void DeepCopy(IValueHolder source, bool isInitiator = true);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn bool CheckMembers(int nestingLevel, bool checkPermissionsForFirstLevel = true);
        ///
        /// \brief Check the common members to all ValueHolders in the tree
        ///        
        /// \par Usage
        ///      - If you want to start a check you should call : `ValueHolder.CheckMembers()`
        ///      - The method does the common checks and call this method to propagate the check to the childrens.  
        ///      - If you want to start a check on a NetworkElement call CheckMembers(string checkName
        ///
        /// \param nestingLevel                 (int) - The nesting level.
        /// \param checkPermissionsForFirstLevel (Optional)  (bool) - true to check permissions for first level.
        ///                                                         This parameter is for NetworkElements
        ///                                                         that are part of the network. In these NetworkElements the permissions
        ///                                                         of the NetworkElement is all false but the permissions of the dictionaries
        ///                                                         is according to the dictionaries usage.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool CheckMembers(int nestingLevel, bool checkPermissionsForFirstLevel = true);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void SetMembers(BaseNetwork network, NetworkElement element, Permissions permissions, IValueHolder parent, bool recursive = true);
        ///
        /// \brief Sets the members common to all value holders. This method is recursive and sets the members
        ///        to all the tree under the IValueHolder.
        ///
        /// \param network      (BaseNetwork) - The network.
        /// \param element      (NetworkElement) - The element.
        /// \param permissions  (Permissions) - The permissions.
        /// \param parent       (IValueHolder) - The parent.
        /// \param recursive   (Optional)  (bool) - true to process recursively, false to process locally only.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void SetMembers(BaseNetwork network, NetworkElement element, Permissions permissions, IValueHolder parent, bool recursive = true);
        #endregion
        #region /// \name Presentation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn bool ShortDescription(dynamic key, ref string keyString, ref string separator, ref string valueString);
        ///
        /// \brief Short description.
        ///        Create a short description of the object.
        ///
        /// \param          key         (dynamic) - The key.
        /// \param [in,out] keyString   (ref string) - The key string of the attribute that holds the object.
        /// \param [in,out] separator   (ref string) - The separator that will be used to separate between the key and value.
        /// \param [in,out] valueString (ref string) - The value string.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool ShortDescription(dynamic key, ref string keyString, ref string separator, ref string valueString);
        #endregion
        #region /// \name Get Child Key

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn dynamic GetChildKey(IValueHolder child);
        ///
        /// \brief Gets a child key.
        ///        Returns the key in the AttributeDictionary/AttributeList/NetworkElement of this
        ///        IValueHolder.
        ///
        /// \param child (IValueHolder) - The child.
        ///
        /// \return The child key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        dynamic GetChildKey(IValueHolder child);
        #endregion
        #region /// \name ElementWindow support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable, bool attributeEdittable);
        ///
        /// \brief Creates a new value field data.
        ///        -#   The ElementWindow needs from the IValueHolder data for 3 fields:
        ///        -#   The Header which holds the key
        ///        -#   The Existing value textbox
        ///        -#   The New value control textbox
        ///        -#   The methods here are the prototypes for the defaults sfor the value holder.
        ///
        /// \param [in,out] elementWindowPrms  (InputFieldPrms) - The input field prms.
        /// \param          mainNetworkElement  (NetworkElement) - The main network element.
        /// \param          mainDictionary     (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary         (ElementDictionaries) - The dictionary.
        /// \param          inputWindow        (InputWindows) - The input window.
        /// \param          windowEdittable    (bool) - true if window edittable.
        /// \param          attributeEdittable (bool) - true if attribute edittable.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary, 
            InputWindows inputWindow, 
            bool windowEdittable, 
            bool attributeEdittable);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void DefaultKeyFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, dynamic key);
        ///
        /// \brief Key field data.
        ///
        /// \param [in,out] elementWindowPrms  (InputFieldPrms) - The input field prms.
        /// \param          mainNetworkElement  (NetworkElement) - The main network element.
        /// \param          mainDictionary     (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary         (ElementDictionaries) - The dictionary.
        /// \param          inputWindow        (InputWindows) - The input window.
        /// \param          key                (dynamic) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void DefaultKeyFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow, 
            dynamic key);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn string DefaultExistingValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow);
        ///
        /// \brief Existing value field data.
        ///
        /// \param [in,out] elementWindowPrms  (InputFieldPrms) - The input field prms.
        /// \param          mainNetworkElement (NetworkElement) - The main network element.
        /// \param          mainDictionary     (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary         (ElementDictionaries) - The dictionary.
        /// \param          inputWindow        (InputWindows) - The input window.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void DefaultExistingValueFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn string MembersStr();
        ///
        /// \brief Members string.
        ///        This method is used to create a string with the members that can
        ///        be used for presentations (like the DefaultExistingValueFieldData)
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        string MembersStr();
       
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void DefaultTypeString(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow);
        ///
        /// \brief Default type string.
        ///
        /// \param [in,out] elementWindowPrms  (ref ElementWindowPrms) - The element window prms.
        /// \param          mainNetworkElement  (NetworkElement) - The main network element.
        /// \param          mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary          (ElementDictionaries) - The dictionary.
        /// \param          inputWindow         (InputWindows) - The input window.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void DefaultTypeString(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow);

        #endregion
    }
}
