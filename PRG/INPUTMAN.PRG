///////////////////////////////////////////////////////////////////////////////
// Input Manager for Div Game Studio 2
///////////////////////////////////////////////////////////////////////////////

program Input_Manager;

const
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

global
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

local
    struct pointer input_actions input_action_ptr;

private
    int input_code;

begin
    set_defaults();

    write(0, 0, 0, 0, "Single Player Input Manager test.");

    write(0, 10, 20, 0, "Input received: ");
    write_int(0, 105, 20, 0, offset input_code);

    print_input(_input_left,    10, 40);
    print_input(_input_up,      10, 50);
    print_input(_input_right,   10, 60);
    print_input(_input_down,    10, 70);

    print_input(_input_submit,  10, 80);
    print_input(_input_back,    10, 90);

    print_input(_input_menu,    10, 110);
    print_input(_input_mode,    10, 120);

    print_input(_input_shoot,   10, 140);
    print_input(_input_special, 10, 150);

    loop
        process_input();
        input_code = read_input();
        frame;
    end
end

// Sample process to print input action states:
process print_input(int a, int x, int y)
private
    int is_pressed, is_down, is_up;
    string message;

begin
    write(0, x, y, 0, message);
    loop
        is_pressed = input_pressed(a);
        is_down = input_down(a);
        is_up = input_up(a);

        message = "Action: " + input_actions[a].name +
                  ", Pressed: " + itoa(is_pressed) +
                  ", Down: " + itoa(is_down) +
                  ", Up: " + itoa(is_up);

        frame;
    end
end

function set_defaults()
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
// Input manager code
///////////////////////////////////////////////////////////////////////////////

function set_input_action(int index, string name, int key_primary, int key_secondary, int joy_button)
begin
    input_action_ptr = offset input_actions[index];

    input_action_ptr.name = name;

    input_action_ptr.bind.key_primary   = key_primary;
    input_action_ptr.bind.key_secondary = key_secondary;
    input_action_ptr.bind.joy_button    = joy_button;
end

function process_input_action(int index)
begin
    input_action_ptr = offset input_actions[index];

    input_action_ptr.state.down = input_action_ptr.state.up = input_action_ptr.state.pressed;

    input_action_ptr.state.pressed = key_check(input_action_ptr.bind.key_primary)   or
                                     key_check(input_action_ptr.bind.key_secondary) or
                                     joy_button(input_action_ptr.bind.joy_button);
end

function key_check(int key_value)
begin
    if (key_value > 0)
        return (key(key_value));
    else
        return (0);
    end
end

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

function process_input()
private
    int i;

begin
    from i = 0 to _max_input_actions;
        process_input_action(i);
    end
end

function input_pressed(int index)
begin
    return (input_actions[index].state.pressed);
end

function input_down(int index)
begin
    return (!input_actions[index].state.down and input_actions[index].state.pressed);
end

function input_up(int index)
begin
    return (input_actions[index].state.up and !input_actions[index].state.pressed);
end

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