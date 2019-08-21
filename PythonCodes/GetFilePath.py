
# Enable Python support and load DesignScript library
import sys 
import clr
import System
from System import Array
from System.Collections.Generic import *
clr.AddReference('ProtoGeometry')
from Autodesk.DesignScript.Geometry import *

clr.AddReference("RevitNodes") # Import RevitNodes

clr.AddReference("RevitServices")
import RevitServices
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager

clr.AddReference('RevitAPI') #import clr
from Autodesk.Revit.DB import * 

pyt_path = r'C:\Program Files (x86)\IronPython 2.7\Lib'
sys.path.append(pyt_path)

import os
from os import path
import glob

path = IN[0]
files = []

for root, dirnames, filenames in os.walk(path):
	temp = [file for file in glob.glob(root + "**/*.rvt")]
	if temp != []:
		files.append(temp)
		
OUT = files		
