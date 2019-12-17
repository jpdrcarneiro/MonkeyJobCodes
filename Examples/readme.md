# Examples
This will be populated

### Needed Packages
Most of the example files requires extra packages, provided below:

1. [Rhythm](https://dynamopackages.com/#) 
2. [BimorphNodes](https://dynamopackages.com/#)
3. [Archi-lab.net](https://archi-lab.net/)
4. [Lunchbox](https://provingground.io/tools/lunchbox/)

At this moment in time, all examples are related to Revit-Dynamo


* BatchUpgrade-MonkeyJobNodes.dyn - This dynamo scripts uses a custom node to open, save, and close file given a path. The example uses a python script to find all Revit projects (.rvt) and output a string. It is advised that you run this script twice, first at native version of the files, and remove the linked revit projects, and then run the script to upgrade. This 2-pass method have a better successful rate. 
<br>

* HideElementsInView-MonkeyJobNodes.dyn - This is a code that works similarly to the filter function as Revit main software.
<br>

* InputDataFromOldRevit-MonkeyJobNodes.dyn - This code was built to retrieve project parameters from an old revit project to a new and updated project version. This code can create duplicates if a project parameter already exists on elements. The code matches element based on the ElementID. 
<br>

* InputDataFromOldRevit(WithNoDuplicates)-MonkeyJobNodes.dyn - This is a working in progress, use with caution. The goal of this code is to fix the previous dynamo code and not create duplicated project parameter. 
<br>

* MatchDataBasedOnLocation.dyn - This is also a working progress, use with caution. The goal of this code is to match elements based on the location of element, instead of elementID. 
<br>

* OverrideColorInView-MonkeyJobNodes.dyn - This code changes element color in a view based on the existance of a project parameter or not. 
<br>

<p>obs.: All dynamos codes will be organized in groups</p>
<br>
The codes are organized in green groups for user input, blue for processes with no need for input, and red for output if existed. 
<!--
    
-->

