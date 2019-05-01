////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Algorithms\Base\Breakpoint.cs
///
///\brief   Implements the breakpoint class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{ 
    class brp
    {
        public enum ork { Name, HostingElementType, Enable, Operator, Parameters, IndexInList, ListParameterOneOrAll, ListEvaluationMode, LastCheckResult, LastRunningResult }
    }   
    /**********************************************************************************************//**
     * A breakpoint.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class Breakpoint : NetworkElement
    {
       
        /**********************************************************************************************//**
         * Exception for signalling evaluation errors.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public class EvaluationException : Exception 
        { 
            /**********************************************************************************************//**
             * Constructor.
             *
             * \author  Ilan Hindy
             * \date    29/09/2016
             *
             * \param   message The message.
             
             **************************************************************************************************/

            public EvaluationException(string message):base(message){}
        }

        /**********************************************************************************************//**
         * A parameters.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private class Parameters
        {
            /** The first parameter. */
            public dynamic parameter1;
            /** The second parameter. */
            public dynamic parameter2;
        }

        /**********************************************************************************************//**
         * Values that represent element attribute keys.
        
         **************************************************************************************************/

        /**********************************************************************************************//**
         * Values that represent operators.
        
         **************************************************************************************************/

        public enum Operators { None, DataEqual, DataNotEqual, Equeal, NotEqual, Bigger, BiggerOrEqual, Smaller, SmalerOrEqual, Not, And, Or, OperationsOnOtherElements}

        /**********************************************************************************************//**
         * Values that represent message handling in other elements operations.
        
         **************************************************************************************************/

        public enum MessageHandlingInOtherElementsOperation{FirstMessageForEachProcessor, AllMessagesForEachProcessor, OneFromFirstMessages, AllMessages, OneMessage}

        /**********************************************************************************************//**
         * Values that represent hosting element types.
        
         **************************************************************************************************/

        public enum HostingElementTypes { Network, Process, Channel, Message };

        /**********************************************************************************************//**
         * Values that represent evaluation modes.
        
         **************************************************************************************************/

        public enum EvaluationMode { Running, Checking, None }

        /**********************************************************************************************//**
         * Values that represent list parameter one or alls.
        
         **************************************************************************************************/

        public enum ListParameterOneOrAll { One, AllList }

        /**********************************************************************************************//**
         * Values that represent list evaluation modes.
        
         **************************************************************************************************/

        public enum ListEvaluationModes { OneTrueFromAllValuesToAllValues, AllTrueFromAllValuesToAllValues, OneTrueFromAllValuesPairs, AllTrueFromAllVluesPairs }

        /**********************************************************************************************//**
         * Evaluate expression.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   operand1    The first operand.
         * \param   operand2    The second operand.
         *
         * \return  A bool.
         .
         **************************************************************************************************/

        delegate bool EvaluateExpression(dynamic operand1, dynamic operand2);
        /** The breakpoint for other elements. */
        private Breakpoint breakpointForOtherElements;
        /** The message handling for other element operation. */
        private MessageHandlingInOtherElementsOperation messageHandlingForOtherElementOperation;
        /** The network for other element operation. */
        private BaseNetwork networkForOtherElementOperation;

        /**********************************************************************************************//**
         * Convert this object into a string representation.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         *  Returns a string that represents the current object.
         *
         * \return  A string that represents the current object.
         .
         **************************************************************************************************/

        public override string ToString()
        {
            if (or.Count > 0)
            {
                return TypesUtility.GetKeyToString(or[brp.ork.HostingElementType]) + ":" + or[brp.ork.Name];
            }
            else
            {
                return "Brakepoint";
            }
        }

        /**********************************************************************************************//**
         * Default constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        //public Breakpoint(BaseNetwork network):base(network, true)
        //{
        //    Init(0);            
        //}

        /**********************************************************************************************//**
         * Constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   hostingElementType  Type of the hosting element.
         *                              
         **************************************************************************************************/
         //This constructor is for the deep copy it generates an empty breakpoint
         // which will be filled during the deep copy scan
        public Breakpoint() : base(true) { }

        public Breakpoint(HostingElementTypes hostingElementType):
            base(true)
        {
            Init(0);
            or[brp.ork.HostingElementType] = hostingElementType;
            or[brp.ork.Name] = TypesUtility.GetKeyToString(hostingElementType) + " Breakpoint";
        }

        /**********************************************************************************************//**
         * S.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   idx                 The index.
         * \param   clearPresentation   (Optional) True to clear presentation.
         *                              
         **************************************************************************************************/

        protected override void InitOperationResults()
        {
            or.Add(brp.ork.HostingElementType, new Attribute { Value = HostingElementTypes.Network, Changed = false });
            or.Add(brp.ork.Name, new Attribute { Value = " Breakpoint", Changed = false });
            or.Add(brp.ork.Enable, new Attribute { Value = true, Changed = false });
            or.Add(brp.ork.Operator, new Attribute { Value = Operators.Or, Changed = false });
            or.Add(brp.ork.IndexInList, new Attribute { Value = 0, Changed = false });
            or.Add(brp.ork.ListParameterOneOrAll, new Attribute { Value = ListParameterOneOrAll.One, Changed = false });
            or.Add(brp.ork.ListEvaluationMode, new Attribute { Value = ListEvaluationModes.OneTrueFromAllValuesToAllValues, Changed = false });
            or.Add(brp.ork.LastCheckResult, new Attribute { Value = new AttributeList(), Editable = false, Changed = false });
            or.Add(brp.ork.LastRunningResult, new Attribute { Value = new AttributeList(), Editable = false, Changed = false });
            or.Add(brp.ork.Parameters, new Attribute { Value = new AttributeList() });
        }

        /**********************************************************************************************//**
         * Creates this object.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   hostingElementType  Type of the hosting element.
         * \param   enable              true to enable, false to disable.
         * \param   oprtr               The oprtr.
         * \param   parameters          Options for controlling the operation.
         * \param   name                (Optional) The name.
         * \param   indexInList         (Optional) List of index INS.
         * \param   parameterOneToAll   (Optional) The parameter one to all.
         * \param   listEvaluationMode  (Optional) The list evaluation mode.
         *                              
         **************************************************************************************************/

        public void Create(HostingElementTypes hostingElementType,
            bool enable,
            Operators oprtr,
            AttributeList parameters,
            string name = "",
            int indexInList = 0,
            ListParameterOneOrAll parameterOneToAll = ListParameterOneOrAll.One,
            ListEvaluationModes listEvaluationMode = ListEvaluationModes.OneTrueFromAllValuesToAllValues)
        {
            or[brp.ork.HostingElementType] = hostingElementType;
            or[brp.ork.Enable] = enable;
            or[brp.ork.Operator] = oprtr;
            or[brp.ork.Parameters] = parameters;
            or[brp.ork.Name] = name;
            or[brp.ork.IndexInList] = indexInList;
            or[brp.ork.ListParameterOneOrAll] = parameterOneToAll;
            or[brp.ork.ListEvaluationMode] = listEvaluationMode;
        }

        /**********************************************************************************************//**
         * Creates results lists.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   processes   The processes.
         *                      
         **************************************************************************************************/

        public void CreateResultsLists(List<BaseProcess> processes)
        {
            int maxId = processes.Max(process => process.ea[ne.eak.Id]);
            AttributeList runningResults = or[brp.ork.LastRunningResult];
            AttributeList checkingResults = or[brp.ork.LastCheckResult];

            for (int idx = runningResults.Count; idx <= maxId; idx++)
            {
                runningResults.Add(new Attribute() { Value = "" });
                checkingResults.Add(new Attribute() { Value = "" });
            }
        }

        /**********************************************************************************************//**
         * Adds a parameter.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   parameter   The parameter.
         *                      
         **************************************************************************************************/

        public void AddParameter(Attribute parameter)
        {
            or[brp.ork.Parameters].Add(parameter);
        }

        /**********************************************************************************************//**
         * Sets single step.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public void SetSingleStep()
        {
            CreateSingleStepAll();
            //CreateSingleStepNext();
        }

        /**********************************************************************************************//**
         * Creates single step all.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private void CreateSingleStepAll()
        {
            Breakpoint breakpoint = new Breakpoint(or[brp.ork.HostingElementType]);
            AttributeList parameters = new AttributeList();
            parameters.Add(new Attribute { Value = bn.ork.SingleStepStatus});
            parameters.Add(new Attribute { Value = true});
            breakpoint.Create(HostingElementTypes.Network, true, Operators.Equeal, parameters, "Single Step");
            this.AddParameter(new Attribute { Value = breakpoint });
        }

        /**********************************************************************************************//**
         * Evaluates.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   evaluationMode  The evaluation mode.
         * \param   network         The network.
         * \param   process         The process.
         * \param   channel         The channel.
         * \param   message         The message.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        public bool Evaluate(EvaluationMode evaluationMode, BaseNetwork network, BaseProcess process, BaseChannel channel, BaseMessage message)
        {
            string resultString;
            bool result;
            if (or[brp.ork.Enable] == false)
            {
                resultString = "False : The breakpoint is enabeled";
                result = false;
            }
            else
            {
                try
                {
                    //Duplicate the list of parameters because it can change for the evaluation (in AdjustForOtherElementsBreakpoints)
                    //and the original list have to stay as the original
                    AttributeList parametersList = new AttributeList(or[brp.ork.Parameters]);
                    AdjustForOtherElementsBreakpoints(parametersList, network);
                    AttributeList parameters = new AttributeList();
                    
                    for (int idx = 0; idx < parametersList.Count; idx++)
                    {
                        parameters.Add(EvaluateParameter(evaluationMode, network, process, channel, message, parametersList.GetAttribute(idx)));
                    }
                    if (parameters.Count == 0)
                    {
                        resultString = "False : There are no parameters";
                        result = false;
                    }
                    else
                    {
                        if (parameters.Count == 1)
                        {
                            //If unary value the operator will work on the first
                            //argument only the first argument is duplicatted
                            //for comaptibillity
                            parameters.Add(parameters.GetAttribute(0));
                        }

                        result = Evaluate(parameters);
                        resultString = result.ToString();
                    }
                }
                catch (EvaluationException exception)
                {
                    resultString = "False : " + exception.Message;
                    result = false;
                }
            }
            int processId = process.ea[ne.eak.Id];
            CreateResultsLists(network.Processes);
            if (evaluationMode == EvaluationMode.Running)
            {
                
                or[brp.ork.LastRunningResult][processId] = resultString;
            }
            else if (evaluationMode == EvaluationMode.Checking)
            {
                or[brp.ork.LastCheckResult][processId] = resultString;
            }
            return result;
        }

        /**********************************************************************************************//**
         * Adjust for other elements breakpoints.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   parametersList  List of parameters.
         * \param   network         The network.
         
         **************************************************************************************************/

        private void AdjustForOtherElementsBreakpoints(AttributeList parametersList, BaseNetwork network)
        {
            // If the breakpoint is on other elements it has 4 parameters
            // 1. List of processors
            // 2. List of channels
            // 3. message handling mode
            // 4. breakpoint
            // The first 2 elements stayes and the other parameters are stored in member variables
            // Save the network in member
            // If the list of the channels is empty add -1 to the list - this will cause the evaluation method
            //    to evaluate with channel = null.
            // The -1 is inserted in a new list in order not effect the original list

            Operators o = or[brp.ork.Operator];
            if (o == Operators.OperationsOnOtherElements)
            {
                breakpointForOtherElements = ((Attribute)parametersList[3]).Value;
                messageHandlingForOtherElementOperation = ((Attribute)parametersList[2]).Value;
                networkForOtherElementOperation = network;
                parametersList.RemoveAt(3);
                parametersList.RemoveAt(2);
                if (((Attribute)parametersList[1]).Value.Count == 0)
                {
                    parametersList[1] = new Attribute() { Value = new List<int>() { -1 } };
                }
            }
        }
        
        /*
         * The possible parameters are:
         * key - in this case find the attribute
         * if the attribute found is a list 
         *  if only one value from the list is requested
         *      return the value according to an index
         *  else return the list
         * else (the value is not a list) return the value
         */

        /**********************************************************************************************//**
         * Evaluate parameter.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \exception   EvaluationException Thrown when an Evaluation error condition occurs.
         *
         * \param   evaluationMode  The evaluation mode.
         * \param   network         The network.
         * \param   process         The process.
         * \param   channel         The channel.
         * \param   message         The message.
         * \param   parameter       The parameter.
         *
         * \return  An Attribute.
         .
         **************************************************************************************************/

        private Attribute EvaluateParameter(EvaluationMode evaluationMode, BaseNetwork network, BaseProcess process, BaseChannel channel, BaseMessage message, Attribute parameter)
        {            
            if (parameter.Value.GetType().IsEnum)
            {
                if (!NetworkElementExist(network, process, channel, message))
                {
                    return new Attribute() { Value = false };
                }
                Attribute attribute = FindAttribute(network, process, channel, message, parameter.Value);
                if (attribute == null)
                {
                    string typeOfHostingElementStr = TypesUtility.GetKeyToString(or[brp.ork.HostingElementType]);
                    throw new EvaluationException("Could not find attribute " + TypesUtility.GetKeyToString(parameter.Value) + " In element of type " + typeOfHostingElementStr);
                }
                else
                {
                    if (Attribute.GetValueCategory(attribute) == Attribute.AttributeCategory.ListOfAttributes)
                    {
                        if (or[brp.ork.ListParameterOneOrAll] == ListParameterOneOrAll.One)
                        {
                            object[] elements = TypesUtility.InvokeMethodOfList(attribute.Value, "ToArray", new object[0], true);
                            int requestedIndex = or[brp.ork.IndexInList];
                            if (elements.Count() <= requestedIndex)
                            {
                                throw new EvaluationException("The index requested : " + requestedIndex.ToString() + " is larger or equal to the number of elements in the list : " + elements.Count());
                            }
                            else
                            {
                                if (Attribute.GetValueCategory(attribute) == Attribute.AttributeCategory.ListOfAttributes)
                                {
                                    return (Attribute)elements[requestedIndex];
                                }
                                else
                                {
                                    return new Attribute() { Value = elements[requestedIndex] };
                                }
                            }
                        }
                    }
                    return attribute;
                }
            }
            if (parameter.Value.GetType().Equals(typeof(Breakpoint)))
            {
                return new Attribute() { Value = parameter.Value.Evaluate(evaluationMode, network, process, channel, message) };
            }
            return parameter;
        }

        /**********************************************************************************************//**
         * Evaluates the given parameters.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   parameters  Options for controlling the operation.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool Evaluate(AttributeList parameters)
        {
            Operators Operator = or[brp.ork.Operator];
            switch (Operator)
            {
                case Operators.None: return false;
                case Operators.OperationsOnOtherElements: return Evaluate(parameters, (x, y) => { return EvaluateForOtherElements(x, y); });
                case Operators.DataEqual: return Evaluate(parameters, (x, y) => { return (x is NetworkElement)? x.EqualsTo(y):x.Equals(y); });
                case Operators.DataNotEqual: return Evaluate(parameters, (x, y) => { return (x is NetworkElement) ? !(x.EqualsTo(y)) : !(x.Equals(y)); });
                case Operators.And: return Evaluate(parameters, (x, y) => { return x && y; });
                case Operators.Bigger: return Evaluate(parameters, (x, y) => { return x > y; });
                case Operators.BiggerOrEqual: return Evaluate(parameters, (x, y) => { return x >= y; });
                case Operators.Equeal: return Evaluate(parameters, (x, y) => { return x == y; });
                case Operators.NotEqual: return Evaluate(parameters, (x, y) => { return x != y; });
                case Operators.Not: return Evaluate(parameters, (x, y) => { return !x; });
                case Operators.Or: return Evaluate(parameters, (x, y) => { return x || y; });
                case Operators.SmalerOrEqual: return Evaluate(parameters, (x,y) => {return x<=y;});
                case Operators.Smaller: return Evaluate(parameters, (x, y) => { return x < y; });
                default: return false;
            }
        }

        /**********************************************************************************************//**
         * Evaluate pair.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \exception   EvaluationException Thrown when an Evaluation error condition occurs.
         *
         * \param   prm1                    The first prm.
         * \param   prm2                    The second prm.
         * \param   evaluateExpressionFunc  The evaluate expression function.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool EvaluatePair(dynamic prm1, dynamic prm2,EvaluateExpression evaluateExpressionFunc)
        {
            try
            {
                return evaluateExpressionFunc(prm1, prm2);
            }
            catch(Exception e)
            {
                string operatorName = TypesUtility.GetKeyToString(or[brp.ork.Operator]);
                throw new EvaluationException("Error while evaluating " + operatorName + "(" + prm1.ToString() + "," + prm2.ToString() + ")" + "\n" + e.Message);
            }
        }

        /**********************************************************************************************//**
         * Evaluate for other elements.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   processId   Identifier for the process.
         * \param   channelId   Identifier for the channel.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool EvaluateForOtherElements(int processId, int channelId)
        {
            // Get the process
            BaseProcess processForEvaluation = networkForOtherElementOperation.Processes.First(p => p.ea[ne.eak.Id] == processId);

            // Get the channel
            // If the channel id is -1 - that meens that no channels where selected and the evaluation operation
            // is done with channel = null
            BaseChannel channelForEvaluation;
            if (channelId == -1)
            {
                channelForEvaluation = null;
            }
            else
            {
                channelForEvaluation = networkForOtherElementOperation.Channels.First(c => c.ea[ne.eak.Id] == channelId);
            }

            // Get the list of messages
            List<BaseMessage> messagesForEvaluation = GetMessagesForOtherElementsEvaluation(processForEvaluation);

            // If the list of the messages is empty activate the evaluation with message = null
            if (messagesForEvaluation.Count == 0)
            {
                messagesForEvaluation.Add(null);
            }

            //Evaluate the result
            if (messageHandlingForOtherElementOperation == MessageHandlingInOtherElementsOperation.OneFromFirstMessages ||
                messageHandlingForOtherElementOperation == MessageHandlingInOtherElementsOperation.OneMessage)
            {
                //At least one that's evaluation is true
                return messagesForEvaluation.Any(message => breakpointForOtherElements.Evaluate(EvaluationMode.None, networkForOtherElementOperation, processForEvaluation, channelForEvaluation, message));
            }
            else
            {
                //All evaluations are true -> not at least one is false
                return !messagesForEvaluation.Any(message => !breakpointForOtherElements.Evaluate(EvaluationMode.None, networkForOtherElementOperation, processForEvaluation, channelForEvaluation, message));
            }
        }

        /**********************************************************************************************//**
         * Gets messages for other elements evaluation.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   processForEvaluation    The process for evaluation.
         *
         * \return  The messages for other elements evaluation.
         .
         **************************************************************************************************/

        private List<BaseMessage> GetMessagesForOtherElementsEvaluation(BaseProcess processForEvaluation)
        {
            AttributeList messageAttributes = new AttributeList();
            AttributeList messagesToOneProcessor;
            switch(messageHandlingForOtherElementOperation)
            {
                case MessageHandlingInOtherElementsOperation.FirstMessageForEachProcessor:
                    messagesToOneProcessor = GetMessageQ(processForEvaluation);
                    if (messagesToOneProcessor.Count > 0)
                    {
                        messageAttributes.Add(messagesToOneProcessor[0]);
                    }
                    break;
                case MessageHandlingInOtherElementsOperation.AllMessagesForEachProcessor:
                    messagesToOneProcessor = GetMessageQ(processForEvaluation);
                    if (messagesToOneProcessor.Count > 0)
                    {
                        messageAttributes = messagesToOneProcessor;
                    }
                    break;
                case MessageHandlingInOtherElementsOperation.OneFromFirstMessages:
                    foreach (BaseProcess p in networkForOtherElementOperation.Processes)
                    {
                        messagesToOneProcessor = GetMessageQ(processForEvaluation);
                        if (messagesToOneProcessor.Count > 0)
                        {
                            messageAttributes.Add(messagesToOneProcessor[0]);
                        }
                    }
                    break;
                case MessageHandlingInOtherElementsOperation.OneMessage:
                    foreach (BaseProcess p in networkForOtherElementOperation.Processes)
                    {
                        messagesToOneProcessor = GetMessageQ(processForEvaluation);
                        if (messagesToOneProcessor.Count > 0)
                        {
                            messageAttributes = messageAttributes.Concat(GetMessageQ(p)).ToList().ToAttributeList();
                        }
                    }
                    break;
                case MessageHandlingInOtherElementsOperation.AllMessages:
                    foreach (BaseProcess p in networkForOtherElementOperation.Processes)
                    {
                        messagesToOneProcessor = GetMessageQ(processForEvaluation);
                        if (messagesToOneProcessor.Count > 0)
                        {
                            messageAttributes = messageAttributes.Concat(GetMessageQ(p)).ToList().ToAttributeList();
                        }
                    }
                    break;
            }
            List<BaseMessage> result = new List<BaseMessage>();
            messageAttributes.ForEach(attribute => result.Add(attribute.Value));
            return result;
        }

        /**********************************************************************************************//**
         * Gets message queue.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   p   The Process to process.
         *
         * \return  The message queue.
         .
         **************************************************************************************************/

        private AttributeList GetMessageQ(BaseProcess p)
        {
            return  p.or[bp.ork.MessageQ];
        }

        /**********************************************************************************************//**
         * Evaluates.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   parameters              Options for controlling the operation.
         * \param   evaluateExpressionFunc  The evaluate expression function.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool Evaluate(AttributeList parameters, EvaluateExpression evaluateExpressionFunc)
        {
            List<List<dynamic>> parametersValuesLists = CreateParametersLists(parameters);
            List<bool> evaluationResults = new List<bool>();
            for (int idx = 0; idx < parametersValuesLists.Count - 1; idx++)
            {
                evaluationResults.Add(Evaluate(parametersValuesLists[idx], 
                    parametersValuesLists[idx + 1],
                    evaluateExpressionFunc));
            }
            return EvaluateResults(evaluationResults);
        }

        /**********************************************************************************************//**
         * Creates parameters lists.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   parameters  Options for controlling the operation.
         *
         * \return  The new parameters lists.
         .
         **************************************************************************************************/

        private List<List<dynamic>> CreateParametersLists(AttributeList parameters)
        {
            List<List<dynamic>> parametersValuesLists = new List<List<dynamic>>();
            foreach (dynamic parameter in parameters)
            {
                List<dynamic> parameterValuesList = new List<dynamic>();
                Attribute.AttributeCategory category = Attribute.GetValueCategory(parameter);
                switch (category)
                {
                    case Attribute.AttributeCategory.ListOfAttributes:
                        foreach (Attribute attribute in parameter.Value)
                        {
                            parameterValuesList.Add(attribute.Value);
                        }
                        break;
                    case Attribute.AttributeCategory.PrimitiveAttribute:
                        parameterValuesList.Add(parameter.Value);
                        break;
                    case Attribute.AttributeCategory.NetworkElementAttribute:
                        parameterValuesList.Add(parameter.Value);
                        break;
                    case Attribute.AttributeCategory.AttributeDictionary:
                        parameterValuesList.Add(parameter.Value);
                        break;
                }
                parametersValuesLists.Add(parameterValuesList);
            }
            return parametersValuesLists;
        }

        /**********************************************************************************************//**
         * Evaluates.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   list1                   The first list.
         * \param   list2                   The second list.
         * \param   evaluateExpressionFunc  The evaluate expression function.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool Evaluate(List<dynamic> list1, List<dynamic> list2, EvaluateExpression evaluateExpressionFunc)
        {
            if (list1.Count == 0 || list2.Count == 0) return false;
            ListEvaluationModes evaluationMode = or[brp.ork.ListEvaluationMode];
            switch (evaluationMode)
            {
                case ListEvaluationModes.AllTrueFromAllValuesToAllValues:
                    return EvaluateAllToAll(list1, list2, evaluateExpressionFunc);
                case ListEvaluationModes.OneTrueFromAllValuesToAllValues:
                    return EvaluateAllToAll(list1, list2, evaluateExpressionFunc);
                case ListEvaluationModes.AllTrueFromAllVluesPairs:
                    return EvaluateByPairs(list1, list2, evaluateExpressionFunc);
                case ListEvaluationModes.OneTrueFromAllValuesPairs:
                    return EvaluateByPairs(list1, list2, evaluateExpressionFunc);
                default: return false;
            }
        }

        /**********************************************************************************************//**
         * Evaluate all to all.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   list1                   The first list.
         * \param   list2                   The second list.
         * \param   evaluateExpressionFunc  The evaluate expression function.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool EvaluateAllToAll(List<dynamic> list1, List<dynamic> list2, EvaluateExpression evaluateExpressionFunc)
        {
            ListEvaluationModes evaluationMode = or[brp.ork.ListEvaluationMode];
            List<Parameters> allPossiblePairs = new List<Parameters>();
            foreach (dynamic parameterFromList1 in list1)
                foreach (dynamic parameterFromList2 in list2)
                    allPossiblePairs.Add(new Parameters() { parameter1 = parameterFromList1, parameter2 = parameterFromList2 });

            if (evaluationMode == ListEvaluationModes.AllTrueFromAllValuesToAllValues)
            {
                return !allPossiblePairs.Any(pair => EvaluatePair(pair.parameter1, pair.parameter2, evaluateExpressionFunc) == false);
            }
            else // evaluationMode == ListEvaluationModes.OneTrueFromAllValuesToAllValues 
            {
                return allPossiblePairs.Any(pair => EvaluatePair(pair.parameter1, pair.parameter2, evaluateExpressionFunc) == true);
            }
        }

        /**********************************************************************************************//**
         * Evaluate by pairs.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   list1                   The first list.
         * \param   list2                   The second list.
         * \param   evaluateExpressionFunc  The evaluate expression function.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool EvaluateByPairs(List<dynamic> list1, List<dynamic> list2, EvaluateExpression evaluateExpressionFunc)
        {
            ListEvaluationModes evaluationMode = or[brp.ork.ListEvaluationMode];
            if (list1.Count != list2.Count)
            {
                if (evaluationMode == ListEvaluationModes.AllTrueFromAllVluesPairs)
                {
                    return false;
                }
            }

            int count = Math.Min(list1.Count, list2.Count);
            List<Parameters> allPossiblePairs = new List<Parameters>();
            for (int idx = 0; idx < count; idx++)
                allPossiblePairs.Add(new Parameters() { parameter1 = list1[idx], parameter2 = list2[idx] });

            if (evaluationMode == ListEvaluationModes.AllTrueFromAllVluesPairs)
            {
                return !allPossiblePairs.Any(pair => EvaluatePair(pair.parameter1, pair.parameter2, evaluateExpressionFunc) == false);
            }
            else  //evaluationMode == ListEvaluationModes.OneTrueFromAllValuesPairs 
            {
                return allPossiblePairs.Any(pair => EvaluatePair(pair.parameter1, pair.parameter2, evaluateExpressionFunc) == true);
            }
        }

        /**********************************************************************************************//**
         * Evaluate results.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   evaluationResults   The evaluation results.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool EvaluateResults(List<bool> evaluationResults)
        {
            ListEvaluationModes evaluationMode = or[brp.ork.ListEvaluationMode];
            if (evaluationMode == ListEvaluationModes.AllTrueFromAllValuesToAllValues ||
                evaluationMode == ListEvaluationModes.AllTrueFromAllVluesPairs)
            {
                return !evaluationResults.Any(evaluationResult => evaluationResult == false);
            }
            else
            {
                return evaluationResults.Any(evaluationResult => evaluationResult == true);
            }
        }

        /**********************************************************************************************//**
         * Network element exist.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   network The network.
         * \param   process The process.
         * \param   channel The channel.
         * \param   message The message.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool NetworkElementExist(BaseNetwork network, BaseProcess process, BaseChannel channel, BaseMessage message)
        {
            HostingElementTypes hostingElementType = or[brp.ork.HostingElementType];
            switch (hostingElementType)
            {
                case HostingElementTypes.Network:
                    if (network == null) return false;
                    break;
                case HostingElementTypes.Process:
                    if (process == null) return false;
                    break;
                case HostingElementTypes.Channel:
                    if (channel == null) return false;
                    break;
                case HostingElementTypes.Message:
                    if (message == null) return false;
                    break;
                default:
                    return false;
            }
            return true;
        }

        /**********************************************************************************************//**
         * Searches for the first attribute.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \exception   EvaluationException Thrown when an Evaluation error condition occurs.
         *
         * \param   network The network.
         * \param   process The process.
         * \param   channel The channel.
         * \param   message The message.
         * \param   key     The key.
         *
         * \return  The found attribute.
         .
         **************************************************************************************************/

        private Attribute FindAttribute(BaseNetwork network, BaseProcess process, BaseChannel channel, BaseMessage message, dynamic key)
        {
            HostingElementTypes hostingElementType = or[brp.ork.HostingElementType];
            Attribute attribute;
            switch (hostingElementType)
            {
                case HostingElementTypes.Network:
                    if ((attribute = network.FindAttribute(TypesUtility.GetKeyToString(key))) != null) return attribute;
                    break;
                case HostingElementTypes.Process:
                    if ((attribute = process.FindAttribute(TypesUtility.GetKeyToString(key))) != null) return attribute;
                    break;
                case HostingElementTypes.Channel:
                    if ((attribute = channel.FindAttribute(TypesUtility.GetKeyToString(key))) != null) return attribute;
                    break;
                case HostingElementTypes.Message:
                    if ((attribute = message.FindAttribute(TypesUtility.GetKeyToString(key))) != null) return attribute;
                    break;
                default:
                    throw new EvaluationException("Unknown hots type : " + TypesUtility.GetKeyToString(hostingElementType));
            }
            throw new EvaluationException("There is no attribute " + TypesUtility.GetKeyToString(key) +
                " In " + TypesUtility.GetKeyToString(hostingElementType));
        }
    }
}
