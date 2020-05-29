/* ----------------------------------------------------------------------------
 * MATH.DLL - Math extenstions for DIV Games Studio.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "math.h"

/** Min function.
*
* @param {int} a - Value a.
* @param {int} b - Value b.
*
* @return {int} Returns the min value between 2 numbers.
*/
void min()
{
    int b = getparm();
    int a = getparm();

    retval(_min(a, b));
}

/** Max function.
*
* @param {int} a - Value a.
* @param {int} b - Value b.
*
* @return {int} Returns the max value between 2 numbers.
*/
void max()
{
    int b = getparm();
    int a = getparm();

    retval(_max(a, b));
}

/** Clamp function.
*
* @param {int} value - Value to fit.
* @param {int} min - Min value range.
* @param {int} max - Max value range.
*
* @return {int} Returns value clamped to the inclusive range of min and max.
*/
void clamp()
{
    int max = getparm();
    int min = getparm();
    int value = getparm();

    retval(_clamp(value, min, max));
}

/** Is value clamped between min and max values?
*
* @param {int} value - Value to check.
* @param {int} min - Min value range.
* @param {int} max - Max value range.
*
* @return {int} Returns 1 if value is between min and max values, 0 if not.
*/
void isClamped()
{
    int max = getparm();
    int min = getparm();
    int value = getparm();

    retval(_isClamped(value, min, max));
}

void __export divlibrary(LIBRARY_PARAMS)
{
    COM_export("min",           min,        2);
    COM_export("max",           max,        2);
    COM_export("clamp",         clamp,      3);
    COM_export("is_clamped",    isClamped,  3);
}

void __export divmain(COMMON_PARAMS)
{
    GLOBAL_IMPORT();
}
