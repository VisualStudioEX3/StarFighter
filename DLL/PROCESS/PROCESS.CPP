/* ----------------------------------------------------------------------------
 * PROCESS.DLL - Functions to operate with processes in DIV Games Studio 2.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "process.h"

static int findProcAddress(const int id)
{
    if (id > 0)
    {
        for (int offset = id_init_offset; 
             offset <= id_end_offset; 
             offset += process_size)
        {
            struct _process *ptr = (struct _process *)&mem[offset];
            if (ptr->reserved.id == id)
            {
                return offset;
            }
        }
    }
    return RESULT_ERROR;
}

static int _getStatus(const int id)
{   
    int offset = findProcAddress(id);
    if (offset != RESULT_ERROR)
    {
        struct _process *ptr = (struct _process *)&mem[offset];
        return ptr->reserved.status;
    }    
    return RESULT_ERROR;
}

/** Get the type of the process.
*
* @param {int} processId - Process identifier.
*
* @return {int} - Returns the process type or zero if process id is invalid or process not exists.
*/
void typeOf()
{
    int offset = findProcAddress(getparm());
    if (offset != RESULT_ERROR)
    {
        struct _process *ptr = (struct _process *)&mem[offset];
        retval(ptr->reserved.block);
        return;
    }
    retval(RESULT_ERROR);
}

/** Get process status.
*
* @param {int} processId - Process identifier.
*
* @return {int} - Returns the process status: 
*                 0 - Inexistent.
*                 1 - Received s_kill signal.
*                 2 - Is alive or awake (s_wakeup)
*                 3 - Is asleep (s_sleep)
*                 4 - Is frozen (s_freeze)
*/
void getStatus()
{
    retval(_getStatus(getparm()));
}

/** Is the process exists?.
*
* @param {int} processId - Process identifier.
*
* @return {int} - Returns true if the process exists.
*/
void exists()
{
    retval(_getStatus(getparm()) == 0);
}

/** Is the process alive?.
*
* @param {int} processId - Process identifier.
*
* @return {int} - Returns true if the process is alive.
*/
void isAlive()
{
    retval(_getStatus(getparm()) == 2);
}

/** Is the process asleep?.
*
* @param {int} processId - Process identifier.
*
* @return {int} - Returns true if the process is asleep.
*/
void isAsleep()
{
    retval(_getStatus(getparm()) == 3);
}

/** Is the process fronzen?.
*
* @param {int} processId - Process identifier.
*
* @return {int} - Returns true if the process is frozen.
*/
void isFrozen()
{
    retval(_getStatus(getparm()) == 4);
}

void __export divlibrary(LIBRARY_PARAMS)
{    
    COM_export("typeof",        typeOf,     1);
    COM_export("get_status",    getStatus,  1);
    COM_export("exists",        exists,     1);
    COM_export("is_alive",      isAlive,    1);
    COM_export("is_asleep",     isAsleep,   1);
    COM_export("is_frozen",     isFrozen,   1);
}

void __export divmain(COMMON_PARAMS)
{
  GLOBAL_IMPORT();
}
