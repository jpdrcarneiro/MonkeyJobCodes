import sys,os
import pandas as pd
import numpy as np
import nltk as nl
import matplotlib.pyplot as plt

data = pd.read_csv("D:\Schedule\Schedule.csv", sep=",", skiprows=0, header=[1], encoding="utf-8")

data = pd.DataFrame(data)#.drop([])

#for col in data.columns:
#    print (col)
#print (data.head())

#isolating activity names
activity_names = data['Activity Name']

activity_names = list(activity_names)
words = []
for activity in activity_names:
    activity = activity.replace("\n", "")
    temp = activity.split(" ")
    words.append(temp)

words = np.concatenate(words)
words = list(words)

#Natural Lanquage Part
#Remove stop word
from nltk.corpus import stopwords

extra_words_clean = ['-', '1)', '2)', '\ufffd', '&', '/', "", ]

print(len(words))
sr = stopwords.words('english')
clean_tokens = words[:]
for token in words:
    if token in sr or token in extra_words_clean:
        words.remove(token)
print(len(words))

#Frequency ditribution
freq = nl.FreqDist(words)
word_frqdst = pd.DataFrame(columns=['word', 'value'])
for key, value in freq.items():
    try:
        #print(key + ": " + str(value))
        word_frqdst = word_frqdst.append({'word': key, 'value': int(value)}, ignore_index=True)
    except Exception as e:
        print(e)

word_frqdst = word_frqdst.sort_values(by=['value'])

word_frqdst.to_csv("D:\Schedule\WordFrqDst.csv")

word_frqdst.tail(10).plot(kind='line', x='word', y='value', color='red')
plt.show()




