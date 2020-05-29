/*  Glue functions for the minIni library, based on the C/C++ stdio library and DIV I/O file functions.
 *
 *  Or better said: this file contains macros that maps the function interface
 *  used by minIni to the standard C/C++ and DIV file I/O functions.
 *
 *  By CompuPhase, 2008-2014
 *  (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 *  DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 *
 *  This "glue file" is in the public domain. It is distributed without
 *  warranties or conditions of any kind, either express or implied.
 */

#define INI_ANSIONLY
#define INI_BUFFERSIZE                  256

#include "divfio.h"

#define INI_FILETYPE                    FILE*
#define ini_openread(filename,file)     ((*(file) = (*file_open)(((char*)filename),"rb")) != NULL)
#define ini_openwrite(filename,file)    ((*(file) = (*file_open)(((char*)filename),"wb")) != NULL)
#define ini_openrewrite(filename,file)  ((*(file) = (*file_open)(((char*)filename),"r+b")) != NULL)
#define ini_close(file)                 ((*file_close)(*(file)))

#define ini_read(buffer,size,file)      (fgets((buffer),(size),*(file)) != NULL)
#define ini_write(buffer,file)          (fputs((buffer),*(file)) >= 0)
#define ini_rename(source,dest)         (rename((source), (dest)) == 0)
#define ini_remove(filename)            (remove(filename) == 0)

#define INI_FILEPOS                     int
#define ini_tell(file,pos)              (*(pos) = ftell(*(file)))
#define ini_seek(file,pos)              (fseek(*(file), *(pos), SEEK_SET) == 0)
