import sys, os
import csv
import PyPDF2
from docx import Document
import pandas as pd
import extract_msg
import datetime
from pathlib import Path


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

    #for p in docxDocument.paragraphs:
        #print (p.text)
    return docxDocument

def OpenExcel(xlsxPath):
    data = pd.ExcelFile(xlsxPath)
    return data

def OpenMsg(msgPath):
    msg = extract_msg.Message(msgPath)

    #print (msg.sender)
    #print (msg.subject)
    #print (msg.date)
    #print (msg.body)

    return msg

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


def main():

    print(OpenPDF(r'D:\TestFile\ECTA\1073 ECTA 100 PCT – ARCH P7 STR PED WALKWAY(P1 P7 VP)\3_Bluebeam Sessions\DA5277-ECTA-03_P7_Ped Walkway_100pct_QC_3.pdf'))

    exit()

    #dirPath = input("Type Directory: ")
    dirPath = r"D:\TestFile"
    #keyWord = input("Type Keyword: ")
    testFiles = CreateListOfFiles(dirPath)
    #print (testFiles)
    errorFiles = []
    for file in testFiles:
        try:
            f = OpenFile(file)
            #f.close()
        except:
            errorFiles.append(str(file) + ", \n")
            continue
    errorFilePath = dirPath + r"\NotOpenedFiles.txt"
    with open(errorFilePath, "w") as e:
        e.writelines(errorFiles)
        e.close()
    print('Total number of files = ' + str(len(testFiles)))
    print("Total Files not opened = " + str(len(errorFiles)))

    
    


if __name__ =="__main__":
    main()