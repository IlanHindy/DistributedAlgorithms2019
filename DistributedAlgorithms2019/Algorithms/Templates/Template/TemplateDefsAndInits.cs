using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms.Algorithms.Templates.Template
{    
    #region /// \name Enums for the Network
    public class n
    {
        public enum pak { }
        public enum ork { }
    }
    #endregion
    #region /// \name Enums for the Process
    public class p
    {
        public enum pak { }
        public enum ork { }
    }
    #endregion
    #region /// \name Enums for the Channel
    public class c
    {
        public enum pak { }
        public enum ork { }
    }
    #endregion
    #region /// \name Enums for the Message
    public class m
    {
        public enum MessageTypes { }
    }
    #endregion

    public partial class TemplateNetwork : BaseNetwork
    {
        
        protected override void InitPrivateAttributes()
        {
        
        }

        protected override void InitOperationResults()
        {
            base.InitOperationResults();

            // Add initialization of OperationResults here
        }
    }

    public partial class TemplateProcess : BaseProcess
    {
        protected override void InitPrivateAttributes()
        {
            base.InitPrivateAttributes();

            // Add initialization of the private attributes here
        }

        protected override void InitOperationResults()
        {
            base.InitOperationResults();

            // Add initialization of OperationResults here
        }
    }

    public partial class TemplateChannel : BaseChannel
    {
        protected override void InitPrivateAttributes()
        {
            base.InitPrivateAttributes();

            // Add initialization of the private attributes here
        }

        protected override void InitOperationResults()
        {
            base.InitOperationResults();

            // Add initialization of OperationResults here
        }
    }
    class TemplateDefsAndInits
    {
    }
}
