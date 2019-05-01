////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file infrastructure\permissions.cs
///
/// \brief Implements the permissions class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class Permissions
    ///
    /// \brief A permissions.
    ///
    /// \par Description.
    ///      -  This class handles the permissions for Set/AddRemove/Clear actions according to the ActivationPhase
    ///         That the program is in.
    ///      -  it is used to make sure that there will be no operations that contredict the program design.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 25/07/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Permissions
    {
        #region /// \name Enums

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum PermitionTypes
        ///
        /// \brief Values that represent permition types.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum PermitionTypes { Set, AddRemove, Clear }
        #endregion
        #region /// \name Members
        /// \brief (int) - Number of phases.
        private int numPhases = Enum.GetNames(typeof(MainWindow.ActivationPhases)).Length;
        /// \brief (int) - Number of permition types.
        private int numPermitionTypes = Enum.GetNames(typeof(PermitionTypes)).Length;
        /// \brief (bool[,]) - The permissions.
        private bool[,] permissions;
        /// \brief (ElementDictionaries) - The dictionary.
        public NetworkElement.ElementDictionaries dictionary = NetworkElement.ElementDictionaries.None;
        #endregion
        #region /// \name getters/Setters

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public bool[,] Data
        ///
        /// \brief Gets the data.
        ///
        /// \return The data.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool[,] Data { get { return permissions; } }
        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Permissions(bool defaultValue)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Init all the permissions with the same value.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param defaultValue (bool) - true to default value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Permissions(bool defaultValue)
        {
            permissions = new bool[numPhases, numPermitionTypes];
            Set(defaultValue);
            Set(MainWindow.ActivationPhases.Temp, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Permissions()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///      Init with nulls.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Permissions()
        {
            permissions = new bool[numPhases, numPermitionTypes];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Permissions(Permissions permissions)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Copy constructor.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param permissions (Permissions) - The permissions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Permissions(Permissions permissions)
        {
            this.permissions = new bool[numPhases, numPermitionTypes];
            this.dictionary = permissions.dictionary;
            Set(permissions);
            Set(MainWindow.ActivationPhases.Temp, true);
        }
        #endregion
        #region /// \name Setting permissions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Set(bool defaultValue, Dictionary<MainWindow.ActivationPhases, PermitionTypes> exections)
        ///
        /// \brief Sets.
        ///
        /// \par Description.
        ///      Set all the permissions to default value except for entries in a dictionaries.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param defaultValue (bool) - true to default value.
        /// \param exections    (ActivationPhases,PermitionTypes>) - The exections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Set(bool defaultValue, Dictionary<MainWindow.ActivationPhases, PermitionTypes> exections)
        {
            Set(defaultValue);
            foreach (var exception in exections)
            {
                permissions[(int)exception.Key, (int)exception.Value] = !defaultValue;
            }
            Set(MainWindow.ActivationPhases.Temp, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Set(bool defaultValue)
        ///
        /// \brief Sets to default.
        ///
        /// \par Description.
        ///      Set all entries.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param defaultValue (bool) - true to default value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Set(bool defaultValue)
        {
            foreach (var phase in TypesUtility.GetAllEnumValues(typeof(MainWindow.ActivationPhases)))
            {
                foreach (var permition in TypesUtility.GetAllEnumValues(typeof(PermitionTypes)))
                {
                    permissions[(int)phase, (int)permition] = defaultValue;
                }
            }
            Set(MainWindow.ActivationPhases.Temp, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Set(MainWindow.ActivationPhases phase, bool value)
        ///
        /// \brief Sets all for phase.
        ///
        /// \par Description.
        ///      Set all permissions of one program activation phase.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param phase (ActivationPhases) - The phase.
        /// \param value (bool) - true to value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Set(MainWindow.ActivationPhases phase, bool value)
        {
            for (int permitionTypeIdx = 0; permitionTypeIdx < numPermitionTypes; permitionTypeIdx++)
            {
                permissions[(int)phase, permitionTypeIdx] = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Set(PermitionTypes permitionType, bool value)
        ///
        /// \brief Sets to all design phases.
        ///
        /// \par Description.
        ///      Set to all the entries of the Design phases.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param permitionType (PermitionTypes) - Type of the permition.
        /// \param value         (bool) - true to value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Set(PermitionTypes permitionType, bool value)
        {
            permissions[(int)MainWindow.ActivationPhases.Initiated, (int)permitionType] = value;
            permissions[(int)MainWindow.ActivationPhases.Loaded, (int)permitionType] = value;
            permissions[(int)MainWindow.ActivationPhases.Checked, (int)permitionType] = value;
            permissions[(int)MainWindow.ActivationPhases.Created, (int)permitionType] = value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Set(Permissions permissions)
        ///
        /// \brief Sets the given permissions.
        ///
        /// \par Description.
        ///      Set all the permissions according to a source permissions oblject.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param permissions The permissions to set.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Set(Permissions permissions)
        {
            for (int phaseIdx = 0; phaseIdx < numPhases; phaseIdx++)
            {
                for (int permitionTypeIdx = 0; permitionTypeIdx < numPermitionTypes; permitionTypeIdx++)
                {
                    this.permissions[phaseIdx, permitionTypeIdx] = permissions.permissions[phaseIdx, permitionTypeIdx];
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Set(MainWindow.ActivationPhases activationPhase, PermitionTypes permitionTypes, bool value)
        ///
        /// \brief Sets.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param activationPhase (ActivationPhases) - The activation phase.
        /// \param permitionTypes  (PermitionTypes) - List of types of the permissions.
        /// \param value           (bool) - true to value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Set(MainWindow.ActivationPhases activationPhase, PermitionTypes permitionTypes, bool value)
        {
            permissions[(int)activationPhase, (int)permitionTypes] = value;
        }
        #endregion
        #region /// \name Check permition

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool CheckPermition(PermitionTypes permitionType, ElementWindow sender)
        ///
        /// \brief Check permition.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param permitionType (PermitionTypes) - Type of the permition.
        /// \param sender        (ElementWindow) - The sender.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CheckPermition(PermitionTypes permitionType, ElementWindow sender)
        {
            // If the sender is an ElementWindow currently presented the check should be bypasses
            if (Permissions.IsWindowPresented(sender))
            {
                return true;
            }
            return permissions[(int)MainWindow.ActivationPhase, (int)permitionType];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool IsWindowPresented(ElementWindow window)
        ///
        /// \brief Query if 'window' is window presented.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param window (ElementWindow) - The window.
        ///
        /// \return True if window presented, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool IsWindowPresented(ElementWindow window)
        {
            if (window != null)
            {
                foreach (Window w in Application.Current.Windows)
                {
                    if (w == window)
                        return true;
                }
            }
            return false;
        }
        #endregion
        #region /// \name Presentation

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
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \return A string that represents this object.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            string s = "The dictionary is : " + TypesUtility.GetKeyToString(dictionary) + "\n";
            s += "The permissions table\n" + "\t\t";
            for (int permitionTypeIdx = 0; permitionTypeIdx < numPermitionTypes; permitionTypeIdx ++)
            {
                string permitionTypeName = TypesUtility.GetKeyToString((PermitionTypes)permitionTypeIdx);
                s += permitionTypeName + "\t";
            }
            s += "\n";
            for (int phaseIdx = 0; phaseIdx < numPhases; phaseIdx++)
            {
                string phaseName = TypesUtility.GetKeyToString((MainWindow.ActivationPhases)phaseIdx);
                if (phaseName.Length >10)
                {
                    s += phaseName + "\t";
                }
                else
                {
                    s += phaseName + "\t\t";
                }
                for (int permitionTypeIdx = 0; permitionTypeIdx < numPermitionTypes; permitionTypeIdx++)
                {
                    if ((PermitionTypes)permitionTypeIdx == PermitionTypes.AddRemove)
                    {
                        s += permissions[phaseIdx, permitionTypeIdx].ToString() + "\t\t";
                    }
                    else
                    {
                        s += permissions[phaseIdx, permitionTypeIdx].ToString() + "\t";
                    }                    
                }
                s += "\n";
            }
            return s;
        }
        #endregion
        #region /// \name Equals

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Equals(Permissions other)
        ///
        /// \brief Tests if this Permissions is considered equal to another.
        ///
        /// \par Description.
        ///      Check if one permissions values are the same as the other
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param other The permissions to compare to this object.
        ///
        /// \return True if the objects are considered equal, false if they are not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Equals(Permissions other)
        {
            for (int rowIdx = 0; rowIdx < numPhases; rowIdx++)
                for (int colIdx = 0; colIdx < numPermitionTypes; colIdx++)
                    if (permissions[rowIdx, colIdx] != other.Data[rowIdx, colIdx])
                        return false;
            return true;
        }
        #endregion
    }
}
