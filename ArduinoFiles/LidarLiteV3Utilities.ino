
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

#include "Maxbotix.h"

bool useIRSensor = false;
bool useLIDAR = false;
float irSensorDistance = 0;
float lidarDistance = 0;

Maxbotix rangeSensor(maxbotix, Maxbotix::PW, Maxbotix::LV);

void LidarLiteV3Setup()
{
  pinMode(lidar, INPUT);
  LogPrintln("LIDAR V3 Lite & MaxBOTIX found!");
}

void ToggleLidarSensor()
{
  useLIDAR = !useLIDAR;
}

void ToggleIRSensor()
{
  useIRSensor = !useIRSensor;
}

typedef union 
{
    short s[4];
    char c[8];
}   distanceStruct;
distanceStruct ud;
void BLESendDistancesAndJoystickPosition()
{
  if(!ble.isConnected()) return;
  
  ud.s[0] = irSensorDistance;
  ud.s[1] = lidarDistance;
  ud.s[2] = jx;
  ud.s[3] = jy;

  ble.print("AT+BLEUARTTXF="); ble.print('D',HEX); ble.print('I',HEX);
  for (int i = 0; i < 8; i++)
  {
    if (ud.c[i] <= 0xF) ble.print("0");
    ble.print(ud.c[i],HEX);
  }
  ble.println();
  if(!ble.waitForOK() ) LogWarning(F("Failed to send?"));

  delay(1);
}

void LidarLiteV3Loop()
{
  if(useLIDAR)
  {
    lidarDistance = pulseIn(lidar, HIGH); // How long the pulse is high (microseconds)
    if(lidarDistance != 0)
    {
      lidarDistance /= 10; // 10usec = 1 cm of distance
    }
  }

  if(useIRSensor)
  {
    irSensorDistance = rangeSensor.getRange(); // + 15; // plus offset
  }

  //int limit = 30;
  //if(lidarDistance < limit) lidarDistance = limit;
  //if(irSensorDistance < limit) irSensorDistance = limit;

  if(DistanceDebug && (useIRSensor||useLIDAR))
  { 
    LogPrint(F("Distances: IR >>> ")); 
    LogPrintNumber(irSensorDistance, 0); 
    LogPrint(F(" LIDAR >>> ")); 
    LogPrintNumber(lidarDistance, 0);
    LogPrintln(F(""));
  }

  if(useIRSensor||useLIDAR)
  { 
    BLESendDistancesAndJoystickPosition();
  }
}

