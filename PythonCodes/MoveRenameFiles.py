#This script is not used inside of Dynamo, but it can be easily adapted to it

import sys, os
import fnmatch
import shutil

to_move = []
counter = 0
destination = os.path.normpath("C:\\Users\\jpdrc\\Documents\\Scripts\\TestFiles\\4D_MOT-WCTA-08-30-2019-OverallIray")
for root, dirnames, filenames in os.walk("C:\\Users\\jpdrc\\Documents\\Scripts\\TestFiles\\4D_MOT-CCTA-08-29-2019-OverallIray\\"):
    for filename in fnmatch.filter(filenames, "*copy*"):
        temp = "4D_MOT-WCTA-08292019-OverallView_" + str(counter).zfill(6) + ".jpg"
        shutil.move(os.path.join(root,filename), os.path.join(destination, temp) )
        to_move.append(os.path.join(root,filename))
        counter += 1

print (to_move)