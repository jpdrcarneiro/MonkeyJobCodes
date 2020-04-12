import sys, os 
from xlrd import open_workbook
from xlutils.copy import copy

new = open_workbook(r"C:\\Users\\CAR11899\\Documents\\Schedule_Repopulating\\LINXS-2002REC-Activities for BIM.xlsx")
old = open_workbook(r"C:\\Users\\CAR11899\\Documents\\Schedule_Repopulating\\LINXS-Current-BIM export 12 19 19 C only.xlsx")

new_write = copy(new)

worksheet_old = old.sheet_by_index(0)
worksheet_new = new.sheet_by_index(0)
worksheet_new_wr = new_write.get_sheet(0)



old_codes = dict()
new_codes = dict()

counter = 0

for row in range(2, worksheet_old.nrows):
    #for row in range(2, worksheet_old.nrows):
    if len(worksheet_old.cell_value(row,4)) > 0:
        #print(worksheet_old.cell_value(row,3))
        #print(worksheet_old.cell_value(row,4))
        old_codes[worksheet_old.cell_value(row,3)] = worksheet_old.cell_value(row,4)
    counter += 1
    if counter % 100 == 0:
        print(counter)

list_keys = old_codes.keys()

counter = 0

errors = []
string_error = "activity: {}, 4d_code: {}, row: {} \n"

for row in range(2, worksheet_new.nrows):
    key = worksheet_new.cell_value(row,3)
    value = worksheet_new.cell_value(row,4)
    if len(value) > 0:
        new_codes[key] = value
    if key in list_keys:
        if value !=  old_codes[key]:
            try:
                worksheet_new_wr.write(row,4, old_codes[key])
            except:
                errors.append(string_error.format(key, value, row))
                continue
    counter += 1
    if counter % 100 == 0:
        print(counter)

with open('C:\\Users\\CAR11899\\Documents\\Schedule_Repopulating\\errorLog.txt', 'w') as f:
    for item in errors:
        f.writelines(item)


new_write.save("C:\\Users\\CAR11899\\Documents\\Schedule_Repopulating\\LINXS-2002REC-Activities for BIM.xlsx")
print(len(old_codes), len(new_codes))