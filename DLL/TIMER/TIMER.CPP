/* ----------------------------------------------------------------------------
 * TIMER.DLL - Advanced timers for DIV Games Studio.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "timer.h"

int isIndexValid(int index)
{
    if (_isClamped(index, 0, MAX_CAPACITY))
    {
        return timers[index].active == 1;
    }
    return(FALSE);
}

/** Get max number of available timers.
*
* @return {int} - Returns the number of available timers.
*/
void getCapacity()
{
    retval(MAX_CAPACITY);
}

/** Get current number of active timers.
*
* @return {int} - Returns the current number of active timers.
*/
void getCount()
{
    retval(count);
}

/** Create new timer.
*
* @return {int} - Returns the index of the new timer or RESULT_ERROR if not have available timers.
*/
void createTimer()
{
    if (count < MAX_CAPACITY)
    {
        for (int i = 0; i < MAX_CAPACITY; i++)
        {
            struct TimerData* t = &timers[i];
            if (!t->active)
            {
                t->active = 1;
                t->startTime = clock();
                count++;
                retval(i);
                return;
            }
        }
    }

    retval(RESULT_ERROR);
}

/** Free active timer.
*
* @param {int} index - Timer index.
*/
void freeTimer()
{
    int index = getparm();

    if (isIndexValid(index))
    {
        timers[index].active = 0;
        count--;
        retval(RESULT_OK);
        return;
    }

    retval(RESULT_ERROR);
}

/** Free all timers. */
void freeAll()
{
    for (int i = 0; i < MAX_CAPACITY; i++)
    {
        timers[i].active = 0;
        count = 0;
    }

    retval(RESULT_OK);
}

/** Get current timer value.
*
* @param {int} index - Timer index.
*
* @return {int} - Returns the current timer value or RESULT_ERROR if the timer index is not valid or timer is not active.
*/
void getTime()
{
   int index = getparm();

   if (isIndexValid(index))
   {
       retval(timers[index].time);
       return;
   }

   retval(RESULT_ERROR);
}

/** Update all active timers. */
void frameTimers()
{
   struct TimerData* t;
   for (int i = 0; i < MAX_CAPACITY; i++)
   {
       t = &timers[i];
       t->time = t->paused ?
                 t->time = t->pauseDelta - t->startTime :
                 t->time = clock() - t->startTime;
   }

   retval(RESULT_OK);
}

/** Pause or resume active timer.
*
* @param {int} index - Timer index.
*
* @return {int} - Returns RESULT_ERROR if the timer index is not valid or timer is not active.
*/
void pause()
{
   int index = getparm();

   if (isIndexValid(index))
   {
       struct TimerData* t = &timers[index];
       if (t->paused)
       {
           t->pauseDelta = clock();
           t->paused = 1;
       }
       else
       {
           t->startTime = clock() - t->pauseDelta;
           t->paused = 0;
       }

       retval(RESULT_OK);
       return;
   }

   retval(RESULT_ERROR);
}

/** Is timer paused?
*
* @param {int} index - Timer index.
*
* @return {int} - Returns true if timer is paused or false if the timer is not paused or timer is not active.
*/
void isPaused()
{
   int index = getparm();

   if (isIndexValid(index))
   {
       retval(timers[index].paused);
       return;
   }

   retval(FALSE); // Return false in case of error instead of RESULT_ERROR.
}

/** Reset active timer.
*
* @param {int} index - Timer index.
*
* @return {int} - Returns RESULT_ERROR if the timer index is not valid or timer is not active.
*/
void reset()
{
   int paused = getparm();
   int index = getparm();

   if (isIndexValid(index))
   {
       struct TimerData* t = &timers[index];
       t->startTime = clock();
       t->pauseDelta = 0;

       retval(RESULT_OK);
       return;
   }

   retval(RESULT_ERROR);
}

void __export divlibrary(LIBRARY_PARAMS)
{
    COM_export("get_timer_capacity",   getCapacity,     0);
    COM_export("get_timer_count",      getCount,        0);
    COM_export("create_timer",         createTimer,     0);
    COM_export("free_timer",           freeTimer,       1);
    COM_export("free_all_timers",      freeAll,         0);
    COM_export("get_time",             getTime,         1);
    COM_export("frame_timers",         frameTimers,     0);
    COM_export("pause_timer",          pause,           1);
    COM_export("is_timer_paused",      isPaused,        1);
    COM_export("reset_timer",          reset,           1);
}

void __export divmain(COMMON_PARAMS)
{
    GLOBAL_IMPORT();
}
