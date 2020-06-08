/* ----------------------------------------------------------------------------
 * LOGGER.DLL - File logger for DIV Games Studio 2.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include <direct.h>
#include "..\common.h"

#define log_folder                  "LOGS"
#define log_ok                      file != NULL
#define separator                   "-------------------------------------------------------------------------------"

FILE*   file;

void init();
int  log(char* message);
void div_log();
