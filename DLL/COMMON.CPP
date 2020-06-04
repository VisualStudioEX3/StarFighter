/* ----------------------------------------------------------------------------
 * Common code for all DIV DLLs.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "common.h"

void *div_realloc(void *ptr, size_t size)
{
    void *newPtr = div_malloc(size);
    memcpy(newPtr, ptr, size);
    div_free(ptr);
    
    return newPtr;
}

char* strAlloc(size_t size)
{
    size_t len = size + 1;
    char* ptr = (char*)div_malloc(len);
    memset(ptr, '\0', len);
    return ptr;
}

void strCase(char* str, const int mode)
{
    int len = strlen(str);
    if (str == NULL || len == 0) return;
    
    for(int i = 0; i < len; i++) 
    {
        str[i] = mode ? tolower(str[i]) : toupper(str[i]);
    }
}

void strTrim(char* str)
{
    int len = strlen(str);
    
    if (str == NULL || len == 0) return;

    int s, e, i;
    
    for (s = 0; s < len; s++)
    {
        if (str[s] != ' ') break;
    }
    
    for (e = len - 1; e >= 0; e--)
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
    int len = strlen(str);
    
    if (str == NULL || len == 0) return;

    for (int i = 0; i < len; i++)
    {
        if (str[i] == o) str[i] = n;
    }
}

struct tm *getDateTime()
{
    time_t now;
    time(&now);
    return localtime(&now);
}
