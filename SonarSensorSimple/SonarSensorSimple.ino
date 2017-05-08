
#include "Maxbotix.h"

unsigned long start;
Maxbotix rangeSensor(6, Maxbotix::PW, Maxbotix::LV);
//Maxbotix rangeSensor(A0, Maxbotix::AN, Maxbotix::LV);

void setup()
{
  Serial.begin(38400);
}

void loop()
{
  start = millis();
  Serial.print(rangeSensor.getRange(),0);
  Serial.print("cm - ");
  Serial.print(millis() - start);
  Serial.println("ms");
  Serial.clear();

  delay(1);
}
