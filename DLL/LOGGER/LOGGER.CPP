/* ----------------------------------------------------------------------------
 * LOGGER.DLL - File logger and console for DIV Games Studio 2.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "logger.h"

void init()
{
    consoleMaxLines = _max(getparm(), default_console_max_lines);
    consoleStrLen = _max(getparm(), default_console_str_length);
    consoleEnable = getparm();
    fileEnable = getparm();

    separator = strAlloc(consoleStrLen);
    memset(separator, '=', consoleStrLen);

    if (fileEnable)
    {
        // Log filename format: "LOGS\yyyyMMdd\HHmmss.LOG"
        char filename[12];
        struct tm *now = getDateTime();

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

        if (file == NULL)
        {
            fileEnable = 0;
        }

        // Restore initial path:
        for (int i = 0; i < 2; i++)
        {
            chdir("..");
        }
    }

    if (consoleEnable)
    {
        consoleBuffer = strAlloc(console_buffer_size);
        consoleLineIndex = 0;
        clearBuffer();
    }
    consoleVisible = 0;

    _log("Log started!");
    _log(separator);

    retval(RESULT_OK);
}

void isFileEnabled()
{
    retval(fileEnable);
}

void isConsoleEnabled()
{
    retval(consoleEnable);
}

void _log(char* message)
{
    if (fileEnable)
    {
        struct tm *now = getDateTime();

        /* This method print wrong value in Watcom/DOS:
        char timeStamp[14];
        strftime(timeStamp, 14, "[%x %X]\n", now);
        fprintf(file,
                "%s %s\n",
                timeStamp,
                message); */

        // "[dd/MM/yyyy HH:mm:ss]: %message%"
        fprintf(file,
                "[%02i/%02i/%04i %02i:%02i:%02i]: %s\n",
                now->tm_mday,
                now->tm_mon + 1,
                now->tm_year + 1900,
                now->tm_hour,
                now->tm_min,
                now->tm_sec,
                message);
    }
    
    if (consoleEnable)
    {
        if (console_write_index == consoleMaxLines - 1)
        {
            for (int i = 1; i < consoleMaxLines; i++)
            {
                memcpy(line(i - 1), line(i), console_full_line_len);
            }
        }

        memcpy(line(console_write_index), 
               message, 
               _min(strlen(message), consoleStrLen));
    }
}

void log()
{
    if (enable)
    {
        _log(getStrParm());
    }
    retval(RESULT_OK);
}

void show()
{
    if (consoleEnable)
    {
        consoleVisible = 1;
    }
    retval(RESULT_OK);
}

void hide()
{
    if (consoleEnable)
    {
        consoleVisible = 0;
    }
    retval(RESULT_OK);
}

void clear()
{
    if (consoleEnable)
    {
        clearBuffer();
        consoleLineIndex = 0;
    }
    retval(RESULT_OK);
}

void post_process_buffer(void)
{
    if (consoleEnable && consoleVisible)
    {
        // TODO: Calculate rect space occuped by visible text lines.
        // TODO: Draw rect ocupped by visible text lines.

        for (int i = 0; i < consoleMaxLines; i++)
        {
            div_text_out(line(i), 0, i * 10);
        }
    }
}

void __export divlibrary(LIBRARY_PARAMS)
{
    COM_export("setup_logger",              init,               4);
    COM_export("is_logfile_enabled",        isFileEnabled,      0);
    COM_export("is_log_console_enabled",    isConsoleEnabled,   0);
    COM_export("log",                       log,                1);
    COM_export("show_log_console",          show,               0);
    COM_export("hide_log_console",          hide,               0);
    COM_export("clear_log_console",         clear,              0);
}

void __export divmain(COMMON_PARAMS)
{
  GLOBAL_IMPORT();

  DIV_export("post_process_buffer", post_process_buffer);
}

void __export divend(COMMON_PARAMS)
{
    _log(separator);
    _log("Program terminated.");

    div_fclose(file);
}
