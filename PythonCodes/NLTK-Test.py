import sys,os
import pandas as pd
import numpy as np
from nltk import *
from nltk.corpus import stopwords
import matplotlib.pyplot as plt
import string

data = pd.read_csv("D:\Schedule\Schedule.csv", sep=",", skiprows=0, header=[1], encoding="utf-8")

data = pd.DataFrame(data)#.drop([])

#cleaning and organizing

all_words = []

for i, activity in enumerate(data['Activity Name']):
    activity = activity.lower()
    try:
        activity = activity.translate(activity.maketrans("","", string.punctuation))
    except:
        print(activity)
    tokenized_words = tokenize.word_tokenize(activity)
    for word in tokenized_words:
        all_words.append(word)
    temp = ne_chunk(pos_tag(tokenized_words))
    # print (temp)
    # lemma_model = stem.WordNetLemmatizer()
    # for i, word in enumerate(temp):
    #     print (word)
    #     try:
    #         temp[i] = lemma_model.lemmatize(word)
    #     except:
    #         print(temp[i])
    list_words = stopwords.words('english')
    temp = [word for word in temp if not word in list_words]
    data['Activity Name'][i] = temp

all_words = [word for word in all_words if not word in list_words]
#print (data['Activity Name'])

freq_dist = FreqDist(all_words)
print (freq_dist.most_common(20))
