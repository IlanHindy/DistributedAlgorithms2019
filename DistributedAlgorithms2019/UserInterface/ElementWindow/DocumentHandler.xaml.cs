////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	UserInterface\DocumentHandler.xaml.cs
//
// summary:	Implements the document handler.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Diagnostics;
using System.Windows.Input;
using DocumentFormat.OpenXml;

namespace DistributedAlgorithms
{
    #region /// \name Program documents presenter

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ProgramDocuments
    ///
    /// \brief A program documents.
    ///
    /// \brief #### Description.
    ///        This class is responsible to show the program documentation on the web browser
    ///
    /// \brief #### Usage Notes.
    ///
    /// \author Ilanh
    /// \date 14/03/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ProgramDocuments
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void ShowProgramDocumentation()
        ///
        /// \brief Shows the program documentation.
        ///
        /// \brief #### Description.
        ///        Shows the documentation on the web browser using a batch file
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ShowProgramDocumentation()
        {
            Process proc = null;

            string _batDir = Config.Instance[Config.Keys.ProgramDocsViewerPath];
            proc = new Process();
            proc.StartInfo.WorkingDirectory = _batDir;
            proc.StartInfo.FileName = Config.Instance[Config.Keys.ProgramDocsViewer];
            proc.StartInfo.CreateNoWindow = false;
            proc.Start();
            proc.WaitForExit();
            proc.StartInfo.Arguments = Config.Instance[Config.Keys.HtmlDocPath];
            //ExitCode = proc.ExitCode;
            proc.Close();
            MessageBox.Show("Open documentation in the Internet...");
        }
    }
    #endregion
    #region /// \name Word document holder

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class WordDocumentHolder
    ///
    /// \brief A word document holder.
    ///
    /// \brief #### Description.
    ///        This class is responsible to handling word documents presentation
    ///
    /// \brief #### Usage Notes.
    ///
    /// \author Ilanh
    /// \date 14/03/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class WordDocumentHolder
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum FileType
        ///
        /// \brief Values that represent file types.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum FileTypes { Source, Temp, Document, PseudoCode };

        /// \brief The open documents.
        private Dictionary<string, Microsoft.Office.Interop.Word.Document> openDocuments = new Dictionary<string, Microsoft.Office.Interop.Word.Document>();
        /// \brief The word application.
        private Microsoft.Office.Interop.Word.Application wordApplication = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Dictionary&lt;FileTypes,string&gt;) - Type of the files by.
        ///        This dictionary is used to ensure we have only one document from each file type.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Dictionary<FileTypes, string> filesByType = new Dictionary<FileTypes, string> {
            { FileTypes.Source, "" },
            { FileTypes.Temp, "" },
            { FileTypes.Document, "" },
            { FileTypes.PseudoCode, "" }
        };

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public Microsoft.Office.Interop.Word.Application WordApplication
        ///
        /// \brief Gets the word application.
        ///
        /// \return The word application.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Microsoft.Office.Interop.Word.Application WordApplication
        {
            get
            {
                // If this is the first time that the application is opened - create the application
                if (wordApplication == null)
                {
                    wordApplication = new Microsoft.Office.Interop.Word.Application();
                    wordApplication.Visible = true;
                }

                // If the application was opened 
                else
                {
                    // Check if the application was closed outside of the program
                    try
                    {
                        int x = wordApplication.Documents.Count;
                    }

                    // If the application was closed outside the program - create a new application
                    catch
                    {
                        wordApplication = new Microsoft.Office.Interop.Word.Application();
                        wordApplication.Visible = true;
                    }
                }
                return wordApplication;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void OpenWordDocument(string filePath, string fileName)
        ///
        /// \brief Opens word document.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param filePath  (string) - Full pathname of the file.
        /// \param fileName  (string) - Filename of the file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool OpenWordDocument(FileTypes fileType, string filePath, string fileName)
        {
            // Try To open the file
            // (This is done in order to open the file if it was closed outside of the application
            if (filesByType[fileType] == fileName)
            {
                try
                {
                    openDocuments.Add(fileName, WordApplication.Documents.Open(Path.GetFullPath(filePath) + fileName));
                    filesByType[fileType] = fileName;

                }
                // If the opening of the file failed that means that the file already open by 
                // the application
                catch { }
                return true;
            }

            // Handle the case that the file to open is different from the file opened
            // Case 1 : There is a (different file) opened for this file type
            if (filesByType[fileType] != "")
            {
                CloseWordDocument(filesByType[fileType]);
            }
            // The algorithm of opening a file
            // As long as the file is already opened (outside of the application)
            //      Create a customized message box with the options
            //      if the result of the message box is cancel - return false
            //      if the result is retry to open

            while (true)
            {
                try
                {
                    openDocuments.Add(fileName, WordApplication.Documents.Open(Path.GetFullPath(filePath) + fileName));
                    filesByType[fileType] = fileName;
                    return true;
                }
                catch (Exception e)
                {
                    string result = CustomizedMessageBox.FileMsgErr("Error while parsing source file",
                        Path.GetFileName(fileName),
                        Path.GetDirectoryName(fileName),
                        e.Message,
                        "Document Creator Message",
                        new List<string> { "Retry", "Cancel" });

                    if (result == "Cancel")
                    {
                        return false;
                    }
                }
            }
        }
               

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CloseWordDocument(string fileName)
        ///
        /// \brief Closes word document.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param fileName  (string) - Filename of the file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CloseWordDocument(string fileName)
        {
            // If the document was opened 
            if (openDocuments.ContainsKey(fileName))
            {
                // Try to close the document
                try
                {
                    openDocuments[fileName].Close();
                    FileTypes fileType = filesByType.First(entry => entry.Value == fileName).Key;
                    filesByType[fileType] = "";
                }

                // If the document was closed outside the program - do nothing
                catch { }
                openDocuments.Remove(fileName);

                // If there are no documents quit the application
                if (openDocuments.Count == 0)
                {
                    QuitApplication();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CloseAllWordDocuments()
        ///
        /// \brief Closes all word documents.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CloseAllWordDocuments()
        {
            for (int idx = openDocuments.Keys.Count - 1; idx >= 0; idx--)
            {
                CloseWordDocument(openDocuments.Keys.ToList()[idx]);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Microsoft.Office.Interop.Word.Document RetrieveOpenedDocument(string fileName)
        ///
        /// \brief Retrieves opened document.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param fileName  (string) - Filename of the file.
        ///
        /// \return A Microsoft.Office.Interop.Word.Document.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Microsoft.Office.Interop.Word.Document RetrieveOpenedDocument(string fileName)
        {
            return openDocuments[fileName];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool ApplicationExists()
        ///
        /// \brief Queries if a given application exists.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ApplicationExists()
        {
            return wordApplication != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void QuitApplication()
        ///
        /// \brief Quit application.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///        This method should be used to close all documents and the application
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void QuitApplication()
        {
            try
            {
                wordApplication.Quit();
            }
            catch { }
            wordApplication = null;
        }

    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class DocumentHandler
    ///
    /// \brief A document creator.
    ///
    /// \par Description.
    ///     -   This class handles the creation of program help documents
    ///     -   The program help documents are created from word file
    ///     -   The class make it possible to remove section from the original file
    ///         So that only the documentation on the algorithm will appear in the algorithm documentation
    ///     -   The program uses 4 files:
    ///        -#   The documentation source file
    ///        -#   A temporary file to view the results
    ///        -#   A destination file
    ///        -#   A pseudo-code file
    ///
    ///     -   The editing process is composed from the following steps
    ///        -#   From the source file create a list of paragraphs
    ///        -#   Set all the paragraphs that are not in the destination file - invisible
    ///        -#   The interactive process set or reset the visibility attribute of paragraphs
    ///        -#   When finished create a word document from the visible paragraphs
    ///
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 14/03/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class DocumentHandler : Window
    {
        #region /// \name Enums

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ControlsInPanel
        ///
        /// \brief Values that represent controls in panels.
        ///        Each paragraph is presented in a grid. This enum is for the components in the grid it determines the
        ///        order of the components in the grid
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private enum ControlsInPanel { ParagraphText, Separator1, StyleId, Separator2, Idx }

        #endregion
        #region /// \name Member Variables

        /// \brief Filename of the source file.
        public string sourceFileName = "";

        /// \brief source file path.
        public string sourceFilePath = "";

        /// \brief Filename of the destination file.
        public string processedFileName = "";

        /// \brief destination file path.
        public string processedFilePath = "";

        /// \brief Filename of the temporary file.
        public string tempFileName = "";

        /// \brief temporary file path.
        public string tempFilePath = "";

        /// \brief  (string) - Filename of the pseudo code file.
        public string pseudoCodeFileName = "";

        /// \brief  (string) - Full pathname of the pseudo code file.
        public string pseudoCodeFilePath = "";

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true if constructor quited.
        ///        This flag is used to signal to the calling dialog that the constructor
        ///        quited and no ShowDialog action is needed.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool constructorQuited = false;

        /// \brief The selected paragraph index.
        private int selectedParagraphIndex = -1;

        /// \brief Name of the algorithm.
        private string algorithm;

        /// \brief The algorithm subject.
        private string subject;

        /// \brief The word document holder.
        private WordDocumentHolder wordDocumentHolder = new WordDocumentHolder();

        #endregion
        #region /// \name static method to show documents

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void LoadDocumentation(WordDocumentHolder wordDocumentHolder)
        ///
        /// \brief Loads a documentation.
        ///
        /// \par Description.
        ///      This method loads a word document of the current algorithm
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      -  The caller of this method has to define WordDocumentsHolder  
        ///      -  Upon exiting the caller of this method has to operate the QuitApplication method  
        ///         of the WordDocumentsHolder
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \param wordDocumentHolder  (WordDocumentHolder) - The word document holder.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void LoadDocumentation(WordDocumentHolder wordDocumentHolder, string subject = "", string algorithm = "")
        {
            if (algorithm == "" || subject == "")
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
                subject = Config.Instance[Config.Keys.SelectedSubject];
            }

            string docFilePath = Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.DocProcessedPath];
            AttributeList docFileNames = Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.DocProcessedFiles];

            if (docFileNames.Count > 0)
            {
                string docFileName = docFileNames[0];
                wordDocumentHolder.OpenWordDocument(WordDocumentHolder.FileTypes.Document, docFilePath, docFileName);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void LoadPseudoCode(WordDocumentHolder wordDocumentHolder, string subject = "", string algorithm = "")
        ///
        /// \brief Loads pseudo code.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param wordDocumentHolder  (WordDocumentHolder) - The word document holder.
        /// \param subject            (Optional)  (string) - The subject.
        /// \param algorithm          (Optional)  (string) - The algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void LoadPseudoCode(WordDocumentHolder wordDocumentHolder, string subject = "", string algorithm = "")
        {
            if (algorithm == "" || subject == "")
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
                subject = Config.Instance[Config.Keys.SelectedSubject];
            }

            string docFilePath = Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.PseudoCodePath];
            AttributeList docFileNames = Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.PseudoCodeFiles];

            if (docFileNames.Count > 0)
            {
                string docFileName = docFileNames[0];
                wordDocumentHolder.OpenWordDocument(WordDocumentHolder.FileTypes.PseudoCode, docFilePath, docFileName);
            }
        }
        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public DocumentHandler()
        ///
        /// \brief Default constructor.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DocumentHandler(string subject = "", string algorithm = "")
        {
            this.subject = subject;
            this.algorithm = algorithm;
            InitializeComponent();
            if (!Load(true))
            {
                CustomizedMessageBox.Show("Create documentation quited", "DocumentHandler Message", null);
                constructorQuited = true;
            }
        }
        #endregion
        #region /// \name File name generation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string GenTempFileName()
        ///
        /// \brief Generates a temporary file name.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/02/2018
        ///
        /// \return The temporary file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GenTempFileName()
        {
            return processedFileName.Substring(0, processedFileName.IndexOf(".")) + "__temp__.docx";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string GenPseudoCodeFileName()
        ///
        /// \brief Generates a pseudo code file name.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/02/2018
        ///
        /// \return The pseudo code file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GenPseudoCodeFileName()
        {
            return processedFileName.Remove(processedFileName.IndexOf(".")) + "_PseudoCode.docx";
        }
        #endregion
        #region /// \name Loading a file

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool Load(bool selectFromConfig)
        ///
        /// \brief Loads.
        ///
        /// \par Description.
        ///      This method is called whenever there is a request to start a new
        ///      document creating It is used when:
        ///      -#   Initialize
        ///      -#   The load button is pressed.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool Load(bool selectFromConfig)
        {
            // Select the algorithm that the documentation is on if not given as parameter to the constructor
            if (algorithm == "" && subject == "")
            {
                string currentAlgorithm = Config.Instance[Config.Keys.SelectedSubject] + "." + Config.Instance[Config.Keys.SelectedAlgorithm];
                if (!SelectAlgorithm(currentAlgorithm))
                {
                    return false;
                }
            }

            // Select source file. The parameter says to try to load the last source file of the algorithm 
            if (!SelectSourceFile(selectFromConfig))
            {
                return false;
            }

            // Select destination file. The parameter says to try and load the last destination file of the algorithm
            if (!SelectDestFile(selectFromConfig))
            {
                return false;
            }

            // Show the files selected
            ShowFiles();

            // Close all open word documents
            wordDocumentHolder.CloseAllWordDocuments();

            // Create the presentation
            Mouse.OverrideCursor = Cursors.Wait;
            DockPanel_Paragraphs.Children.Clear();
            if (!CreatePresentation(sourceFilePath + "\\" + sourceFileName))
            {
                return false;
            }

            // Open the source document
            wordDocumentHolder.OpenWordDocument(WordDocumentHolder.FileTypes.Source, sourceFilePath, sourceFileName);

            // Filter the presentation according to the destination file
            // (set all paragraphs that are not in the destination - invisible)
            FilterAccordingToDestFile();
            Mouse.OverrideCursor = null;

            CustomizedMessageBox.Show("Finished Loading ", "Document Creator Message", Icons.Success);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ShowFiles()
        ///
        /// \brief Shows the files.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ShowFiles()
        {
            Label_Algorithm.Content = subject + "." + algorithm;
            Label_SourceFile.Content = sourceFilePath + sourceFileName;
            Label_DestFile.Content = processedFilePath + processedFileName;
            Label_TempFile.Content = tempFilePath + tempFileName;
            Label_PseudoCodeFile.Content = pseudoCodeFilePath + pseudoCodeFileName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SelectAlgorithm()
        ///
        /// \brief Select algorithm.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool SelectAlgorithm(string currentAlgorithm)
        {
            // Get a list of all the algorithms and open a select dialog to select one of them
            List<string> algorithms = TypesUtility.GetAlgorithms();
            List<int> initialySelected = new List<int> { algorithms.IndexOf(currentAlgorithm) };
            SelectDialog selectDialog = new SelectDialog("Select the algorithm", "Document Creator Message", algorithms, initialySelected);
            selectDialog.ShowDialog();

            // If the select dialog returned Quited - Get the current algorithm and subject
            if (selectDialog.Result == SelectDialog.SelectDialogResult.Quit)
            {
                return false;
            }

            // If the select dialog returned OK - get the algorithm and subject from the results
            else
            {
                string selectedAlgorithm = algorithms[selectDialog.Selection[0]];
                int pointIndex = selectedAlgorithm.IndexOf(".");
                subject = selectedAlgorithm.Substring(0, pointIndex);
                algorithm = selectedAlgorithm.Substring(pointIndex + 1);
            }
            CustomizedMessageBox.Show("The algorithm selected is : " + subject + "." + algorithm, "Document Creator Message", Icons.Info);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool SelectSourceFile(bool loadFromConfig)
        ///
        /// \brief Select source file.
        ///
        /// \brief #### Description.
        ///        Select the source file of the documentation
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param loadFromConfig  (bool) - true to load from configuration.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool SelectSourceFile(bool loadFromConfig)
        {
            // Get the default source file name (the first one in the algorithm's documentation sources
            // It is the last one edited because the list is organized by file date

            AttributeList sourceFiles = Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.DocSourceFiles];


            if (sourceFiles.Count != 0)
            {
                sourceFileName = sourceFiles[0];
            }
            else
            {
                sourceFileName = "";
            }

            // If load from config and there is a source file - select the source file from the config
            if (loadFromConfig)
            {
                if (sourceFileName != "")
                {
                    sourceFilePath = Config.Instance.GenerateSourceDocPath(subject, algorithm);
                    CustomizedMessageBox.FileMsg("The document Source file is :", sourceFileName, sourceFilePath, "", "Document Creator Message", null, Icons.Info);
                    return true;
                }
            }

            // If not to load from config select a source file and copy it to the algorithm's documentation
            // source files
            string documentationSourcePath = Config.Instance.GenerateSourceDocPath(subject, algorithm);
            if (!FileUtilities.SelectInputFile("docx", ref sourceFileName, ref documentationSourcePath))
            {
                return false;
            }

            // Copy the file to the algorithms folder
            sourceFilePath = Config.Instance.GenerateSourceDocPath(subject, algorithm);
            if (documentationSourcePath != sourceFilePath)
            {
                if (!FileUtilities.ReplaceFile(documentationSourcePath + "\\" + sourceFileName, sourceFilePath + "\\" + sourceFileName))
                {
                    return false;
                }

                CustomizedMessageBox.FileMsg("The document Source file is : ", sourceFileName, documentationSourcePath,
                    new List<System.Windows.Controls.Control> {CustomizedMessageBox.SetLabel("And it was transfered to : "),
                    CustomizedMessageBox.SetLabel("File", new Font{fontWeight = FontWeights.Bold }),
                    CustomizedMessageBox.SetLabel(sourceFileName),
                    CustomizedMessageBox.SetLabel("Path", new Font{fontWeight = FontWeights.Bold }),
                    CustomizedMessageBox.SetLabel(Path.GetFullPath(sourceFilePath)) },
                    "Document Creator Message", null, Icons.Info);
            }
            else
            {
                CustomizedMessageBox.FileMsg("The document Source file is : ", sourceFileName, sourceFilePath, "", "Documentation Creator Message");
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool SelectDestFile(bool loadFromConfig = true)
        ///
        /// \brief Select destination file.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param loadFromConfig (Optional)  (bool) - true to load from configuration.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool SelectDestFile(bool loadFromConfig = true)
        {
            // Get the destination documentation path of the algorithm selected
            processedFilePath = Config.Instance.GenerateProcessedDocPath(subject, algorithm);

            // If load from config - Get the last destination file from the config. If there was return the file
            if (loadFromConfig)
            {
                AttributeList processedFiles = Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.DocProcessedFiles];
                if (processedFiles.Count != 0)
                {
                    processedFileName = processedFiles[0];
                    CustomizedMessageBox.FileMsg("The documentation destination file is :", processedFileName, processedFilePath, "", "Document Creator Message");
                    return true;
                }
            }

            // If not to load the last file open a dialog to select the destination file name
            DocsSelect docsSelect = new DocsSelect(DocsSelect.SelectSource.Processed, "Select destination file", subject, algorithm);
            docsSelect.ShowDialog();
            if (docsSelect.result == DocsSelect.SelectResult.Quit)
            {
                return false;
            }
            else
            {
                processedFileName = docsSelect.fileSelected;
                return true;
            }
        }

        #endregion
        #region /// \name Presentation creating

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool GetParagraphsFromFile(string fileName, ref List<OpenXmlElement> paragraphs)
        ///
        /// \brief Gets paragraphs from file.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2018
        ///
        /// \param          fileName    (string) - Filename of the file.
        /// \param [in,out] paragraphs (ref List&lt;OpenXmlElement&gt;) - The paragraphs.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool GetParagraphsFromFile(string fileName, ref List<OpenXmlElement> paragraphs)
        {
            // The algorithm of opening a file
            // As long as the file is already opened (outside of the application)
            //      Create a customized message box with the options
            //      if the result of the message box is cancel - return false
            //      if the result is retry to open

            while (true)
            {
                try
                {
                    using (WordprocessingDocument fileDocument = Open(fileName, true))
                    {
                        paragraphs = fileDocument.MainDocumentPart.Document.Body.Elements<OpenXmlElement>().ToList();
                    }
                    return true;
                }
                catch (Exception e)
                {
                    string result = CustomizedMessageBox.FileMsgErr("Error while parsing source file",
                        Path.GetFileName(fileName),
                        Path.GetDirectoryName(fileName),
                        e.Message,
                        "Document Creator Message",
                        new List<string> { "Retry", "Cancel" });

                    if (result == "Cancel")
                    {
                        return false;
                    }
                }
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static string GetParagraphStyleId(Paragraph p)
        ///
        /// \brief Gets paragraph style identifier.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param p  (Paragraph) - The Paragraph to process.
        ///
        /// \return The paragraph style identifier.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static string GetParagraphStyleId(OpenXmlElement p)
        {
            if (p is Table)
            {
                return "Table";
            }

            ParagraphProperties pPr = p.GetFirstChild<ParagraphProperties>();
            if (pPr != null)
            {
                ParagraphStyleId paraStyle = pPr.ParagraphStyleId;
                if (paraStyle != null)
                {
                    return paraStyle.Val.Value;
                }
            }
            return "Default";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CreateParagraphPresentation(OpenXmlElement paragraph, int paragraphIdx)
        ///
        /// \brief Creates paragraph presentation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/02/2018
        ///
        /// \param paragraph     (OpenXmlElement) - The paragraph.
        /// \param paragraphIdx  (int) - Zero-based index of the paragraph.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CreateParagraphPresentation(OpenXmlElement paragraph, int paragraphIdx)
        {
            if (!(paragraph is Paragraph || paragraph is Table))
                return;
            Grid grid = new Grid();
            DockPanel.SetDock(grid, Dock.Top);
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
           
            // Paragraph text
            TextBlock paragraphText = new TextBlock() { Text = paragraph.InnerText, Width = 500, TextWrapping = TextWrapping.Wrap, FlowDirection = FlowDirection.RightToLeft, Margin = new Thickness(0)};
            Grid.SetColumn(paragraphText, (int)ControlsInPanel.ParagraphText);
            grid.Children.Add(paragraphText);
            paragraphText.MouseLeftButtonDown += SelectParagraph;

            //Separator :
            TextBlock separator1 = new TextBlock() { Text = ":", Width = 20, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0)};
            Grid.SetColumn(separator1, (int)ControlsInPanel.Separator1);
            grid.Children.Add(separator1);
            separator1.MouseLeftButtonDown += SelectParagraph;

            //Style
            TextBlock styleId = new TextBlock() {Text = GetParagraphStyleId(paragraph), Width = 50, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0)};
            Grid.SetColumn(styleId, (int)ControlsInPanel.StyleId);
            grid.Children.Add(styleId);
            styleId.MouseLeftButtonDown += SelectParagraph;

            //Separator :
            TextBlock separator2 = new TextBlock() { Text = ":", Width = 20, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0)};
            Grid.SetColumn(separator2, (int)ControlsInPanel.Separator2);
            grid.Children.Add(separator2);
            separator2.MouseLeftButtonDown += SelectParagraph;

            //Index
            TextBlock idxText = new TextBlock() { Text = paragraphIdx.ToString(), Width = 50, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0)};
            Grid.SetColumn(idxText, (int)ControlsInPanel.Idx);
            idxText.MouseLeftButtonDown += SelectParagraph;
            grid.Children.Add(idxText);
            DockPanel_Paragraphs.Children.Add(grid);
            SetColors(DockPanel_Paragraphs.Children.Count - 1, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool CreatePresentation(string fileName)
        ///
        /// \brief Creates a presentation.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param fileName  (string) - Filename of the file.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CreatePresentation(string fileName)
        {
            List<OpenXmlElement> paragraphs = new List<OpenXmlElement>();
            if (!GetParagraphsFromFile(fileName, ref paragraphs))
            {
                return false;
            }
            for (int idx = 0; idx < paragraphs.Count; idx++)
            {
                CreateParagraphPresentation(paragraphs[idx], idx);
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool FilterAccordingToDestFile()
        ///
        /// \brief Determines if we can filter according to destination file.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool FilterAccordingToDestFile()
        {
            // Check if the destination file exists
            if (!File.Exists(processedFilePath + processedFileName))
            {
                return true;
            }

            // Get the paragraphs from the destination file
            List<OpenXmlElement> destFileParagraphs = new List<OpenXmlElement>();
            if (!GetParagraphsFromFile(processedFilePath + processedFileName, ref destFileParagraphs))
            {
                return false;
            }

            // Remove all the elements that are not Paragraph and lists
            for (int idx = destFileParagraphs.Count - 1; idx >= 0; idx --)
            {
                if (!(destFileParagraphs[idx] is Paragraph || destFileParagraphs[idx] is Table))
                {
                    destFileParagraphs.RemoveAt(idx);
                }
            }


            // The algorithm for filtering
            // For each paragraph in the destination:
            //      Search (starting from the previous paragraph found) for the paragraph
            //      if found : set all the paragraphs from the last found to the new found position collapsed
            int firstSeachIdx = 0;  // This variable holds the place of the end of the previous search
            int paragraphSearchIdx; // This variable holds the index of the current search
            foreach (OpenXmlElement paragraph in destFileParagraphs)
            {
                string style = GetParagraphStyleId(paragraph);
                string text = paragraph.InnerText;
                if (!(style == "" && text == ""))
                {

                    // Search for the destination paragraph in the source paragraphs starting from the last paragraph that was found
                    for (paragraphSearchIdx = firstSeachIdx; paragraphSearchIdx < DockPanel_Paragraphs.Children.Count; paragraphSearchIdx++)
                    {
                        Grid grid = (Grid)DockPanel_Paragraphs.Children[paragraphSearchIdx];
                        string presentationParagraphText = ((TextBlock)grid.Children[0]).Text;
                        string presentationParagraphStyle = ((TextBlock)grid.Children[2]).Text;

                        // If the paragraph was found set all the paragraphs from the last found to the current found - collapsed
                        if (presentationParagraphStyle == style && presentationParagraphText == text)
                        {
                            for (; firstSeachIdx < paragraphSearchIdx; firstSeachIdx++)
                            {
                                ((Grid)DockPanel_Paragraphs.Children[firstSeachIdx]).Visibility = Visibility.Collapsed;
                            }
                            paragraphSearchIdx++;
                            firstSeachIdx++;
                            break;
                        }
                    }
                }
            }

            // Delete all the paragraphs from the last found to the end
            for (; firstSeachIdx < DockPanel_Paragraphs.Children.Count; firstSeachIdx++)
            {
                ((Grid)DockPanel_Paragraphs.Children[firstSeachIdx]).Visibility = Visibility.Collapsed;
            }
            return true;
        }

        #endregion
        #region /// \name paragraph selection

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SelectParagraph(object sender, RoutedEventArgs e)
        ///
        /// \brief Select paragraph.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SelectParagraph(object sender, RoutedEventArgs e)
        {
            if (selectedParagraphIndex != -1)
            {
                // UnSelect the previous selection
                SetColors(selectedParagraphIndex, false);
            }

            //Get the new index
            Grid grid = (Grid)((TextBlock)sender).Parent;
            selectedParagraphIndex = DockPanel_Paragraphs.Children.IndexOf(grid);

            // Select the new selection
            SetColors(selectedParagraphIndex, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetColors(int idx, bool selected)
        ///
        /// \brief Sets the colors.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/02/2018
        ///
        /// \param idx       (int) - The index.
        /// \param selected  (bool) - true if selected.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetColors(int idx, bool selected)
        {
            Grid grid = (Grid)DockPanel_Paragraphs.Children[idx];
            Brush foreground;
            Brush background;

            //Change the brush for headers
            if (!selected)
            {
                switch (((TextBlock)grid.Children[(int)ControlsInPanel.StyleId]).Text)
                {
                    case "1":
                        background = Brushes.DarkBlue;
                        foreground = Brushes.White;
                        break;
                    case "2":
                        background = Brushes.Blue;
                        foreground = Brushes.White;
                        break;
                    case "3":
                        background = Brushes.LightBlue;
                        foreground = Brushes.White;
                        break;
                    case "Table":
                        background = Brushes.Green;
                        foreground = Brushes.White;
                        break;
                    default:
                        background = Brushes.White;
                        foreground = Brushes.Black;
                        break;
                }
            }
            else
            {
                background = Brushes.DarkGray;
                foreground = Brushes.White; ;
            }

            foreach (TextBlock child in grid.Children)
            {
                child.Background = background;
                child.Foreground = foreground;
            }
        }
        #endregion
        #region /// \name buttons

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_LoadFile_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_LoadFile for click events.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_LoadFile_Click(object sender, RoutedEventArgs e)
        {
            wordDocumentHolder.CloseAllWordDocuments();
            Load(false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_RemoveParagraph_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_RemoveParagraph for click events.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_RemoveParagraph_Click(object sender, RoutedEventArgs e)
        {
            // Check the index
            if (!CheckParagraphIndexValid())
            {
                return;
            }

            //Prompt for one or many paragraphs to remove
            MessageBoxResult result = CustomizedMessageBox.Show("Do you want to remove all paragraphs with the same style ?",
                "Document Creator Message", MessageBoxButton.YesNo, Icons.Question);

            if (result == MessageBoxResult.No)
            {
                //One remove
                DockPanel_Paragraphs.Children[selectedParagraphIndex].Visibility = System.Windows.Visibility.Collapsed;
                CustomizedMessageBox.Show("Removed paragraph with index : " + selectedParagraphIndex.ToString(),
                    "Document Creator Message", Icons.Success);
            }
            else
            {
                //remove all items with the same style
                Mouse.OverrideCursor = Cursors.Wait;
                string styleId = GetParagraphStyleId(selectedParagraphIndex);
                for (int idx = 0; idx < DockPanel_Paragraphs.Children.Count; idx++)
                {
                    if (styleId == GetParagraphStyleId(idx))
                    {
                        ((Grid)(DockPanel_Paragraphs.Children[idx])).Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                Mouse.OverrideCursor = null;
                CustomizedMessageBox.Show("Removed all paragraphs with StyleId : " + styleId,
                    "Document Creator Message", Icons.Success);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_LeaveSelected_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_LeaveSelected for click events.
        ///
        /// \par Description.
        ///      Remove all paragraphs except for the indexed paragraph
        ///      (To be used for easily creating PseudoCode files)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 17/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_LeavePseudoCode_Click(object sender, RoutedEventArgs e)
        {
            // Check the index
            if (!CheckParagraphIndexValid())
            {
                return;
            }

            //Prompt for one or many paragraphs to remove
            MessageBoxResult result = CustomizedMessageBox.Show("This action will leave only the selected paragraph. \nDo you want to continue ?",
                "Document Creator Message", MessageBoxButton.YesNo, Icons.Question);

            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                selectedParagraphIndex++;
                int idx = 0;
                for (; idx < selectedParagraphIndex; idx++)
                {
                    ((Grid)(DockPanel_Paragraphs.Children[idx])).Visibility = System.Windows.Visibility.Collapsed;
                }

                for (idx++; GetParagraphStyleId(idx) == "Table"; idx++) ;

                //remove all items with the same style
                for (; idx < DockPanel_Paragraphs.Children.Count; idx++)
                {
                    ((Grid)(DockPanel_Paragraphs.Children[idx])).Visibility = System.Windows.Visibility.Collapsed;
                }
                Mouse.OverrideCursor = null;

                CustomizedMessageBox.Show("Removed all paragraphs except : " + selectedParagraphIndex.ToString(),
                "Document Creator Message", Icons.Success);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_ViewResults_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_ViewResults for click events.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_ViewResults_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            // Close the temporary file if exists
            tempFileName = GenTempFileName(); 
            tempFilePath = processedFilePath;
            wordDocumentHolder.CloseWordDocument(tempFileName);

            // Update the file
            UpdateTargetFile(tempFilePath, tempFileName);

            // Open the temp document
            wordDocumentHolder.OpenWordDocument(WordDocumentHolder.FileTypes.Temp, tempFilePath, tempFileName);
            UpdateTOC(tempFileName);

            ShowFiles();
            Mouse.OverrideCursor = null;
            CustomizedMessageBox.FileMsg("The results are presented if file :", tempFileName, tempFilePath, "",
                "Document Creator Message");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_IncludeContent_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_IncludeContent for click events.
        ///
        /// \brief #### Description.
        ///        Includes all contents of the paragraph selected
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_IncludeContent_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ParagraphWithContentAction(Visibility.Visible);
            Mouse.OverrideCursor = null;

            CustomizedMessageBox.Show("Include contents of paragraph index " + selectedParagraphIndex.ToString() + " Ended",
                "Document Creator Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Save_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Save for click events.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            UpdateTargetFile(processedFilePath, processedFileName);
            wordDocumentHolder.OpenWordDocument(WordDocumentHolder.FileTypes.Document, processedFilePath, processedFileName);
            UpdateTOC(processedFileName);
            Mouse.OverrideCursor = null;
            CustomizedMessageBox.FileMsg("The document was saved to :", processedFileName, processedFilePath, "", "Document Creator Message", null, Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_PseudoCode_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_PseudoCode for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///      -# Check that the selected grid is a table
        ///      -# Close the source and pseudo code files
        ///      -# Get a cloned version of all the elements in the paragraph of the table
        ///         (In order to collect pseudo code that is composed from several tables
        ///      -# Copy an empty word document to the pseudo code file
        ///      -# Insert the cloned elements to the new document
        ///      -# Open the source file and the pseudo code file  
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/02/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_PseudoCode_Click(object sender, RoutedEventArgs e)
        {
            if (!PointerOnTable())
            {
                return;
            }
            Mouse.OverrideCursor = Cursors.Wait;

            // Generate the file names close the files and copy the empty document to the pseudo code file
            pseudoCodeFileName = GenPseudoCodeFileName();
            pseudoCodeFilePath = Config.Instance.GeneratePeudoCodePath(subject, algorithm);
            string emptyFileName = Config.Instance[Config.Keys.EmptyDocFileName];
            string emptyFilePath = Config.Instance[Config.Keys.EmptyDocFilePath]; 
            wordDocumentHolder.CloseWordDocument(sourceFileName);
            wordDocumentHolder.CloseWordDocument(pseudoCodeFileName);
            if (!FileUtilities.ReplaceFile(emptyFilePath + emptyFileName, pseudoCodeFilePath + pseudoCodeFileName))
            {
                Mouse.OverrideCursor = null;
                return;
            }

            // Insert the cloned elements to the pseudo code file
            List<OpenXmlElement> elements = CreateClonedElements();   
            using (WordprocessingDocument fileDocument = Open(pseudoCodeFilePath + pseudoCodeFileName, true))
            {                
                foreach (OpenXmlElement element in elements)
                {
                    fileDocument.MainDocumentPart.Document.Body.Append(element);
                }
            }

            // Open the documents for viewing
            wordDocumentHolder.OpenWordDocument(WordDocumentHolder.FileTypes.Source, sourceFilePath, sourceFileName);
            wordDocumentHolder.OpenWordDocument(WordDocumentHolder.FileTypes.PseudoCode, pseudoCodeFilePath, pseudoCodeFileName);
            ShowFiles();
            Mouse.OverrideCursor = null;
            CustomizedMessageBox.FileMsg("The pseudoCode document was saved to :", pseudoCodeFileName, pseudoCodeFilePath, "", "Document Creator Message", null, Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool PointerOnTable()
        ///
        /// \brief Determines if we can pointer on table.
        ///
        /// \par Description.
        ///      Check if the selection is on a table
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/02/2018
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool PointerOnTable()
        {
            if (selectedParagraphIndex >= 0)
            {
                if (GetParagraphStyleId(selectedParagraphIndex) == "Table")
                {
                    return true;
                }
            }
            CustomizedMessageBox.Show("The cursor has to point a table", "DocumentHandler Message", Icons.Error);
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn List<OpenXmlElement> CreateClonedElements()
        ///
        /// \brief Creates cloned elements.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/02/2018
        ///
        /// \return The new cloned elements.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        List<OpenXmlElement> CreateClonedElements()
        {
            List<OpenXmlElement> elements = CollectElementsOfParagraph();
            List<OpenXmlElement> result = new List<OpenXmlElement>();
            foreach (OpenXmlElement element in elements)
            {
                result.Add((OpenXmlElement)element.Clone());
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn List<OpenXmlElement> CollectElementsOfParagraph()
        ///
        /// \brief Collect elements of paragraph.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/02/2018
        ///
        /// \return A List&lt;OpenXmlElement&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        List<OpenXmlElement> CollectElementsOfParagraph()
        {

            // Get the text of the table
            string text = ((DockPanel_Paragraphs.Children[selectedParagraphIndex] as Grid)
                .Children[(int)ControlsInPanel.ParagraphText] as TextBlock).Text;

            // get the elements from the source file 
            List<OpenXmlElement> elements;
            using (WordprocessingDocument fileDocument = Open(sourceFilePath + sourceFileName, false))
            {
                elements = fileDocument.MainDocumentPart.Document.Body.Elements<OpenXmlElement>().ToList();
            }

            // Find the Table in the elements of the source file
            int idx;
            for (idx = 0; idx < elements.Count; idx++)
            {
                if (elements[idx].InnerText == text)
                {
                    break;
                }
            }

            // Find the header of the table
            int headerLevel = 0;
            while (headerLevel == 0)
            {
                idx--;
                int.TryParse(GetParagraphStyleId(elements[idx]), out headerLevel);
            }

            // Find all the elements to the end of the paragraph
            List<OpenXmlElement> result = new List<OpenXmlElement>();
            int level;
            while (true)
            {
                idx++;

                if (int.TryParse(GetParagraphStyleId(elements[idx]), out level))
                    if (level <= headerLevel)
                        break;
                result.Add(elements[idx]);
            }

            return result;
        }

 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Reset_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Reset for click events.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
             Mouse.OverrideCursor = Cursors.Wait;
            for (int idx = 0; idx < DockPanel_Paragraphs.Children.Count; idx++)
            {
                DockPanel_Paragraphs.Children[idx].Visibility = Visibility.Visible;
            }
            Mouse.OverrideCursor = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Exit_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Exit for click events.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            // Warning for not saved changes
            if (CustomizedMessageBox.Show("If changes where made but not saved they will be discarded \n Do you want to exit? ?",
                "Document Creator Message", MessageBoxButton.YesNo, Icons.Question) == MessageBoxResult.No)
            {
                return;
            }

            // Close word documents
            if (wordDocumentHolder.ApplicationExists())
            {
                // Quit if the word application was not closed
                try
                {
                    wordDocumentHolder.QuitApplication();
                }
                catch { }
            }

            // Update config
            Config.Instance.AddAlgorithmsData(false);
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_RemoveParagraphWithContent_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_RemoveParagraphWithContent for click events.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_RemoveParagraphWithContent_Click(object sender, RoutedEventArgs e)
        {
            // Check the index
            if (!CheckParagraphIndexValid())
            {
                return;
            }
            Mouse.OverrideCursor = Cursors.Wait;
            ParagraphWithContentAction(Visibility.Collapsed);
            Mouse.OverrideCursor = null;
            CustomizedMessageBox.Show("Exclude contents of paragraph index " + selectedParagraphIndex.ToString() + " Ended",
                "Document Creator Message", Icons.Success);

        }
        #endregion
        #region /// \name utilities

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string GetParagraphStyleId(int paragraphIndex)
        ///
        /// \brief Gets paragraph style identifier.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param paragraphIndex  (int) - Zero-based index of the paragraph.
        ///
        /// \return The paragraph style identifier.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GetParagraphStyleId(int paragraphIndex)
        {
            return ((TextBlock)((Grid)(DockPanel_Paragraphs.Children[paragraphIndex])).Children[(int)ControlsInPanel.StyleId]).Text;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool CheckParagraphIndexValid()
        ///
        /// \brief Determines if we can check paragraph index valid.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CheckParagraphIndexValid()
        {
            if (!(selectedParagraphIndex >= 0 && selectedParagraphIndex < DockPanel_Paragraphs.Children.Count))
            {
                CustomizedMessageBox.Show("Paragraph index has to be between 0 and " +
                    DockPanel_Paragraphs.Children.Count.ToString() + " Select Paragraph First",
                    "Document Creator Message", Icons.Error);
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ParagraphWithContentAction(Visibility visibility)
        ///
        /// \brief Paragraph with content action.
        ///
        /// \brief #### Description.
        ///        -    Set the contents of a paragraph visible or collapsed  
        ///        -    The following is the behavior of the method  
        ///             -#  If the selected paragraph is not a header (Style cannot be parsed to int) - set only
        ///                 the selected paragraph
        ///             -#  If the selected paragraph is a header (Style can be parsed to int) - set all
        ///                 the paragraph until a paragraph with the save level or higher level 
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param visibility  (Visibility) - The visibility.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ParagraphWithContentAction(Visibility visibility)
        {
            int mainStyleId;
            try
            {

                mainStyleId = int.Parse(GetParagraphStyleId(selectedParagraphIndex));
            }
            catch
            {
                // If the paragraph is not a header - delete only the paragraph and return
                ((Grid)(DockPanel_Paragraphs.Children[selectedParagraphIndex])).Visibility = visibility;
                return;
            }

            // If the paragraph is a header set all the paragraphs while not a header or lower header
            int paragraphIndex = selectedParagraphIndex;
            int styleId;
            do
            {
                ((Grid)(DockPanel_Paragraphs.Children[paragraphIndex])).Visibility = visibility;
                paragraphIndex++;
                try
                {
                    styleId = int.Parse(GetParagraphStyleId(paragraphIndex));
                }
                catch
                {
                    styleId = 1000;
                }
            } while (paragraphIndex < DockPanel_Paragraphs.Children.Count &&
            styleId > mainStyleId &&
            ((Grid)(DockPanel_Paragraphs.Children[paragraphIndex])).Visibility != visibility);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private WordprocessingDocument Open(string fileName, bool editable)
        ///
        /// \brief Opens the given file.
        ///
        /// \par Description.
        ///      Opens WordProcessingDocument
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/02/2018
        ///
        /// \param fileName   (string) - Filename of the file.
        /// \param editable  (bool) - true if editable.
        ///
        /// \return A WordprocessingDocument.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private WordprocessingDocument Open(string fileName, bool editable)
        {
            while (true)
            {
                try
                {
                    return WordprocessingDocument.Open(fileName, true);
                }
                catch (Exception e)
                {
                    string result = CustomizedMessageBox.FileMsgErr("Error while opening file",
                        Path.GetFileName(fileName),
                        Path.GetDirectoryName(fileName),
                        e.Message,
                        "Document Creator Message",
                        new List<string> { "Retry", "Cancel" });

                    if (result == "Cancel")
                    {
                        return null;
                    }
                }
            }
        }
        #endregion
        #region /// \name destination and temp file generating

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateTargetFile(string filePath, string fileName)
        ///
        /// \brief Updates the target file.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param filePath  (string) - Full pathname of the file.
        /// \param fileName  (string) - Filename of the file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateTargetFile(string filePath, string fileName)
        {
            // Copy the source to the destination
            wordDocumentHolder.CloseWordDocument(fileName);
            if (!FileUtilities.ReplaceFile(Path.Combine(sourceFilePath, sourceFileName), filePath + fileName))
            {
                return;
            }

            // Open the destination
            using (WordprocessingDocument fileDocument = Open(filePath + fileName, true))
            {

                // Read all the paragraphs in the destination to a list
                MainDocumentPart mainPart = fileDocument.MainDocumentPart;
                Document doc = fileDocument.MainDocumentPart.Document;
                List<OpenXmlElement> tmpFileParagraphs = fileDocument.MainDocumentPart.Document.Body.Elements<OpenXmlElement>().ToList();
                
                foreach (Grid grid in DockPanel_Paragraphs.Children)
                {
                    int idx = int.Parse(((TextBlock)grid.Children[(int)ControlsInPanel.Idx]).Text);
                    if (grid.Visibility == Visibility.Collapsed)
                    {
                        tmpFileParagraphs[idx].Remove();
                    }
                }

                // The file is saved when exit the using
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateTOC(string fileName)
        ///
        /// \brief Updates the TOC described by fileName.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2017
        ///
        /// \param fileName  (string) - Filename of the file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateTOC(string fileName)
        {
            Microsoft.Office.Interop.Word.Document wordDocument = wordDocumentHolder.RetrieveOpenedDocument(fileName);
            try
            {
                wordDocument.TablesOfContents[1].Update();
            }
            catch
            {
                Microsoft.Office.Interop.Word.Range range = wordDocument.Range(0, 0);
                wordDocument.TablesOfContents.Add(range);
            }
            wordDocument.Save();
        }
        #endregion
    }
}
