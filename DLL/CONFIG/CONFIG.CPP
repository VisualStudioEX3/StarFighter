/* ----------------------------------------------------------------------------
 * CONFIG.DLL - INI parser for DIV Games Studio.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 *
 * minINI (C) CompuPhase, 2008-2017
 * https://github.com/compuphase/minIni
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * ---------------------------------------------------------------------------- */

#include "config.h"

/** Open INI file.
*
* @param {string} filename - INI filename.
*/
void setFile()
{
    filename = getStrParm();
    retval(RESULT_OK);
}

/** Select INI section or create new one.
*
* @param {string} section - INI section. Set empty string to select root.
*/
void setSection()
{
    section = getStrParm();
    retval(RESULT_OK);
}

/** Read a boolean value.
*
* @param {string} key - Key name.
* @param {int} defValue - Default value if key not exists.
*
* @return {int} - Returns 1 for true or 0 for false.
*/
void readBool()
{
    int defValue = getparm();
    char* key = getStrParm();

    retval((int)ini_getbool(section, key, defValue, filename));
}

/** Read an integer value.
*
* @param {string} key - Key name.
* @param {int} defValue - Default value if key not exists.
*
* @return {int} - Returns integer value.
*/
void readInt()
{
    int defValue = getparm();
    char* key = getStrParm();

    retval((int)ini_getl(section, key, defValue, filename));
}

/** Read a string value.
*
* @param {string} key - Key name.
* @param {string} defValue - Default value if key not exists.
* @param {string} dest - String variable to store name.
*
* @return {int} - Returns number of characters readed or zero if ocurr any error.
*/
void readString()
{
    char* dest = getStrParm();
    char* defValue = getStrParm();
    char* key = getStrParm();

    retval(ini_gets(section, key, defValue, dest, TEMP_BUFFER, filename));    
}

/** Write a boolean value.
*
* @param {string} key - Key name.
* @param {int} value - Integer value to write. Even numbers are true values, odd numbers are false values.
*
* @return {int} - Returns RESULT_OK if the value is written, RESULT_ERROR if occur any error.
*/
void writeBool()
{
    int value = getparm();
    char* key = getStrParm();

    retval(ini_puts(section, 
                    key, 
                    value % 2 == 0 ? 
                        "true" : 
                        "false", 
                    filename));
}

/** Write an integer value.
*
* @param {string} key - Key name.
* @param {int} value - Integer value to write.
*
* @return {int} - Returns RESULT_OK if the value is written, RESULT_ERROR if occur any error.
*/
void writeInt()
{
    int value = getparm();
    char* key = getStrParm();

    retval(ini_putl(section, key, (long)value, filename));
}

/** Write a string value.
*
* @param {string} key - Key name.
* @param {string} value - String value to write.
*
* @return {int} - Returns RESULT_OK if the value is written, RESULT_ERROR if occur any error.
*/
void writeString()
{
    char* value = getStrParm();
    char* key = getStrParm();

    retval(ini_puts(section, key, value, filename));
}

void __export divlibrary(LIBRARY_PARAMS)
{
    file_open = div_fopen;
    file_close = div_fclose;
    
    COM_export("config_set_file",       setFile,        1);
    COM_export("config_set_section",    setSection,     1);
    COM_export("config_read_bool",      readBool,       2);
    COM_export("config_read_int",       readInt,        2);
    COM_export("config_read_string",    readString,     3);
    COM_export("config_write_bool",     writeBool,      2);
    COM_export("config_write_int",      writeInt,       2);
    COM_export("config_write_string",   writeString,    2);
}

void __export divmain(COMMON_PARAMS)
{
    GLOBAL_IMPORT();
}
