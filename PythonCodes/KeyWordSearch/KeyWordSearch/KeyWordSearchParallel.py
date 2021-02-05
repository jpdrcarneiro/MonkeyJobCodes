import os,sys
import multiprocessing
import time
from KeyWordFunctions import *
from functools import partial


def main():
    main.__name__ = "__main__"

    startTime = time.time()

    #testFiles = [r'D:\TestFiles\ECTA\DA-5270-ECTA-Stations 30% Submittal to DBJV\3_Bluebeam Sessions\ECTA_Architectural_Summary.pdf', 
    #            r'D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-STL-REV D IFC Vertical Structures VC1\2_QC\3_Bluebeam Sessions\594-132-2.4_Vertical Structure VC1_100% QCR - 709-893-848.pdf',
    #            r"D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-REV D 100% Viewing Platform VC2\2_QC\2_Reviews\DD and QC Process Documentation FormIFC View Platform VC2_ECTA 100%.docx",
    #            r"D:\TestFiles\ECTA\DA-5270-ECTA-Stations 30% Submittal to DBJV\3_Bluebeam Sessions\ECTA_Comm Systems.xlsx",
    #            r"D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-STL-REV D IFC Vertical Structures VC1\2_QC\2_Reviews\FW_ Draft WITF_ ECTA and MSF Extreme Loading Implementation Reports.msg"]


    #dirPath = input("Type Directory: ")
    dirPath = r"D:\TestFiles"
    #dirPath = r"D:\New folder"

    #print("Creating file list........................................................")
    testFiles = CreateListOfFiles(dirPath)
    testFiles = FlattenList(testFiles)

    #keyWords = input("Type Keywords separated by space: ")
    keywordsList = "tonage tonnage ton increase"
    keywordsList = keyWordProcessing(keywordsList)

    #processPool = multiprocessing.Pool(processes=len(testFiles))
    #processes determine the number of processes open
    processPool = multiprocessing.Pool(processes=10)
    partialFunc = partial(ProcessFile, stTime=startTime, keyWords = keywordsList)

    allFiles = processPool.map(partialFunc, testFiles)
    errorFiles = []
    matchedFiles = []
    noneFiles = []
    readFiles = []
    #print(allFiles)
    for file in allFiles:
        errorFiles.append(file[0])
        matchedFiles.append(file[1])
        noneFiles.append(file[2])
        readFiles.append(file[3])
    #errorFiles, matchedFiles, noneFiles, readFiles = processPool.map(partialFunc, testFiles)
    errorFiles = list(filter(None, errorFiles)) 
    matchedFiles = list(filter(None, matchedFiles)) 
    noneFiles = list(filter(None, noneFiles)) 
    readFiles = list(filter(None, readFiles))
    SaveReadFiles(dirPath, readFiles)
    SaveFiles(dirPath, testFiles, errorFiles, noneFiles, matchedFiles)

    RunTime(startTime)


if __name__=="__main__":
    main()