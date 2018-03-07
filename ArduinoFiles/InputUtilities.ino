
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

float jx = 0;
float jy = 0;
float firstjx = 0;
float firstjy = 0;

void InputSetup()
{
  jx = -analogRead(leftRight);   
  jy = -analogRead(upDown);
  if(firstjx == 0) firstjx = jx;
  if(firstjy == 0) firstjy = jy;
}

void InputLoop()
{
  jx = (-analogRead(leftRight) - firstjx)/256;   
  jy = (-analogRead(upDown) - firstjy)/256;
  if(JoystickDebug) {LogPrint(F("XY: ")); LogPrintNumber(jx, 0); LogPrint(F(" ")); LogPrintNumber(jy, 0); LogPrintln("");} 
  
  /*
  //**************************************************************************

	if(digitalRead(pushButtonA) == 0)
	{
		if(buttonAState == 0)
		{
      buttonAState = 1;
      if(HapBandDebug) LogPrintln(F("Button A pressed"));
      digitalWrite(led, HIGH);

      digitalWrite(solenoid, 1);
      DRV2667Play(2, 255, 60, 20, 2);
    }

    DRV2605Play(1, 4); // Effect
    //DRV2605Play(1, 60); // Real-time value
	}
	else
	{
		if(buttonAState == 1)
		{
			buttonAState = 0;
      if(HapBandDebug) LogPrintln(F("Button A released"));
			digitalWrite(led, LOW);

      DRV2605Stop(1);
      digitalWrite(solenoid, 0);
      DRV2667Stop(2);
		}
	}
  */

  //**************************************************************************
}


