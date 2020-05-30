/* ----------------------------------------------------------------------------
 * TIMER.DLL - Advanced timers for DIV Games Studio.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include <time.h>
#include "..\common.h"

#define MAX_CAPACITY        64

struct TimerData
{
    int active;
    int startTime;
    int pauseDelta;
    int time;
    int paused;
};

int count = 0;
struct TimerData timers[MAX_CAPACITY];

void getCapacity();
void getCount();
void createTimer();
void freeTimer();
void freeAll();
void getTime();
void frameTimers();
void pause();
void isPaused();
void reset();
