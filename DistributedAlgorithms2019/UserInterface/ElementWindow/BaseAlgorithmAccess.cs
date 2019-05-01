////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\BaseAlgorithmAccess.cs
///
/// \brief Implements the base algorithm access class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class BaseAlgorithmAccess
    ///
    /// \brief A base algorithm access.
    ///
    /// \par Description.
    ///      -  Create the code file that make it possible to access dictionaries attributes 
    ///         using getters/setters
    ///      -  The following is the structure of the code file  
    ///         -#  A partial class for NetworkElement in DistributedAlgorithms namespace
    ///         -#  In DistributedAlgorithms.Algorithms.Base.Base namespace:
    ///             -#  A partial class for the BaseMessage
    ///             -#  A partial class for the BaseProcess
    ///             -#  A partial class for the BaseNetwork
    ///             -#  A partial class for the BaseChannel
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 18/03/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class BaseAlgorithmAccess : AddAlgorithmWindow
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseAlgorithmAccess():base(true)
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      To use this class:
        ///      Create an instance of the class and call CreateCodeFile()
        ///
        /// \author Ilanh
        /// \date 18/03/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseAlgorithmAccess():base(true)
        { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CreateCodeFile()
        ///
        /// \brief Creates code file.
        ///
        /// \par Description.
        ///      The main method for creating the code file
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/03/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateCodeFile()
        {
            InitSyntaxHighlight();

            // The header of the file
            string fileText = eol + "using System.Collections.Generic;";
            fileText += eol + "using DistributedAlgorithms;";
            fileText += eol + "using DistributedAlgorithms.Algorithms.Base.Base;";

            // The NetworkElement partial class
            if (!CreateNetworkElementText(ref fileText))
            {
                CustomizedMessageBox.Show("Base algorithm code building cancelled by the user", "BaseAlgorithmAccess", null);
                return;
            }

            // The base algorithm partial class
            NetworkElement[] networkElements = { new BaseMessage(), new BaseNetwork(), new BaseProcess(), new BaseChannel() };
            classNames[0] = "Message";
            string[] enumClasses = { "bm", "bn", "bp", "bc" };

            fileText += eol + @"namespace DistributedAlgorithms.Algorithms." + "Base" + "." + "Base" + eol + "{";
            for (classIdx = 0; classIdx < classNames.Count; classIdx++)
            {
                networkElements[classIdx].Init(0);
                if (!CreateBaseClassText(ref fileText, networkElements[classIdx], enumClasses[classIdx]))
                {
                    CustomizedMessageBox.Show("Base algorithm code building cancelled by the user", "BaseAlgorithmAccess", null);
                    return;
                }
            }
            fileText += eol + "}";
            classNames[0] = "Process";
            File.WriteAllText(ClassFactory.GenerateAlgorithmPath("Base", "Base") + "\\" + "Base" + "DefsAndInits" + ".cs", fileText);
            CustomizedMessageBox.Show("Base algorithm code building finished", "BaseAlgorithmAccess", null, Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool CreateBaseClassText(ref string fileText, NetworkElement networkElement, string enumClass)
        ///
        /// \brief Creates base class text.
        ///
        /// \par Description.
		//       Create the code to one Base algorithm class
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/03/2018
        ///
        /// \param [in,out] fileText       (ref string) - The file text.
        /// \param          networkElement  (NetworkElement) - The network element.
        /// \param          enumClass       (string) - The enum class.
        ///
        /// \return True if the user quitted .
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CreateBaseClassText(ref string fileText, NetworkElement networkElement, string enumClass)
        {
            string classText = eol + "\t#region /// \\name partial class for " + "Base" + classNames[classIdx];
            classText += eol + "\tpublic partial class " + "Base" + classNames[classIdx] + @": " + "NetworkElement";
            classText += eol + "\t{";
            classText += networkElement.CreateGettersSetters(enumClass, "Base", "Base");
            classText += eol + "\t}";
            classText += eol + "\t#endregion";
            if (ShowClass(ref fileText, "partial", classText, "Base" + classNames[classIdx]))
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool CreateNetworkElementText(ref string fileText)
        ///
        /// \brief Creates network element text.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 18/03/2018
        ///
        /// \param [in,out] fileText (ref string) - The file text.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CreateNetworkElementText(ref string fileText)
        {
            NetworkElement networkElement = new NetworkElement();
            networkElement.Init(0);
            string text = eol + @"namespace DistributedAlgorithms" + eol + "{" + eol;
            text += eol + "\t#region /// \\name partial class for " + "NetworkElement";
            text += eol + "\tpublic partial class " + "NetworkElement" + @": " + "IValueHolder";
            text += eol + "\t{";
            text += networkElement.CreateGettersSetters("ne");
            text += eol + "\t}";
            text += eol + "\t#endregion";
            text += eol + "}";            
            if (ShowClass(ref fileText, "partial", text, "NetworkElement"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
