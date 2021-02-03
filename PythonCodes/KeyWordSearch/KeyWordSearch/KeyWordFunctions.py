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
    print("Script Run Time = " + str(timedelta(seconds=totalTime))+ "\n") 
    return None

def drawProgressBar(percent, barLen = 50):
    sys.stdout.write("\r")
    progress = ""
    for i in range(barLen):
        if i < int(barLen * percent):
            progress += "="
        else:
            progress += " "
    sys.stdout.write("[ %s ] %.2f%% \n" % (progress, percent * 100))
    sys.stdout.flush()


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
    if q.exists():
        mtime= datetime.datetime.fromtimestamp(q.stat().st_mtime).date()
    else:
        mtime = datetime.date(2020,1,1)
    if q.exists() == False or mtime != datetime.date.today() or q.stat().st_size <= 1:
        f = q.open(mode="w", buffering=-1, encoding=None, errors=None, newline=None)
        for root, dirs, files in os.walk(dirPath):
            for file in files:
                filePath = os.path.join(root, file)
                fileToCheck.append(filePath)
                filePathStr = str(filePath) + ","
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

def FlattenList(unflattenList):
    if len(unflattenList)>=1 and type(unflattenList[0]) == list:
        while type(unflattenList[0]) == list:
            unflattenList = list(itertools.chain(*unflattenList))
        strList = unflattenList
        return strList
    else: 
        return unflattenList

def SaveFiles(dirPath, testFiles, errorFiles, noneFiles, matchedFiles):
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
    
    print('Total number of files = ' + str(len(testFiles)))
    print("Total Files not opened = " + str(len(errorFiles)))
    print("Total Matched = " + str(len(matchedFiles)))
    print("Total number of nones = " + str(len(noneFiles)))

def SaveReadFiles(dirPath, readFiles):
    readFilesPath = dirPath + r"\ReadFiles.txt"
    with open(readFilesPath, 'w') as e:
        e.writelines(readFiles)
        e.close()

def ReadSavedData(dirPath):
    errorFilePath = dirPath + r"\NotOpenedFiles.txt"
    matchedFilesPath = dirPath + r"\MatchedFiles.txt"
    noneFilesPath = dirPath + r"\NoneFiles.txt"
    readFilesPath = dirPath + r"\ReadFiles.txt"
    readFiles = []
    noneFiles = []
    matchedFiles = []
    errorFiles =[]
    q = Path(readFilesPath)
    if q.exists ():
        f = q.open()
        spamreader = csv.reader(f, delimiter = ',', quotechar = "|")
        for file in spamreader:
            readFiles.append(file)
        f.close()
    q = Path(noneFilesPath)
    if q.exists ():
        f = q.open()
        spamreader = csv.reader(f, delimiter = ',', quotechar = "|")
        for file in spamreader:
            noneFiles.append(file)
        f.close()
    q = Path(matchedFilesPath)
    if q.exists ():
        f = q.open()
        spamreader = csv.reader(f, delimiter = ',', quotechar = "|")
        for file in spamreader:
            matchedFiles.append(file)
        f.close()
    q = Path(errorFilePath)
    if q.exists ():
        f = q.open()
        spamreader = csv.reader(f, delimiter = ',', quotechar = "|")
        for file in spamreader:
            errorFiles.append(file)
        f.close()
    errorFiles = FlattenList(errorFiles)
    matchedFiles = FlattenList(matchedFiles)
    noneFiles = FlattenList(noneFiles)
    readFiles = FlattenList(readFiles)
    return errorFiles, matchedFiles, noneFiles, readFiles