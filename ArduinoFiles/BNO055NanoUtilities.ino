
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

#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BNO055.h>
#include <utility/imumaths.h>

#define Q_FACTOR 32767

Adafruit_BNO055 bno = Adafruit_BNO055(55);
sensor_t sensor;
sensors_event_t event;

float q[4] = {1.0f, 0.0f, 0.0f, 0.0f};
float* getQuaternion(){return q;}

uint8_t cs[4] = {0.0f, 0.0f, 0.0f, 0.0f}; // sys, gyro, accel, mag - not calibrated = 0, fully calibrated = 3 

adafruit_bno055_offsets_t calibrationData;

uint16_t calibrationOffsets_[11];

bool calibrating = false;
void setCalibrationStatus(bool status)
{
    calibrating = status;
}

bool sendIMUData = false;
void BNO055NanoToggleSendIMUData(void)
{
  sendIMUData = !sendIMUData;
}

//***************************************************************************

void PrintQuaternion()
{
  LogPrint(F( "qw = ")); LogPrintNumber(q[0],2);
  LogPrint(F(" qx = ")); LogPrintNumber(q[1],2); 
  LogPrint(F(" qy = ")); LogPrintNumber(q[2],2); 
  LogPrint(F(" qz = ")); LogPrintNumber(q[3],2);
  
  bno.getCalibration(&cs[0], &cs[1], &cs[2], &cs[3]);
  LogPrint(F(" C[sgam] = ")); 
  LogPrintNumber(cs[0],0); LogPrintNumber(cs[1],0); LogPrintNumber(cs[2],0); LogPrintNumber(cs[3],0);
  LogPrintln("");
}

void displaySensorOffsets(void)
{
    LogPrint(F("A: "));
    LogPrintNumber(calibrationData.accel_offset_x,2); LogPrint(F(" "));
    LogPrintNumber(calibrationData.accel_offset_y,2); LogPrint(F(" "));
    LogPrintNumber(calibrationData.accel_offset_z,2); LogPrint(F(" "));
    LogPrint(F(" G: "));
    LogPrintNumber(calibrationData.gyro_offset_x,2); LogPrint(F(" "));
    LogPrintNumber(calibrationData.gyro_offset_y,2); LogPrint(F(" "));
    LogPrintNumber(calibrationData.gyro_offset_z,2); LogPrint(F(" "));
    LogPrint(F(" M: "));
    LogPrintNumber(calibrationData.mag_offset_x,2); LogPrint(F(" "));
    LogPrintNumber(calibrationData.mag_offset_y,2); LogPrint(F(" "));
    LogPrintNumber(calibrationData.mag_offset_z,2);
    LogPrintln("");
    LogPrint(F(" AR: "));
    LogPrintNumber(calibrationData.accel_radius,2);
    LogPrint(F(" MR: "));
    LogPrintNumber(calibrationData.mag_radius,2);
    LogPrintln("");
}

void setCalibrationOffset(char* buffer, byte index, byte num)
{
  String s = buffer;
  s = s.substring(3);
  char* ptr = NULL;
  char * b = new char [s.length()+1];
  strcpy(b, s.c_str());
  ptr = strtok(b, ",;");  // list of delimiters
  int counter = 0;
  while(counter < num)
  {
      calibrationOffsets_[index++] = atoi(ptr);
      ptr = strtok(NULL, ",;");
      counter++;
  }

  if(num==2)
  {
    calibrationData.accel_offset_x = calibrationOffsets_[0];
    calibrationData.accel_offset_y = calibrationOffsets_[1];
    calibrationData.accel_offset_z = calibrationOffsets_[2];
    calibrationData.gyro_offset_x = calibrationOffsets_[3];
    calibrationData.gyro_offset_y = calibrationOffsets_[4];
    calibrationData.gyro_offset_z = calibrationOffsets_[5];
    calibrationData.mag_offset_x = calibrationOffsets_[6];
    calibrationData.mag_offset_y = calibrationOffsets_[7];
    calibrationData.mag_offset_z = calibrationOffsets_[8];
    calibrationData.accel_radius = calibrationOffsets_[9];
    calibrationData.mag_radius = calibrationOffsets_[10];
    LogPrintln("Updating calibration on IMU...");
    bno.setSensorOffsets(calibrationData);
    displaySensorOffsets();
  }

  delete b;
}

void calibrate()
{
  if(bno.isFullyCalibrated())
  {
    BLESendCalibrationOffsets();
    LogPrintln("Updating calibration on IMU...");
    bno.setSensorOffsets(calibrationData);
    calibrating = false;
  }
  else
  {
    bno.getCalibration(&cs[0], &cs[1], &cs[2], &cs[3]);
    BLESendCalibrationStatus();
    calibrating = true;
  }
}

void BNO055NanoSetup(void)
{
  if(!bno.begin())
  {
    LogError(F("BNO055 not found"));
    while(1);
  }
  
  delay(1000);
  bno.setExtCrystalUse(true);
  
  sensor_t sensor;
  bno.getSensor(&sensor);
  LogPrint(F(sensor.name));
  LogPrintln(" found!");
  delay(500);
}

void BNO055NanoLoop(void)
{
  bno.getQuat().get(q);
  
  if(calibrating == true)
  {
    calibrate();
  }

  if(sendIMUData)
  {
    BLESendQuaternion();
    if(BNO055Debug) PrintQuaternion();
  }

  delay(1);
}

//***************************************************************************

typedef union 
{
    short s[4];
    char c[8];
}   quaternionStruct;
quaternionStruct quaternion;
float* qptr;
void BLESendQuaternion()
{
  if(! ble.isConnected()) return;
  
  qptr = getQuaternion(); //memcpy(quaternion.f, getQuaternion(), sizeof quaternion.f);
  quaternion.s[0] = (int)(qptr[0]*Q_FACTOR);
  quaternion.s[1] = (int)(qptr[1]*Q_FACTOR);
  quaternion.s[2] = (int)(qptr[2]*Q_FACTOR);
  quaternion.s[3] = (int)(qptr[3]*Q_FACTOR);
  if(BNO055Debug) PrintQuaternion();

  ble.print("AT+BLEUARTTXF="); ble.print('Q',HEX); ble.print('1',HEX);
  for (int i = 0; i < 8; i++)
  {
    if (quaternion.c[i] <= 0xF) ble.print("0");
    ble.print(quaternion.c[i],HEX);
  }
  ble.println();
  if(!ble.waitForOK() ) LogWarning(F("Failed to send?"));

  delay(1);
}

//***************************************************************************

void BLESendCalibrationStatus()
{
  if(! ble.isConnected()) return;
  ble.print("AT+BLEUARTTXF="); ble.print('C',HEX); ble.print('S',HEX);
  ble.print(cs[0]);
  ble.print(cs[1]);
  ble.print(cs[2]);
  ble.print(cs[3]);
  ble.println();
  if(!ble.waitForOK() ) LogWarning(F("Failed to send?"));

  delay(1);
}

//***************************************************************************

typedef union 
{
    uint16_t ui[4];
    char c[8];
}   calibrationOffsetStruct;
calibrationOffsetStruct calibrationOffset;

void BLESendCalibrationOffset(char index)
{
  if(! ble.isConnected()) return;
  ble.print("AT+BLEUARTTXF="); ble.print('C', HEX); ble.print(index, HEX);
  for (int i = 0; i < 8; i++)
  {
    if (calibrationOffset.c[i] <= 0xF) ble.print("0");
    ble.print(calibrationOffset.c[i],HEX);
  }
  ble.println();
  if(!ble.waitForOK() ) LogWarning(F("Failed to send?"));

  delay(1);
}

//***************************************************************************

void BLESendCalibrationOffsets()
{
  bno.getSensorOffsets(calibrationData);
  displaySensorOffsets();

  calibrationOffset.ui[0] = calibrationData.accel_offset_x;
  calibrationOffset.ui[1] = calibrationData.accel_offset_y;
  calibrationOffset.ui[2] = calibrationData.accel_offset_z;
  calibrationOffset.ui[3] = calibrationData.gyro_offset_x;
  BLESendCalibrationOffset('1');

  calibrationOffset.ui[0] = calibrationData.gyro_offset_y;
  calibrationOffset.ui[1] = calibrationData.gyro_offset_z;
  calibrationOffset.ui[2] = calibrationData.mag_offset_x;
  calibrationOffset.ui[3] = calibrationData.mag_offset_y;
  BLESendCalibrationOffset('2');

  calibrationOffset.ui[0] = calibrationData.mag_offset_z;
  calibrationOffset.ui[1] = calibrationData.accel_radius;
  calibrationOffset.ui[2] = calibrationData.mag_radius;
  calibrationOffset.ui[3] = 0;
  BLESendCalibrationOffset('3');
}



