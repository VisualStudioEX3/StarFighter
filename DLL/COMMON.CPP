/* ----------------------------------------------------------------------------
 * Common code for all DIV DLLs.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "common.h"

char* strCase(const char* input, const int mode)
{
    int len = strlen(input);
    char* output = strAlloc(len);
    
    for(int i = 0; i < len; i++) 
    {
        output[i] = mode ? 
                    tolower(input[i]) : 
                    toupper(input[i]);
    }
    output[len] = '\0';
    
    return output;
}
