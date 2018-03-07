
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

#define DRV2667_ADDR			0x59

byte piezoIndex;
byte amplitude;
byte frequency;
byte cycles;
byte envelope;

void writeRegisterOnDRV2667(byte reg, byte val) 
{
	Wire.beginTransmission(DRV2667_ADDR);
	Wire.write((byte)reg);
	Wire.write((byte)val);
	Wire.endTransmission();
}

void DRV2667Setup(byte index)
{
  writeRegisterOnDRV2667(0x02, 0x00); //Take device out of standby mode
	LogPrintln(F("DRV2667 found!"));
}

// https://github.com/yurikleb/DRV2667
// WaveForm Array: [Amplitude, Freq, Cycles, Envelope] 
// Amplitude    --  min:0=50v max: 255=100v 
// Frequency    --  (0-255) >>> (7.8125-1992.1875)Hz
// Duration     --  Cycles 0-255
// Envelope     --  Ramp up + down
void DRV2667Play(byte index, byte amplitude, byte frequency, byte cycles, byte envelope) 
{
  if(DRV2667Debug) 
	{
		LogPrint(F("Piezo >> "));
    LogPrint(F(" Index: "));     LogPrint(index);  
		LogPrint(F(" Amplitude: ")); LogPrint(amplitude);  
		LogPrint(F(" Frequency: ")); LogPrint(frequency*7.8125); LogPrint(F("Hz"));
		LogPrint(F(" Cycles: "));    LogPrint(cycles); 
		LogPrint(F(" Envelope: "));  LogPrintln(envelope);
	}

  I2CSwitchChannelOn(index);

	//control
	writeRegisterOnDRV2667(0x02, 0x00); //Take device out of standby mode
	writeRegisterOnDRV2667(0x01, 0x03); //Set Gain 0-3 (0x00-0x03 25v-100v)
	writeRegisterOnDRV2667(0x03, 0x01); //Set sequencer to play WaveForm ID #1
	writeRegisterOnDRV2667(0x04, 0x00); //End of sequence
	//header
	writeRegisterOnDRV2667(0xFF, 0x01); //Set memory to page 1
	writeRegisterOnDRV2667(0x00, 0x05); //Header size -1
	writeRegisterOnDRV2667(0x01, 0x80); //Start address upper byte (page), also indicates Mode 3
	writeRegisterOnDRV2667(0x02, 0x06); //Start address lower byte (in page address)
	writeRegisterOnDRV2667(0x03, 0x00); //Stop address upper byte
	writeRegisterOnDRV2667(0x04, 0x06+3); //Stop address Lower byte // 3 = sizeof(WaveForm)-1
	writeRegisterOnDRV2667(0x05, 0x01); //Repeat count, play WaveForm once
	//WaveForm Data From the array
	writeRegisterOnDRV2667(0x06+0, amplitude); 
	writeRegisterOnDRV2667(0x06+1, frequency); 
	writeRegisterOnDRV2667(0x06+2, cycles); 
	writeRegisterOnDRV2667(0x06+3, envelope);
	//Control
	writeRegisterOnDRV2667(0xFF, 0x00); //Set page register to control space
	writeRegisterOnDRV2667(0x02, 0x01); //Set GO bit (execute WaveForm sequence)

	//delay( 1000 * (cycles / (7.8125 * frequency)) );
	//delay(duration+1);

  I2CSwitchChannelOff(index);
}

void DRV2667Stop(byte index)
{
  I2CSwitchChannelOn(index);
	  writeRegisterOnDRV2667(0x02, 0x00); //Take device out of standby mode
  I2CSwitchChannelOff(index);

  if(DRV2667Debug) LogPrintln(F("Piezo >> Stop"));
}

void DRV2667ParseArray(char* buffer)
{
  String s = buffer;
  s = s.substring(3);
  char* ptr = NULL;
  char * b = new char [s.length()+1];
  strcpy(b, s.c_str());
  ptr = strtok(b, ",");  // list of delimiters
  piezoIndex = atoi(ptr); ptr = strtok(NULL, ",");
  amplitude = atoi(ptr); ptr = strtok(NULL, ",");
  frequency = atoi(ptr); ptr = strtok(NULL, ",");
  cycles    = atoi(ptr); ptr = strtok(NULL, ",");
  envelope  = atoi(ptr);
  delete b;
  DRV2667Play(piezoIndex, amplitude, frequency, cycles, envelope);
}

void DRV2667Loop()
{
	/*
  I2CSwitchChannelOn(index);

	if(DRV2667Debug) 
	{
		LogPrintln(F(""));
	}

  I2CSwitchChannelOff(index);
  */
}



