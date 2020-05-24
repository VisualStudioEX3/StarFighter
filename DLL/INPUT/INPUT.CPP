/* ----------------------------------------------------------------------------
 * INPUT.DLL - Input Manager for DIV Games Studio.
 * (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

#include "input.h"

/** Get actions max capacity.
*
* @return {int} - Returns the max number of input action allowed.
*/
void getCapacity()
{
    retval(MAX_CAPACITY);
}

/** Get actions count.
*
* @return {int} - Returns the number of input action defined.
*/
void getCount()
{
    retval(count);
}

int _create(char* name)
{
    if (count == MAX_CAPACITY || 
        strlen(name) == 0)
    {
        return RESULT_ERROR;
    }
    
    count++;

    struct InputAction *action = &actions[lastIndex];    
    strcpy(action->name, strUpr(name));
    
    return lastIndex;
}

/** Create new input action.
*
* @param {string} name - Action name.
*
* @return {int} - Returns the index of the new action.
*/
void create()
{
    retval(_create(getStringParm(getparm())));
}

int _setBindings(int index, int keyPrimary, int keySecondary, int joyButton)
{    
    if (count == 0 ||
        isValidIndex(index) == FALSE ||
        isValidKey(keyPrimary) == FALSE ||
        isValidKey(keySecondary) == FALSE ||
        isValidJoyButton(joyButton) == FALSE)
    {
        return RESULT_ERROR;
    }
    
    struct InputAction *action = &actions[index];
    action->bind.keyPrimary = keyPrimary;
    action->bind.keySecondary = keySecondary;
    action->bind.joyButton = joyButton;
    
    return RESULT_OK;
}

/** Set binding codes.
*
* @param {int} index - Action index.
* @param {int} keyPrimary - Primary key. Allow KEY_NONE.
* @param {int} keySecondary - Secondary key. Allow KEY_NONE.
* @param {int} joyButton - Joystick button. Allow JOY_NONE.
*
* @return {int} - Returns RESULT_ERROR if any parameter has invalid value.
*/
void setBindings()
{
    int joyButton = getparm();
    int keySecondary = getparm();
    int keyPrimary = getparm();
    int index = getparm();
    
    retval(_setBindings(index, keyPrimary, keySecondary, joyButton));
}

/** Gets the action name.
*
* @param {int} index - Action index.
* @param {string} dest - String variable to store name.
*
* @return {int} - Returns RESULT_ERROR if any parameter has invalid value.
*/
void getName()
{
    int varAddr = getparm();
    int index = getparm();
    
    if (count == 0 || 
        isValidIndex(index) == FALSE)
    {
        retval(RESULT_ERROR);
        return;
    }
    
    struct InputAction *action = &actions[index];
    char* src = (char*)&action->name;
    char* dest = (char*)&mem[varAddr];
    
    strcpy(dest, src);
    
    retval(RESULT_OK);
}

/** Gets the action key.
*
* @param {int} index - Action index.
* @param {int} type - Key type: 0 - Primary, 1 - Secondary.
*
* @return {int} - Returns key value. Results RESULT_ERROR if any parameter has invalid value.
*/
void getKey()
{
    int type = getparm();
    int index = getparm();
    
    if (count == 0 || 
        isValidIndex(index) == FALSE)
    {
        retval(RESULT_ERROR);
        return;
    }
    
    struct InputAction *action = &actions[index];
    retval(type <= 0 ? action->bind.keyPrimary : 
                       action->bind.keySecondary);
}

/** Gets the action joystick button.
*
* @param {int} index - Action index.
*
* @return {int} - Returns joystick button value. Results RESULT_ERROR if any parameter has invalid value.
*/
void getJoyButton()
{
    int index = getparm();
    
    if (count == 0 || 
        isValidIndex(index) == FALSE)
    {
        retval(RESULT_ERROR);
        return;
    }
        
    retval(actions[index].bind.joyButton);
}

int getJoyButtonState(int button)
{
    switch (button)
    {
        case JOY_LEFT:      return JOY->left;
        case JOY_UP:        return JOY->up;
        case JOY_RIGHT:     return JOY->right;
        case JOY_DOWN:      return JOY->down;
        case JOY_BUTTON1:   return JOY->button1;
        case JOY_BUTTON2:   return JOY->button2;
        case JOY_BUTTON3:   return JOY->button3;
        case JOY_BUTTON4:   return JOY->button4;
        default:            return 0;
    }
}

/** Update all action states. */
void frame()
{
    struct InputAction *action;
    
    for (int i = 0; i < count; i++)
    {
        action = &actions[i];
        action->state.down = action->state.up = action->state.pressed;    
        action->state.pressed = key[action->bind.keyPrimary] ||
                                key[action->bind.keySecondary] ||
                                getJoyButtonState(action->bind.joyButton);
    }
    
    retval(RESULT_OK);
}

/** Check if an action is pressed.
*
* @param {int} index - Action index.
*
* @return {int} - Returns TRUE if the keys or joystick button is pressed.
*/
void inputPressed()
{
    int index = getparm();
    
    if (count == 0 || 
        isValidIndex(index) == FALSE)
    {
        retval(RESULT_ERROR);
        return;
    }
    
    retval(actions[index].state.pressed);
}

/** Check if an action has been pressed (single hit).
*
* @param {int} index - Action index.
*
* @return {int} - Returns TRUE if the keys or joystick button has been pressed.
*/
void inputDown()
{
    int index = getparm();
    
    if (count == 0 || 
        isValidIndex(index) == FALSE)
    {
        retval(RESULT_ERROR);
        return;
    }
    
    struct InputAction *action = &actions[index];
    retval(!action->state.down && action->state.pressed);
}

/** Check if an action is released.
*
* @param {int} index - Action index.
*
* @return {int} - Returns TRUE if the keys or joystick button is released.
*/
void inputUp()
{
    int index = getparm();
    
    if (count == 0 || 
        isValidIndex(index) == FALSE)
    {
        retval(RESULT_ERROR);
        return;
    }
    
    struct InputAction *action = &actions[index];
    retval(action->state.up && !action->state.pressed);
}

/** Get the key pressed.
*
* @return {int} - Returns key code of the first button pressed. KEY_NONE if not has any key pressed.
*/
void readKeyboard()
{
    for (int i = KEY_ESC; i < KEY_F2; i++)
    {
        if (key[i])
        {
            retval(i);
            return;
        }
    }
    
    retval(KEY_NONE);
}

/** Get the joystick button pressed.
*
* @return {int} - Returns joystick button code of the first button pressed. JOY_NONE if not has any key pressed. Uses the JOY enumeration.
*/
void readJoystick()
{    
    for (int i = JOY_LEFT; i < JOY_BUTTON4; i++)
    {
        switch (i)
        {
            case JOY_LEFT:      if (JOY->left)      retval(JOY_LEFT);       return;
            case JOY_UP:        if (JOY->up)        retval(JOY_UP);         return;
            case JOY_RIGHT:     if (JOY->right)     retval(JOY_RIGHT);      return;
            case JOY_DOWN:      if (JOY->down)      retval(JOY_DOWN);       return;
            case JOY_BUTTON1:   if (JOY->button1)   retval(JOY_BUTTON1);    return;
            case JOY_BUTTON2:   if (JOY->button2)   retval(JOY_BUTTON2);    return;
            case JOY_BUTTON3:   if (JOY->button3)   retval(JOY_BUTTON3);    return;
            case JOY_BUTTON4:   if (JOY->button4)   retval(JOY_BUTTON4);    return;
        }
    }
    
    retval(JOY_NONE);
}

const char* _toKeyName(int keyCode)
{
    switch (keyCode)
    {
        case KEY_NONE:
        default:                    return STR_KEY_NONE;

        case KEY_ESC:               return STR_KEY_ESC;
        case KEY_F1:                return STR_KEY_F1;
        case KEY_F2:                return STR_KEY_F2;
        case KEY_F3:                return STR_KEY_F3;
        case KEY_F4:                return STR_KEY_F4;
        case KEY_F5:                return STR_KEY_F5;
        case KEY_F6:                return STR_KEY_F6;
        case KEY_F7:                return STR_KEY_F7;
        case KEY_F8:                return STR_KEY_F8;
        case KEY_F9:                return STR_KEY_F9;
        case KEY_F10:               return STR_KEY_F10;
        case KEY_F11:               return STR_KEY_F11;
        case KEY_F12:               return STR_KEY_F12;
        // case KEY_PRN_SCR:           return STR_KEY_PRN_SCR;
        case KEY_SCROLL_LOCK:       return STR_KEY_SCROLL_LOCK;

        case KEY_WAVE:              return STR_KEY_WAVE;
        case KEY_1:                 return STR_KEY_1;
        case KEY_2:                 return STR_KEY_2;
        case KEY_3:                 return STR_KEY_3;
        case KEY_4:                 return STR_KEY_4;
        case KEY_5:                 return STR_KEY_5;
        case KEY_6:                 return STR_KEY_6;
        case KEY_7:                 return STR_KEY_7;
        case KEY_8:                 return STR_KEY_8;
        case KEY_9:                 return STR_KEY_9;
        case KEY_0:                 return STR_KEY_0;
        case KEY_MINUS:             return STR_KEY_MINUS;
        case KEY_PLUS:              return STR_KEY_PLUS;

        case KEY_BACKSPACE:         return STR_KEY_BACKSPACE;
        case KEY_TAB:               return STR_KEY_TAB;
        case KEY_Q:                 return STR_KEY_Q;
        case KEY_W:                 return STR_KEY_W;
        case KEY_E:                 return STR_KEY_E;
        case KEY_R:                 return STR_KEY_R;
        case KEY_T:                 return STR_KEY_T;
        case KEY_Y:                 return STR_KEY_Y;
        case KEY_U:                 return STR_KEY_U;
        case KEY_I:                 return STR_KEY_I;
        case KEY_O:                 return STR_KEY_O;
        case KEY_P:                 return STR_KEY_P;
        case KEY_L_BRACHET:         return STR_KEY_L_BRACHET;
        case KEY_R_BRACHET:         return STR_KEY_R_BRACHET;
        case KEY_ENTER:             return STR_KEY_ENTER;

        case KEY_CAPS_LOCK:         return STR_KEY_CAPS_LOCK;
        case KEY_A:                 return STR_KEY_A;
        case KEY_S:                 return STR_KEY_S;
        case KEY_D:                 return STR_KEY_D;
        case KEY_F:                 return STR_KEY_F;
        case KEY_G:                 return STR_KEY_G;
        case KEY_H:                 return STR_KEY_H;
        case KEY_J:                 return STR_KEY_J;
        case KEY_K:                 return STR_KEY_K;
        case KEY_L:                 return STR_KEY_L;
        case KEY_SEMICOLON:         return STR_KEY_SEMICOLON;
        case KEY_APOSTROPHE:        return STR_KEY_APOSTROPHE;
        case KEY_BACKSLASH:         return STR_KEY_BACKSLASH;

        case KEY_L_SHIFT:           return STR_KEY_L_SHIFT;
        case KEY_Z:                 return STR_KEY_Z;
        case KEY_X:                 return STR_KEY_X;
        case KEY_C:                 return STR_KEY_C;
        case KEY_V:                 return STR_KEY_V;
        case KEY_B:                 return STR_KEY_B;
        case KEY_N:                 return STR_KEY_N;
        case KEY_M:                 return STR_KEY_M;
        // case KEY_COMMA:             return STR_KEY_COMMA;
        // case KEY_POINT:             return STR_KEY_POINT;
        case KEY_SLASH:             return STR_KEY_SLASH;
        case KEY_R_SHIFT:           return STR_KEY_R_SHIFT;

        case KEY_CONTROL:           return STR_KEY_CONTROL;
        case KEY_ALT:               return STR_KEY_ALT;
        case KEY_SPACE:             return STR_KEY_SPACE;

        case KEY_INS:               return STR_KEY_INS;
        case KEY_HOME:              return STR_KEY_HOME;
        case KEY_PGUP:              return STR_KEY_PGUP;
        case KEY_DEL:               return STR_KEY_DEL;
        case KEY_END:               return STR_KEY_END;
        case KEY_PGDN:              return STR_KEY_PGDN;

        case KEY_UP:                return STR_KEY_UP;
        case KEY_DOWN:              return STR_KEY_DOWN;
        case KEY_LEFT:              return STR_KEY_LEFT;
        case KEY_RIGHT:             return STR_KEY_RIGHT;

        case KEY_NUM_LOCK:          return STR_KEY_NUM_LOCK;
        case KEY_C_BACKSLASH:       return STR_KEY_C_BACKSLASH;
        case KEY_C_ASTERISK:        return STR_KEY_C_ASTERISK;
        case KEY_C_MINUS:           return STR_KEY_C_MINUS;
        // case KEY_C_HOME:            return STR_KEY_C_HOME;
        // case KEY_C_UP:              return STR_KEY_C_UP;
        // case KEY_C_PGUP:            return STR_KEY_C_PGUP;
        // case KEY_C_LEFT:            return STR_KEY_C_LEFT;
        case KEY_C_CENTER:          return STR_KEY_C_CENTER;
        // case KEY_C_RIGHT:           return STR_KEY_C_RIGHT;
        // case KEY_C_END:             return STR_KEY_C_END;
        // case KEY_C_DOWN:            return STR_KEY_C_DOWN;
        // case KEY_C_PGDN:            return STR_KEY_C_PGDN;
        // case KEY_C_INS:             return STR_KEY_C_INS;
        // case KEY_C_DEL:             return STR_KEY_C_DEL;
        case KEY_C_PLUS:            return STR_KEY_C_PLUS;
        // case KEY_C_ENTER:           return STR_KEY_C_ENTER;
    }
}

/** Get the constant key name.
*
* @param {int} key - Key value.
* @param {string} dest - String variable to store key name.
*/
void toKeyName()
{
    int varAddr = getparm();
    int key = getparm();
    
    char* src = (char*)_toKeyName(key);
    char* dest = (char*)&mem[varAddr];
    
    strcpy(dest, src);
    
    retval(RESULT_OK);
}

const char* _toJoyButtonName(int button)
{
    switch (button)
    {
        case JOY_NONE:      
        default:                    return STR_JOY_NONE;

        case JOY_LEFT:              return STR_JOY_LEFT;   
        case JOY_UP:                return STR_JOY_UP;     
        case JOY_RIGHT:             return STR_JOY_RIGHT;  
        case JOY_DOWN:              return STR_JOY_DOWN;   
        case JOY_BUTTON1:           return STR_JOY_BUTTON1;
        case JOY_BUTTON2:           return STR_JOY_BUTTON2;
        case JOY_BUTTON3:           return STR_JOY_BUTTON3;
        case JOY_BUTTON4:           return STR_JOY_BUTTON4;
    }
}

/** Get the constant joystick button name.
*
* @param {int} button - Joystick button value.
* @param {string} dest - String variable to store key name.
*/
void toJoyButtonName()
{
    int varAddr = getparm();
    int button = getparm();
    
    char* src = (char*)_toJoyButtonName(button);
    char* dest = (char*)&mem[varAddr];
    
    strcpy(dest, src);
    
    retval(RESULT_OK);
}

int _parseKey(char* keyName)
{
    if      (strcmp(keyName, STR_KEY_ESC))          return KEY_ESC;
    else if (strcmp(keyName, STR_KEY_F1))           return KEY_F1;
    else if (strcmp(keyName, STR_KEY_F2))           return KEY_F2;
    else if (strcmp(keyName, STR_KEY_F3))           return KEY_F3;
    else if (strcmp(keyName, STR_KEY_F4))           return KEY_F4;
    else if (strcmp(keyName, STR_KEY_F5))           return KEY_F5;
    else if (strcmp(keyName, STR_KEY_F6))           return KEY_F6;
    else if (strcmp(keyName, STR_KEY_F7))           return KEY_F7;
    else if (strcmp(keyName, STR_KEY_F8))           return KEY_F8;
    else if (strcmp(keyName, STR_KEY_F9))           return KEY_F9;
    else if (strcmp(keyName, STR_KEY_F10))          return KEY_F10;
    else if (strcmp(keyName, STR_KEY_F11))          return KEY_F11;
    else if (strcmp(keyName, STR_KEY_F12))          return KEY_F12;
    else if (strcmp(keyName, STR_KEY_PRN_SCR))      return KEY_PRN_SCR;
    else if (strcmp(keyName, STR_KEY_SCROLL_LOCK))  return KEY_SCROLL_LOCK;
    
    else if (strcmp(keyName, STR_KEY_WAVE))         return KEY_WAVE;
    else if (strcmp(keyName, STR_KEY_1))            return KEY_1;
    else if (strcmp(keyName, STR_KEY_2))            return KEY_2;
    else if (strcmp(keyName, STR_KEY_3))            return KEY_3;
    else if (strcmp(keyName, STR_KEY_4))            return KEY_4;
    else if (strcmp(keyName, STR_KEY_5))            return KEY_5;
    else if (strcmp(keyName, STR_KEY_6))            return KEY_6;
    else if (strcmp(keyName, STR_KEY_7))            return KEY_7;
    else if (strcmp(keyName, STR_KEY_8))            return KEY_8;
    else if (strcmp(keyName, STR_KEY_9))            return KEY_9;
    else if (strcmp(keyName, STR_KEY_0))            return KEY_0;
    else if (strcmp(keyName, STR_KEY_MINUS))        return KEY_MINUS;
    else if (strcmp(keyName, STR_KEY_PLUS))         return KEY_PLUS;

    else if (strcmp(keyName, STR_KEY_BACKSPACE))    return KEY_BACKSPACE;
    else if (strcmp(keyName, STR_KEY_TAB))          return KEY_TAB;
    else if (strcmp(keyName, STR_KEY_Q))            return KEY_Q;
    else if (strcmp(keyName, STR_KEY_W))            return KEY_W;
    else if (strcmp(keyName, STR_KEY_E))            return KEY_E;
    else if (strcmp(keyName, STR_KEY_R))            return KEY_R;
    else if (strcmp(keyName, STR_KEY_T))            return KEY_T;
    else if (strcmp(keyName, STR_KEY_Y))            return KEY_Y;
    else if (strcmp(keyName, STR_KEY_U))            return KEY_U;
    else if (strcmp(keyName, STR_KEY_I))            return KEY_I;
    else if (strcmp(keyName, STR_KEY_O))            return KEY_O;
    else if (strcmp(keyName, STR_KEY_P))            return KEY_P;
    else if (strcmp(keyName, STR_KEY_L_BRACHET))    return KEY_L_BRACHET;
    else if (strcmp(keyName, STR_KEY_R_BRACHET))    return KEY_R_BRACHET;
    else if (strcmp(keyName, STR_KEY_ENTER))        return KEY_ENTER;

    else if (strcmp(keyName, STR_KEY_CAPS_LOCK))    return KEY_CAPS_LOCK;
    else if (strcmp(keyName, STR_KEY_A))            return KEY_A;
    else if (strcmp(keyName, STR_KEY_S))            return KEY_S;
    else if (strcmp(keyName, STR_KEY_D))            return KEY_D;
    else if (strcmp(keyName, STR_KEY_F))            return KEY_F;
    else if (strcmp(keyName, STR_KEY_G))            return KEY_G;
    else if (strcmp(keyName, STR_KEY_H))            return KEY_H;
    else if (strcmp(keyName, STR_KEY_J))            return KEY_J;
    else if (strcmp(keyName, STR_KEY_K))            return KEY_K;
    else if (strcmp(keyName, STR_KEY_L))            return KEY_L;
    else if (strcmp(keyName, STR_KEY_SEMICOLON))    return KEY_SEMICOLON;
    else if (strcmp(keyName, STR_KEY_APOSTROPHE))   return KEY_APOSTROPHE;
    else if (strcmp(keyName, STR_KEY_BACKSLASH))    return KEY_BACKSLASH;

    else if (strcmp(keyName, STR_KEY_L_SHIFT))      return KEY_L_SHIFT;
    else if (strcmp(keyName, STR_KEY_Z))            return KEY_Z;
    else if (strcmp(keyName, STR_KEY_X))            return KEY_X;
    else if (strcmp(keyName, STR_KEY_C))            return KEY_C;
    else if (strcmp(keyName, STR_KEY_V))            return KEY_V;
    else if (strcmp(keyName, STR_KEY_B))            return KEY_B;
    else if (strcmp(keyName, STR_KEY_N))            return KEY_N;
    else if (strcmp(keyName, STR_KEY_M))            return KEY_M;
    else if (strcmp(keyName, STR_KEY_COMMA))        return KEY_COMMA;
    else if (strcmp(keyName, STR_KEY_POINT))        return KEY_POINT;
    else if (strcmp(keyName, STR_KEY_SLASH))        return KEY_SLASH;
    else if (strcmp(keyName, STR_KEY_R_SHIFT))      return KEY_R_SHIFT;

    else if (strcmp(keyName, STR_KEY_CONTROL))      return KEY_CONTROL;
    else if (strcmp(keyName, STR_KEY_ALT))          return KEY_ALT;
    else if (strcmp(keyName, STR_KEY_SPACE))        return KEY_SPACE;

    else if (strcmp(keyName, STR_KEY_INS))          return KEY_INS;
    else if (strcmp(keyName, STR_KEY_HOME))         return KEY_HOME;
    else if (strcmp(keyName, STR_KEY_PGUP))         return KEY_PGUP;
    else if (strcmp(keyName, STR_KEY_DEL))          return KEY_DEL;
    else if (strcmp(keyName, STR_KEY_END))          return KEY_END;
    else if (strcmp(keyName, STR_KEY_PGDN))         return KEY_PGDN;

    else if (strcmp(keyName, STR_KEY_UP))           return KEY_UP;
    else if (strcmp(keyName, STR_KEY_DOWN))         return KEY_DOWN;
    else if (strcmp(keyName, STR_KEY_LEFT))         return KEY_LEFT;
    else if (strcmp(keyName, STR_KEY_RIGHT))        return KEY_RIGHT;

    else if (strcmp(keyName, STR_KEY_NUM_LOCK))     return KEY_NUM_LOCK;
    else if (strcmp(keyName, STR_KEY_C_BACKSLASH))  return KEY_C_BACKSLASH;
    else if (strcmp(keyName, STR_KEY_C_ASTERISK))   return KEY_C_ASTERISK;
    else if (strcmp(keyName, STR_KEY_C_MINUS))      return KEY_C_MINUS;
    else if (strcmp(keyName, STR_KEY_C_HOME))       return KEY_C_HOME;
    else if (strcmp(keyName, STR_KEY_C_UP))         return KEY_C_UP;
    else if (strcmp(keyName, STR_KEY_C_PGUP))       return KEY_C_PGUP;
    else if (strcmp(keyName, STR_KEY_C_LEFT))       return KEY_C_LEFT;
    else if (strcmp(keyName, STR_KEY_C_CENTER))     return KEY_C_CENTER;
    else if (strcmp(keyName, STR_KEY_C_RIGHT))      return KEY_C_RIGHT;
    else if (strcmp(keyName, STR_KEY_C_END))        return KEY_C_END;
    else if (strcmp(keyName, STR_KEY_C_DOWN))       return KEY_C_DOWN;
    else if (strcmp(keyName, STR_KEY_C_PGDN))       return KEY_C_PGDN;
    else if (strcmp(keyName, STR_KEY_C_INS))        return KEY_C_INS;
    else if (strcmp(keyName, STR_KEY_C_DEL))        return KEY_C_DEL;
    else if (strcmp(keyName, STR_KEY_C_PLUS))       return KEY_C_PLUS;
    else if (strcmp(keyName, STR_KEY_C_ENTER))      return KEY_C_ENTER;
    
    else                                            return KEY_NONE;
}

/** Parse constant key name.
*
* @param {string} key - Constant key name.
*
* @return {int} - Returns key value. Returns KEY_NONE if the value is invalid.
*/
void parseKey()
{
    retval(_parseKey(getStringParm(getparm())));
}

int _parseJoyButton(char* buttonName)
{   
    if      (strcmp(buttonName, STR_JOY_LEFT))      return JOY_LEFT;   
    else if (strcmp(buttonName, STR_JOY_UP))        return JOY_UP;     
    else if (strcmp(buttonName, STR_JOY_RIGHT))     return JOY_RIGHT;  
    else if (strcmp(buttonName, STR_JOY_DOWN))      return JOY_DOWN;   
    else if (strcmp(buttonName, STR_JOY_BUTTON1))   return JOY_BUTTON1;
    else if (strcmp(buttonName, STR_JOY_BUTTON2))   return JOY_BUTTON2;
    else if (strcmp(buttonName, STR_JOY_BUTTON3))   return JOY_BUTTON3;
    else if (strcmp(buttonName, STR_JOY_BUTTON4))   return JOY_BUTTON4;
    else                                            return JOY_NONE;
}

/** Parse constant joy button name.
*
* @param {string} key - Constant key name.
*
* @return {int} - Returns key value. Returns KEY_NONE if the value is invalid.
*/
void parseJoyButton()
{
    retval(_parseJoyButton(getStringParm(getparm())));
}

void __export divlibrary(LIBRARY_PARAMS)
{   
    COM_export("get_input_capacity",    getCapacity,        0);
    COM_export("get_input_count",       getCount,           0);
    COM_export("define_input",          create,             1);
    COM_export("set_input_binds",       setBindings,        4);
    COM_export("get_input_name",        getName,            2);
    COM_export("get_input_key",         getKey,             2);
    COM_export("get_input_joy",         getJoyButton,       1);
    COM_export("frame_input",           frame,              0);
    COM_export("input_pressed",         inputPressed,       1);
    COM_export("input_down",            inputDown,          1);
    COM_export("input_up",              inputUp,            1);
    COM_export("read_input_key",        readKeyboard,       0);
    COM_export("read_input_joy",        readJoystick,       0);
    COM_export("get_input_key_name",    toKeyName,          2);
    COM_export("get_input_joy_name",    toJoyButtonName,    2);
    COM_export("parse_input_key",       parseKey,           1);
    COM_export("parse_input_joy",       parseJoyButton,     1);
}

void __export divmain(COMMON_PARAMS)
{
    GLOBAL_IMPORT();
}
