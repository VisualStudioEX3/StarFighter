/* ----------------------------------------------------------------------------
 * LOGGER.DLL - File logger and console for DIV Games Studio 2.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include <direct.h>
#include "..\common.h"

#define default_console_str_length  80
#define default_console_max_lines   15
#define log_folder                  "LOGS"

#define enable                      fileEnable || consoleEnable
#define console_full_line_len       consoleStrLen + 1 // strlen + \0
#define console_buffer_size         console_full_line_len * consoleMaxLines

int     fileEnable;
FILE*   file;

char*   separator;

int     consoleEnable;
int     consoleVisible;
int     consoleStrLen;
int     consoleMaxLines;        // Max lines to show.
char*   consoleBuffer;          // (strLength + 1) * maxLines.
int     consoleWriteIndex;      // Line to write.

void _log(char* message);
void createLogFolder();
void cwrite(int index, char* text);
char* cread(int index);
void cclear();

void init();
void isFileEnabled();
void isConsoleEnabled();
void log();
void show();
void hide();
void clear();
