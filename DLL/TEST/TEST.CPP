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

void testTextOut()
{
    char* text = (char *)&mem[text_offset + getparm()];
    div_text_out(text, 0, 0);
    retval(0);
}

void testGetProcessId()
{
    struct _process* p = (struct _process*)&mem[id_start_offset];
    retval(p->reserved.id);
}

void typeOf()
{
    int processId = getparm();
    
    if (processId == 0)
    {
        retval(0);
        return;
    }
    
    struct _process* p;    
    for (int offset = id_init_offset; 
         offset <= id_end_offset; 
         offset += process_size)
    {
        p = (struct _process*)&mem[offset];
        if (p->reserved.id == processId)
        {
            retval(p->reserved.block);
            return;
        }
    }
    
    retval(0);
}

void post_process_buffer(void)
{
    div_text_out("Hello, world!", 0, 0);
}

void __export divlibrary(LIBRARY_PARAMS)
{
    COM_export("initTimer", initTimer, 0);
    COM_export("testTime", getTime, 0);
    COM_export("getText", getText, 1);
    COM_export("test_text_out", testTextOut, 1);
    COM_export("test_get_process_id", testGetProcessId, 0);
    COM_export("typeof", typeOf, 1);
}

void __export divmain(COMMON_PARAMS)
{
  //AutoLoad();
  GLOBAL_IMPORT();
  
  DIV_export("post_process_buffer", post_process_buffer);
}
