///////////////////////////////////////////////////////////////////////////////
// DIV2 GAME JAM Project
//
// A "Space Shoot 'Em Up" developed in Div Game Studio 2 'vanilla'.
//
// <project_name> (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
// Div Game Studio 2 (C) Hammer Technologies - 1998 - 1999
///////////////////////////////////////////////////////////////////////////////

program DIV2_GAME_JAM_PROJECT;

const
///////////////////////////////////////////////////////////////////////////////
// Input Manager
///////////////////////////////////////////////////////////////////////////////

    _max_input_actions = 9;

    // Input action list:
    _input_left     = 0;
    _input_up       = 1;
    _input_right    = 2;
    _input_down     = 3;
    _input_submit   = 4;
    _input_back     = 5;
    _input_menu     = 6;
    _input_mode     = 7;
    _input_shoot    = 8;
    _input_special  = 9;

    // Joystick button constants:
    _joy_left       = 100;
    _joy_up         = 101;
    _joy_right      = 102;
    _joy_down       = 103;
    _joy_button1    = 104;
    _joy_button2    = 105;
    _joy_button3    = 106;
    _joy_button4    = 107;

///////////////////////////////////////////////////////////////////////////////

global
///////////////////////////////////////////////////////////////////////////////
// Resources
///////////////////////////////////////////////////////////////////////////////

    int fpg_background;

///////////////////////////////////////////////////////////////////////////////
// Input Manager
///////////////////////////////////////////////////////////////////////////////

    // Input action definitions:
    struct input_actions[_max_input_actions]
        string name;            // Action name.
        struct bind
            int key_primary;    // Primary key.
            int key_secondary;  // Secondary key.
            int joy_button;     // Joystick button.
        end
        struct state
            int pressed;        // Is the key/button pressed?
            int down;           // Has been pressed the key/button?
            int up;             // Has been released the key/button?
        end
    end

///////////////////////////////////////////////////////////////////////////////

local
///////////////////////////////////////////////////////////////////////////////
// Input Manager
///////////////////////////////////////////////////////////////////////////////

    struct pointer input_actions input_action_ptr;

///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Main process
///////////////////////////////////////////////////////////////////////////////

begin
    initialize();

    ////////////////////////////////////////
    // Test code
    ////////////////////////////////////////
    background();

    repeat
        process_input();
        frame;
    until(input_down(_input_menu));

    exit("(C) Visual Studio EX3 - 2020", 0);
    ////////////////////////////////////////
end

///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Initialization functions
///////////////////////////////////////////////////////////////////////////////

function initialize()
begin
    setup_video_mode();
    load_resources();
    set_default_input();
end

function setup_video_mode()
begin
    set_mode(m640x400);
    set_fps(60, 0);
    vsync = 1;
end

function load_resources()
begin
    // Resolve working path:
    if (!chdir("projects\div2gjam\")) // My working directory.
        chdir("ex3\div2gjam\"); // DIV2 Game Jam required path.
    end

    load_pal("space.pal");
    fpg_background = load_fpg("fpg/space.fpg");
end

function set_default_input()
begin
    set_input_action(_input_left,       "LEFT",     _left,      _a,     _joy_left);
    set_input_action(_input_up,         "UP",       _up,        _w,     _joy_up);
    set_input_action(_input_right,      "RIGHT",    _right,     _d,     _joy_right);
    set_input_action(_input_down,       "DOWN",     _down,      _s,     _joy_down);

    set_input_action(_input_submit,     "SUBMIT",   _enter,     _space, _joy_button1);
    set_input_action(_input_back,       "BACK",     _esc,       0,      _joy_button2);
    set_input_action(_input_menu,       "MENU",     _esc,       0,      _joy_button4);
    set_input_action(_input_mode,       "MODE",     _tab,       0,      _joy_button3);

    set_input_action(_input_shoot,      "SHOOT",    _control,   _space, _joy_button2);
    set_input_action(_input_special,    "SPECIAL",  _alt,       _e,     _joy_button1);
end


///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Helper functions
///////////////////////////////////////////////////////////////////////////////

function clamp(int value, int min, int max)
begin
    if (value < min) return (min); end
    if (value > max) return (max); end
    return (value);
end

///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Input manager
///////////////////////////////////////////////////////////////////////////////

// Setup an input action bindings and name:
function set_input_action(int index, string name, int key_primary, int key_secondary, int joy_button)
begin
    input_action_ptr = offset input_actions[index];

    input_action_ptr.name = name;

    input_action_ptr.bind.key_primary   = key_primary;
    input_action_ptr.bind.key_secondary = key_secondary;
    input_action_ptr.bind.joy_button    = joy_button;
end

// Private: Process the input action state:
function process_input_action(int index)
begin
    input_action_ptr = offset input_actions[index];

    input_action_ptr.state.down = input_action_ptr.state.up = input_action_ptr.state.pressed;

    input_action_ptr.state.pressed = key_check(input_action_ptr.bind.key_primary)   or
                                     key_check(input_action_ptr.bind.key_secondary) or
                                     joy_button(input_action_ptr.bind.joy_button);
end

// Private: Read the keyboard key state:
function key_check(int key_value)
begin
    if (key_value > 0)
        return (key(key_value));
    else
        return (0);
    end
end

// Private: Read the joystick button state:
function joy_button(int joy_button_value)
begin
    switch (joy_button_value)
        case _joy_left:     return (joy.left);      end
        case _joy_up:       return (joy.up);        end
        case _joy_right:    return (joy.right);     end
        case _joy_down:     return (joy.down);      end
        case _joy_button1:  return (joy.button1);   end
        case _joy_button2:  return (joy.button2);   end
        case _joy_button3:  return (joy.button3);   end
        case _joy_button4:  return (joy.button4);   end
        default:            return (0);             end
    end
end

// Process all input actions states for the current frame:
function process_input()
private
    int i;

begin
    from i = 0 to _max_input_actions;
        process_input_action(i);
    end
end

// Is button action pressed?
function input_pressed(int index)
begin
    return (input_actions[index].state.pressed);
end

// Is button action hitted?
function input_down(int index)
begin
    return (!input_actions[index].state.down and input_actions[index].state.pressed);
end

// Is button action released?
function input_up(int index)
begin
    return (input_actions[index].state.up and !input_actions[index].state.pressed);
end

// Returns the pressed key or joystick button:
function read_input()
private
    int i;

begin
    from i = _esc to _f12;
        if (key(i)) return (i); end
    end

    from i = _joy_left to _joy_button4;
        switch (i)
            case _joy_left:     if (joy.left)       return (_joy_left);     end end
            case _joy_up:       if (joy.up)         return (_joy_up);       end end
            case _joy_right:    if (joy.right)      return (_joy_right);    end end
            case _joy_down:     if (joy.down)       return (_joy_down);     end end
            case _joy_button1:  if (joy.button1)    return (_joy_button1);  end end
            case _joy_button2:  if (joy.button2)    return (_joy_button2);  end end
            case _joy_button3:  if (joy.button3)    return (_joy_button3);  end end
            case _joy_button4:  if (joy.button4)    return (_joy_button4);  end end
        end
    end

    return (0);
end

///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Scrolled background
///////////////////////////////////////////////////////////////////////////////

// Create the scrollable background process:
process background()
private
    struct plane[1]
        int width;
        int height;
    end = 160, 80,
          80,  40;

begin
    start_scroll(0, fpg_background, 2, 1, 0, 15);

    ctype = c_scroll;

    scroll.x0 = plane[0].width / 2; scroll.y0 = plane[0].height / 2;
    scroll.x1 = plane[1].width / 2; scroll.y1 = plane[1].height / 2;

    loop
        scroll.x0 = clamp(scroll.x0, 0, plane[0].width);
        scroll.y0 = clamp(scroll.y0, 0, plane[0].height);

        scroll.x1 = clamp(scroll.x1, 0, plane[1].width);
        scroll.y1 = clamp(scroll.y1, 0, plane[1].height);

        frame;
    end // TODO: Define exit condition.

    stop_scroll(0);
end

// Move background in single steps:
function move_background(int x, int y)
begin
    if (x < 0) x = -1; end
    if (x > 0) x = 1; end

    if (y < 0) y = -1; end
    if (y > 0) y = 1; end

    scroll.x0 += x * 2;
    scroll.y0 += y * 2;

    scroll.x1 += x;
    scroll.y1 += y;
end

///////////////////////////////////////////////////////////////////////////////