/* ----------------------------------------------------------------------------
 * PROCESS.DLL - Functions to operate with process in DIV Games Studio 2.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "..\common.h"

static int findProcAddress(const int id);
static int _getStatus(const int id);

void typeof();
void getStatus();
void exists();
void isAlive();
void isAsleep();
void isFrozen();
