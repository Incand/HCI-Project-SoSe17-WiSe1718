
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

#include "SPI.h"
#include "Adafruit_BLE.h"
#include "Adafruit_BluefruitLE_SPI.h"
#include "Adafruit_BluefruitLE_UART.h"

#define BLUEFRUIT_SPI_CS  8
#define BLUEFRUIT_SPI_IRQ 7
#define BLUEFRUIT_SPI_RST 4 // Optional but recommended, set to -1 if unused
Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_CS, BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);

byte actuatorIndex = 0;

void BLESetup()
{
	randomSeed(micros());

	if(!ble.begin(BLEDebug))
  {
    LogError(F("NRF51822 not found!"));
  }
  else
  {
    LogPrintln(F("NRF51822 found!"));
  }

  ble.echo(false);
  ///ble.info();
  ///ble.setInterCharWriteDelay(5); // ms

  if (! ble.sendCommandCheckOK(F("AT+GAPDEVNAME=HCI_HapStick")) ) 
  {
    LogError(F("Error setting up the device name!"));
  }
  
  if (! ble.sendCommandCheckOK(F("AT+BAUDRATE=1000000")) ) 
  {
    LogError(F("Error setting up the baud rate!"));
  }
}

void BLELoop() 
{
	if(! ble.isConnected()) return;
  
  //*******************************************************************
  // Read commands from terminal (when connected via USB)
  /*
  if(UseSerial)
  {
    int incomingBytes = Serial.available();
    if(incomingBytes > 0) 
    {
      char inputs[incomingBytes];  
      Serial.readBytes(inputs, incomingBytes);
      Serial.println(inputs);
      ///AT+BLEUARTTX=T
      ble.sendCommandCheckOK(inputs);
    }
  }
  */
  //*******************************************************************

  // Check for incoming characters from Bluefruit using commands
  ble.println("AT+BLEUARTRX");
  ble.readline();

  if (strcmp(ble.buffer, "OK") == 0)
  { 
    //
  }
  else
  {
    if(UseSerial) LogPrint(F("RX: ")); LogPrintln(F(ble.buffer));

    if(strncmp(ble.buffer, "CO-REQ", 6) == 0) // Calibration offsets request
    { 
        setCalibrationStatus(true);
    }
    else if(strncmp(ble.buffer, "CO1", 3) == 0) // Calibration offsets.1 upload
    { 
        setCalibrationOffset(ble.buffer, 0, 3);
    }
    else if(strncmp(ble.buffer, "CO2", 3) == 0) // Calibration offsets.2 upload
    { 
        setCalibrationOffset(ble.buffer, 3, 3);
    }
    else if(strncmp(ble.buffer, "CO3", 3) == 0) // Calibration offsets.3 upload
    { 
        setCalibrationOffset(ble.buffer, 6, 3);
    }
    else if(strncmp(ble.buffer, "CO4", 3) == 0) // Calibration offsets.4 upload
    { 
        setCalibrationOffset(ble.buffer, 9, 2);
    }
    else if(strncmp(ble.buffer, "TOGIMU", 6) == 0) // Toggle IMU data transfer
    { 
        BNO055NanoToggleSendIMUData();
    }
    else if(strncmp(ble.buffer, "DSLT", 4) == 0) // Toggle Lidar Sensor
    {
        ToggleLidarSensor();
    }
    else if(strncmp(ble.buffer, "DSIT", 4) == 0) // Toggle IR Sensor
    {
        ToggleIRSensor();
    }
    else if(strncmp(ble.buffer, "APE", 3) == 0) // Enable the piezo actuator    
    {
        DRV2667ParseArray(ble.buffer);
    }
    else if(strncmp(ble.buffer, "APD", 3) == 0) // Disable the piezo actuator 
    { 
        actuatorIndex = atoi(&(ble.buffer[3]));
        DRV2667Stop(actuatorIndex);
    }

    if( !ble.waitForOK() ) LogWarning(F("BLE Failed!"));
  }
}

//***************************************************************************

void BLESendBatteryLevel()
{
  char packet1[4];
  float battery1Level = readBattery1Level();
  unsigned int integer1 = (unsigned int) battery1Level;
  packet1[0] = '0' + integer1;
  packet1[1] = '.';
  packet1[2] = '0' + (unsigned int) (10*(battery1Level-integer1));
  packet1[3] = '\0';  // End of string character
  
  char packet2[4];
  float battery2Level = readBattery2Level();
  unsigned int integer2 = (unsigned int) battery2Level;
  packet2[0] = '0' + integer2;
  packet2[1] = '.';
  packet2[2] = '0' + (unsigned int) (10*(battery2Level-integer2));
  packet2[3] = '\0';  // End of string character

  if(HapBandDebug)
  { 
    LogPrint(F("Battery Levels >>> ")); 
    LogPrint(battery1Level); 
    LogPrintln(F("V >>> ")); 
  }

  if(! ble.isConnected()) return;
  
  ble.print("AT+BLEUARTTXF="); 
  ble.print('B', HEX); ble.print('A', HEX);
  ble.print(packet1[0]);
  ble.print(packet1[2]);
  ble.print(packet2[0]);
  ble.print(packet2[2]);
  ble.println();
  if(!ble.waitForOK() ) LogWarning(F("Failed to send?"));
  delay(1);
}

int ReadFIFOAvailability()
{
  ble.println("AT+BLEUARTFIFO=TX");
  ble.readline();
  LogPrintln(F("[FIFO Buffer] ")); LogPrintln((int)ble.buffer);
  return (int)ble.buffer;
}

/*
  // Check for incoming characters from Bluefruit using data API
  incomingBytes = ble.available();
  if(incomingBytes > 0) 
  {
    char buffer[incomingBytes]; 
    bufferIndex = 0;
    while(bufferIndex++ < incomingBytes)
    {
      buffer[bufferIndex] = (char)ble.read();
    }
    LogPrintln(F(buffer));
  }
*/

/*
  int32_t serviceId;
  int32_t orientationCharId;
  int32_t solenoidACharId;

  //////////////////////////////////////////////////////////////////////
  
  boolean success;
  success = ble.sendCommandWithIntReply( F("AT+GATTADDSERVICE=UUID=0x181C"), &serviceId); // User data service
  if (! success) 
  {
    LogError(F("Could not add service!"));
  }

  success = ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0xFFA1, PROPERTIES=0x10, MIN_LEN=16, MAX_LEN=16, VALUE=00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00"), &orientationCharId);
  if (! success) 
  {
    LogError(F("Could not add characteristic!"));
  }

  success = ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0xFFA2, PROPERTIES=0x10, MIN_LEN=1, MAX_LEN=1, VALUE=00"), &solenoidACharId);
  if (! success) 
  {
    LogError(F("Could not add characteristic!"));
  }
  ble.reset();

  //////////////////////////////////////////////////////////////////////

  ble.print( F("AT+GATTCHAR=") ); ble.print( orientationCharId ); ble.println(qd.c);
*/



