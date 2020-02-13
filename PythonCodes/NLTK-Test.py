import time
t0 = time.time()

import sys,os
import pandas as pd
import numpy as np
from nltk import *
from nltk.corpus import stopwords
import matplotlib.pyplot as plt
import string

import spacy
#Loading laguage model 
nlp =  spacy.load('en_core_web_lg')

from spacy.matcher import Matcher
m_tool = Matcher(nlp.vocab)

print("dependencies load complete !")


data = pd.read_csv("D:\\Schedule\\20190913.csv", sep=",", skiprows=0, header=[1], encoding="utf-8")

data = pd.DataFrame(data)#.drop([])

del data['Delete This Row']
del data['Unnamed: 8']
del data['Unnamed: 9']

# print(data.head())

only_4d_coded = data[(data['BIM 4D Code'].notnull()) & (data['BIM 4D Code'].str.len() >= 3)]
print("data load and clean complete!")

# print(only_4d_coded.size , data.size) 
# print(only_4d_coded)

# only_4d_coded = only_4d_coded[only_4d_coded['BIM 4D Code'].str.len() >= 3]

# print(only_4d_coded.head())
# print(only_4d_coded.size , data.size) 

#cleaning and organizing

def createActivityPatterns(dataFrame, colName):
    all_activities_patterns = []
    for i, activity in enumerate(dataFrame[colName]):
        single_pattern = []
        activity = activity.lower()
        temp = str(activity).split()
        for j, word in enumerate(temp):
            if str(word) in string.punctuation:
                pattern = {'IS_PUNCT': True}
            elif str(word).isdigit():
                pattern = {'IS_NUMBER': True}
            else:
                pattern = {'LOWER' : word}
            single_pattern.append(pattern)
        all_activities_patterns.append(single_pattern)
    return all_activities_patterns
        
new_patterns = createActivityPatterns(only_4d_coded, 'Activity Name')

print (new_patterns[0])g
exit()

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

all_words = Organize_Tokenize_Data(only_4d_coded, 'Activity Name')

all_words = [word for word in all_words if not word in stopwords.words('english')]
# print (data['Activity Name'])

all_words = FreqDist(all_words)
# print (all_words.most_common(20))
word_features = list([i[0] for i in all_words.most_common(300)])
# print (word_features)
# exit()

#verify if word is in the most frequent list
def Classify_word_feature(word):
    result = word in word_features
    return result

Organize_Tokenize_Data(data, 'Activity Name')

def Add_most_classification (dataFrame, columnName):
    for i, activity in enumerate(dataFrame[columnName]):
        for j, word in enumerate(dataFrame[columnName]):
            if (j >= len(dataFrame[columnName][i])):
                break
            dataFrame[columnName][i][j] = (dataFrame[columnName][i][j][0], dataFrame[columnName][i][j][1], Classify_word_feature(dataFrame[columnName][i][j][0]))
            # print (i, j, data['Activity Name'][i][j])
        # exit()
    return True

Add_most_classification(data, 'Activity Name')

data['4D classification'] = ((data['BIM 4D Code'].notnull()) & (only_4d_coded['BIM 4D Code'].str.len() >= 3))

test_data = pd.read_csv("D:\\Schedule\\20191113.csv", sep=",", skiprows=0, header=[1], encoding="utf-8")

test_data = pd.DataFrame(test_data)#.drop([])

del test_data['Delete This Row']
del test_data['Unnamed: 8']
del test_data['Unnamed: 9']

Organize_Tokenize_Data(test_data, 'Activity Name')

Add_most_classification(test_data, 'Activity Name')

# def ColsToDict(col1, col2):
#     newDict = {}
#     if (col1.size != col2.size):
#         return None
#     for i, value in enumerate(col1):
#         newDict[col1[i]] = col2[i]
#     return newDict

training_set = data[['Activity Name', '4D classification']].copy()
test_set = test_data[['Activity Name']].copy()
test_set['4D classification'] = None

# print(training_set)
# print(test_set)

classifier = NaiveBayesClassifier.train(training_set)
data_class = pd.DataFrame(classifier.classify_many(test_set))

data_class.to_csv('D:\\Schedule\\appliedSet.csv')

t1 = time.time()
elapsed_time = (t1-t0)/60
print('time: ', str(elapsed_time))
