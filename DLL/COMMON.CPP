/* ----------------------------------------------------------------------------
 * Common code for all DIV DLLs.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "common.h"

void strCase(char* str, const int mode)
{
    if (str == NULL || strlen(str) == 0) return;
    
    for(int i = 0; i < strlen(str); i++) 
    {
        str[i] = mode ? tolower(str[i]) : toupper(str[i]);
    }
}

void strTrim(char* str)
{
    if (str == NULL || strlen(str) == 0) return;

    int s, e, i;
    
    for (s = 0; s < strlen(str); s++)
    {
        if (str[s] != ' ') break;
    }
    
    for (e = strlen(str) - 1; e >= 0; e--)
    {
        if (str[e] != ' ') 
        {
            e++;
            break;
        }
    }

    for (i = 0; s < e; i++, s++)
    {
        str[i] = str[s];
    }
    
    str[i] = '\0';
}

void strReplace(char * str, const char o, const char n)
{
    if (str == NULL || strlen(str) == 0) return;

    for (int i = 0; i < strlen(str); i++)
    {
        if (str[i] == o) str[i] = n;
    }
}