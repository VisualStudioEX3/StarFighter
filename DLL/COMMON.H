/* ----------------------------------------------------------------------------
 * Common code for all DIV DLLs.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#ifndef __COMMON_H_
#define __COMMON_H_

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include <time.h>

#define GLOBALS
#include "div.h"

#define TRUE            1
#define FALSE           0

#define RESULT_OK       0
#define RESULT_ERROR   -1

// Math function macros:
#define _min(a, b) a < b ? a : b
#define _max(a, b) a > b ? a : b
#define _clamp(x, a, b) x < a ? a : (x > b ? b : x)
#define _isClamped(x, a, b) x >= a && x <= b

// Gets the string parameter from DIV call:
#define getStrParm() (char*)&mem[text_offset + getparm()]
// Convert string to upper case:
#define strUpr(s) strCase(s, 0)
// Convert string to lower case:
#define strLwr(s) strCase(s, 1)

// Custom implementation of realloc to works with DIV memory:
void *div_realloc(void *ptr, size_t size);

// Allocate string pointer in div memory (full set to null chars):
char* strAlloc(size_t size);

// String functions:
void strCase(char* str, const int mode);
void strTrim(char* str);
void strReplace(char * str, const char o, const char n);

// Return current date time:
struct tm *getDateTime();

#endif
