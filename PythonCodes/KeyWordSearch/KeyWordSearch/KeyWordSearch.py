import sys, os
print(os.getcwd())
from KeyWordFunctions import *
import time

def main():
    startTime = time.time()

    #testFiles = [r'D:\TestFiles\ECTA\DA-5270-ECTA-Stations 30% Submittal to DBJV\3_Bluebeam Sessions\ECTA_Architectural_Summary.pdf', 
    #             r'D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-STL-REV D IFC Vertical Structures VC1\2_QC\3_Bluebeam Sessions\594-132-2.4_Vertical Structure VC1_100% QCR - 709-893-848.pdf',
    #             r"D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-REV D 100% Viewing Platform VC2\2_QC\2_Reviews\DD and QC Process Documentation FormIFC View Platform VC2_ECTA 100%.docx",
    #             r"D:\TestFiles\ECTA\DA-5270-ECTA-Stations 30% Submittal to DBJV\3_Bluebeam Sessions\ECTA_Comm Systems.xlsx",
    #             r"D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-STL-REV D IFC Vertical Structures VC1\2_QC\2_Reviews\FW_ Draft WITF_ ECTA and MSF Extreme Loading Implementation Reports.msg"]

    #dirPath = input("Type Directory: ")
    #dirPath = r"D:\New folder"
    dirPath = r"D:\TestFiles"

    #keyWords = input("Type Keywords separated by space: ")
    keywords = "tonage tonnage ton increase"
    keywords = keyWordProcessing(keywords)
    print("Creating file list........................................................")
    testFiles = CreateListOfFiles(dirPath)
    testFiles = FlattenList(testFiles)

    errorFiles = []
    matchedFiles = []
    noneFiles = []
    readFiles = []
    counter = -1
    for file in testFiles:
        if len(readFiles) == 0:
            errorFiles, matchedFiles, noneFiles, readFiles = ReadSavedData(dirPath)
            print(str(len(readFiles)))
        print(str(file))
        if file in readFiles:
            continue
        else:
            RunTime(startTime)
            counter += 1
            drawProgressBar(counter/len(testFiles))
            try:
                print("Opening file............" + str(file))
                f = OpenFile(file)
                if type(f)==None:
                    continue
                #add time out
            except Exception as e:
                errorFiles.append(str(file) + ",")
                print(e)
                continue
            try:
                print("tokenizing file..............................")
                fTokens = Tokenize(f)
            except TypeError as error:
                noneFiles.append(str(file) + ",")
                print(str(error))
                continue
            for word in keywords:
                print('word search in progress.................' + word)
                if word in fTokens and type(f) != None:
                    matchedFiles.append(str(file) + ",")
            print(RunTime(startTime))
            readFiles.append(str(file) + ",")
            SaveReadFiles(dirPath, readFiles)
            SaveFiles(dirPath, testFiles, errorFiles, noneFiles, matchedFiles)
        print(RunTime(startTime))
    SaveReadFiles(dirPath, readFiles)
    SaveFiles(dirPath, testFiles, errorFiles, noneFiles, matchedFiles)
    
    


if __name__ =="__main__":
    main()