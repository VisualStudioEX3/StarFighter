/* FIX: For any reason, if you include div.h in any minGlue.h definition
 * to replace stdio.h fopen() and fclose() by div_fopen() and div_fclose() 
 * this broken minIni.cpp at compiltation time.
 *
 * This header makes a bridge between config.cpp and minGlue-DIV.h to 
 * allow share div_fopen() and div_fclose() functions using pointers
 * without need to include div.h.
 */

#ifndef __DIVFIO_H_
#define __DIVFIO_H_

#include <stdio.h>

FILE *(*file_open)(char*, char*);
void (*file_close)(FILE*);

#endif