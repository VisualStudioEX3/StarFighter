/* ----------------------------------------------------------------------------
 * GIF.DLL - GIF player for DIV Games Studio 2.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 *
 * gifDec (C) lecram - 2018
 * https://github.com/lecram/gifdec
 * ---------------------------------------------------------------------------- */

#include "gif.h"

/** Load and start GIF animation.
*
* @param {int} filename - GIF filename to load.
* @param {int} x - X coordinate to render the animation.
* @param {int} y - Y coordinate to render the animation.
*
* @return {int} - Returns RESULT_OK if GIF is loaded and start to reproduce or RESULT_ERROR if any error ocurrs.
*/
void startGIF()
{
    int y = getparm();
    int x = getparm();
    char* filename = getStrParm();
    
    animation = gd_open_gif(filename);
    startTime = clock();
    
    retval(animation != NULL ? RESULT_OK : RESULT_ERROR);
}

/** End and unload GIF animation. */
void endGIF()
{
    gd_close_gif(animation);
    animation = NULL;
    startTime = 0;
    retval(RESULT_OK);
}

/** Reset GIF animation. */
void resetGIF()
{
    gd_rewind(animation);
    startTime = 0;
    retval(RESULT_OK);
}

/** Update GIF animation.
*
* @return {int} - Returns TRUE if has frames left or FALSE if not has frames left or RESULT_ERROR is occurr any error.
*/
void frameGIF()
{
    int res = RESULT_ERROR;
    
    if (file_loaded)
    {
        if (update_frame)
        {
            startTime = clock();
            res = gd_get_frame(animation);
            
            if (res != 0)
            {
                // TODO: Write animation->palette
                // TODO: Write animation->frame data to DIV screen buffer.
            }
        }
    }
    
    retval(ret);
}

void post_process_buffer(void) 
{
    
}

void __export divlibrary(LIBRARY_PARAMS)
{
    COM_export("start_gif",     startGIF,   3);
    COM_export("end_gif",       endGIF,     0);
    COM_export("reset_gif",     resetGIF,   0);
    COM_export("frame_gif",     frameGIF,   0);
}

void __export divmain(COMMON_PARAMS)
{
  GLOBAL_IMPORT();
  
  DIV_export("post_process_buffer",post_process_buffer);
}
