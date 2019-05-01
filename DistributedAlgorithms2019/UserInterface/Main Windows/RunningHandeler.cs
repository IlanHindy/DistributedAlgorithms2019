////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\RunningHandeler.cs
///
/// \brief Implements the running handeler class.
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
    /// \class RunningHandeler
    ///
    /// \brief A running handeler.
    ///
    /// \brief #### Usage Notes.
    ///
    /// \author Main
    /// \date 24/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class RunningHandeler
    {
        /// \brief The network.
        private BaseNetwork network = null;

        /// \brief True to enable in debug mode, false to disable it.
        private bool inDebugMode = false;

        /// \brief The running processes.
        private List<BaseProcess> runningProcesses = new List<BaseProcess>();

        /// \brief The vector clock.
        private List<int> vectorClock = new List<int>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum RunningType
        ///
        /// \brief Values that represent running types.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private enum RunningType  {RunToEnd, SingleStepAll, SingleStepOne, FirstStep}

        /// \brief The processes in step.
        private List<BaseProcess> processesInStep;

        /// \brief The finish detector.
        private TerminationDetectionAlgorithm finishDetector;

        /// \brief Amount to increment by.
        private int step = 0;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public RunningHandeler(MainWindow mainWindow, BaseNetwork network, bool inDebugMode)
        ///
        /// \brief Constructor.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ///
        /// \param mainWindow  The main window.
        /// \param network     The network.
        /// \param inDebugMode True to enable in debug mode, false to disable it.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public RunningHandeler(BaseNetwork network, bool inDebugMode)
        {
            this.network = network;
            this.inDebugMode = inDebugMode;
            if (inDebugMode)
            {
                processesInStep = network.Processes;
            }
            for (int idx = 0; idx < network.Processes.Count; idx++) vectorClock.Add(0);
            Logger.Init();
            Logger.SetVectorClock(vectorClock);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RunToEnd()
        ///
        /// \brief Executes to end operation.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RunToEnd(bool alreadyRunning = true)
        {
            network.or[bn.ork.SingleStepStatus] = false;
            processesInStep = network.Processes;
            runningProcesses = network.Processes;
            Run(alreadyRunning);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SingleStepAll()
        ///
        /// \brief Single step all.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SingleStepAll(bool alreadyRunning = true)
        {
            network.or[bn.ork.SingleStepStatus] = true;
            processesInStep = new List<BaseProcess>(network.Processes);
            runningProcesses = network.Processes;
            Run(alreadyRunning);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Run(bool activateFirstStep = true)
        ///
        /// \brief Runs the given activate first step.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ///
        /// \param activateFirstStep (Optional) True to activate first step.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Run(bool alreadyRunning = true, bool runFirstStep = true)
        {
            finishDetector = new TerminationDetectionAlgorithm();
            if (!alreadyRunning)
            {
                network.Activate(inDebugMode, runFirstStep);
                return;
            }
            else
            {
                string processesActivated = "";
                foreach (BaseProcess process in processesInStep)
                {

                    process.BreakpointEvent.Set();
                    processesActivated += process.ea[ne.eak.Id].ToString() + ',';
                }
                Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler", "Run", "Processes activated " + processesActivated, "", "RunningHandler");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RunSingleStep(BaseProcess process)
        ///
        /// \brief Executes the single step operation.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ///
        /// \param process The process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RunSingleStep(BaseProcess process, bool alreadyRunning = true)
        {
            if (runningProcesses.Exists(p => p == process))
            {
                if (process.or[bp.ork.MessageQ].Count == 0)
                {
                    CustomizedMessageBox.Show("The process : " + process.ToString() + " does not have a waiting message", "Running Window Message", null, Icons.Error);
                    return;
                }

            }
            else
            {
                runningProcesses.Add(process);
            }
            network.or[bn.ork.SingleStepStatus] = true;
            processesInStep = new List<BaseProcess>() { process };
            Run(alreadyRunning);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void InitializeRunning()
        ///
        /// \brief Initializes the running.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void InitializeRunning()
        {
            if (inDebugMode)
            {
                CustomizedMessageBox.Show("The algorithem is running already press one processor to continue", "Running Window Message", null, Icons.Error);
            }
            else
            {
                Run(false,false);
                CustomizedMessageBox.Show("Press the processes to activate them \n Note that all the processes has to be activated", "Running window Message", null, Icons.Info);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Terminate()
        ///
        /// \brief Terminates this object.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Terminate(bool alreadyRunning = true)
        {
            if (!alreadyRunning)
            {
                MessageRouter.ReportFinishRunning();
                return;
            }
            //First empty all the message queues of all the processors
            foreach (BaseProcess process in network.Processes)
            {
                BaseMessage message = new BaseMessage(network);
                process.MessageQHandling(ref message, MessageQOperation.EmptyQueue);
                process.or[bp.ork.TerminationStatus] = BaseProcess.TerminationStatuses.NotTerminated;
            }

            //If the process read a message but waiting for the breakpoint event to be released
            //Replace the message waiting with an empty message
            foreach (BaseProcess process in network.Processes)
            {
                if (process.WaitingForBreakpointEvent)
                {
                    process.MessageInProcess = new BaseMessage(network);
                }
            }

            //Activate terminate off all the processors
            network.or[bn.ork.SingleStepStatus] = false;
            foreach (BaseProcess process in network.Processes)
            {
                process.Terminate();
            }

            //Activating the processes
            network.or[bn.ork.SingleStepStatus]= false;

            foreach (BaseProcess process in network.Processes)
            {
                process.BreakpointEvent.Set();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void MessageSendReport(BaseProcess process, BaseMessage message)
        ///
        /// \brief Message send report.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ///
        /// \param process The process.
        /// \param message The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MessageSendReport(BaseProcess process, BaseMessage message)
        {
            vectorClock[process.ea[ne.eak.Id]] += 1;
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler", "MessageSendReport", "MessageSendReport ", message, "RunningHandler");
            finishDetector.MessageSendReport(process, message);
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler TerminationAlgorithmStatus", "TerminationAlgorithmStatus", " TerminationAlgorithmStatus ", finishDetector.ReportStatus(), "RunningHandler TerminationAlgorithmStatus");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void MessageReceiveReport(BaseProcess process, BaseMessage message)
        ///
        /// \brief Message receive report.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ///
        /// \param process The process.
        /// \param message The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MessageReceiveReport(BaseProcess process, BaseMessage message)
        {
            vectorClock[process.ea[ne.eak.Id]] += 1;
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler", "MessageReceiveReport", " MessageReceiveReport ", message, "RunningHandler");
            if (finishDetector.MessageReceiveReport(process, message, network))
            {
                network.UpdateRunningStatus(new object[] { processesInStep });
                Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler TerminationAlgorithmStatus", "TerminationAlgorithmStatus", " TerminationAlgorithmStatus ", finishDetector.ReportStatus(), "RunningHandler TerminationAlgorithmStatus");
                Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler", "", "-----------------------End Step " + step.ToString() + " -------------", "", "RunningHandler");
                MessageRouter.UpdateInitRunningPresentation();
                step++;
            }
            else
            {
                Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler TerminationAlgorithmStatus", "TerminationAlgorithmStatus", " TerminationAlgorithmStatus ", finishDetector.ReportStatus(), "RunningHandler TerminationAlgorithmStatus");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void InternalOperationReport(BaseProcess process, string operationDescription)
        ///
        /// \brief Internal operation report.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ///
        /// \param process              The process.
        /// \param operationDescription Information describing the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void InternalOperationReport(BaseProcess process, string operationDescription)
        {
            vectorClock[process.ea[ne.eak.Id].Value] += 1;
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler", "InternalOperationReport", " InternalOperationReport ", operationDescription, "RunningHandler");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CheckFinishProcessingStep(BaseProcess process)
        ///
        /// \brief Check finish processing step.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Main
        /// \date 24/01/2017
        ///
        /// \param process The process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CheckFinishProcessingStep(BaseProcess process)
        {
            //Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler", "ProcessStartedReceiveIterationReport", " ProcessStartedReceiveIterationReport process id = " + process[ElementDictionaries.ElementAttributes].Value[NetworkElement.ElementAttributeKeys.Id].Value, "", "RunningHandler");
            if (finishDetector.CheckTermination(process, network))
            {
                network.UpdateRunningStatus(new object[] { processesInStep });
                Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler TerminationAlgorithmStatus", "TerminationAlgorithmStatus", " TerminationAlgorithmStatus ", finishDetector.ReportStatus(), "RunningHandler TerminationAlgorithmStatus");
                Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler" , "", " -----------------------End Step " + step.ToString() + " -------------", "", "RunningHandler");
                step++;
                MessageRouter.UpdateRunningPresentation();
            }
            else
            {
                Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "RunningHandler TerminationAlgorithmStatus", "TerminationAlgorithmStatus", " TerminationAlgorithmStatus ", finishDetector.ReportStatus(), "RunningHandler TerminationAlgorithmStatus");
            }
        }
    }
}
