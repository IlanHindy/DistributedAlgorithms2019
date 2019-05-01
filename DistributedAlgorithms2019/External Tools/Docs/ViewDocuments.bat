REM cd "C:\Users\ilanh\Documents\Distributed Algorithms\Program\DistributedAlgorithms\docs\html"
cd %1%
cd "..\docs\html"
start chrome "file:///%CD%/index.html"
REM start chrome "file:///C:\Users\ilanh\Documents\Distributed Algorithms\Program\DistributedAlgorithms\docs\html\index.html"
REM start chrome "%1%"