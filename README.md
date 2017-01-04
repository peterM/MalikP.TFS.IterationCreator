# Team-Foundation-Server-Automation
Automatically create Iteration hierarchy and assign dates

```
###################################################################################################################################
------------------------------------------------------------| Options |------------------------------------------------------------
To define variable 'Team Foundation Server Uri' use this argument: '-s:'
          Example: '-s:https://tfs.domain.com/tfs'
To define variable 'Team Foundation Server Collection Name' use this argument: '-c:'
          Example: '-c:DefaultCollection'
To define variable 'Team Foundation Server Project Name' use this argument: '-p:'
          Example: '-p:ALL'
          Example: '-p:MyProjectName'
To define variable 'Target Year' use this argument: '-t:'
          Example: '-s:https://tfs.domain.com/tfs'
-----------------------------------------------------------------------------------------------------------------------------------
Full command example:
           MalikP.TFS.ExecutionConsole.exe -s:https://tfs.domain.com/tfs -c:DefaultCollection -p:MyProjectName -t:2017
-----------------------------------------------------------------------------------------------------------------------------------
###################################################################################################################################
```


Example execution will create iterations for Year 2017 in project with this structure:
```
MyProjectName
   |-Year 2017
      |-January
      |-February
      |-March
      |-...
      |-...
      |-...
      |-December
```
