import sys,os
import pandas as pd
import numpy as np
from nltk import *
from nltk.corpus import stopwords
import matplotlib.pyplot as plt
import string
import timeit


data = pd.read_csv("D:\\Schedule\\20190913.csv", sep=",", skiprows=0, header=[1], encoding="utf-8")

data = pd.DataFrame(data)#.drop([])

# print(data.head())

only_4d_coded = data[data['BIM 4D Code'].notnull()]
# print(only_4d_coded.size , data.size) 
only_4d_coded = only_4d_coded[only_4d_coded['BIM 4D Code'].str.len() >= 3]

# print(only_4d_coded.head())
# print(only_4d_coded.size , data.size) 

#cleaning and organizing



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
#print (data['Activity Name'])

all_words = FreqDist(all_words)
print (all_words.most_common(20))
word_features = list([i[0] for i in all_words.most_common(300)])
# print (word_features)
# exit()

#verify if word is in the most frequent list
def Classify_word_feature(word):
    result = word in word_features
    return result

Organize_Tokenize_Data(data, 'Activity Name')

for i, activity in enumerate(data['Activity Name']):
    for j, word in enumerate(data['Activity Name']):
        if (j >= len(data['Activity Name'][i])):
            break
        data['Activity Name'][i][j] = (data['Activity Name'][i][j][0], data['Activity Name'][i][j][1], Classify_word_feature(data['Activity Name'][i][j][0]))
        print (i, j, data['Activity Name'][i][j])
    exit()



elapsed_time = timeit.timeit(code_to_test, number=100)/100
print('time: ',elapsed_time)
