////////////////////////////////////////////////////////////////////////////////////////////////////
using DistributedAlgorithms.Algorithms.Base.Base;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class MainWindow
    ///
    /// \brief The application's main form.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 19/12/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class MainWindow : Window
    {
        #region /// \name Enums

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ActivationPhases
        ///
        /// \brief Values that represent activation phases.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ActivationPhases { Temp, Init, Initiated, Loaded, Checked, Created, BeforeRunning, Running, AfterRunning }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum SelectedStatus
        ///
        /// \brief Values that represent selected status.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum SelectedStatus { Selected, NotSelected, SelectedForChannel, DoNotShow, RunningNextProcess, RunningNotNextProcess }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ChannelAddState
        ///
        /// \brief Values that represent channel add states.The channel add status is composed from activating
        ///        the same method in phases. This enum is the phases
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ChannelAddState { NotInProcess, BeforeFirstProcessSelection, BeforeSecondProcessSelection, ProcessSelectionFinished }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum LastInitiationAction
        ///
        /// \brief Values that represent last initiation actions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private enum LastInitiationAction { Clear, Init, Load, Debug }
        #endregion
        #region /// \name Member Variables
        #region /// \name General handling of the program

        /// \brief The net.
        public BaseNetwork net = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (static ActivationPhases) - The activation phase.
        ///        The activation phase of the program.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static ActivationPhases activationPhase = ActivationPhases.Init;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public static ActivationPhases ActivationPhase
        ///
        /// \brief Gets the activation phase.
        ///
        /// \return The activation phase.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ActivationPhases ActivationPhase { get { return activationPhase; } }


        /// \brief true if network was changed.
        public bool networkWasChanged = false;
        
        /// \brief The selected process.
        public BaseProcess SelectedProcess;

        /// \brief The selected channel.
        public BaseChannel SelectedChannel;
        #endregion
        #region /// \name Channel adding process members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (ChannelAddState) - State of the channel add.
        ///        The channel adding is implemented by calling the same method for all the phases
        ///        The behavior of the method changes according to the phase.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChannelAddState channelAddState = ChannelAddState.NotInProcess;

        /// \brief The add channel source process.
        private BaseProcess addChannel_sourceProcess;

        /// \brief The add channel destination process.
        private BaseProcess addChannel_destProcess;

        #endregion
        #region /// \name Members used to handle the presentation

        /// \brief true to show, false to hide the channels presentation.
        private bool showChannelsPresentation = false;

        /// \brief The last initiation action.
        private LastInitiationAction lastInitiationAction;

        /// \brief (bool) - true to show, false to hide the breakpoints in running mode.
        public bool showBreakpoints = true;

        /// \brief (bool) - true to show, false to hide the short description in running mode.
        public bool showFloatingSummary = true;

        /// \brief (bool) - true to show, false to hide the messages in running mode.
        public bool showMessages = true;

        #endregion
        #region

        /// \brief  (MessageWindow) - The message window.
        public MessageWindow messageWindow = null;

        /// \brief  (RunningHandeler) - The running handler.
        public RunningHandeler runningHandler = null;

        /// \brief (WordDocumentHolder) - The word document holder.
        private WordDocumentHolder wordDocumentHolder = new WordDocumentHolder();
        #endregion
        #region /// \name Members holding Handlers and windows

        /// \brief  (Label) - The edit operation label.
        private Label editOperationLabel;

        /// \brief  (string) - The edit operation empty spaces.
        private string editOperationEmptySpaces;

        /// \brief  (string) - The edit operation log empty spaces.
        private string editOperationLogEmptySpaces;
        #endregion
        #region
        /// \brief The check icons phase. - This parameter is used for a method that checks the icons.
        private int checkIconsPhase = 0;
        #endregion
        #endregion
        #region /// \name Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public MainWindow()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MainWindow()
        {
            // The algorithm is working on different threads from the MainWindow and the GUI
            // So the reports it gives are using Event handlers
            MessageRouter.ReportMessageEvent += (s, e) =>
            {
                Dispatcher.Invoke(delegate () { PrintDubugMessage((string)e[0], (string)e[1], (string)e[2]); });
            };

            MessageRouter.ReportFinishEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate() { UpdateEndRunningPresenation(); });
            };

            MessageRouter.ReportStartEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate() { UpdateInitRunningPresentation(); });
            };

            MessageRouter.ReportChangePresentationEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate() { ChangePresentationOfComponent((NetworkElement)s, (object[])e); });
            };

            MessageRouter.ReportMessageSentEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate() {if (ActivationPhase == ActivationPhases.Running) 
                    runningHandler.MessageSendReport((BaseProcess)s, (BaseMessage)((object[])e)[0]); });
            };

            MessageRouter.ReportMessageReceiveEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate() { if (ActivationPhase == ActivationPhases.Running)
                    runningHandler.MessageReceiveReport((BaseProcess)s, (BaseMessage)((object[])e)[0]); });
            };


            MessageRouter.CheckFinishProcessingStepEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate ()
                {
                    if (ActivationPhase == ActivationPhases.Running)
                        runningHandler.CheckFinishProcessingStep((BaseProcess)s);
                });
            };

            MessageRouter.UpdateInitRunningPresentationEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate () {
                    if (ActivationPhase == ActivationPhases.Running)
                        UpdateInitRunningPresentation();
                });
            };

            MessageRouter.UpdateRunningPresentationEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate () {
                    if (activationPhase == ActivationPhases.Running)
                        UpdateRunningPresentation();
                });
            };

            MessageRouter.AddEditOperationEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate () { AddEditOperation((string)e[0], (string)e[1], (Icons)e[2], (Font)e[3]); });
            };

            MessageRouter.AddEditOperationResultEvent += (s, e) =>
            {
                Dispatcher.Invoke((Action)delegate () { AddEditOperationResult((string)e[0], (Font)e[1]); });
            };

            MessageRouter.MessageBoxEvent += (s, e) =>
            {
               return  Dispatcher.Invoke(delegate () { return CustomizedMessageBox.Show((List<string>)e[0], (string)e[1], (List<string>)e[2], (Icons)e[3], (bool)e[4]); });
            };

            MessageRouter.CustomizedMessageBoxEvent += (s, e) =>
            {
                return Dispatcher.Invoke(delegate () { return CustomizedMessageBox.Show((List<MessageBoxElementData>)e[0], (string)e[1], (List<MessageBoxElementData>)e[2], (Icons)e[3], (bool)e[4]); });
            };


            // Program Initiation
            InitializeComponent();
            Test test = new Test();
            test.Button_EnumType_Click(null, null);

            // Load the config
            try
            {
                Config.Instance.LoadConfig();
            }
            catch (TypeInitializationException e)
            {
                MessageBox.Show(e.InnerException.Message);
            }

            // Open Edit log
            Logger.EditLogInit();

            // Load pre-activated network (name froud in the config)
            LoadPrevActivationNetwork();
        }
        #endregion /// \name Constructor
        #region /// \name File menu items handlers 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_ResetNetwork(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for reset network events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_ResetNetwork(object sender, RoutedEventArgs e)
        {
            foreach (BaseProcess process in net.Processes)
            {
                List<BaseChannel> channelsToDelete = net.CollectChannelsConnectedToProcess(process.ea[ne.eak.Id]);
                foreach (BaseChannel channel in channelsToDelete)
                {
                    DeleteChannel(channel);
                }
                process.Clear();
            }
            net.Processes.Clear();
            UpdatePresentation();
            activationPhase = ActivationPhases.Init;
            networkWasChanged = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_New(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for new events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_New(object sender, RoutedEventArgs e)
        {
            SaveQuestion();
            FileSelect fileSelect = new FileSelect(FileSelect.SelectSource.New, "Select Subject, Algorithm, Network and new Data file name");
            fileSelect.ShowDialog();

            switch (fileSelect.result)
            {
                case FileSelect.SelectResult.New:
                    NewNetwork();
                    break;
                case FileSelect.SelectResult.Open:
                    LoadNetwork();
                    break;
                default:
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_Open(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for open events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_Open(object sender, RoutedEventArgs e)
        {
            SaveQuestion();
            FileSelect fileSelect = new FileSelect(FileSelect.SelectSource.Open, "Select Subject, Algorithm, Network and existing Data file name");
            fileSelect.ShowDialog();

            switch (fileSelect.result)
            {
                case FileSelect.SelectResult.New:
                    NewNetwork();
                    break;
                case FileSelect.SelectResult.Open:
                    LoadNetwork();
                    break;
                default:
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_ReOpen(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for re open events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_ReOpen(object sender, ExecutedRoutedEventArgs e)
        {
            LoadNetwork();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_Save(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for save events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_Save(object sender, RoutedEventArgs e)
        {
            SaveNetwork();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_SaveAs(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for save as events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_SaveAs(object sender, RoutedEventArgs e)
        {
            FileSelect fileSelect = new FileSelect(FileSelect.SelectSource.SaveAs, "Select Subject, Algorithm, Network and new Data file name");
            fileSelect.ShowDialog();

            if (!(fileSelect.result == FileSelect.SelectResult.Quit))
            {
                SaveNetwork();
                UpdatePresentation();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_Debug(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for debug events.
        ///
        /// \par Description.
        ///      start a running from a debug file (start the running from the middle)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_Debug(object sender, RoutedEventArgs e)
        {
            FileSelect fileSelect = new FileSelect(FileSelect.SelectSource.Debug, "Select Debug file");
            fileSelect.ShowDialog();
            if (fileSelect.result == FileSelect.SelectResult.Quit)
            {
                return;
            }
            DebugNetwork();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_SetLogFile(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for set log file events.
        ///
        /// \par Description.
        ///      Set the log file name
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_SetLogFile(object sender, ExecutedRoutedEventArgs e)
        {
            FileSelect fileSelect = new FileSelect(FileSelect.SelectSource.Log, "Select Log file");
            fileSelect.ShowDialog();
            if (fileSelect.result == FileSelect.SelectResult.Quit)
            {
                return;
            }
            SetHeaderLabels();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_LogFiltersSetting(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for log filters setting events.
        ///
        /// \par Description.
        ///      Opens a dialog to handle the log filters
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_LogFiltersSetting(object sender, RoutedEventArgs e)
        {
            LogFiltersSetting logFiltersSetting = new LogFiltersSetting();
            logFiltersSetting.ShowDialog();
            networkWasChanged = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_Exit(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for exit events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_Exit(object sender, ExecutedRoutedEventArgs e)
        {
            Window_Close(null, null);
        }
        #endregion
        #region /// \name Methods used in the files handling commands

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SaveQuestion()
        ///
        /// \brief Saves the question.
        ///        When opening a new network this method is used to ask whether to save the previous network
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SaveQuestion()
        {
            if (networkWasChanged)
            {
                if (CustomizedMessageBox.Show("Do you want to save the current network?",
                    "MainWindow Message",
                    MessageBoxButton.YesNo,
                    Icons.Question) == MessageBoxResult.Yes)
                    SaveNetwork();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void LoadPrevActivationNetwork()
        ///
        /// \brief Loads previous activation network.
        ///
        /// \par Description.
        ///      This method is used when the program initiated to load the network that was used in the
        ///      previous operation
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void LoadPrevActivationNetwork()
        {
            if (!LoadNetwork())
            {
                Command_New(null, null);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void NewNetwork()
        ///
        /// \brief Creates a new network.
        ///
        /// \par Description.
        ///      This method is used to create a new network after selecting the network from the dialog
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void NewNetwork()
        {
            activationPhase = ActivationPhases.Init;
            try
            {
                net = ClassFactory.GenerateNetwork();
            }
            catch
            {
                CustomizedMessageBox.Show("The network cannot be loaded \n Probably it was not inserted to the Visual Studio project", "Main Window - Init Failed", Icons.Error);
            }
            activationPhase = ActivationPhases.Init;
            FileUtilities.CreateDirsForNeteork();
            Logger.Init();
            net.InitNetwork((int)Canvas_Draw.ActualWidth, (int)Canvas_Draw.ActualHeight);
            activationPhase = ActivationPhases.Initiated;
            networkWasChanged = true;
            lastInitiationAction = LastInitiationAction.Init;
            CreateAllPresentations();
            SetSelectedProcess(net.Processes[0]);
            SetSelectedChannel(net.Channels[0]);
            CustomizedMessageBox.Show("finished initiating", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool LoadNetwork()
        ///
        /// \brief Loads the network.
        ///
        /// \par Description.
        ///      This method is used to load a network after selecting the network from the dialog
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool LoadNetwork()
        {
            try
            {
                activationPhase = ActivationPhases.Init;
                net = ClassFactory.GenerateNetwork();
                net.Init(0);
                int buildVersion = net.GetVersion();
                Mouse.OverrideCursor = Cursors.Wait;
                net.LoadNetwork();
                UpdateNetworkIfNeeded(buildVersion);
                Mouse.OverrideCursor = null;
                networkWasChanged = false;
                CreateAllPresentations();
                lastInitiationAction = LastInitiationAction.Load;
                activationPhase = ActivationPhases.Loaded;
                UpdatePresentation();
                CustomizedMessageBox.FileMsg("The network was loaded from :",
                    Config.Instance[Config.Keys.SelectedDataFileName],
                    Config.Instance[Config.Keys.SelectedDataPath],
                    "",
                    "MainWindow Message", null, Icons.Success);
                return true;
            }
            catch (Exception e)
            {
                Mouse.OverrideCursor = null;
                CustomizedMessageBox.FileMsgErr("Failed to load the network from file :",
                    Config.Instance[Config.Keys.SelectedDataFileName],
                    Config.Instance[Config.Keys.SelectedDataPath],
                    e.Message,
                    "MainWindow Window Message");
                activationPhase = ActivationPhases.Init;
                lastInitiationAction = LastInitiationAction.Clear;
                ClearAllPresentations();
                net = null;
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateNetworkIfNeeded(int buildVersion)
        ///
        /// \brief Updates the network if needed described by buildVersion.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 03/01/2018
        ///
        /// \param buildVersion  (int) - The build version.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateNetworkIfNeeded(int buildVersion)
        {
            if (net.GetVersion() < buildVersion)
            {
                if (CustomizedMessageBox.Show(new List<string>
                { "The version of the network is : " + net.GetVersion().ToString() + " and it is smaller than the version of the code :" + buildVersion.ToString(),
                    "Do you eant to update the network ?" }, 
                    "MainWindow Message",  MessageBoxButton.YesNo, 
                    Icons.Question) == MessageBoxResult.Yes)
                {
                    NetworkUpdate networkUpdate = new NetworkUpdate();
                    net = networkUpdate.Update(net);
                }
                else
                {
                    return;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool DebugNetwork()
        ///
        /// \brief Determines if we can debug network.
        ///
        /// \par Description.
        ///      Debug a network after selecting a debug file
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool DebugNetwork()
        {
            string file = Config.Instance[Config.Keys.SelectedDebugFileName];
            string path = Config.Instance[Config.Keys.SelectedDebugPath];
            try
            {
                net = ClassFactory.GenerateNetwork();                
                net.LoadNetwork(file, path);
                networkWasChanged = false;
            }
            catch (Exception e)
            {
                CustomizedMessageBox.FileMsgErr("Failed to load the network from file :",
                     file,path, e.Message, "MainWindow Window Message");
                return false;
            }
            
            lastInitiationAction = LastInitiationAction.Debug;
            activationPhase = ActivationPhases.Running;
            CreateAllPresentations();
            net.CheckAndCorrectBuild();
            net.Create();
            net.CreateDebug();
            runningHandler = new RunningHandeler(net, true);
            runningHandler.Run(false);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SaveNetwork()
        ///
        /// \brief Saves the network.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SaveNetwork()
        {
            networkWasChanged = false;
            net.SaveNetwork(false, Config.Instance[Config.Keys.SelectedDataFileName],
                    Config.Instance[Config.Keys.SelectedDataPath]);
            CustomizedMessageBox.FileMsg("The network was saved to",
                Config.Instance[Config.Keys.SelectedDataFileName],
                Config.Instance[Config.Keys.SelectedDataPath],
                "",
                "MainWindow Message");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Window_Close(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Window for close events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_Close(object sender, EventArgs e)
        {
            // If the program is in running phase and exits we don't save in order not
            // to save the running status which will cause destruction of the network definition
            if (activationPhase != ActivationPhases.Running)
            {
                if (networkWasChanged)
                {
                    if (CustomizedMessageBox.Show("The network was changed do you want to save it?",
                        "MainWindow Message",
                        MessageBoxButton.YesNo,
                        Icons.Question) == MessageBoxResult.Yes)
                    {
                        SaveNetwork();
                    }
                }
            }
            wordDocumentHolder.QuitApplication();
            Config.Instance.SaveConfig();
            Logger.CloseEditLogFile();
            Application.Current.Shutdown();
        }
        #endregion
        #region /// \name Build menu items handlers

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_EditNetwork(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for edit network events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_EditNetwork(object sender, RoutedEventArgs e)
        {
            ElementInputWindow elementInput = new ElementInputWindow(new List<NetworkElement>() { net });
            elementInput.ShowDialog();
            networkWasChanged = networkWasChanged || elementInput.elementWasChanged;
            CustomizedMessageBox.Show("Edit Network Ended", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_AddProcess(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for add process events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_AddProcess(object sender, RoutedEventArgs e)
        {
            BaseProcess newProcess = ClassFactory.GenerateProcess(net);
            int processId = net.Processes.Last().ea[ne.eak.Id] + 1;
            newProcess.Init(processId);
            Random random = new Random();
            newProcess.pp[bp.ppk.FrameTop] = random.Next(0, (int)(Canvas_Draw.ActualHeight - newProcess.pp[bp.ppk.FrameHeight]));
            newProcess.pp[bp.ppk.FrameLeft] = random.Next(0, (int)(Canvas_Draw.ActualWidth - newProcess.pp[bp.ppk.FrameWidth]));
            newProcess.Presentation = new ProcessPresentation(Canvas_Draw, newProcess, this);
            net.Processes.Add(newProcess);
            SetSelectedProcess(newProcess);
            CreateNextChannel(processId, processId);
            UpdatePresentation();
            networkWasChanged = true;
            CustomizedMessageBox.Show("Add Process Ended", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_EditProcess(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for edit process events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_EditProcess(object sender, RoutedEventArgs e)
        {
            ElementInputWindow elementInput = new ElementInputWindow(new List<NetworkElement>() { SelectedProcess });
            elementInput.ShowDialog();
            UpdatePresentation();
            networkWasChanged = networkWasChanged || elementInput.elementWasChanged;
            SelectedProcess.Presentation.UpdatePresentation();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_DeleteProcess(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for delete process events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_DeleteProcess(object sender, RoutedEventArgs e)
        {
            List<BaseChannel> channelsToDelete = net.CollectChannelsConnectedToProcess(SelectedProcess.ea[ne.eak.Id]);
            foreach (BaseChannel channel in channelsToDelete)
            {
                DeleteChannel(channel);
            }

            SelectedProcess.Clear();
            net.Processes.Remove(SelectedProcess);
            UpdatePresentation();
            SetSelectedProcess(net.Processes[0]);
            SetSelectedChannel(net.Channels[0]);
            networkWasChanged = true;
            CustomizedMessageBox.Show("Delete Process Ended", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_ReIndexProcesses(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for re index processes events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_ReIndexProcesses(object sender, RoutedEventArgs e)
        {
            bool changeProcessNames = false;
            MessageBoxResult dialogResult = CustomizedMessageBox.Show("Do you want to change process names also ?", "ReIndex Processes dialog", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                changeProcessNames = true;
            }
            net.ReindexProcesses(changeProcessNames);
            UpdatePresentation();
            networkWasChanged = true;
            CustomizedMessageBox.Show("Reindex Processes Ended", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Command_AddChannel(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for add channel events.
        ///
        /// \par Description.
        ///      -  Add new channel. The process in inserting is :
        ///         -#  Select source process
        ///         -#  Select destination process
        ///         -#  Create the channel and(if not already exist and requested by the user) Create backword Channel
        ///      -  All the stages call this function with different channelAddState
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Command_AddChannel(object sender, RoutedEventArgs e)
        {

            switch (channelAddState)
            {
                case ChannelAddState.NotInProcess:
                    //Label_Instructions.Content = "Select the source process";
                    channelAddState = ChannelAddState.BeforeFirstProcessSelection;
                    break;
                case ChannelAddState.BeforeFirstProcessSelection:
                    //Label_Instructions.Content = "Select the destination process";
                    addChannel_sourceProcess = (BaseProcess)sender;
                    addChannel_sourceProcess.Presentation.SetSelected(addChannel_sourceProcess, SelectedStatus.SelectedForChannel);
                    channelAddState = ChannelAddState.BeforeSecondProcessSelection;
                    break;
                case ChannelAddState.BeforeSecondProcessSelection:
                    //Label_Instructions.Content = "";
                    channelAddState = ChannelAddState.NotInProcess;
                    addChannel_destProcess = (BaseProcess)sender;
                    int sourceProcessId = addChannel_sourceProcess.ea[ne.eak.Id];
                    int destProcessId = addChannel_destProcess.ea[ne.eak.Id];

                    //Create the forword channels
                    if (net.Channels.Exists(channel => channel.ea[bc.eak.SourceProcess] == sourceProcessId &&
                        channel.ea[bc.eak.DestProcess] == destProcessId))
                    {
                        CustomizedMessageBox.Show("The Channel already exist", "Main Window Message", Icons.Error);
                        SetSelectedProcess(SelectedProcess);
                        return;
                    }
                    BaseChannel forwordChannel = CreateNextChannel(sourceProcessId, destProcessId);

                    //Create the backword channel
                    BaseChannel backwordChannel = net.Channels.FirstOrDefault(channel => channel.ea[bc.eak.SourceProcess] == destProcessId &&
                        channel.ea[bc.eak.DestProcess] == sourceProcessId);
                    if (backwordChannel == null)
                    {
                        /*
                         * If the network is not directed add backwork channel
                         */
                        if (net.ea[bn.eak.DirectedNetwork] == false)
                        {
                            backwordChannel = CreateNextChannel(destProcessId, sourceProcessId);
                        }
                        else
                        {
                            /*
                             * The backword channel does not exist prompt if to create a backword channel
                             */
                            MessageBoxResult dialogResult = CustomizedMessageBox.Show("Do you want to create a backword channel ?", "Add backword channel dialog", MessageBoxButton.YesNo);
                            if (dialogResult == MessageBoxResult.Yes)
                            {
                                /*
                                 * The decition was to create a backword channel
                                 * Create a new backword channel and a new presentation to the backword and foreword channels
                                 */
                                backwordChannel = CreateNextChannel(destProcessId, sourceProcessId);
                            }
                        }
                    }
                    UpdatePresentation();
                    SetSelectedProcess(SelectedProcess);
                    SetSelectedChannel(forwordChannel);
                    networkWasChanged = true;
                    CustomizedMessageBox.Show("Add Channel Ended", "Main Window Message", Icons.Success);
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_EditChannel(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for edit channel events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_EditChannel(object sender, RoutedEventArgs e)
        {
            ElementInputWindow elementInput = new ElementInputWindow(new List<NetworkElement>() { SelectedChannel });
            elementInput.ShowDialog();
            UpdatePresentation();
            SelectedChannel.Presentation.UpdatePresentation();
            networkWasChanged = networkWasChanged || elementInput.elementWasChanged;
            CustomizedMessageBox.Show("Edit Channel Ended", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_DeleteChannel(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for delete channel events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_DeleteChannel(object sender, RoutedEventArgs e)
        {
            if (SelectedChannel.ea[bc.eak.SourceProcess] == SelectedChannel.ea[bc.eak.DestProcess])
            {
                CustomizedMessageBox.Show("Cannot delete channel from process to itself", "Main Window Message", Icons.Error);
                return;
            }
            DeleteChannel(SelectedChannel);
            SetChannelList();
            SetSelectedChannel(net.Channels[0]);
            networkWasChanged = true;
            CustomizedMessageBox.Show("Delete Channel Ended", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_ReIndexChannels(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for re index channels events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_ReIndexChannels(object sender, RoutedEventArgs e)
        {
            net.ReindexChannels();
            UpdatePresentation();
            foreach (BaseChannel channel in net.Channels)
            {
                channel.Presentation.UpdatePresentation();
            }
            networkWasChanged = true;
            CustomizedMessageBox.Show("Reindex Channels Ended", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CheckNetwork(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for check network events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CheckNetwork(object sender, RoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Init || activationPhase == ActivationPhases.Running)
            {
                CustomizedMessageBox.Show("Network was not initiated or loaded or in running phase", "Check network error", Icons.Error);
            }
            net.CheckNetworkkBuild();
            UpdatePresentation();
            networkWasChanged = true;
            activationPhase = ActivationPhases.Checked;
            CustomizedMessageBox.Show("finished checking", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CreateNetwork(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for create network events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CreateNetwork(object sender, RoutedEventArgs e)
        {
            if (activationPhase != ActivationPhases.Checked)
            {
                CustomizedMessageBox.Show("Network was not initiated or loaded or in running phase", "Create network error", Icons.Error);
            }
            else
            {
                net.Create();
                net.SaveNetwork(false, Config.Instance[Config.Keys.SelectedDataFileName],
                    Config.Instance[Config.Keys.SelectedDataPath]);
                activationPhase = ActivationPhases.Created;
                CustomizedMessageBox.Show("finished creating", "Main Window Message", Icons.Success);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_IncreaseWidth(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for increase width events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_IncreaseWidth(object sender, RoutedEventArgs e)
        {
            Canvas_Draw.Width = Canvas_Draw.ActualWidth + 100;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_DecreaseWidth(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for decrease width events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_DecreaseWidth(object sender, RoutedEventArgs e)
        {
            Canvas_Draw.Width = Canvas_Draw.ActualWidth - 100;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_IncreaseHeight(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for increase height events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_IncreaseHeight(object sender, RoutedEventArgs e)
        {
            Canvas_Draw.Height = Canvas_Draw.ActualHeight + 100;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_DecreaseHeight(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for decrease height events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_DecreaseHeight(object sender, RoutedEventArgs e)
        {
            Canvas_Draw.Height = Canvas_Draw.ActualHeight - 100;
        }

        # endregion
        #region /// \name Creating/deleting elements 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseChannel CreateNextChannel(int sourceProcessId, int destProcessId)
        ///
        /// \brief Creates next channel.
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
        /// \param sourceProcessId  (int) - Identifier for the source process.
        /// \param destProcessId    (int) - Identifier for the destination process.
        ///
        /// \return The new next channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseChannel CreateNextChannel(int sourceProcessId, int destProcessId)
        {
            /*
             * Create a channel with the next id on the list and with the source and destination process
             */
            int channelId = net.Channels.Last().ea[ne.eak.Id] + 1;
            BaseChannel channel = ClassFactory.GenerateChannel(net, channelId, sourceProcessId, destProcessId);

            /*
             * Check if there is a channel with oposite direction
             */
            BaseChannel backwordChannel = net.Channels.FirstOrDefault(c => c.ea[bc.eak.SourceProcess] == destProcessId &&
                        c.ea[bc.eak.DestProcess] == sourceProcessId);
            if (backwordChannel != null)
            {
                /*
                 * If there is a channel with oposite direction add the new channel to the presentation
                 */
                ChannelPresentation backwordChannelPresentation = (ChannelPresentation)backwordChannel.Presentation;
                backwordChannelPresentation.AddChannel(channel);
                channel.Presentation = backwordChannelPresentation;
            }
            else
            {
                /*
                 * If there is no channel with oposite direction create a new presentation
                 */
                channel.Presentation = new ChannelPresentation(this, Canvas_Draw, channel, null);
            }

            /*
             * Add the new channel to the channels list
             */
            net.Channels.Add(channel);
            return channel;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void DeleteChannel(BaseChannel channel)
        ///
        /// \brief Deletes the channel described by channel.
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
        /// \param channel  (BaseChannel) - The channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void DeleteChannel(BaseChannel channel)
        {
            channel.Clear();
            net.Channels.Remove(channel);
        }


        #endregion
        #region /// \name Run menu items

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_Breakpoints(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for breakpoints events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_Breakpoints(object sender, RoutedEventArgs e)
        {
            //If the program is in running state - this button is disabled because
            //The breakpoints are from the main window
            if (activationPhase == ActivationPhases.Running)
            {
                CustomizedMessageBox.Show(" The program is in running state \n The breakpoints windows can be accessed by selecting the processes", "Breakpoints window not active", null, Icons.Error);
                return;
            }
            BaseProcess process;
            BaseChannel channel;
            List<NetworkElement> breakpoints;
            //if (BreakpointWindow.SelectProcess(net, out breakpoints, out process, out channel))
            //{
            //    BreakpointWindow breakpointsWindow = new BreakpointWindow(BreakpointWindow.ProgramStatus.BeforeRunning, breakpoints, net, process, channel, null);
            //    breakpointsWindow.ShowDialog();
            //}
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void InitRunning()
        ///
        /// \brief Init running.
        ///
        /// \par Description.
        ///      -  This method is called from all the commands that might initiate running.  
        ///      -  All the commands check if the program is in running mode and if not call this method
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitRunning()
        {
            // Save the status of the network and the config 
            // This is done in order that if the program terminates with error
            // the next operation will start from the current network and config status
            SaveNetwork();
            Config.Instance.SaveConfig();

            // Backup the OperationResults and the Presentation dictionary for recovery after running
            activationPhase = ActivationPhases.BeforeRunning;
            net.BackupInitOperationResults();
            net.BackupPresentation();

            // Set the program phase and create a RunningHandler
            activationPhase = ActivationPhases.Running;
            runningHandler = new RunningHandeler(net, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_RunToEnd(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for run to end events.
        ///
        /// \par Description.
        ///      Run the algorithm until the end
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_RunToEnd(object sender, RoutedEventArgs e)
        {
            if (ActivationPhase != ActivationPhases.Running)
            {
                InitRunning();
                runningHandler.RunToEnd(false);
            }
            else
            {
                runningHandler.RunToEnd(true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_SingleStepAll(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for single step all events.
        ///
        /// \par Description.
        ///      Run Single step in all the processors that has a waiting message
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_SingleStepAll(object sender, RoutedEventArgs e)
        {
            if (ActivationPhase != ActivationPhases.Running)
            {
                InitRunning();
                runningHandler.SingleStepAll(false);
            }
            else
            {
                runningHandler.SingleStepAll(true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_InitializeRunning(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for initialize running events.
        ///
        /// \par Description.
        ///      This method activates the network which the following:
        ///      -# If started from debug - recover the messages Qs
        ///      -# For each initiator run the initiation method
        ///      -# Enter receive loop in all the processors
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_InitializeRunning(object sender, RoutedEventArgs e)
        {      
            if (ActivationPhase != ActivationPhases.Running)
            {
                                          InitRunning();
                runningHandler.InitializeRunning();
            }
            else
            {
                CustomizedMessageBox.Show("The algorithm is already running", "Running Handler Message", null, Icons.Error);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_SaveCurrentStatus(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for save current status events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_SaveCurrentStatus(object sender, RoutedEventArgs e)
        {
            FileSelect fileSelect = new FileSelect(FileSelect.SelectSource.SaveDebug, "Select Debug file name");
            fileSelect.ShowDialog();

            if (!(fileSelect.result == FileSelect.SelectResult.Quit))
            {
                net.SaveNetwork(true, Config.Instance[Config.Keys.SelectedDebugFileName],
                    Config.Instance[Config.Keys.SelectedDebugPath]);
                UpdatePresentation();
            }
            CustomizedMessageBox.FileMsg("Saved current status to :",
                Config.Instance[Config.Keys.SelectedDebugFileName],
                Config.Instance[Config.Keys.SelectedDebugPath],
                "",
                "MainWindow Message");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_Terminate(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for terminate events.
        ///
        /// \par Description.
        ///      This method starts the termination algorithm which causes the running to end
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_Terminate(object sender, RoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Running)
            {
                runningHandler.Terminate();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_ShowBreakpoints(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for show breakpoints events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_ShowBreakpoints(object sender, ExecutedRoutedEventArgs e)
        {
            showBreakpoints = !showBreakpoints;
            foreach (BaseProcess process in net.Processes)
            {
                ((ProcessPresentation)process.Presentation).SetBreakpoinsVisibility(process, (showBreakpoints) ? Visibility.Visible : Visibility.Hidden);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_ShowFloatingSummary(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for show floating summary events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_ShowFloatingSummary(object sender, ExecutedRoutedEventArgs e)
        {
            showFloatingSummary = !showFloatingSummary;
        }

        private void Command_ShowMessages(object sender, ExecutedRoutedEventArgs e)
        {
            showMessages = !showMessages;
            foreach (BaseChannel channel in net.Channels)
            {
                ((ChannelPresentation)channel.Presentation).SetMessagesVisibility(channel, (showMessages) ? Visibility.Visible : Visibility.Hidden);
            }
        }

        # endregion
        #region /// \name Program menu Items handlers

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_AddAlgorithm(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for add algorithm events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_AddAlgorithm(object sender, ExecutedRoutedEventArgs e)
        {
            ActivationPhases currentPhase = activationPhase;
            activationPhase = ActivationPhases.Temp;
            AddAlgorithmWindow addAlgorithmWindow = new AddAlgorithmWindow();
            if (!addAlgorithmWindow.quited)
            {
                addAlgorithmWindow.ShowDialog();
            }
            activationPhase = currentPhase;
        }

        private void Command_BaseAlgorithm(object sender, ExecutedRoutedEventArgs e)
        {
            BaseAlgorithmAccess baseAlgorithmAccess = new BaseAlgorithmAccess();
            baseAlgorithmAccess.CreateCodeFile();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_EditConfiguration(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for edit configuration events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_EditConfiguration(object sender, ExecutedRoutedEventArgs e)
        {
            string saveFileName = Config.Instance[Config.Keys.SelectedDataFileName];
            string saveFilePath = Config.Instance[Config.Keys.SelectedDataPath];
            ConfigWindow configWindow = new ConfigWindow();
            configWindow.ShowDialog();

            if (configWindow.selectionChanged)
            {
                UpdateForSelectedAlgorithmChangedByConfig(saveFileName, saveFilePath);
                return;
            }

        }
        #endregion
        #region /// \name Documentation menu items handlers

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_OpenAlgorithmDocumentation(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for open algorithm documentation events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_OpenAlgorithmDocumentation(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentHandler.LoadDocumentation(wordDocumentHolder);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_OpenAlgorithmPseudoCode(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for open algorithm pseudo code events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_OpenAlgorithmPseudoCode(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentHandler.LoadPseudoCode(wordDocumentHolder);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_OpenProgramDocumentation(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for open program documentation events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_OpenProgramDocumentation(object sender, ExecutedRoutedEventArgs e)
        {
            ProgramDocuments.ShowProgramDocumentation();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CreateDocumentation(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for create documentation events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CreateDocumentation(object sender, ExecutedRoutedEventArgs e)
        {
            wordDocumentHolder.QuitApplication();
            DocumentHandler documentationCreator = new DocumentHandler();
            if (!documentationCreator.constructorQuited)
            {
                documentationCreator.ShowDialog();
            }
            DocumentHandler.LoadDocumentation(wordDocumentHolder);
        }

        #endregion
        #region /// \name Checks menu items handlers

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_CheckIcons_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_CheckIcons for click events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_CheckIcons_Click(object sender, RoutedEventArgs e)
        {
            switch (checkIconsPhase)
            {
                case 0:
                    CustomizedMessageBox.Show("Error Icon", "Icons Check", Icons.Error);
                    break;
                case 1:
                    CustomizedMessageBox.Show("Warning Icon", "Icons Check", Icons.Warning);
                    break;
                case 2:
                    CustomizedMessageBox.Show("Question Icon", "Icons Check", Icons.Question);
                    break;
                case 3:
                    CustomizedMessageBox.Show("Info Icon", "Icons Check", Icons.Info);
                    break;
                case 4:
                    CustomizedMessageBox.Show("Success Icon", "Icons Check", Icons.Success);
                    break;
            }
            checkIconsPhase++;
            if (checkIconsPhase > 4) checkIconsPhase = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CheckDeepCopy(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for check deep copy events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CheckDeepCopy(object sender, ExecutedRoutedEventArgs e)
        {
            ActivationPhases tempactivationPhase = activationPhase;
            activationPhase = ActivationPhases.Temp;
            BaseNetwork baseNetwork = new BaseNetwork();
            baseNetwork.DeepCopy(net);
            if (baseNetwork.CheckEqual(0, "BaseNetwork", net, true))
            {
                CustomizedMessageBox.Show("DeepCopy EqualsTo succeeded", "MainWindow Message", null, Icons.Success);
            }
            else
            {
                CustomizedMessageBox.Show("DeepCopy EqualsTo failed", "MainWindow Message", null, Icons.Error);
            }
            baseNetwork.CheckMembers("Check After DeepCopy");
            activationPhase = tempactivationPhase;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CheckDictionaryDeepCopy(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for check dictionary deep copy events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CheckDictionaryDeepCopy(object sender, ExecutedRoutedEventArgs e)
        {
            ActivationPhases tempactivationPhase = activationPhase;
            activationPhase = ActivationPhases.Temp;
            net.Processes[0].orb.DeepCopy(net.Processes[0].or);
            if (net.Processes[0].orb.CheckEqual(0, "BaseNetwork", net.Processes[0].or, true))
            {
                CustomizedMessageBox.Show("DeepCopy EqualsTo succeeded", "MainWindow Message", null, Icons.Success);
            }
            else
            {
                CustomizedMessageBox.Show("DeepCopy EqualsTo failed", "MainWindow Message", null, Icons.Error);
            }
            net.Processes[0].CheckMembers("Check After Dictionary DeepCopy");
            activationPhase = tempactivationPhase;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CheckFileSelect(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for check file select events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CheckFileSelect(object sender, ExecutedRoutedEventArgs e)
        {
            FileSelect fileSelect = new FileSelect(FileSelect.SelectSource.New, "Header");
            fileSelect.ShowDialog();
            CustomizedMessageBox.Show(new List<string> { "result is " + fileSelect.result.ToString() }, "Test FileSelect result");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CheckMembers(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for check members events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (ExecutedRoutedEventArgs) - Executed routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CheckMembers(object sender, ExecutedRoutedEventArgs e)
        {
            net.CheckNetwork("Check Members");
        }
        #endregion
        #region /// \name Presentation Utilities

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CreateAllPresentations()
        ///
        /// \brief Creates all presentations.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CreateAllPresentations()
        {
            Canvas_Draw.Children.Clear();
            CreateProcessPresentation();
            CreateChannelsPresentation();
            UpdatePresentation();
            SetSelectedProcess(net.Processes[0]);
            SetSelectedChannel(net.Channels[0]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ClearAllPresentations()
        ///
        /// \brief Clears all presentations.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ClearAllPresentations()
        {
            Canvas_Draw.Children.Clear();
            ListBox_Processes.Items.Clear();
            ListBox_Channels.Items.Clear();
            SetHeaderLabels();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CreateChannelsPresentation()
        ///
        /// \brief Creates channels presentation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CreateChannelsPresentation()
        {
            foreach (BaseChannel channel in net.Channels)
            {
                if (channel.Presentation == null)
                {
                    BaseProcess sourceProcess = net.Processes.First(process => process.ea[ne.eak.Id] == channel.ea[bc.eak.SourceProcess]);
                    BaseProcess destProcess = net.Processes.First(process => process.ea[ne.eak.Id] == channel.ea[bc.eak.DestProcess]);

                    BaseChannel reverseChannel;
                    if (sourceProcess.ea[ne.eak.Id] == destProcess.ea[ne.eak.Id])
                    {
                        reverseChannel = null;
                    }
                    else
                    {
                        reverseChannel = net.Channels.FirstOrDefault(rChannel => channel.ea[bc.eak.SourceProcess] == rChannel.ea[bc.eak.DestProcess] &&
                            channel.ea[bc.eak.DestProcess] == rChannel.ea[bc.eak.SourceProcess]);
                    }

                    //If there is a reverse channel create a presentation with both channels
                    //(When the 
                    if (reverseChannel != null)
                    {
                        reverseChannel.Presentation = channel.Presentation = new ChannelPresentation(this, Canvas_Draw, channel, reverseChannel);
                    }
                    else
                    {
                        channel.Presentation = new ChannelPresentation(this, Canvas_Draw, channel, null);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CreateProcessPresentation()
        ///
        /// \brief Creates process presentation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CreateProcessPresentation()
        {
            foreach (BaseProcess process in net.Processes)
            {
                process.Presentation = new ProcessPresentation(Canvas_Draw, process, this);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateForNewAlgorithmsDataSelectedByConfig(string prevNetSaveFileName, string prevNetSaveFilePath)
        ///
        /// \brief Updates for new algorithms data selected by configuration.
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
        /// \param prevNetSaveFileName  (string) - Filename of the previous net save file.
        /// \param prevNetSaveFilePath  (string) - Full pathname of the previous net save file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateForNewAlgorithmsDataSelectedByConfig(string prevNetSaveFileName, string prevNetSaveFilePath)
        {
            Logger.LogPath = Config.Instance[Config.Keys.SelectedLogPath];            
            net.SaveNetwork(false, prevNetSaveFileName, prevNetSaveFilePath);
            Config.Instance[Config.Keys.SelectedDataPath] = Config.Instance[Config.Keys.SelectedDataPath];
            Config.Instance[Config.Keys.SelectedDataFileName] = Config.Instance[Config.Keys.SelectedDataPath];
            activationPhase = ActivationPhases.Init;
            LoadNetwork();
            activationPhase = ActivationPhases.Loaded;
            lastInitiationAction = LastInitiationAction.Load;

            
            CustomizedMessageBox.FileMsg("The data file of the current status of the algorithm was saved to: ",
                Config.Instance[Config.Keys.SelectedDataFileName],
                Config.Instance[Config.Keys.SelectedDataPath],
                "(In the new data folder selected)",
                "MainWindowMessage");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateForSelectedAlgorithmChangedByConfig(string prevNetSaveFileName, string prevNetSaveFilePath)
        ///
        /// \brief Updates for selected algorithm changed by configuration.
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
        /// \param prevNetSaveFileName  (string) - Filename of the previous net save file.
        /// \param prevNetSaveFilePath  (string) - Full pathname of the previous net save file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateForSelectedAlgorithmChangedByConfig(string prevNetSaveFileName, string prevNetSaveFilePath)
        {
            // Saving the current network
            if (net != null)
            {
                net.SaveNetwork(false, prevNetSaveFileName, prevNetSaveFilePath);
                CustomizedMessageBox.FileMsg("The data file of the current status of the algorithm was saved to:",
                    prevNetSaveFileName, prevNetSaveFilePath,  "(In the previous Algorithm Data environment)", "MainWindowMessage");
            }

            // Loading the new network
            activationPhase = ActivationPhases.Init;
            LoadNetwork();
            activationPhase = ActivationPhases.Loaded;
            lastInitiationAction = LastInitiationAction.Load;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateForResetConfig(string prevNetSaveFileName, string prevNetSaveFilePath)
        ///
        /// \brief Updates for reset configuration.
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
        /// \param prevNetSaveFileName  (string) - Filename of the previous net save file.
        /// \param prevNetSaveFilePath  (string) - Full pathname of the previous net save file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateForResetConfig(string prevNetSaveFileName, string prevNetSaveFilePath)
        {
            //Logger.LogPath = Config.Instance.GenerateDefaultLogFilePath();
            //string message = "";
            //if (net != null)
            //{
            //    //net.ea[bn.eak.DataFilePath] = ClassFactory.GetDefaultDataFilePath();
            //    //net.ea[bn.eak.DebugFilePath] = ClassFactory.GetDefaultDebugFilePath();
            //    net.SaveNetwork(false, prevNetSaveFileName, prevNetSaveFilePath);
            //    message = "The data file of the current status of the algorithm was saved to: \n" +
            //        prevNetSaveFileName + prevNetSaveFilePath + "\n" +
            //        "(In the previous data folder selected)\n";
            //}
            //LoadPrevActivationNetwork();
            //message += "The network was loaded from default setting: \n" +
            //    Config.Instance[Config.Keys.SelectedDataPath] + Config.Instance[Config.Keys.SelectedDataFileName] + "\n" +
            //    "(In the new data folder selected)";

            //CustomizedMessageBox.Show(message, "MainWindowMessage", Icons.Info);
        }
        #endregion
        #region /// \name Handle lists and header and canvas presentations

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void UpdatePresentation()
        ///
        /// \brief Updates the presentation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdatePresentation()
        {
            SetProcessList();
            SetChannelList();
            SetHeaderLabels();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void UpdateRunningPresentation()
        ///
        /// \brief Updates the running presentation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateRunningPresentation()
        {
            SetProcessesRunningStatus();
            SetChannelsRunningStatus();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetHeaderLabels()
        ///
        /// \brief Sets header labels.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetHeaderLabels()
        {
            TextBlock_NetworkTypeName.Text = ClassFactory.GenerateNamespace();
            TextBlock_NetworkName.Text = Config.Instance[Config.Keys.SelectedNetwork];
            switch (lastInitiationAction)
            {
                case LastInitiationAction.Debug:
                    TextBlock_LoadFileName.Text = Config.Instance[Config.Keys.SelectedDebugPath] +
                    Config.Instance[Config.Keys.SelectedDebugFileName];
                    TextBlock_LoadFileHeader.Text = "Debug File Name:";
                    break;
                case LastInitiationAction.Load:
                    TextBlock_LoadFileName.Text = Config.Instance[Config.Keys.SelectedDataPath] + Config.Instance[Config.Keys.SelectedDataFileName];
                    TextBlock_LoadFileHeader.Text = "Data File Name : ";
                    break;
                case LastInitiationAction.Init:
                    TextBlock_LoadFileName.Text = Config.Instance[Config.Keys.SelectedDataPath] + Config.Instance[Config.Keys.SelectedDataFileName];
                    TextBlock_LoadFileHeader.Text = "Data File Name:";
                    break;
                default:
                    TextBlock_LoadFileName.Text = "";
                    TextBlock_LoadFileHeader.Text = "Data File Name:";
                    TextBlock_NetworkTypeName.Text = "Algorithms was not Loaded or initiated";
                    TextBlock_NetworkName.Text = "Network was not loaded or initiated";
                    break;
            }

            if (lastInitiationAction == LastInitiationAction.Clear)
            {
                TextBlock_LogFileName.Text = "";
            }
            else
            {
                TextBlock_LogFileName.Text = Config.Instance[Config.Keys.SelectedLogPath] + Config.Instance[Config.Keys.SelectedLogFileName];
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetProcessList()
        ///
        /// \brief Sets process list.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetProcessList()
        {
            /*
             * First order the list by Id
             */
            net.Processes = net.Processes.OrderBy(process => process.ea[ne.eak.Id]).ToList();

            /*
             * Create the processes list box
             */
            ListBox_Processes.Items.Clear();
            foreach (BaseProcess process in net.Processes)
            {
                /*
                 * Each line in the list box contains:
                 * The ToString method of the process
                 * If it was editted
                 * If the process should be editted it is colored bold
                 */
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = process.ToString() + " Editted:" + process.ea[ne.eak.Edited];
                if (process.ShouldBeEditted())
                {
                    listBoxItem.FontWeight = FontWeights.Bold;
                }
                else
                {
                    listBoxItem.FontWeight = FontWeights.Normal;
                }
                ListBox_Processes.Items.Add(listBoxItem);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetProcessesRunningStatus()
        ///
        /// \brief Sets processes running status.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetProcessesRunningStatus()
        {
            ListBox_Processes.Items.Clear();
            foreach (BaseProcess process in net.Processes)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                string text = process.pp[bp.ppk.Text];
                listBoxItem.Content = ReplaceChar(text, '\n', ' ');
                listBoxItem.Foreground = new SolidColorBrush(Presentation.CreateColorFromEnum(process.pp[bp.ppk.Foreground]));
                ListBox_Processes.Items.Add(listBoxItem);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetChannelList()
        ///
        /// \brief Sets channel list.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetChannelList()
        {
            net.Channels = net.Channels.OrderBy(channel => channel.ea[ne.eak.Id]).ToList();
            ListBox_Channels.Items.Clear();
            foreach (BaseChannel channel in net.Channels)
            {

                /*
                 * Each line in the list box contains:
                 * The ToString method of the process
                 * If the flag to present the channels presentation is true - display the data of the channel presentation
                 * If it was editted
                 * If the process should be editted it is colored bold
                 */
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = channel.ToString() + " Editted:" + channel.ea[ne.eak.Edited].ToString();
                if (showChannelsPresentation)
                {
                    listBoxItem.Content += " " + channel.Presentation.ToString() + " Editted:" + channel.ea[ne.eak.Edited].ToString();
                }
                if (channel.ShouldBeEditted())
                {
                    listBoxItem.FontWeight = FontWeights.Bold;
                }
                else
                {
                    listBoxItem.FontWeight = FontWeights.Normal;
                }
                ListBox_Channels.Items.Add(listBoxItem);
            }
            ListBox_Channels.SelectedIndex = net.Channels.IndexOf(SelectedChannel);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetChannelsRunningStatus()
        ///
        /// \brief Sets channels running status.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetChannelsRunningStatus()
        {
            ListBox_Channels.Items.Clear();
            foreach (BaseChannel channel in net.Channels)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                string text = channel.pp[bc.ppk.PresentationTxt].ToString();
                listBoxItem.Content = ReplaceChar(text, '\n', ' ');
                listBoxItem.Foreground = new SolidColorBrush(Presentation.CreateColorFromEnum(channel.pp[bc.ppk.Foreground]));
                ListBox_Channels.Items.Add(listBoxItem);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string ReplaceChar(string s, char from, char to)
        ///
        /// \brief Replace character.
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
        /// \param s     (string) - The string.
        /// \param from  (char) - Source for the.
        /// \param to    (char) - to.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string ReplaceChar(string s, char from, char to)
        {
            string result = "";
            foreach (char c in s)
            {
                if (c == from)
                {
                    result += to;
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetSelectedProcess(BaseProcess selectedProcess)
        ///
        /// \brief Sets selected process.
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
        /// \param selectedProcess  (BaseProcess) - The selected process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetSelectedProcess(BaseProcess selectedProcess)
        {
            this.SelectedProcess = selectedProcess;
            ListBox_Processes.SelectedIndex = net.Processes.IndexOf(selectedProcess);
            foreach (BaseProcess process in net.Processes)
            {
                if (process == selectedProcess)
                {
                    process.Presentation.SetSelected(process, SelectedStatus.Selected);
                }
                else
                {
                    process.Presentation.SetSelected(process, SelectedStatus.NotSelected);
                }
            }
        }

       
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetSelectedChannel(BaseChannel selectedChannel)
        ///
        /// \brief Sets selected channel.
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
        /// \param selectedChannel  (BaseChannel) - The selected channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetSelectedChannel(BaseChannel selectedChannel)
        {
            this.SelectedChannel = selectedChannel;
            ListBox_Channels.SelectedIndex = net.Channels.IndexOf(selectedChannel);
            foreach (BaseChannel channel in net.Channels)
            {
                if (channel.Presentation != null)
                {
                    if (channel == selectedChannel)
                    {
                        channel.Presentation.SetSelected(channel, SelectedStatus.Selected);
                    }
                    else
                    {
                        channel.Presentation.SetSelected(channel, SelectedStatus.NotSelected);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ListBox_Processes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief Event handler. Called by ListBox_Processes for selection changed events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (SelectionChangedEventArgs) - Selection changed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ListBox_Processes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((System.Windows.Controls.ListBox)sender).SelectedIndex >= 0)
            {
                SetSelectedProcess(net.Processes[((System.Windows.Controls.ListBox)sender).SelectedIndex]);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ListBox_Channels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief Event handler. Called by ListBox_Channels for selection changed events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (SelectionChangedEventArgs) - Selection changed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ListBox_Channels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((System.Windows.Controls.ListBox)sender).SelectedIndex >= 0)
            {
                SetSelectedChannel(net.Channels[((System.Windows.Controls.ListBox)sender).SelectedIndex]);

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_ShowChanelPresentation(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command for show chanel presentation events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_ShowChanelPresentation(object sender, RoutedEventArgs e)
        {
            showChannelsPresentation = !showChannelsPresentation;
            SetChannelList();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void PrintDubugMessage(string sender, string vectorClock, string message)
        ///
        /// \brief Print dubug message.
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
        /// \param sender       (string) - The sender.
        /// \param vectorClock  (string) - The vector clock.
        /// \param message      (string) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void PrintDubugMessage(string sender, string vectorClock, string message)
        {
            if (messageWindow == null)
            {
                messageWindow = new MessageWindow(this);
                messageWindow.Show();
            }
            messageWindow.PrintDebugMessage(sender, vectorClock, message);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void UpdateInitRunningPresentation()
        ///
        /// \brief Updates the init running presentation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateInitRunningPresentation()
        {
            foreach (BaseProcess process in net.Processes)
            {
                process.Presentation.SetSelected(process, SelectedStatus.NotSelected);
            }
            foreach (BaseChannel channel in net.Channels)
            {
                channel.Presentation.SetSelected(channel, SelectedStatus.NotSelected);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateEndRunningPresenation()
        ///
        /// \brief Updates the end running presenation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateEndRunningPresenation()
        {
            CustomizedMessageBox.Show("finished running pressing OK will return to the design mode", "Main Window Message", Icons.Success);
            if (runningHandler != null)
            {
                runningHandler = null;
            }
            activationPhase = ActivationPhases.AfterRunning;
            net.RestoreInitOperationResults();
            net.RestoreNetworkPresentationParameters();
            Config.Instance.AddAlgorithmsData(false);
            activationPhase = ActivationPhases.Loaded;
            net.UpdateRunningStatus(new object[] { new List<BaseProcess>() });
            net.Processes.ForEach(process => process.Presentation.UpdateEndRunning(process));
            Logger.CloseAllLogFiles();
            SetSelectedChannel(SelectedChannel);
            SetSelectedProcess(SelectedProcess);
            UpdatePresentation();
            CustomizedMessageBox.Show("finished running returned to design mode", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ChangePresentationOfComponent(NetworkElement networkElement, object[] parameters)
        ///
        /// \brief Change presentation of component.
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
        /// \param networkElement  (NetworkElement) - The network element.
        /// \param parameters      (object[]) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ChangePresentationOfComponent(NetworkElement networkElement, object[] parameters)
        {
            //The first parameter is the component that was changed
            //The rest of the parameters are parameters to the presentation
            if (typeof(BaseChannel).IsAssignableFrom(networkElement.GetType()))
            {
                ((BaseChannel)networkElement).Presentation.UpdateRunningStatus(networkElement, parameters);
                return;
            }
            if (typeof(BaseProcess).IsAssignableFrom(networkElement.GetType()))
            {
                ((BaseProcess)networkElement).Presentation.UpdateRunningStatus(networkElement, parameters);
                return;
            }
        }
        #endregion
        #region /// \name CanExecute conditions to execute commands

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfNotRunning(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if not running events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfNotRunning(object sender, CanExecuteRoutedEventArgs e)
        {
            if (activationPhase != ActivationPhases.Running)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfNotRunningOrInit(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if not running or init events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfNotRunningOrInit(object sender, CanExecuteRoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Running || activationPhase == ActivationPhases.Init)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfLastInitWasLoad(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if last init was load events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfLastInitWasLoad(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lastInitiationAction == LastInitiationAction.Load)
            {
                e.CanExecute = true;;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_ReIndexNeeded(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for re index needed events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_ReIndexNeeded(object sender, CanExecuteRoutedEventArgs e)
        {
            // Avoid reIndex in the middle of running
            if (activationPhase == ActivationPhases.Running || activationPhase == ActivationPhases.Init)
            {
                e.CanExecute = false;
                return;
            }

            // Set the list to test according to the sender
            IList elementsList;
            if (net != null)
            {
                if (((RoutedUICommand)e.Command).Text == "ReIndexProcesses")
                {
                    elementsList = net.Processes;
                }
                else
                {
                    elementsList = net.Channels;
                }
            }
            else
            {
                e.CanExecute = false;
                return;
            }

            // Check if there are hols in the id in the list
            // The lists are ordered so the only thing to check is whether there is a sep of one
            // in each iteration
            int lastElementId = -1;
            foreach (NetworkElement networkElement in elementsList)
            {
                int elementId = networkElement.ea[ne.eak.Id];
                if (elementId > lastElementId + 1)
                {
                    e.CanExecute = true;
                    return;
                }
                lastElementId = elementId;
            }
            e.CanExecute = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfNotInit(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if not init events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfNotInit(object sender, CanExecuteRoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Init)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfChecked(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if checked events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfChecked(object sender, CanExecuteRoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Checked)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfCreated(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if created events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfCreated(object sender, CanExecuteRoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Created)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfCreatedOrRunning(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if created or running events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfCreatedOrRunning(object sender, CanExecuteRoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Running || activationPhase == ActivationPhases.Created)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfRunning(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if running events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfRunning(object sender, CanExecuteRoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Running)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Command_CanExecute_IfSaveNeeded(object sender, CanExecuteRoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Command_CanExecute for if save needed events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (CanExecuteRoutedEventArgs) - Can execute routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Command_CanExecute_IfSaveNeeded(object sender, CanExecuteRoutedEventArgs e)
        {
            if (activationPhase == ActivationPhases.Running || activationPhase == ActivationPhases.Init)
            {
                e.CanExecute = false;
            }
            else
            {
                if (networkWasChanged)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }
        #endregion
        #region /// \name Buttons for  tests

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Tests_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Tests for click events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Tests_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test();
            test.ShowDialog();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Check_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Check for click events.
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
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Check_Click(object sender, RoutedEventArgs e)
        {

            ProgramDocuments.ShowProgramDocumentation();
        }
        #endregion
        #region

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void AddEditOperation(string sender, string message, Icons imageIcon, Font font)
        ///
        /// \brief Adds an edit operation.
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
        /// \param sender     (string) - The sender.
        /// \param message    (string) - The message.
        /// \param imageIcon  (Icons) - The image icon.
        /// \param font       (Font) - The font.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AddEditOperation(string sender, string message, Icons imageIcon, Font font)
        {
            font.fontFamily = new FontFamily("Consolas");
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            editOperationLabel = new Label();
            sender += " : ";
            editOperationEmptySpaces = new string(' ', sender.Length);
            string[] messageLines = Regex.Split(message, @"\r?\n|\r|\n");
            editOperationLabel.Content = sender + messageLines[0] + "\n";
            for (int idx = 1; idx < messageLines.Length; idx++)
            {
                editOperationLabel.Content += editOperationEmptySpaces + messageLines[idx] + "\n";
            }
            font.SetFontAndDimensions(editOperationLabel, (string)editOperationLabel.Content);
            if (imageIcon == Icons.Error)
            {
                editOperationLabel.Foreground = Brushes.Red;
            }
            Image image = CustomizedMessageBox.SetImage(imageIcon, 18, 18);
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(editOperationLabel);
            ListBoxItem item = new ListBoxItem();
            item.Content = stackPanel;
            listBox_OperationsLog.Items.Add(item);
            editOperationLogEmptySpaces = Logger.WriteOperationToEditLogFile(sender, messageLines, imageIcon, false);
            ScrollViewer_EditOperations.ScrollToEnd();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void AddEditOperationResult(string result, Font font)
        ///
        /// \brief Adds an edit operation result to 'font'.
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
        /// \param result  (string) - The result.
        /// \param font    (Font) - The font.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AddEditOperationResult(string result, Font font)
        {
            editOperationLabel.Content += editOperationEmptySpaces + "Result : " + result;
            font.fontFamily = new FontFamily("Consolas");
            font.SetFontAndDimensions(editOperationLabel, (string)editOperationLabel.Content);
            Logger.WriteOperationResultToEditLogFile(editOperationLogEmptySpaces, result);
        }
        #endregion
        private void Command_CheckDataGrid(object sender, ExecutedRoutedEventArgs e)
        {
            List<List<string>> data = new List<List<string>>();
            for (int rowIdx = 0; rowIdx < 10; rowIdx++)
            {
                List<string> row = new List<string>();
                for (int colIdx = 0; colIdx < 5; colIdx++)
                {
                    row.Add(rowIdx.ToString() + "_" + colIdx.ToString());
                }
                data.Add(row);
            }
            DataGrid dataGrid = CustomizedMessageBox.SetDataGrid(data);
            CustomizedMessageBox.Show(dataGrid);
        }

        //private void Command_CheckListMerge(object sender, ExecutedRoutedEventArgs e)
        //{
        //    ListMerge listMerge = new ListMerge(net.Processes[0].or[bp.tst.TestList], net.Processes[0].or[bp.tst.TestList]);
        //    listMerge.ShowDialog();
        //}
    }
}
