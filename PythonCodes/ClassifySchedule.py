import sys,os
import pandas as pd
import numpy as np
import nltk as nl

data = pd.read_csv("Data/LINXS-Current-2-Des-Activities.csv", sep=",", skiprows=0, header=[1], encoding="utf-8")

data = pd.DataFrame(data).drop([])

print (data)

