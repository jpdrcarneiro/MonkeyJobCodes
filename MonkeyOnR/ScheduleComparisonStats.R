library(stringr)
library(plyr)
library(janitor)
library(openNLP)

august = read.csv("Data/20190807.csv")
august$status_code = NULL
august$delete_record_flag = NULL
august$start_date = NULL
august$end_date = NULL
august = august[-c(1),]
colnames(august)[3] = "4d_code"
august = august[(str_length(august$`4d_code`)>= 3), ]
august$location_4d = factor(substr(august$`4d_code`, 1, 2))
august$task_4d = factor(substr(august$`4d_code`,3, str_length(august$`4d_code`)))
summary(august)
table(august$location_4d)


september = read.csv("Data/20190913.csv")
september$status_code = NULL
september$delete_record_flag = NULL
september$start_date = NULL
september$end_date = NULL
september$X = NULL
september$X.1 = NULL
september = september[-c(1),]
colnames(september)[4] = "4d_code"
september = september[(str_length(september$`4d_code`)>= 3), ]
september$location_4d = substr(september$`4d_code`, 1, 2)
september$task_4d = substr(september$`4d_code`,3, str_length(september$`4d_code`))
summary(september)


november = read.csv("Data/20191113.csv")
november$status_code = NULL
november$delete_record_flag = NULL
november$start_date = NULL
november$end_date = NULL
november$X = NULL
november$X.1 = NULL
november = november[-c(1),]
colnames(november)[4] = "4d_code"
november = november[(str_length(november$`4d_code`)>= 3), ]
november$location_4d = substr(november$`4d_code`, 1, 2)
november$task_4d = substr(november$`4d_code`,3, str_length(november$`4d_code`))
summary(november)




total = merge(data.frame(table(august$location_4d)), 
              data.frame (table(september$location_4d)),
                    by="Var1")
total = merge(total, data.frame (table(november$location_4d)), by="Var1")
colnames(total)= c("location", "august", "september", "november")
row.names(total) = total$location
total$location = NULL
write.csv(total, "Output/Location_Summary.csv")
