# Enable Python support and load DesignScript library
import clr
clr.AddReference('ProtoGeometry')
from Autodesk.DesignScript.Geometry import *

# The inputs to this node will be stored as a list in the IN variables.
dataEnteringNode = IN

filePath = IN[0]

result = IN[1]

originalDirectory = IN[2]

OUT = []

for i, x in enumerate(filePath):
	tempString = str(x) + "|" + result[i] + "\n"
	OUT.append(tempString)
# Place your code below this line

txtLog = open(str(originalDirectory)+"\BatchUpgradeLog.txt", "w+")
txtLog.writelines(OUT)
txtLog.close()