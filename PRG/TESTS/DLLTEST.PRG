program dlltest;

import "math.dll";
import "input.dll";
import "test.dll";

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
    int i;
    int k;
    string s;

    int _input_left;
    int _input_up;
    int _input_right;
    int _input_down;
    int _input_exit;

begin
    i = 0;

    _max(15, 20); // 20
    _max(25, 20); // 25

    _min(15, 20); // 15
    _min(25, 20); // 20

    _clamp(5, 20, 25); // 20
    _clamp(25, 20, 50); // 25

    _isClamped(15, 20, 50); // 0
    _isClamped(25, 20, 50); // 1

    write_int(0, 0, 80, 0, offset timer[0]);
    write_int(0, 0, 90, 0, offset k);

    getText(s);

    s = "'" + s + "'";

    write(0, 0, 100, 0, s);

    test_input();

    repeat
        frame_input();
        frame;
    until(input_pressed(_input_exit))

    let_me_alone();
end

process test_input()
private
    int mode;
    string name;
    int count;
    string keyName;
    string joyName;

begin
    mode = 0;

    _input_left     = 0;
    _input_up       = 1;
    _input_right    = 2;
    _input_down     = 3;
    _input_exit     = 4;

    if (mode == 0)

        // Create definitions and save to file:

        define_input("left");
        define_input("up");
        define_input("right");
        define_input("down");
        define_input("exit");

        set_input_binds(_input_left,  _a,     _left,  _joy_left);
        set_input_binds(_input_up,    _w,     _up,    _joy_up);
        set_input_binds(_input_right, _d,     _right, _joy_right);
        set_input_binds(_input_down,  _s,     _down,  _joy_down);
        set_input_binds(_input_exit,  _esc,   _space, _joy_button1);


    else

        // Import definitions from text file:

    end

    load_fpg("help\help.fpg");

    x = 160;
    y = 100;
    graph = 100;

    count = get_input_count();
    get_input_name(_input_left, name);

    write(0, 0, 120, 0, name);
    write_int(0, 0, 130, 0, offset count);

    get_input_key_name(get_input_key(_input_right, 0), keyName);
    get_input_joy_name(get_input_joy(_input_right), joyName);

    write(0, 0, 150, 0, keyName);
    write(0, 0, 160, 0, joyName);

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

process print(text, var)
private
    string message;

begin
    message = text + itoa(var);
    write(0, 0, i, 0, message);
    i += 10;

    loop
        frame;
    end
end

process _max(a, b)
private
    int ret;

begin
    ret = max(a, b);
    print("Max:", ret);
end

process _min(a, b)
private
    int ret;

begin
    ret = min(a, b);
    print("Min:", ret);
end

process _clamp(a, b, c)
private
    int ret;

begin
    ret = clamp(a, b, c);
    print("Clamp:", ret);
end

process _isClamped(a, b, c)
private
    int ret;

begin
    ret = is_clamped(a, b, c);
    print("isClamped:", ret);
end
