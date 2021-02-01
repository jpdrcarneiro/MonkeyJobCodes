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

from spacy.matcher import PhraseMatcher
phrase_matcher = PhraseMatcher(nlp.vocab)


from FunctionsNLTK import *
print("dependencies load complete !", ComputeRunTime(t0))


t0 = time.time()
data = pd.read_csv("D:\\Schedule\\20190913.csv", sep=",", skiprows=0, header=[1], encoding="utf-8")

data = pd.DataFrame(data)#.drop([])

del data['Delete This Row']
del data['Unnamed: 8']
del data['Unnamed: 9']
      
#Create tags and values for the schedule data
data['ActivityPattern'] = np.vectorize(CreateActivityPatterns)(data['Activity Name'])
data['boolValue'] = np.where(data['BIM 4D Code'].notnull() & data['BIM 4D Code'].str.len() >= 3, True, False)
#preparing data for classifier

training_set= data[['ActivityPattern', 'boolValue']] #OrganizeDict(data, 'ActivityPattern', 'boolValue')
#training = training.to_dict('records')
#training_set = dict(zip(training.ActivityPattern, training_set.boolValue))

#print (training)
print("Train data load, clean and organize complete!", ComputeRunTime(t0))
#exit()




t0 = time.time()
#Prepare test data to classifier
test_data = pd.read_csv("D:\\Schedule\\20191113.csv", sep=",", skiprows=0, header=[1], encoding="utf-8")
test_data = pd.DataFrame(test_data)#.drop([])

del test_data['Delete This Row']
del test_data['Unnamed: 8']
del test_data['Unnamed: 9']

#Create tags and values for the schedule data
test_data['ActivityPattern'] = np.vectorize(CreateActivityPatterns)(test_data['Activity Name'])
test_data['boolValue'] = np.empty()
#preparing data for classifier

test_set= data[['ActivityPattern', 'boolValue']] #OrganizeDict(data, 'ActivityPattern', 'boolValue')
print("Test data load, clean and organize complete!", ComputeRunTime(t0))
#exit()


t0 = time.time()
classifier = NaiveBayesClassifier.train(training_set)
print("Learn phase complete!", ComputeRunTime(t0))

t0 = time.time()
data_class = pd.DataFrame(classifier.classify_many(test_set))
print("Apply phase complete!", ComputeRunTime(t0))
data_class.to_csv('D:\\Schedule\\appliedSet.csv')


