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

#define GLOBALS
#include "div.h"

#define TRUE            1
#define FALSE           0

#define RESULT_OK      -1
#define RESULT_ERROR   -2

// Math function macros:
#define _min(a, b) a < b ? a : b
#define _max(a, b) a > b ? a : b
#define _clamp(x, a, b) x < a ? a : (x > b ? b : x)
#define _isClamped(x, a, b) x >= a && x <= b

// Gets the string parameter from DIV call:
#define getStringParm(offset) (char*)&mem[text_offset + offset]
// Allocate string pointer in div memory:
#define strAlloc(len) (char*)div_malloc(len + 1)
// Convert string to upper case:
#define strUpr(s) strCase(s, 0)
// Convert string to lower case:
#define strLwr(s) strCase(s, 1)

// String functions:
char* strCase(const char* input, const int mode);

#endif
