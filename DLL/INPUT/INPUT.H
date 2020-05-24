/* ----------------------------------------------------------------------------
 * INPUT.DLL - Input Manager for DIV Games Studio.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#ifndef __INPUT_H_
#define __INPUT_H_

#include "..\common.h"

const int MAX_CAPACITY                  = 32;
const int NAME_LENGTH                   = 32;

// DIV input key codes:
const int KEY_NONE                      = 0;

const int KEY_ESC                       = 1;
const int KEY_F1                        = 59;
const int KEY_F2                        = 60;
const int KEY_F3                        = 61;
const int KEY_F4                        = 62;
const int KEY_F5                        = 63;
const int KEY_F6                        = 64;
const int KEY_F7                        = 65;
const int KEY_F8                        = 66;
const int KEY_F9                        = 67;
const int KEY_F10                       = 68;
const int KEY_F11                       = 87;
const int KEY_F12                       = 88;
const int KEY_PRN_SCR                   = 55;
const int KEY_SCROLL_LOCK               = 70;

const int KEY_WAVE                      = 41;
const int KEY_1                         = 2;
const int KEY_2                         = 3;
const int KEY_3                         = 4;
const int KEY_4                         = 5;
const int KEY_5                         = 6;
const int KEY_6                         = 7;
const int KEY_7                         = 8;
const int KEY_8                         = 9;
const int KEY_9                         = 10;
const int KEY_0                         = 11;
const int KEY_MINUS                     = 12;
const int KEY_PLUS                      = 13;

const int KEY_BACKSPACE                 = 14;
const int KEY_TAB                       = 15;
const int KEY_Q                         = 16;
const int KEY_W                         = 17;
const int KEY_E                         = 18;
const int KEY_R                         = 19;
const int KEY_T                         = 20;
const int KEY_Y                         = 21;
const int KEY_U                         = 22;
const int KEY_I                         = 23;
const int KEY_O                         = 24;
const int KEY_P                         = 25;
const int KEY_L_BRACHET                 = 26;
const int KEY_R_BRACHET                 = 27;
const int KEY_ENTER                     = 28;

const int KEY_CAPS_LOCK                 = 58;
const int KEY_A                         = 30;
const int KEY_S                         = 31;
const int KEY_D                         = 32;
const int KEY_F                         = 33;
const int KEY_G                         = 34;
const int KEY_H                         = 35;
const int KEY_J                         = 36;
const int KEY_K                         = 37;
const int KEY_L                         = 38;
const int KEY_SEMICOLON                 = 39;
const int KEY_APOSTROPHE                = 40;
const int KEY_BACKSLASH                 = 43;

const int KEY_L_SHIFT                   = 42;
const int KEY_Z                         = 44;
const int KEY_X                         = 45;
const int KEY_C                         = 46;
const int KEY_V                         = 47;
const int KEY_B                         = 48;
const int KEY_N                         = 49;
const int KEY_M                         = 50;
const int KEY_COMMA                     = 51;
const int KEY_POINT                     = 51;
const int KEY_SLASH                     = 51;
const int KEY_R_SHIFT                   = 54;

const int KEY_CONTROL                   = 29;
const int KEY_ALT                       = 56;
const int KEY_SPACE                     = 57;

const int KEY_INS                       = 82;
const int KEY_HOME                      = 71;
const int KEY_PGUP                      = 73;
const int KEY_DEL                       = 83;
const int KEY_END                       = 79;
const int KEY_PGDN                      = 81;

const int KEY_UP                        = 72;
const int KEY_DOWN                      = 80;
const int KEY_LEFT                      = 75;
const int KEY_RIGHT                     = 77;

const int KEY_NUM_LOCK                  = 69;
const int KEY_C_BACKSLASH               = 53;
const int KEY_C_ASTERISK                = 55;
const int KEY_C_MINUS                   = 74;
const int KEY_C_HOME                    = 71;
const int KEY_C_UP                      = 72;
const int KEY_C_PGUP                    = 73;
const int KEY_C_LEFT                    = 75;
const int KEY_C_CENTER                  = 76;
const int KEY_C_RIGHT                   = 77;
const int KEY_C_END                     = 79;
const int KEY_C_DOWN                    = 80;
const int KEY_C_PGDN                    = 81;
const int KEY_C_INS                     = 82;
const int KEY_C_DEL                     = 83;
const int KEY_C_PLUS                    = 78;
const int KEY_C_ENTER                   = 28;

// Joystick button constants:
const int JOY_NONE                      = 0;
const int JOY_LEFT                      = 1;
const int JOY_UP                        = 2;
const int JOY_RIGHT                     = 3;
const int JOY_DOWN                      = 4;
const int JOY_BUTTON1                   = 5;
const int JOY_BUTTON2                   = 6;
const int JOY_BUTTON3                   = 7;
const int JOY_BUTTON4                   = 8;

// Parsed key values:
const char* const STR_KEY_NONE          = "KEY_NONE";

const char* const STR_KEY_ESC           = "KEY_ESC";
const char* const STR_KEY_F1            = "KEY_F1";
const char* const STR_KEY_F2            = "KEY_F2";
const char* const STR_KEY_F3            = "KEY_F3";
const char* const STR_KEY_F4            = "KEY_F4";
const char* const STR_KEY_F5            = "KEY_F5";
const char* const STR_KEY_F6            = "KEY_F6";
const char* const STR_KEY_F7            = "KEY_F7";
const char* const STR_KEY_F8            = "KEY_F8";
const char* const STR_KEY_F9            = "KEY_F9";
const char* const STR_KEY_F10           = "KEY_F10";
const char* const STR_KEY_F11           = "KEY_F11";
const char* const STR_KEY_F12           = "KEY_F12";
const char* const STR_KEY_PRN_SCR       = "KEY_PRN_SCR";
const char* const STR_KEY_SCROLL_LOCK   = "KEY_SCROLL_LOCK";

const char* const STR_KEY_WAVE          = "KEY_WAVE";
const char* const STR_KEY_1             = "KEY_1";
const char* const STR_KEY_2             = "KEY_2";
const char* const STR_KEY_3             = "KEY_3";
const char* const STR_KEY_4             = "KEY_4";
const char* const STR_KEY_5             = "KEY_5";
const char* const STR_KEY_6             = "KEY_6";
const char* const STR_KEY_7             = "KEY_7";
const char* const STR_KEY_8             = "KEY_8";
const char* const STR_KEY_9             = "KEY_9";
const char* const STR_KEY_0             = "KEY_0";
const char* const STR_KEY_MINUS         = "KEY_MINUS";
const char* const STR_KEY_PLUS          = "KEY_PLUS";

const char* const STR_KEY_BACKSPACE     = "KEY_BACKSPACE";
const char* const STR_KEY_TAB           = "KEY_TAB";
const char* const STR_KEY_Q             = "KEY_Q";
const char* const STR_KEY_W             = "KEY_W";
const char* const STR_KEY_E             = "KEY_E";
const char* const STR_KEY_R             = "KEY_R";
const char* const STR_KEY_T             = "KEY_T";
const char* const STR_KEY_Y             = "KEY_Y";
const char* const STR_KEY_U             = "KEY_U";
const char* const STR_KEY_I             = "KEY_I";
const char* const STR_KEY_O             = "KEY_O";
const char* const STR_KEY_P             = "KEY_P";
const char* const STR_KEY_L_BRACHET     = "KEY_L_BRACHET";
const char* const STR_KEY_R_BRACHET     = "KEY_R_BRACHET";
const char* const STR_KEY_ENTER         = "KEY_ENTER";

const char* const STR_KEY_CAPS_LOCK     = "KEY_CAPS_LOCK";
const char* const STR_KEY_A             = "KEY_A";
const char* const STR_KEY_S             = "KEY_S";
const char* const STR_KEY_D             = "KEY_D";
const char* const STR_KEY_F             = "KEY_F";
const char* const STR_KEY_G             = "KEY_G";
const char* const STR_KEY_H             = "KEY_H";
const char* const STR_KEY_J             = "KEY_J";
const char* const STR_KEY_K             = "KEY_K";
const char* const STR_KEY_L             = "KEY_L";
const char* const STR_KEY_SEMICOLON     = "KEY_SEMICOLON";
const char* const STR_KEY_APOSTROPHE    = "KEY_APOSTROPHE";
const char* const STR_KEY_BACKSLASH     = "KEY_BACKSLASH";

const char* const STR_KEY_L_SHIFT       = "KEY_L_SHIFT";
const char* const STR_KEY_Z             = "KEY_Z";
const char* const STR_KEY_X             = "KEY_X";
const char* const STR_KEY_C             = "KEY_C";
const char* const STR_KEY_V             = "KEY_V";
const char* const STR_KEY_B             = "KEY_B";
const char* const STR_KEY_N             = "KEY_N";
const char* const STR_KEY_M             = "KEY_M";
const char* const STR_KEY_COMMA         = "KEY_COMMA";
const char* const STR_KEY_POINT         = "KEY_POINT";
const char* const STR_KEY_SLASH         = "KEY_SLASH";
const char* const STR_KEY_R_SHIFT       = "KEY_R_SHIFT";

const char* const STR_KEY_CONTROL       = "KEY_CONTROL";
const char* const STR_KEY_ALT           = "KEY_ALT";
const char* const STR_KEY_SPACE         = "KEY_SPACE";

const char* const STR_KEY_INS           = "KEY_INS";
const char* const STR_KEY_HOME          = "KEY_HOME";
const char* const STR_KEY_PGUP          = "KEY_PGUP";
const char* const STR_KEY_DEL           = "KEY_DEL";
const char* const STR_KEY_END           = "KEY_END";
const char* const STR_KEY_PGDN          = "KEY_PGDN";

const char* const STR_KEY_UP            = "KEY_UP";
const char* const STR_KEY_DOWN          = "KEY_DOWN";
const char* const STR_KEY_LEFT          = "KEY_LEFT";
const char* const STR_KEY_RIGHT         = "KEY_RIGHT";

const char* const STR_KEY_NUM_LOCK      = "KEY_NUM_LOCK";
const char* const STR_KEY_C_BACKSLASH   = "KEY_C_BACKSLASH";
const char* const STR_KEY_C_ASTERISK    = "KEY_C_ASTERISK";
const char* const STR_KEY_C_MINUS       = "KEY_C_MINUS";
const char* const STR_KEY_C_HOME        = "KEY_C_HOME";
const char* const STR_KEY_C_UP          = "KEY_C_UP";
const char* const STR_KEY_C_PGUP        = "KEY_C_PGUP";
const char* const STR_KEY_C_LEFT        = "KEY_C_LEFT";
const char* const STR_KEY_C_CENTER      = "KEY_C_CENTER";
const char* const STR_KEY_C_RIGHT       = "KEY_C_RIGHT";
const char* const STR_KEY_C_END         = "KEY_C_END";
const char* const STR_KEY_C_DOWN        = "KEY_C_DOWN";
const char* const STR_KEY_C_PGDN        = "KEY_C_PGDN";
const char* const STR_KEY_C_INS         = "KEY_C_INS";
const char* const STR_KEY_C_DEL         = "KEY_C_DEL";
const char* const STR_KEY_C_PLUS        = "KEY_C_PLUS";
const char* const STR_KEY_C_ENTER       = "KEY_C_ENTER";

// Parsed joystick button values:
const char* const STR_JOY_NONE          = "JOY_NONE";
const char* const STR_JOY_LEFT          = "JOY_LEFT";
const char* const STR_JOY_UP            = "JOY_UP";
const char* const STR_JOY_RIGHT         = "JOY_RIGHT";
const char* const STR_JOY_DOWN          = "JOY_DOWN";
const char* const STR_JOY_BUTTON1       = "JOY_BUTTON1";
const char* const STR_JOY_BUTTON2       = "JOY_BUTTON2";
const char* const STR_JOY_BUTTON3       = "JOY_BUTTON3";
const char* const STR_JOY_BUTTON4       = "JOY_BUTTON4";

// Structures:
struct InputBind
{
    int keyPrimary;
    int keySecondary;
    int joyButton;
};

struct InputState
{
    int pressed;
    int down;
    int up;
};

struct InputAction
{
    char name[NAME_LENGTH];
    struct InputBind bind;
    struct InputState state;
};

// Macros:
#define lastIndex           count - 1
#define isValidIndex(i)     _isClamped(i, 0, lastIndex)
#define isValidKey(k)       _isClamped(k, KEY_NONE, KEY_F12)
#define isValidJoyButton(b) _isClamped(b, JOY_NONE, JOY_BUTTON4)

// Variables:
int count = 0;
struct InputAction actions[MAX_CAPACITY];

// Methods & Functions:
void getCapacity();
void getCount();

int _create(char* name);
void create();
int _setBindings(int index, int keyPrimary, int keySecondary, int joyButton);
void setBindings();

void getName();
void getKey();
void getJoyButton();

int getJoyButtonState(int button);

void frame();

void inputPressed();
void inputDown();
void inputUp();

void readKeyboard();
void readJoystick();

const char* _toKeyName(int keyCode);
void toKeyName();
const char* _toJoyButtonName(int button);
void toJoyButtonName();

int _parseKey(char* keyName);
void parseKey();
int _parseJoyButton(char* buttonName);
void parseJoyButton();

#endif