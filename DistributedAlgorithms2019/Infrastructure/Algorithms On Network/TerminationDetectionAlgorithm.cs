////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\TerminationDetectionAlgorithm.cs
///
///\brief   Implements the termination detection algorithm class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * A termination detection algorithm.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    class TerminationDetectionAlgorithm
    {
        /**********************************************************************************************//**
         * The process data.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        class ProcessData
        {
            /** true to initiator. */
            public bool initiator = false;
            /** Identifier for the parent. */
            public int parentId = -1;
            /** Number of childs. */
            public int numberOfChilds = 0;
            /** The process. */
            public BaseProcess process;
        }

        /*
         * Description of the algorithm:
         * Data for each processor:
         * - Parent process id
         * - Number of childrens
         * The algorithm creates a dynamic tree which increases and decreases.
         * The conditions to enter the tree are:
         * - If the process is initiator (It did not receive a message before it's first send)
         *      The process is added as a root for the tree When it sends it's first message
         *      (There can be many trees simultanissaly)
         * - If the process is not an initiator
         *      The process is added to the tree if it is not in the tree already when it 
         *      received it's first message
         * Handling the number of childrens
         * - When a process sends message it increases the number of the childrens
         * - When a process receives a message 
         *      if it is in the tree it decreases the number of childres in the sending process
         *      if it is not in the tree it enters to the tree and the number of childrens in the sending
         *          process stays the same
         * The conditions to leave the tree are
         * - If the process does not have childrens
         * - If the process stopped
         * If these conditions are fullfiled the process leaves the tree
         * When a process leaves the tree it decreases the number of childrens in the parent process
         *  and the parent process trys to leave the tree according to the same conditions
         */

        /** Information describing the processes. */
        private Dictionary<int, ProcessData> processesData = new Dictionary<int,ProcessData>();

        /*
         * Send message report
         * If the process has no record create a record as initiator (because it sended a message without
         *      recieving a message)
         * Increase the number of childrens
         */

        /**********************************************************************************************//**
         * Message send report.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sendingProcess  The sending process.
         * \param   message         The message.
         
         **************************************************************************************************/

        public void MessageSendReport(BaseProcess sendingProcess, BaseMessage message)
        {
            int messageSourceProcess = message.GetHeaderField(bm.pak.SourceProcess);
            int messageDestProcess = message.GetHeaderField(bm.pak.DestProcess);
            if (!processesData.ContainsKey(messageSourceProcess))
            {
                ProcessData processData = new ProcessData()
                {
                    initiator = true,
                    numberOfChilds = 1,
                    process = sendingProcess
                };
                processesData.Add(messageSourceProcess, processData);
            }
            else
            {
                processesData[messageSourceProcess].numberOfChilds++;
            }
        }

        /*
         * Receive message report
         * If the process has no record create a record as none initiator (because it received a message
         *      before it seneded a message)
         *      There is a possibility that the process is in sttopped status there for a termination has to be check starting the process
         * Else (If a recored exist for the receiving process)
         *      Decrease the number of childrens in the sending process
         *      Check if the sending process should terminate because the number of it's childrens might be 0
         *      
         * Else decrease the number of children in the sending process
         */

        /**********************************************************************************************//**
         * Message receive report.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   recivingProcess The reciving process.
         * \param   message         The message.
         * \param   network         The network.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        public bool MessageReceiveReport(BaseProcess recivingProcess, BaseMessage message, BaseNetwork network)
        {
            int messageSourceProcess = message.GetHeaderField(bm.pak.SourceProcess);
            int messageDestProcess = message.GetHeaderField(bm.pak.DestProcess);
            if (!processesData.ContainsKey(messageDestProcess))
            {
                ProcessData processData = new ProcessData() 
                {
                    initiator = false, 
                    numberOfChilds = 0, 
                    parentId = messageSourceProcess,
                    process = recivingProcess
                };
                processesData.Add(messageDestProcess, processData);
                return CheckTermination(recivingProcess, network);
            }
            else
            {
                processesData[messageSourceProcess].numberOfChilds --;
                BaseProcess process = network.Processes.First(p => p.ea[ne.eak.Id] == messageSourceProcess);
                return CheckTermination(process, network);
            }
        }

        /*
         * CheckTermination
         * There are 2 conditions for removing from the tree:
         * 1. There are no messages or the breakpoint evaluation returned true
         * 2. The number of childes is 0
         * The operation of this method is done:
         * 1. Every time that the process enters the receive loop iteration (because it's status might be stopped)
         * 2. Every time that the process ended receive loop iteration (because it's status might be stopped)
         * 3. Evry time the number of childrens reached 0
         * If these conditions fullfiled the following has to be done
         * 1.   If the process is initiator 
         *      1.1 remove the record from the dictionary
         *      1.2 check that there are no initiators left (this can be done
         *          by checking that there are no records in the dictionary)
         *          If this is the case return true
         * 2.   Decrease the number of childres in the parent process
         *      Remove the record from the dictionary
         *      return CheckTermination of the parent 
         */

        /**********************************************************************************************//**
         * Check termination.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process The process.
         * \param   network The network.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        public bool CheckTermination(BaseProcess process, BaseNetwork network)
        {
            
            AttributeList messageQueue = process.or[bp.ork.MessageQ];
            int processId = process.ea[ne.eak.Id];

            //If the list is empty return success
            if (processesData.Count == 0) return true;

            //If the process has no record - this is the beginning of a first iteration - return false
            if (!processesData.ContainsKey(processId)) return false;

            int parentId = processesData[processId].parentId;

            //Checking the conditions
            //1. If the message queue is empty the process stopped
            //2. If there is a true breakpoint the process stopped
            //3. If the process did not stop return false
            if (!(messageQueue.Count == 0 || process.WaitingForBreakpointEvent == true))  return false;

            //Check if the process has no children
            if (processesData[processId].numberOfChilds > 0) return false;

            //If the process finished remove it from the dictionary
            processesData.Remove(processId);

            //Check if all the processes terminated
            if (processesData.Count == 0) return true;

            //If this is not a root (and the process finished) check if the parent should terminate
            if (parentId != -1)
            {
                processesData[parentId].numberOfChilds --;
                BaseProcess parentProcess = network.Processes.First(p => p.ea[ne.eak.Id] == parentId);
                return CheckTermination(parentProcess, network);
            }
            return false;
        }

        /**********************************************************************************************//**
         * Reports the status.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  A string.
         .
         **************************************************************************************************/

        public string ReportStatus()
        {
            string result = "";
            foreach (var processDataEntry in processesData)
            {
                result += "\n\t ProcessId:" + processDataEntry.Key.ToString();
                result += " Parent:" + processDataEntry.Value.parentId.ToString();
                result += " Num Chileds:" + processDataEntry.Value.numberOfChilds.ToString();
                BaseProcess process = processDataEntry.Value.process;
                result += " Num of messages:" + process.or[bp.ork.MessageQ].Count.ToString();
                result += " breakpoint status:" + process.WaitingForBreakpointEvent;
            }
            return result;
        }
    }
}
