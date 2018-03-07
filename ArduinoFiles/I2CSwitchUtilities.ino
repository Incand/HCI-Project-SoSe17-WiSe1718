
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

#include <FyberLabs_PCA9548.h>

FyberLabs_PCA9548 i2cswitch = FyberLabs_PCA9548();

void I2CSwitchSetup()
{
	i2cswitch.begin();
	LogPrintln(F("I2CSwitch found!"));
}

void I2CSwitchStop()
{
	i2cswitch.offAllSwitchChannels();
}

void I2CSwitchChannelOn(uint8_t channel)
{
	i2cswitch.onSwitchChannel(channel);
}

void I2CSwitchChannelOff(uint8_t channel)
{
	i2cswitch.offSwitchChannel(channel);
}

void I2CSwitchLoop()
{
	if(I2CSwitchDebug) 
	{
		for (uint8_t i=1; i<9; i += 1)
		{
			i2cswitch.onSwitchChannel(i);
			uint8_t cur = __builtin_ffs(i2cswitch.readSwitchChannel());
			LogPrint(F("Current switch channel is "));
			LogPrint(cur);
			LogPrintln(F("."));
			i2cswitch.offSwitchChannel(i);    
		}
	}
}

