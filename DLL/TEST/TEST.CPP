#include <stdlib.h> 
#include <string.h>
#include <ctype.h>
#include <time.h>

#define GLOBALS
#include "..\div.h"

int startTime;

void initTimer()
{
    startTime = clock();
    retval(0);
}

void getTime()
{
    retval(clock() - startTime);
}

char *upr(const char *input)
{
    int len = strlen(input);
    char *output = (char *)malloc(len + 1);
    
    for (int i = 0; i < len; i++)
    {
        output[i] = toupper(input[i]);
    }
    output[len] = '\0';
    
    return output;
}

void getText()
{    
    //char *text = "This is a test!";
    char *text = (char *)malloc(sizeof(char) * 15);
    text = "This is a test!";
    
    char *test = upr(text);
    
    char *dest = (char *)&mem[text_offset + getparm()];
    
    //strcpy(dest, test);
    memcpy(dest, test, strlen(test) - 1);
    
    retval(0);
}

void __export divlibrary(LIBRARY_PARAMS)
{
    COM_export("initTimer", initTimer, 0);
    COM_export("testTime", getTime, 0);
    COM_export("getText", getText, 1);
}

void __export divmain(COMMON_PARAMS)
{
  GLOBAL_IMPORT();
}
