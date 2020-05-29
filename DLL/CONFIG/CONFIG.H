/* ----------------------------------------------------------------------------
 * CONFIG.DLL - INI parser for DIV Games Studio.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 *
 * minINI (C) CompuPhase, 2008-2017
 * https://github.com/compuphase/minIni
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * ---------------------------------------------------------------------------- */
 
#ifndef __CONFIG_H_
#define __CONFIG_H_

#include "..\common.h"
#include "divfio.h"
#include "minIni.h"

#define TEMP_BUFFER     256

char* filename;
char* section;

void setFile();
void setSection();

void readBool();
void readInt();
void readString();

void writeBool();
void writeInt();
void writeString();

#endif
