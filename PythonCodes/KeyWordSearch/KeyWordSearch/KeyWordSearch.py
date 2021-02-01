import sys, os
from multiprocessing import Process
import csv
import PyPDF2
from docx import Document
import pandas as pd
import extract_msg
import datetime
from pathlib import Path
import tensorflow_text as text
from tensorflow.strings import lower
import nltk
from nltk.stem import WordNetLemmatizer
from nltk.corpus import stopwords
import itertools
import string
import numpy as np
from time import sleep
import time
from datetime import timedelta

nltk.download('wordnet')
nltk.download('stopwords')

def RunTime(startTime):
    totalTime = time.time() - startTime
    print("Script Run Time = " + str(timedelta(seconds=totalTime)))
    return None

def OpenPDF(pdfPath):
    pdfFileObj = open( pdfPath, 'rb')
    pdfReader = PyPDF2.PdfFileReader(pdfFileObj)

    fileContent = []
    numberOfPages = pdfReader.getNumPages()
    print(str(range(numberOfPages)))
    for pageNumber in range(numberOfPages):
        print(pageNumber)
        pdfPage = pdfReader.getPage(pageNumber)
        pageContent = pdfPage.extractText()
        fileContent.append(pageContent)

    return fileContent

def OpenDocx(docxPath):
    docxDocument = Document(docxPath)
    fileContent = []
    for p in docxDocument.paragraphs:
        fileContent.append(p.text)
    return fileContent

def OpenExcel(xlsxPath):
    data = pd.read_excel(xlsxPath, index_col=0)
    return data.to_string()


def OpenMsg(msgPath):
    msg = extract_msg.Message(msgPath)

    #print (msg.sender)
    #print (msg.subject)
    #print (msg.date)
    #print (msg.body)

    data = [msg.subject, msg.body]

    return data

def CreateListOfFiles(dirPath):
    fileToCheck = []
    txtPath = dirPath + "\FilesToRead.txt"
    q = Path(txtPath)
    if q.exists ():
        mtime= datetime.datetime.fromtimestamp(q.stat().st_mtime).date()
    else:
        mtime = datetime.date(2020,1,1)
    if q.exists() == False or mtime != datetime.date.today() or q.stat().st_size <= 1:
        f = q.open(mode="w", buffering=-1, encoding=None, errors=None, newline=None)
        for root, dirs, files in os.walk(dirPath):
            for file in files:
                filePath = os.path.join(root, file)
                fileToCheck.append(filePath)
                filePathStr = str(filePath) + "," + "\n"
                f.write(filePathStr)
        f.close()
    else:
        f = q.open()
        spamreader = csv.reader(f, delimiter = ',', quotechar = "|")
        for file in spamreader:
            fileToCheck.append(file)
        f.close()
    
    return fileToCheck

def CheckFileType(file):
    fileType = str(file)
    fileType = fileType.split(".")[-1]
    return fileType

def OpenFile(filePath):
    fileText = []
    fileType = CheckFileType(filePath)
    if fileType == "pdf":
        f = OpenPDF(filePath)
        print ("pdf was opened")
    elif fileType == "docx" or fileType == "doc":
        f = OpenDocx(filePath)
        print("docx was opened")
    elif fileType == "xlsx" or fileType == "xls":
        f = OpenExcel(filePath)
        print("Excel file was opened")
    elif fileType == "msg":
        f = OpenMsg(filePath)
        print("MSG file was opened")
    else:
        #print ("file type %s" %fileType)
        return None
    return f

def keyWordProcessing(strKeyWord):
    strKeyWord = strKeyWord.translate(str.maketrans('', '', string.punctuation))
    strKeyWord = strKeyWord.lower()
    keyWordsList = strKeyWord.split()
    return keyWordsList


def Tokenize(strList):
    if type(strList[0]) == list:
        strList = list(itertools.chain(*strList))
    lemma = WordNetLemmatizer()
    lemmaList = list(map(lemma.lemmatize, strList))
    #print(lemmaList)
    if type(lemmaList[0]) == list or len(lemmaList)>= 2:
        for i, phrase in enumerate(lemmaList):
            lemmaList[i] = lemmaList[i].translate(str.maketrans('', '', string.punctuation))
    else:
        lemmaList[0] = lemmaList[0].translate(str.maketrans('', '', string.punctuation))
    #print(strList)
    lowerList = lower(strList)
    #print(lowerList)
    tokenizer =text.WhitespaceTokenizer()
    tokens = tokenizer.tokenize(lowerList)
    #print(tokens.to_list())
    try:
        tokensList = tokens.to_list()
    except AttributeError as error:
        tokensList = tokens.numpy().tolist()
    #print(tokensList)
    if type(tokensList[0]) == list:
        tokensList = list(itertools.chain(*tokensList))
    #print(tokensList)
    filteredList= []
    for word in tokensList:
        if word.decode('UTF-8') not in stopwords.words('english'):
            filteredList.append(word.decode('UTF-8'))
    #print(filteredList)
    return filteredList


def main():
    startTime = time.time()

    #testFiles = [r'D:\TestFiles\ECTA\DA-5270-ECTA-Stations 30% Submittal to DBJV\3_Bluebeam Sessions\ECTA_Architectural_Summary.pdf', 
    #             r'D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-STL-REV D IFC Vertical Structures VC1\2_QC\3_Bluebeam Sessions\594-132-2.4_Vertical Structure VC1_100% QCR - 709-893-848.pdf',
    #             r"D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-REV D 100% Viewing Platform VC2\2_QC\2_Reviews\DD and QC Process Documentation FormIFC View Platform VC2_ECTA 100%.docx",
    #             r"D:\TestFiles\ECTA\DA-5270-ECTA-Stations 30% Submittal to DBJV\3_Bluebeam Sessions\ECTA_Comm Systems.xlsx",
    #             r"D:\TestFiles\ECTA\L8PM-HDR-BRD-ECTA-STL-REV D IFC Vertical Structures VC1\2_QC\2_Reviews\FW_ Draft WITF_ ECTA and MSF Extreme Loading Implementation Reports.msg"]

    #dirPath = input("Type Directory: ")
    dirPath = r"D:\New folder"
    #dirPath = r"D:\TestFiles"

    #keyWords = input("Type Keywords separated by space: ")
    keywords = "tonage tonnage ton increase"
    keywords = keyWordProcessing(keywords)
    print("Creating file list........................................................")
    testFiles = CreateListOfFiles(dirPath)

    errorFiles = []
    matchedFiles = []
    noneFiles = []
    for file in testFiles:
        RunTime(startTime)
        try:
            print("Opening file............" + str(file))
            f = OpenFile(file)
            #add time out
        except Exception as e:
            errorFiles.append(str(file) + ", \n")
            print(e)
            continue
        try:
            print("tokenizing file..............................")
            fTokens = Tokenize(f)
        except TypeError as error:
            noneFiles.append(str(file) + ", \n")
            print(str(error))
            continue
        for word in keywords:
            print('word search in progress.................' + word)
            if word in fTokens and type(f) != None:
                matchedFiles.append(str(file) + ", \n")
    print("sving files......................")
    errorFilePath = dirPath + r"\NotOpenedFiles.txt"
    with open(errorFilePath, "w") as e:
        e.writelines(errorFiles)
        e.close()
    matchedFilesPath = dirPath + r"\MatchedFiles.txt"
    with open(matchedFilesPath, 'w') as e:
        e.writelines(matchedFiles)
        e.close() 
    matchedFiles = list(set(matchedFiles))
    noneFilesPath = dirPath + r"\NoneFiles.txt"
    with open(noneFilesPath, 'w') as e:
        e.writelines(noneFiles)
        e.close()
    noneFiles = list(set(noneFiles))
    print("Done! and Stats")
    RunTime(startTime)
    print('Total number of files = ' + str(len(testFiles)))
    print("Total Files not opened = " + str(len(errorFiles)))
    print("Total Matched = " + str(len(matchedFiles)))
    print("Total number of nones = " + str(len(noneFiles)))
    
    


if __name__ =="__main__":
    main()