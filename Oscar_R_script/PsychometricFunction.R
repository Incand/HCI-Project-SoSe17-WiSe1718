
path = "D:/Dropbox/Statistics/HapStick/"

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
p1

library("psyphy")
model <- glm(cbind(dat$nCorrect, dat$nIncorrect) ~ position, family = binomial(mafc.probit(2)))

xseq <- seq(min(xset)-5, max(xset)+5, len = 1000)  #I used, for example, a 1000 points
yseq <- predict(model, data.frame(position = xseq), type = "response")
curve <- data.frame(xseq, yseq)
p1 <- p1 + geom_line(data = curve, aes(x = xseq, y = yseq))
p1

(m <- -coef(model)[[1]]/coef(model)[[2]])  #The brakets are to show the result

(std <- 1/coef(model)[[2]])

p <- 0.8
(thre <- qnorm((p - 0.5)/0.5, m, std))  #( p-.5)/.5 because the asymptote is .5

threshold <- data.frame(p, thre)
p1 <- p1 + geom_linerange(data = threshold, aes(x = thre, ymin = 0.5, ymax = p))
p1

library("modelfree")
threshold_slope(yseq, xseq, 0.8)


library("plyr")
sampling <- function(df) {
    n <- length(dat$intensity)
    size <- dat$nCorrect + dat$nIncorrect
    intensity <- dat$intensity
    nCorrect <- rbinom(n, size, prob = dat$p)
    nIncorrect <- size - nCorrect
    data.frame(intensity, nCorrect, nIncorrect)
}
fake.dat <- ddply(data.frame(sample = 1:100), .(sample), sampling)

calculate.threshold <- function(df) {
    tryCatch({
        model <- glm(cbind(df$nCorrect, df$nIncorrect) ~ df$intensity, family = binomial(mafc.probit(2)))
        m <- -coef(model)[[1]]/coef(model)[[2]]
        std <- 1/coef(model)[[2]]
        p <- 0.8
        thre <- qnorm((p - 0.5)/0.5, m, std)
        data.frame(thre)
    }, error = function(e) message(paste("Fitting error in sample: ", unique(df$sample), 
                                         "\n")), warning = function(w) message(paste("Fitting warning in sample: ", 
                                                                                     unique(df$sample), "\n")))
}
fake.thresholds <- ddply(fake.dat, .(sample), calculate.threshold)

ci <- quantile(fake.thresholds$thre, c(0.025, 0.975))
ci.df <- data.frame(p, rbind(ci))
p1 + geom_errorbarh(data = ci.df, aes(xmax = 0.975, xmin = 0.025), height = 0.01)















