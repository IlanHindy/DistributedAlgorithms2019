using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;
using DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle;

namespace DistributedAlgorithms
{
    public class DrivedTest : BaseMessage { }
    public class BaseMessageTest
    {
        List<MessageBoxElementData> results = new List<MessageBoxElementData>();
        public BaseMessageTest(BaseMessage source, dynamic messageType)
        {
            MainWindow.ActivationPhase = MainWindow.ActivationPhases.Temp;
            results.Add(new MessageBoxElementData(TypesUtility.GetKeyToString(messageType)));
            TestResults(source);
            TestResults(source, false, "source");
            TestResults(source, true, "source");
            TestResults(new BaseMessage());
            TestResults(new BaseMessage(source.Network));
            TestResults(new BaseMessage(source.Network, source));
            //TestResults(new BaseMessage(source.Network, source.MessageString()));
            TestResults(new BaseMessage(source.Network, source, new BaseChannel(source.Network, 10, 1, 2)));
            TestResults(new BaseMessage(source.Network, bm.MessageTypes.Forewared, new BaseChannel(source.Network, 10, 1, 2), "Name", 0, 0));
            TestResults(new BaseMessage(source.Network, bm.MessageTypes.Forewared, 10, 1100, 11, 1111, "Name", 0, 0));
            PrintResults();
            MainWindow.ActivationPhase = MainWindow.ActivationPhases.Running;

        }

        private void TestResults(BaseMessage message, bool DriveToDt = false, string sourceName = " new ")
        {
            dynamic result = null;
            if (!DriveToDt)
            {
                result = message as ChandyLamport_NewStyleMessage;
            }
            else
            {
                result = message as DrivedTest;
            }
            if (result == null)
            {
                results.Add(new MessageBoxElementData(results.Count + " null " + DriveToDt + " " + sourceName));
            }
            else
            {
                results.Add(new MessageBoxElementData(results.Count + " not null " + DriveToDt + " " + sourceName));
            }
        }

        private void PrintResults()
        {
            MessageRouter.CustomizedMessageBox(results);
        }
    }
}
