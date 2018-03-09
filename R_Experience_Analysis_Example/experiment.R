
path = "D:/experiment/"
fileNames = list.files(path=paste(path,"data/", sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrame = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, header=TRUE, sep=",", dec=".", stringsAsFactors=FALSE)}))
is.data.frame(dataFrame)
ncol(dataFrame)
nrow(dataFrame)
rows = nrow(dataFrame)
head(dataFrame, 30)

data = dataFrame[,c( 
    "Subject"
    ,"trial_id"
    ,"condition_id"
    ,"duration"
)]
head(data)

subjects = unique(data["Subject"])
sort(unlist(subjects))

nSubjects = nrow(subjects)
nSubjects

conditions = unique(data["condition_id"])
conditions

nConditions = nrow(conditions)
nConditions

conditionNames = c(
    "Increasing"
    ,"Decreasing"
    ,"NoFeedback"
)
conditionNames

nTrials = length(unlist(data["duration"]))
nTrials

nTrials/nConditions/nSubjects

################################################################################### Outliers

ExperimentTime = data$duration

ExperimentTime = subset(data, duration > 0.7, duration)
print(length(unlist(ExperimentTime)))
print(summary(unlist(ExperimentTime)))

boxplot(ExperimentTime)

threshold.upper = 0;
threshold.lower = 0;
threshold.factor = 1.5
lowerq = quantile(unlist(ExperimentTime))[2]
upperq = quantile(unlist(ExperimentTime))[4]
iqr = upperq - lowerq # IQR(ExperimentDiscrepancy) can be used as an alternative
threshold.upper = (iqr * threshold.factor) + upperq
print(threshold.upper)
threshold.lower = lowerq - (iqr * threshold.factor)
print(threshold.lower)

Outliers=list()
ExperimentClean=list()
for(t in unlist(ExperimentTime))
{
    if(t > threshold.upper || t < threshold.lower)
    {
        Outliers=c(Outliers, as.numeric(t))
    }
    else
    {
        ExperimentClean=c(ExperimentClean, as.numeric(t))
    }
}

Outliers = unlist(Outliers)
print(length(Outliers))
print(summary(Outliers))

ExperimentClean = unlist(ExperimentClean)
print(length(ExperimentClean))
print(summary(ExperimentClean))

boxplot(ExperimentClean)

time <- unlist(ExperimentClean)

################################################################################### Normality

hist(rnorm(2000)+3, breaks="FD") ## Example of normal distribution

qqnorm(rnorm(2000)+3)
qqline(rnorm(2000)+3)

#####

hist(time, breaks="FD")

qqnorm(time)
qqline(time)

shapiro.test(time)

ks.test(unique(time), "pnorm", mean=mean(time), sd=sd(time))

#####

ttime = log(time) # transformed time
print(summary(ttime))
ttime = ttime + abs(summary(ttime)[1])
print(summary(ttime))

ttime = sqrt(time) # transformed time

#####

hist(ttime, breaks="FD")

qqnorm(ttime)
qqline(ttime)

shapiro.test(ttime)

ks.test(unique(ttime), "pnorm", mean=mean(ttime), sd=sd(ttime))

################################################################################### Data for statistical tests

Participant = c();
Condition = c();
Time = c();
TTime = c();

for(i in 1:nrow(data))
{
    row <- data[i,]
    if( row["duration"] > 0.7 & row["duration"] < threshold.upper)
    {
        Participant = c(Participant, row["Subject"]);
        Condition = c(Condition, conditionNames[as.numeric(row["condition_id"])]);
        Time = c(Time, row["duration"]);
        TTime = c(TTime, sqrt(row["duration"]));
    }
}

################################################################################### ANOVA

Participant = unlist(Participant) 
Condition = unlist(Condition) 
TTime = unlist(TTime) 
Time = unlist(Time) 

anovaData <- data.frame(Participant, Condition, TTime, Time)
head(anovaData)

matrix <- with(anovaData, cbind(TTime[Condition==conditionNames[1]], TTime[Condition==conditionNames[2]], TTime[Condition==conditionNames[3]])) 
model <- lm(matrix ~ 1)
condition <- factor(conditionNames)

library(car)
options(contrasts=c("contr.sum", "contr.poly"))
aov <- Anova(model, idata=data.frame(condition), idesign=~condition, type="III")
summary(aov, multivariate=F)

################################################################################### Post-hoc

aovPH <- aov(unlist(TTime) ~ unlist(Condition), anovaData)
TukeyHSD(aovPH)

################################################################################### Means

INC = mean(unlist((subset(anovaData, Condition=='Increasing', select=Time))))
DEC = mean(unlist((subset(anovaData, Condition=='Decreasing', select=Time))))
NOF = mean(unlist((subset(anovaData, Condition=='NoFeedback', select=Time))))

(NOF-INC)*100/NOF # % of time reduced by INC condition
(NOF-DEC)*100/NOF # % of time reduced by DEC condition

###################################################################################

boxplot(split(anovaData$Time,anovaData$Condition),main='Time by Condition')

library("yarrr")
library("ggplot2")
par(mfrow = c(1, 1))

pirateplot(
    formula = Time ~ Condition,
    data = anovaData,
    main = "Selection Time",
    xlab = "Feedback Type",
    ylab = "Time to Find the Target (seconds)",
    line.fun = mean,
    bean.o = 0.12,
    bean.lwd = 2.0,
    point.pch = 16,
    point.lwd = 0,
    point.cex = 0.85,
    point.o = 0.1,
    bar.o = .45,
    gl.col = gray(0.95),
    back.col = gray(0.9),
    inf.o = 0.8,
    cut.min = 0,
    pal = "southpark"
)


















