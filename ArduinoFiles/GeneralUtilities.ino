
/**
 *  Piezo Tactile Interface
 *
 *  Copyright 2017 by HCI Group - Universität Hamburg (https://www.inf.uni-hamburg.de/de/inst/ab/mci.html)
 *
 *  Licensed under "The MIT License (MIT) – military use of this product is forbidden – V 0.2".
 *  Some rights reserved. See LICENSE.
 */

/*
 *   Author: Oscar Javier Ariza Nunez <ariza@informatik.uni-hamburg.de>
 */

void LogPrintln(const int n, bool hex = false) 
{
  if(UseSerial) (hex? Serial.println(n, HEX): Serial.println(n));
}

void LogPrint(const int n, bool hex = false) 
{
  if(UseSerial) (hex? Serial.print(n, HEX): Serial.print(n));
}

void LogPrintln(const float f) 
{
  if(UseSerial) Serial.println(f);
}

void LogPrint(const float f) 
{
  if(UseSerial) Serial.print(f);
}

void LogPrintNumber(const float f, int dp) 
{
  if(UseSerial) Serial.print(f, dp);
}

void LogPrintln(const __FlashStringHelper* s) 
{
  if(UseSerial) Serial.println(s);
}

void LogPrintln(const char* s) 
{
  if(UseSerial) Serial.println(s);
}

void LogPrint(const __FlashStringHelper* s) 
{
  if(UseSerial) Serial.print(s);
}

void LogWarning(const __FlashStringHelper* warning) 
{
  if(UseSerial) Serial.println(warning);
  ShowLedSequenceC();
}

void LogError(const __FlashStringHelper* err) 
{
  if(UseSerial) Serial.println(err);
  while (1);
  {
    ShowLedSequenceB();
  }
}

void ShowLedSequenceA()
{
  for(byte i=1; i<=20; i++)
  {
    digitalWrite(led, HIGH);
    delay(3 * i);           
    digitalWrite(led, LOW); 
    delay(20);           
  }  
}

void ShowLedSequenceB()
{
  digitalWrite(led, HIGH);
  delay(1000);
  for (byte i=0; i<10; i++)
  {
    digitalWrite(led, LOW); 
    delay(50);           
    digitalWrite(led, HIGH);
    delay(50);           
  }         
  digitalWrite(led, LOW); 
  delay(1000);
}

void ShowLedSequenceC()
{
  for (byte i=0; i<5; i++)
  {
    digitalWrite(led, HIGH);
    delay(10);           
    digitalWrite(led, LOW); 
    delay(20);           
  }  
}

/*
https://learn.adafruit.com/adafruit-feather-m0-bluefruit-le/power-management
Lipoly batteries are 'maxed out' at 4.2V and stick around 3.7V for much of the battery life, 
then slowly sink down to 3.2V or so before the protection circuitry cuts it off. 
By measuring the voltage you can quickly tell when you're heading below 3.7V
*/
float readBattery1Level()
{
	float measuredvbat = analogRead(battery1);
	measuredvbat *= 2;    // we divided by 2, so multiply back
	measuredvbat *= 3.3;  // Multiply by 3.3V, our reference voltage
	measuredvbat /= 1024; // convert to voltage
	return measuredvbat;
}

float readBattery2Level()
{
  //float measuredvbat = analogRead(battery2);
  //measuredvbat *= 2;    // we divided by 2, so multiply back
  //measuredvbat *= 3.3;  // Multiply by 3.3V, our reference voltage
  //measuredvbat /= 1024; // convert to voltage
  //return measuredvbat;
  //return 0;
}

void I2CScan()
{
	byte error, address;
  int nDevices = 0;
	
	LogPrintln(F("Scanning I2C ..."));

	for(address = 1; address < 127; address++ ) 
  {
    Wire.beginTransmission(address);
    error = Wire.endTransmission();

    if (error == 0)
    {
			LogPrint(F("Found address: "));
			LogPrint(address);
			LogPrint(F(" (0x"));
			LogPrint(address, HEX);
			LogPrintln(F(")"));
			nDevices++;
			delay (1);
		}
    else if (error==4) 
    {
      LogPrint(F("Unknow error at address 0x"));
      if (address<16) 
        LogPrint(F("0"));
      LogPrint(address,HEX);
      LogPrintln(F(""));
    }    
	}
	
	LogPrintln(F("Done."));
	LogPrint(nDevices);
	LogPrintln(F(" device(s) found."));
}


