/* ----------------------------------------------------------------------------
 * GIF.DLL - GIF player for DIV Games Studio 2.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 *
 * gifDec (C) lecram - 2018
 * https://github.com/lecram/gifdec
 * ---------------------------------------------------------------------------- */

#include <time.h>
#include "..\common.h"
#include "gifdec.h"

#define file_loaded     animation != NULL

#define palette_size    animation->palette->size
#define palette_data    animation->palette->colors

#define frame_length    animation->width * animation->height
#define frame_duration  animation->gce.delay
#define frame_data      animation->frame

#define update_frame    (clock() - startTime) > frame_duration;

uint8_t *screen_buffer;
uint8_t *palette_buffer;

gd_GIF *animation;
int startTime;

void startGIF();
void endGIF();
void resetGIF();
void frameGIF();
