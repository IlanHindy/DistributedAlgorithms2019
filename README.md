# DistributedAlgorithms2019
# Perpouse
The perpouse of the project is to create easy developing environment for developing Didtributted Algorithms.
Distributed Algorithms are utility algorithms that are used to handle different tasks in the network.
Like :  
- Get the status of all the processors in the network
- Know if a job handled by the network is done
- etc.

Distributted Algorithms like any messaging algorithms are composed from 2 parts
- The initialization (Sending the first messages)
- The message handling loop (Get the messages one by one and handle them)

Most of the algorithms are simple when redusing them to pseudo-code.
The perpouse of the program is to make the programming as neer to the 
pseudo-code as possible
# Note
I know this might not be the easiest way to acomplish this task but my
perpouse was also to learn C# and WPF
# Status
Not all the features work and check.
To get general impression of the project limit youself to what suggested 
in the following section
# Download
- The project is a Visual Studio project and work within the Visual Studio 2019
- Use the Download to Visual Studio option of GiHub
# Get initial impression
- The are 3 algorithms working
  - ChandyLamport_OneRound (Get a snapshot of the status of the processors)
  - ChandyLamport (Get continuouse snapshot of the processors and report this snapshot to the Initiator)
  - ChandyLamport_NewStyle (The same as above but with programming style close to pseudo code)
- To see a running of an algorithm select open and select one of the existing networks
- After the network was loaded select the "V" button and then the "hummer" button
- The are 3 options to run the algorithms :
  - The blue running button - Run untile the end of the algorithm
  - The orange walking button - Perform one cycle in each processor (handle one message)
  - The magenta button starts the algorithm but with no message sending. After that you 
  can click on the processors to activate one cycle at each processor
  - A combination of the action is also available
  - You can see the description and the pseudo-code of the algorithm under the "Documents"
  menue entry. 
  
  # What else the program does
  - Automatic code generation which is ment to create everithing you need to program only what is needed (meens only the algorithm in pseudo-code)
  - Building a network and design running test (meeds when messages will be sent, Processor internal events will occure etc.)
  - An easy tool to generate the documents (It's main feature is to take word document and generate a document with only the needed parts and a pseudo code document)
  - Logging 
  - Saving and loading to XML files
  - And much more
  
