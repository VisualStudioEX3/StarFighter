/* ----------------------------------------------------------------------------
 * LOGGER.DLL - File logger for DIV Games Studio 2.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "logger.h"

void init()
{
    struct tm *now = getDateTime();

    // Log filename format: "LOGS\yyyyMMdd\HHmmss.LOG"
    char filename[12];

    // Move or create base log's folder:
    mkdir(log_folder);
    chdir(log_folder);

    // Move or create today's folder:
    sprintf(filename,
            "%i%02i%02i",   // DOS foldername 8 max chars length.
            now->tm_year + 1900,
            now->tm_mon + 1,
            now->tm_mday);

    mkdir(filename);
    chdir(filename);

    // Create log file:
    sprintf(filename,
            "%02i%02i%02i.LOG", // DOS filename 8:3 format.
            now->tm_hour,
            now->tm_min,
            now->tm_sec);

    file = div_fopen(filename, "a+");

    // Restore initial path:
    for (int i = 0; i < 2; i++)
    {
        chdir("..");
    }

    char timeStamp[80];
    strftime(timeStamp, 80, "Log started on %c", now);
    log(timeStamp);
    log(separator);
}

int log(char* message)
{
    if (log_ok)
    {
        // "[dd/MM/yyyy HH:mm:ss] %message%"
        struct tm *now = getDateTime();
        fprintf(file,
                "[%02i:%02i:%02i] %s\n",
                now->tm_hour,
                now->tm_min,
                now->tm_sec,
                message);
        
        return RESULT_OK;
    }

    return RESULT_ERROR;
}

void div_log()
{
    retval(log(getStrParm()));
}

void __export divlibrary(LIBRARY_PARAMS)
{
    COM_export("log", div_log, 1);
}

void __export divmain(COMMON_PARAMS)
{
    GLOBAL_IMPORT();
    init();
}

void __export divend(COMMON_PARAMS)
{
    log(separator);
    log("Program terminated.");

    div_fclose(file);
}
