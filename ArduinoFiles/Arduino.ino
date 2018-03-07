
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


#define Serial SERIAL_PORT_USBVIRTUAL // Required for M0 based boards

#define WaitForSerial  false
#define UseSerial      false
#define HapBandDebug 	 false
#define BLEDebug       false
#define I2CSwitchDebug false
#define DRV2667Debug   false
#define BNO055Debug    false
#define DistanceDebug  false
#define JoystickDebug  false

/// Do not #define pins, it causes BLE errors. Do not ask why :,-(
/// Do not use pin 9 (A7) on Feather M0 (It is reserved for batery measurement)
int led             = 13;
int battery1        = A7;
int leftRight       = A5;
int upDown          = A4;
int lidar           = 10;
int maxbotix        = 11;
 
unsigned long BATstartTime;
unsigned long BATendTime;

void setup()
{
  BATstartTime = millis();
  
  if(UseSerial)
  {
	  if(WaitForSerial) while (!Serial);
    Serial.begin(9600);
  }

	LogPrintln(F("HapStick is starting..."));

	pinMode(led, OUTPUT);

	I2CSwitchSetup();
  DRV2667Setup(1);
  DRV2667Setup(2);
  DRV2667Setup(3);
  DRV2667Setup(4);
  DRV2667Setup(5);
  InputSetup();
  BNO055NanoSetup();
  BLESetup();
  LidarLiteV3Setup();
  
  BATendTime = millis();
  LogPrint(F("Loading time (ms): ")); LogPrintln(BATendTime - BATstartTime);

  LogPrintln(F("HapStick is ready!"));
  ShowLedSequenceA();
}

void loop()
{
	I2CSwitchLoop();
	DRV2667Loop();
  InputLoop();
  BNO055NanoLoop();
  BLELoop();
  LidarLiteV3Loop();
  
  //*******************************************************************
  // Read commands from terminal (when connected via USB)
  if(UseSerial)
  {
    int incomingBytes = Serial.available();
    if(incomingBytes > 0) 
    {
      char inputs[incomingBytes];  
      Serial.readBytes(inputs, incomingBytes);
      LogPrint(F("Serial Command Received: ")); LogPrintln(F(inputs));
      String message(inputs);
      message.trim();
    
      if(message.length() == 1)
      {
        if (inputs[0] == '1')
        {
          DRV2667Play(1,200,100,20,2);
        }
        else if (inputs[0] == '2')
        {
          DRV2667Play(2,200,100,20,2);
        }
        else if (inputs[0] == '3')
        {
          DRV2667Play(3,200,100,20,2);
        }
        else if (inputs[0] == '4')
        {
          DRV2667Play(4,200,100,20,2);
        }
        else if (inputs[0] == '5')
        {
          DRV2667Play(5,200,100,20,2);
        }
        else if (inputs[0] == '6')
        {
          ToggleLidarSensor();
        }
        else if (inputs[0] == '7')
        {
          ToggleIRSensor();
        }
        else if (inputs[0] == '8')
        {
          DRV2667Stop(1);
          DRV2667Stop(2);
          DRV2667Stop(3);
          DRV2667Stop(4);
          DRV2667Stop(5);
        }
      }
      Serial.flush();
    }
  }

  //*******************************************************************
  if((millis() - BATendTime) > 10000) // Send battery level every X milliseconds
  {
    BATendTime = millis();
    BLESendBatteryLevel();
  }
}

