
path = "/home/flip/Documents/project/HapStick/"

fileNames = list.files(path=paste(path, "prestudy", sep=""), pattern="*.csv", full.names=TRUE)
print(fileNames)

dataFrame = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, header=TRUE, sep=",")}))
print(is.data.frame(dataFrame))
print(ncol(dataFrame))
rows = nrow(dataFrame)
rows
head(dataFrame)

str(dataFrame)
names(dataFrame) <- c("subject", "depth", "shifting", "answer")
head(dataFrame)

dataFrameBK <- dataFrame
dataFrame <- dataFrameBK
dataFrame <- subset(dataFrame, subject!=4)
head(dataFrame)

unique(dataFrame$subject)
unique(dataFrame$depth)
unique(dataFrame$shifting)

rows = nrow(dataFrame)
rows




data <- subset(dataFrame, shifting>0)
#data <- subset(dataFrame, depth%in%c(55, 125, 195, 265))
head(data)
nrow(data)

library(plyr)
library(dplyr)
position   <- c()
positionN  <- c()
nCorrect   <- c()
nIncorrect <- c()
xset <- unique(data$shifting)
xset
xset <- sort(xset, decreasing=T)
xset

for(i in 1:length(xset))
{
    pos <- xset[i]
    pos
    data1 <- subset(data, shifting==pos)

    position <- c(position, pos)
    positionN <- c(positionN, nrow(data1))
    nCorrect <- c(nCorrect, nrow(subset(data1, answer=="True")))
    nIncorrect <- c(nIncorrect, nrow(subset(data1, answer=="False")))
}

dat <- data.frame(position, positionN, nCorrect, nIncorrect)
dat$p <- dat$nCorrect/(dat$nCorrect + dat$nIncorrect)
dat
dat <- dat[order(dat$p, decreasing = T),]
dat


library("ggplot2")
p1 <- ggplot(data = dat, aes(x = position, y = p)) + geom_point()
p1 <- p1 + labs(x= "Shift (positive) in cm", y= "Probability of correct decisions in %") + theme_minimal()

library("psyphy")
model <- glm(cbind(dat$nCorrect, dat$nIncorrect) ~ position, family = binomial(mafc.probit(2)))

xseq <- seq(min(xset)-5, max(xset)+5, len = 1000)  #I used, for example, a 1000 points
yseq <- predict(model, data.frame(position = xseq), type = "response")
curve <- data.frame(xseq, yseq)
p1 <- p1 + geom_line(data = curve, aes(x = xseq, y = yseq))

print("mean")
(m <- -coef(model)[[1]]/coef(model)[[2]])  #The brakets are to show the result
mean <- m

print("stdev")
(std <- 1/coef(model)[[2]])
standarddeviation <- std

p <- 0.9
print("threshold")
(thre <- qnorm((p - 0.5)/0.5, m, std))  #( p-.5)/.5 because the asymptote is .5
threOld <- thre

threshold <- data.frame(p, thre)

p1 <- p1 + geom_linerange(data = threshold, aes(x = thre, ymin = yseq[1], ymax = p, color ="#000099"),size=1.3) + scale_color_discrete(guide=FALSE)
#p1 <- p1 + geom_point(data = threshold, aes(x = thre, y = yseq[1]))
p1<- p1 + geom_text(data=threshold, aes(x = thre, y = yseq[1], label="Threshold"), vjust = 1,size = 6)


library("modelfree")
threshold_slope(yseq, xseq, 0.9)

library("plyr")
sampling <- function(df){
  n <- length(dat$position)
  size <- dat$nCorrect + dat$nIncorrect
  shifting <- dat$position
  nCorrect <- rbinom(n, size, prob = dat$p)
  nIncorrect <- size - nCorrect
  data.frame(shifting, nCorrect, nIncorrect)
}
fake.dat <- ddply(data.frame(sample=1:100), .(sample), sampling)

calculate.threshold <- function(df) {
    tryCatch({
        model <- glm(cbind(df$nCorrect, df$nIncorrect) ~ df$shifting, family = binomial(mafc.probit(2)))
        m <- -coef(model)[[1]]/coef(model)[[2]]
        std <- 1/coef(model)[[2]]
        p <- 0.9
        thre <- qnorm((p - 0.5)/0.5, m, std)
        data.frame(thre)
    }, error = function(e) message(paste("Fitting error in sample: ", unique(df$sample),
        "\n")), warning = function(w) message(paste("Fitting warning in sample: ",
        unique(df$sample), "\n")))
}
fake.thresholds <- ddply(fake.dat, .(sample), calculate.threshold)

png("/home/flip/Documents/project/HapStick/positive.png")
ci <- quantile(fake.thresholds$thre, c(0.025, 0.975))
ci.df <- data.frame(p, rbind(ci))
print("mean")
mean
print("std")
standarddeviation
print("threshold")
threOld
print("CI at 0.9")
ci.df
p1 <- p1 +geom_errorbarh(aes(xmax = ci.df$X97.5., xmin = ci.df$X2.5., y= 0.9), height = 0.01) + theme(axis.text= element_text(size=14, face="bold"),axis.title=element_text(size=21))
p1
dev.off()
