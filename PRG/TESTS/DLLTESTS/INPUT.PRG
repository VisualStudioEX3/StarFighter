program INPUT_DLL_TEST;

import "input.dll";

const
    // Joystick button constants:
    _joy_none                   = 0;
    _joy_left                   = 1;
    _joy_up                     = 2;
    _joy_right                  = 3;
    _joy_down                   = 4;
    _joy_button1                = 5;
    _joy_button2                = 6;
    _joy_button3                = 7;
    _joy_button4                = 8;

global
    int     _input_left;
    int     _input_up;
    int     _input_right;
    int     _input_down;
    int     _input_exit;

local
    int     count;
    string  actionName;
    string  pKeyName;
    string  sKeyName;
    string  joyName;

begin
    write(0, 0, 0, 0, "Press esc or space to exit.");

    create_input_definitions();

    print_input_action_count();
    print_input_action_name(_input_exit);
    print_input_action_values(_input_down);

    ball();

    repeat
        frame_input();
        frame;
    until(input_pressed(_input_exit))

    let_me_alone();
end

function create_input_definitions()
begin
    _input_left     = define_input("left");
    _input_up       = define_input("up");
    _input_right    = define_input("right");
    _input_down     = define_input("down");
    _input_exit     = define_input("exit");

    set_input_binds(_input_left,  _a,     _left,  _joy_left);
    set_input_binds(_input_up,    _w,     _up,    _joy_up);
    set_input_binds(_input_right, _d,     _right, _joy_right);
    set_input_binds(_input_down,  _s,     _down,  _joy_down);
    set_input_binds(_input_exit,  _esc,   _space, _joy_button1);
end

process print_input_action_count()
begin
    count = get_input_count();
    write_int(0, 0, 20, 0, offset count);

    loop
        frame;
    end
end

process print_input_action_name(action)
begin
    get_input_name(action, actionName);

    write(0, 0, 40, 0, actionName);

    loop
        frame;
    end
end

process print_input_action_values(action)
begin
    get_input_key_name(get_input_key(action, 0), pKeyName);
    get_input_key_name(get_input_key(action, 1), sKeyName);
    get_input_joy_name(get_input_joy(action), joyName);

    write(0, 0, 60, 0, pKeyName);
    write(0, 0, 70, 0, sKeyName);
    write(0, 0, 80, 0, joyName);

    loop
        frame;
    end
end

process ball()
begin
    load_fpg("help\help.fpg");

    x = 160;
    y = 100;
    graph = 100;

    loop
        if (input_pressed(_input_left))
            x--;
        end

        if (input_pressed(_input_right))
            x++;
        end

        if (input_down(_input_up))
            y--;
        end

        if (input_up(_input_down))
            y++;
        end

        frame;
    end
end
