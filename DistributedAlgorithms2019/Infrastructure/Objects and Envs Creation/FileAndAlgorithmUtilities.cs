////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\Utilities.cs
///
///\brief   Implements the utilities class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class FileUtilities
    ///
    /// \brief A file utilities.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 10/12/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class FileUtilities
    {
        #region /// \name File Dialogs


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool SelectOutputFile(string fileType, ref string fileName, ref string path)
        ///
        /// \brief Select output file.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param          fileType  (string) - Type of the file.
        /// \param [in,out] fileName (ref string) - Filename of the file.
        /// \param [in,out] path     (ref string) - Full pathname of the file.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool SelectOutputFile(string fileType,
            ref string fileName,
            ref string path)
        {
            //Get the data file name and path
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.CheckFileExists = false;
            fileDialog.CheckPathExists = true;
            fileDialog.Filter = fileType + " Files (." + fileType + ")|*." + fileType;
            fileDialog.FileName = fileName;
            fileDialog.InitialDirectory = System.IO.Path.GetFullPath(path);
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "Select " + fileType + " file";
            if (fileDialog.ShowDialog() == true)
            {
                fileName = System.IO.Path.GetFileName(fileDialog.FileName);
                path = RelativePath(Directory.GetCurrentDirectory(), Path.GetDirectoryName(fileDialog.FileName));
                return true;
            }
            else
            {
                CustomizedMessageBox.Show("File Selection canceled", "Select File Dialog", null, Icons.Error);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool SelectInputFile(string fileType, ref string fileName, ref string path, bool checkFileExist = true)
        ///
        /// \brief Select input file.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param          fileType        (string) - Type of the file.
        /// \param [in,out] fileName       (ref string) - Filename of the file.
        /// \param [in,out] path           (ref string) - Full pathname of the file.
        /// \param          checkFileExist (Optional)  (bool) - true to check file exist.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool SelectInputFile(string fileType, ref string fileName, ref string path, bool checkFileExist = true, string title = null)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.CheckFileExists = checkFileExist;
            fileDialog.CheckPathExists = true;
            //fileDialog.RestoreDirectory = true;
            fileDialog.FileName = fileName;
            fileDialog.InitialDirectory = System.IO.Path.GetFullPath(path);
            if (title is null)
            {
                fileDialog.Title = "Select " + fileType + " source file";
            }
            else
            {
                fileDialog.Title = title;
            }
            fileDialog.Filter = fileType + " Files (." + fileType + "|*." + fileType;
            if (fileDialog.ShowDialog() == true)
            {
                fileName = fileDialog.SafeFileName;
                path = RelativePath(Directory.GetCurrentDirectory(), Path.GetDirectoryName(fileDialog.FileName));
                return true;
            }
            else
            {
                CustomizedMessageBox.Show("File Selection canceled", "Select File Dialog", null, Icons.Error);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool SelectFolder(bool allowCreateNewFolder, string description, ref string folderName)
        ///
        /// \brief Select folder.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param          allowCreateNewFolder  (bool) - true to allow, false to deny create new folder.
        /// \param          description           (string) - The description.
        /// \param [in,out] folderName           (ref string) - Pathname of the folder.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool SelectFolder(bool allowCreateNewFolder, string description, ref string folderName)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = description;
            folderDialog.SelectedPath = System.IO.Path.GetFullPath(folderName);
            folderDialog.ShowNewFolderButton = allowCreateNewFolder;
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                folderName = RelativePath(Directory.GetCurrentDirectory(), folderDialog.SelectedPath);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
        #region /// \name Directories Utilities

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static List<string> GetSubDirectories(string directory)
        ///
        /// \brief Gets sub directories.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param directory  (string) - Pathname of the directory.
        ///
        /// \return The sub directories.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<string> GetSubDirectories(string directory)
        {
            return Directory.GetDirectories(directory).ToList().Select(dir => RelativePath(Directory.GetCurrentDirectory(), dir)).ToList();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static AttributeList GetAllFileNames(string directory, string fileExtention)
        ///
        /// \brief Gets all file names.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param directory      (string) - Pathname of the directory.
        /// \param fileExtention  (string) - The file extension.
        ///
        /// \return all file names.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static AttributeList GetAllFileNames(string directory, string fileExtension)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                List<string> lst = info.GetFiles().Where(p => !p.Attributes.HasFlag(FileAttributes.Hidden)).
                    OrderByDescending(p => p.CreationTime).
                    Select(fileInfo => fileInfo.Name).
                    Where(fileName => fileName.Contains("." + fileExtension)).
                    Where(fileName => !(fileName.Contains("__temp__.docx"))).ToList();
                return lst.ToAttributeList();
            }
            catch
            {
                CreateDirectory(directory);
                return new AttributeList();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void CopyDirFiles(string sourcePath, string destPath)
        ///
        /// \brief Copies the directory files.
        ///
        /// \par Description.
        ///      Copy all files from source folder to a destination folder
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param sourcePath  (string) - Full pathname of the source file.
        /// \param destPath    (string) - Full pathname of the destination file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void CopyDirFiles(string sourcePath, string destPath)
        {
            foreach (string fullFileName in Directory.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly))
            {
                string text = File.ReadAllText(fullFileName);
                File.WriteAllText(fullFileName.Replace(sourcePath, destPath), text);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string RelativePath(string fromPath, string toPath)
        ///
        /// \brief Relative path.
        ///
        /// \par Description.
        ///      Find relative path from source to destination
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param fromPath  (string) - Full pathname of from file.
        /// \param toPath    (string) - Full pathname of to file.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string RelativePath(string fromPath, string toPath)
        {
            string result = "";
            int idx = 0;
            string[] fromPathDirs = Regex.Split(fromPath, @":|\\");
            string[] toPathDirs = Regex.Split(toPath, @":|\\");

            //If the paths are not from the same disk return the toPath
            if (fromPath[0] != toPath[0])
            {
                return toPath;
            }

            //Replace the common path start with ~
            for (idx = 1; idx < Math.Min(fromPathDirs.Length, toPathDirs.Length); idx++)
            {
                if (fromPathDirs[idx] != toPathDirs[idx])
                {
                    break;
                }
            }

            result += "~\\";
            int commonPathLastIdx = idx;

            //ForEach directory left in the fromPath add "../"
            for (idx = commonPathLastIdx - 1; idx < fromPathDirs.Length; idx++)
            {
                result += "..\\";
            }

            //Add the rest of the toPath
            for (idx = commonPathLastIdx; idx < toPathDirs.Length; idx++)
            {
                result += toPathDirs[idx];
                result += "\\";
            }

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string GetPathToData(string currentPath)
        ///
        /// \brief Gets path to data.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param currentPath  (string) - Full pathname of the current file.
        ///
        /// \return The path to data.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetPathToData(string currentPath)
        {
            string[] fromPathDirs = Regex.Split(currentPath, @":|\\");
            string result = "";
            for (int idx = 0; idx < fromPathDirs.Length - 2; idx++)
            {
                result += fromPathDirs[idx];
                if (idx == 0)
                {
                    result += @":";
                }
                else
                {
                    result += @"\";
                }

            }
            result += @"Network data\";
            if (!Directory.Exists(result))
            {
                result = currentPath;
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void CreateDirectory(string path)
        ///
        /// \brief Creates a directory.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param path  (string) - Full pathname of the file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        #endregion
        #region /// \name Data Directories handeling support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void CreateDefaultDirsForNeteork(string subjectAndAlgorith, string algorithmsDataPath)
        ///
        /// \brief Creates default directories for network.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param subjectAndAlgorith  (string) - The subject and algorithm.
        /// \param algorithmsDataPath  (string) - Full pathname of the algorithms data file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void CreateDefaultDirsForNetwork(string subjectAndAlgorithm, string algorithmsDataPath)
        {
            string subject = subjectAndAlgorithm.Substring(0, subjectAndAlgorithm.IndexOf("."));
            string algorithm = subjectAndAlgorithm.Replace(subject + ".", "");
            CreateDirsForNeteork(subject, algorithm, algorithm, algorithmsDataPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void CreateDirsForNeteork(string subject = null, string algorithm = null, string network = null, string algorithmsDataPath = null)
        ///
        /// \brief Creates directories for network.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param subject            (Optional)  (string) - The subject.
        /// \param algorithm          (Optional)  (string) - The algorithm.
        /// \param network            (Optional)  (string) - The network.
        /// \param algorithmsDataPath (Optional)  (string) - Full pathname of the algorithms data file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void CreateDirsForNeteork(string subject = null, string algorithm = null, string network = null, string algorithmsDataPath = null)
        {
            // Create directory for the subject if not exists
            if (algorithmsDataPath is null)
            {
                algorithmsDataPath = Config.Instance[Config.Keys.AlgorithmsDataPath];
            }

            // Create directory for the subject if not exists
            if (subject is null)
            {
                subject = Config.Instance[Config.Keys.SelectedSubject];
            }
            if (!Directory.Exists(Path.GetFullPath(algorithmsDataPath + "\\" + subject)))
            {
                CreateDirectory(Path.GetFullPath(algorithmsDataPath + "\\" + subject));
            }

            // Create directory for the algorithm if not exists
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            string algorithmPath = algorithmsDataPath + subject + "\\" + algorithm;
            if (!Directory.Exists(Path.GetFullPath(algorithmPath)))
            {
                DirsForAlgorithm(algorithmPath);
            }

            // If the directory for the network exist - return
            if (network is null)
            {
                network = Config.Instance[Config.Keys.SelectedNetwork];
            }
            string networkPath = algorithmPath + "\\Networks\\" + network;
            if (!Directory.Exists(Path.GetFullPath(networkPath)))
            {
                DirsForNetwork(networkPath);
            }           
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static void DirsForAlgorithm(string algorithmPath)
        ///
        /// \brief Directories for algorithm.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param algorithmPath  (string) - Full pathname of the algorithm file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void DirsForAlgorithm(string algorithmPath)
        {
            CreateDirectory(Path.GetFullPath(algorithmPath));
            CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "Networks"));
            CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "Documents"));
            CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "Documents\\Source"));
            CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "Documents\\Processed"));
            CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "Documents\\PseudoCode"));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static void DirsForNetwork(string networkPath)
        ///
        /// \brief Directories for network.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param networkPath  (string) - Full pathname of the network file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void DirsForNetwork(string networkPath)
        {
            CreateDirectory(Path.GetFullPath(networkPath));
            CreateDirectory(Path.GetFullPath(networkPath + "\\" + "Data"));
            CreateDirectory(Path.GetFullPath(networkPath + "\\" + "Logs"));
            CreateDirectory(Path.GetFullPath(networkPath + "\\" + "Debug"));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void RemoveAlgorithmDataDirectory(string algorithmName, string algorithmsDataPath)
        ///
        /// \brief Removes the algorithm data directory.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param algorithmName       (string) - Name of the algorithm.
        /// \param algorithmsDataPath  (string) - Full pathname of the algorithms data file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RemoveAlgorithmDataDirectory(string algorithmName, string algorithmsDataPath)
        {
            // Get the subject and the algorithm names
            string subject = algorithmName.Substring(0, algorithmName.IndexOf("."));
            string algorithm = algorithmName.Replace(subject + ".", "");

            // Delete the algorithm data directory
            string algorithmPath = algorithmsDataPath + subject + "\\" + algorithm;
            Directory.Delete(algorithmPath, true);

            // Delete the subject directory if it is empty
            string subjectPath = algorithmsDataPath + subject;
            int numOfAlgorithmsInSubject = Directory.GetDirectories(subjectPath, "*", SearchOption.TopDirectoryOnly).Length;
            if (numOfAlgorithmsInSubject == 0)
            {
                Directory.Delete(subjectPath, true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void RemoveEmpty(bool confirmNeeded, string subject, string algorithm )
        ///
        /// \brief Removes the empty.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 13/12/2017
        ///
        /// \param confirmNeeded  (bool) - true if confirm needed.
        /// \param subject        (string) - The subject.
        /// \param algorithm      (string) - The algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RemoveDataDir(bool confirmNeeded, string subject, string algorithm )
        {
            // If the subject is empty - remove the subject
            string subjectPath = Config.Instance.GenerateSubjectPath(subject);
            if (!HasFiles(subjectPath))
            {
                if (CustomizedMessageBox.Show(new List<string> { "The subject DATA directories are empty. Do you want to delete them?" },
                    "FileUtils Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(subjectPath, true);
                    return;
                }
            }

            string algorithmPath = Config.Instance.GenerateAlgorithmPath(subject, algorithm);
            if (!HasFiles(algorithmPath))
            {
                if (CustomizedMessageBox.Show(new List<string> { "The algorithm DATA directories are empty. Do you want to delete them?" },
                    "FileUtils Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(algorithmPath);
                    return;
                }
            }
        }
        #endregion
        #region /// \name Code creation support
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void ReplaceAlgorithmName(string className, string newAlgorithmName, string subjectName)
        ///
        /// \brief Replace algorithm name.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param className         (string) - Name of the class.
        /// \param newAlgorithmName  (string) - Name of the new algorithm.
        /// \param subjectName       (string) - Name of the subject.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ReplaceAlgorithmName(string className, string newAlgorithmName, string subjectName)
        {
            string text = File.ReadAllText(Config.Instance[Config.Keys.AlgorithmsPath] + "\\System\\Template\\Template" + className + ".cs");
            text = text.Replace("System.Template", subjectName + "." + newAlgorithmName);
            text = text.Replace("Template", newAlgorithmName);
            File.WriteAllText(Config.Instance[Config.Keys.AlgorithmsPath] + "\\" + subjectName + "\\" + newAlgorithmName + "\\" + newAlgorithmName + className + ".cs", text);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool ReplaceFile(string sourceFileName, string destFileName)
        ///
        /// \brief Replace file.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param sourceFileName  (string) - Filename of the source file.
        /// \param destFileName    (string) - Filename of the destination file.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool ReplaceFile(string sourceFileName, string destFileName)
        {
            sourceFileName = System.IO.Path.GetFullPath(sourceFileName);
            destFileName = System.IO.Path.GetFullPath(destFileName);

            //Delete the destination file
            if (File.Exists(destFileName))
            {

                while (true)
                {
                    try
                    {
                        File.Delete(destFileName);
                        break;
                    }
                    catch (Exception e)
                    {
                        string result = CustomizedMessageBox.FileMsgErr("Error while deleting destination file",
                            Path.GetFileName(destFileName),
                            Path.GetDirectoryName(destFileName),
                            e.Message,
                            "FilesAndAlgorithmUtilities",
                            new List<string> { "Retry", "Cancel" });

                        if (result == "Cancel")
                        {
                            return false;
                        }
                    }
                }
            }

                
            //Copy the source file to the destination file
            File.Copy(sourceFileName, destFileName, true);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void CreateCodeDir(string subject, string algorithm)
        ///
        /// \brief Creates code dir.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 13/12/2017
        ///
        /// \param subject    (string) - The subject.
        /// \param algorithm  (string) - The algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void CreateCodeDir(string subject, string algorithm)
        {
            string subjectPath = ClassFactory.GenerateSubjectPath(subject);
            if (!Directory.Exists(subjectPath))
            {
                CreateDirectory(Path.GetFullPath(subjectPath));
            }

            string algorithmPath = ClassFactory.GenerateAlgorithmPath(subject, algorithm);
            if (!Directory.Exists(algorithmPath))
            {
                CreateDirectory(Path.GetFullPath(algorithmPath));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void RemoveCodeDir(bool confirmNeeded, string subject, string algorithm)
        ///
        /// \brief Removes the code dir.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 13/12/2017
        ///
        /// \param confirmNeeded  (bool) - true if confirm needed.
        /// \param subject        (string) - The subject.
        /// \param algorithm      (string) - The algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RemoveCodeDir(bool confirmNeeded, string subject, string algorithm)
        {
            string subjectPath = ClassFactory.GenerateSubjectPath(subject);
            if (!HasFiles(subjectPath))
            {
                if (CustomizedMessageBox.Show(new List<string> { "The subject CODE directories are empty. Do you want to delete them?" },
                    "FileUtils Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(subjectPath, true);
                    return;
                }
            }

            string algorithmPath = ClassFactory.GenerateAlgorithmPath(subject, algorithm);
            if (!HasFiles(algorithmPath))
            {
                if (CustomizedMessageBox.Show(new List<string> { "The algorithm CODE directories are empty. Do you want to delete them?" },
                    "FileUtils Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(algorithmPath);
                    return;
                }
            }
        }

        public static void RemoveCodeAndDataDirs(bool confirmNeeded, string subject, string algorithm)
        {
            string subjectCodePath = ClassFactory.GenerateSubjectPath(subject);
            string subjectDataPath = Config.Instance.GenerateSubjectPath(subject);
            if (!(HasFiles(subjectCodePath) || HasFiles(subjectDataPath)))
            {
                if (CustomizedMessageBox.Show(new List<string> { "The subject CODE and DATA directories are empty. Do you want to delete them?" },
                    "FileUtils Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(subjectCodePath, true);
                    Directory.Delete(subjectDataPath, true);
                    return;
                }
            }

            string algorithmCodePath = ClassFactory.GenerateAlgorithmPath(subject, algorithm);
            string algorithmDataPath = Config.Instance.GenerateAlgorithmPath(subject, algorithm);
            if (!(HasFiles(algorithmCodePath) || HasFiles(algorithmDataPath)))
            {
                if (CustomizedMessageBox.Show(new List<string> { "The algorithm CODE and DATA directories are empty. Do you want to delete them?" },
                    "FileUtils Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(algorithmCodePath, true);
                    Directory.Delete(algorithmDataPath, true);
                    return;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static bool HasFiles(string rootDir)
        ///
        /// \brief Query if 'rootDir' has files.
        ///
        /// \par Description.
        ///      A recursive method that checks if there are no files in a directory tree
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 13/12/2017
        ///
        /// \param rootDir  (string) - The root dir.
        ///
        /// \return True if files, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool HasFiles(string rootDir)
        {
            // First Check if the directory has files - if it has return true
            string[] rootFiles = Directory.GetFiles(rootDir);
            if (rootFiles.Length > 0) return true;
            
            // Second Check if the sub directories has files - if one of them has - return true
            string[] nodeDirs = Directory.GetDirectories(rootDir);
            foreach(string nodeDir in nodeDirs)
            {
                if (HasFiles(nodeDir))
                {
                    return true;
                }
            }

            // If there are no files in the root and it subs return false
            return false;
        }
        #endregion
    }
}
