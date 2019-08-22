#Generate log.txt file with the results of the opening, save, and close node
#Good for a big list of projects
import clr
clr.AddReference('ProtoGeometry')
from Autodesk.DesignScript.Geometry import *

# The inputs to this node will be stored as a list in the IN variables.

filePath = IN[0]

#result of open, save, and close node from monkey job nodes
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