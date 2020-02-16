import time
import sys,os
import pandas as pd
import numpy as np
from nltk import *
from nltk.corpus import stopwords
import matplotlib.pyplot as plt
import string

from textblob import TextBlob

import spacy
#Loading laguage model 
nlp =  spacy.load('en_core_web_lg')

from spacy.matcher import Matcher
m_tool = Matcher(nlp.vocab)

def AddPosTag (activity, word):
    output = TextBlob(activity)
    for broken, tag in output.tags:
        if word == broken:
            return tag
        else:
            continue

def AddEntityRecognition (activity,word):
    temp = ne_chunk((pos_tag(str(activity).split())))
    for taggedWord in temp:
        if word == taggedWord[0]:
            return taggedWord[1]
        else:
            continue

def CreateActivityPatterns(activity):
    single_pattern = []
    activity = str(activity)
    activity = activity.lower()
    temp = str(activity).split()
    for j, word in enumerate(temp):
        if str(word) in string.punctuation:
            pattern = {'IS_PUNCT': True, 'OP':'*'}
        elif str(word).isdigit():
            pattern = {'IS_DIGIT': True}
        else:
            pattern = {'LOWER' : word , 'POS' : AddPosTag(activity, word),
                'ENT_TYPE' : AddEntityRecognition(activity, word)}
        single_pattern.append(pattern)
    return single_pattern


def Organize_Tokenize_Data(dataFrame, column):
    all_words = []
    for i, activity in enumerate(dataFrame[column]):
        activity = activity.lower()
        try:
            activity = activity.translate(activity.maketrans("","", string.punctuation))
        except:
            print(activity)
        tokenized_words = tokenize.word_tokenize(activity)
        for word in tokenized_words:
            all_words.append(word)
        temp = ne_chunk(pos_tag(tokenized_words))
        list_words = stopwords.words('english')
        temp = [word for word in temp if not word in list_words]
        dataFrame[column][i] = temp
    return all_words

#verify if word is in the most frequent list
def Classify_word_feature(word, all_words):
    word_features = list([i[0] for i in all_words.most_common(300)])
    result = word in word_features
    return result


def ComputeRunTime (intialTime):
    t1 = time.time()
    elapsed_time = (t1-intialTime)/60
    #print('time: ', str(elapsed_time))
    return 'time: ' + str(elapsed_time)


def Add_most_classification (dataFrame, columnName):
    for i, activity in enumerate(dataFrame[columnName]):
        for j, word in enumerate(dataFrame[columnName]):
            if (j >= len(dataFrame[columnName][i])):
                break
            dataFrame[columnName][i][j] = (dataFrame[columnName][i][j][0], dataFrame[columnName][i][j][1], Classify_word_feature(dataFrame[columnName][i][j][0]))
            # print (i, j, data['Activity Name'][i][j])
        # exit()
    return True

def OrganizeDict (dataFrame, col1, col2):
    returnDict = dict()
    for i, row in enumerate(dataFrame[col1]):
        returnDict[row] = dataFrame[col2][i]
    return returnDict