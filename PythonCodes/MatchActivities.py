import sys, os 
from xlrd import open_workbook
from xlutils.copy import copy

import openpyxl

import pandas as pd


new = "C:\\Users\\jpdrc\\Documents\\Schedule_Repopulating\\LINXS-2002REC-Activities for BIM.xlsx"
old = "C:\\Users\\jpdrc\\Documents\\Schedule_Repopulating\\LINXS-Current-BIM export 12 19 19 C only_JPC.xlsx"

new_workbook = pd.read_excel(new)
old_workbook =  pd.read_excel(old)


old_codes = dict()
new_codes = dict()

counter = 0

for row in range(3, len(old_workbook.index)):
    #for row in range(2, worksheet_old.nrows):
    if pd.notna(old_workbook['Column5'][row]) and len(str(old_workbook['Column5'][row])) > 2:
        old_codes[old_workbook['Column4'][row]] = old_workbook['Column5'][row]
    counter += 1
    if counter % 100 == 0:
        print(counter)

list_keys = old_codes.keys()

print(len(old_codes))

counter = 0

errors = []
string_error = "activity: {}, 4d_code: {}, row: {} \n"

for row in range(2, len(new_workbook.index)):
    key = new_workbook['task_name'][row]
    value = new_workbook['user_field_679'][row]
    if key in list_keys:
        new_codes[key] = value
        try:
            new_workbook['user_field_679'][row] = old_codes[key]
        except:
            errors.append(string_error.format(key, value, row))
            continue
    counter += 1
    if counter % 100 == 0:
        print(counter)

print(len(old_codes), len(new_codes))

with open('C:\\Users\\jpdrc\\Documents\\Schedule_Repopulating\\errorLog.txt', 'w') as f:
    for item in errors:
        f.writelines(item)

new_workbook.to_excel("C:\\Users\\jpdrc\\Documents\\Schedule_Repopulating\\LINXS-2002REC-Activities for BIM(updated).xlsx")

