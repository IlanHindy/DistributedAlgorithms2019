////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    UserInterface\IScanConsumer.cs
///
///\brief   Declares the IScanConsumer interface.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \interface IScanConsumer
    ///
    /// \brief Interface for scan consumer.
    /// \par
    ///      -  The scan consumer is a class that asks for all the attributes of a network element 
    ///         or attribute dictionary
    ///      -  The process of activating the scanning is as follows:
    ///         -# Declare a stack in the class
    ///         -# Activate the ScanAndReport method of the object you want to scan
    ///         -# Implement the methods of this interface
    ///
    /// \author Ilanh
    /// \date 26/04/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public interface IScanConsumer
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void OpenComplexAttribute(Attribute attribute, dynamic key, string name, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary);
        ///
        /// \brief Opens complex attribute.
        ///        -    This method reports of a beginning of a complex attribute (like AttributeList,
        ///        -    AttributeDictionary, NetworkElement)
        ///        -    usually (after doing what you want to do) you have to push the attribute to the stack.
        ///
        /// \param attribute      (object) - The attribute.
        /// \param key            (dynamic) - The key.
        /// \param name           (string) - The name.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void OpenComplexAttribute(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void CloseComplexAttribute(Attribute attribute, dynamic key, string name, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary);
        ///
        /// \brief Closes complex attribute.
        ///        -    This method reports the end of a complex attribute (like the indication that all attributes
        ///             in a list where reported). 
        ///        -    Usually (after doing what you want to do) you have to pop the
        ///             top-most attribute in the stack.
        ///
        /// \param attribute      (object) - The attribute.
        /// \param key            (dynamic) - The key.
        /// \param name           (string) - The name.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void CloseComplexAttribute(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn void AttributeReport(Attribute attribute, dynamic key, string name, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary);
        ///
        /// \brief Attribute report.
        ///        This method reports on a simple value attribute.
        ///
        /// \param attribute      (object) - The attribute.
        /// \param key            (dynamic) - The key.
        /// \param name           (string) - The name.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void AttributeReport(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn bool ScanCondition(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary);
        ///
        /// \brief decide if to report an attribute.
        ///        -    This method is used by the scan consumer for filtering what attributes will be reported.
        ///             (before activating the Report of an Attribute or Complex Attribute the scan reported is
        ///             activating this method to decide if to report the attribute)
        ///
        /// \param key            (dynamic) - The key.
        /// \param attribute      (Attribute) - The attribute.
        /// \param mainDictionary (ElementDictionaries) - The dictionary key.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool ScanCondition(dynamic key, 
            Attribute attribute, 
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary);
    }
}
