import os, sys
import csv
import pandas as pd
import numpy as np


august = pd.read_csv("C:\\Users\\car11899\\Documents\\ScheduleComparison\\20190807.csv", sep=",", skiprows=0, header=[1])

september = pd.read_csv("C:\\Users\\car11899\\Documents\\ScheduleComparison\\20190913.csv", sep=",", skiprows=0, header=[1])

november = pd.read_csv("C:\\Users\\car11899\\Documents\\ScheduleComparison\\20191113.csv", sep=",", skiprows=0, header=[1])

# print (august.head())
# print (september.head())
# print (november.head())

print ("Overall 4D code Count:")
print (august['BIM 4D Code'].count())
print (september['BIM 4D Code'].count())
print (november['BIM 4D Code'].count())

print ("ECTA" in august['Activity Name'])


