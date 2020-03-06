/* ----------------------------------------------------------------------------
 * Star Fighter - A DIV Game Studio 2 Game Jam Project
 *
 * A "Space Shoot 'Em Up" developed in Div Game Studio 2 'vanilla'.
 *
 * Star Fighter (C) VisualStudioEX3, Jos‚ Miguel S nchez Fern ndez - 2020
 * DIV Game Studio 2 (C) Hammer Technologies - 1998, 1999
 * ---------------------------------------------------------------------------- */

compiler_options _max_process = 1024, _case_sensitive;

program StarFighter;

const
// System setup
// ----------------------------------------------------------------------------
    _working_path_dev           = "projects/strfight/";
    _working_path_prod          = "ex3/strfight/";
    _log_enable                 = 1;
    _max_fps                    = 60;
    _skip_fps                   = 1;
    _vsync                      = 1;
    _restore_mode               = no_restore;
    _show_console_at_start      = 0;
    _show_fps_counter           = 1;

// Input Manager
// ----------------------------------------------------------------------------
    _max_input_actions = 15;

    // Input action list:
    _input_left                 = 0;
    _input_up                   = 1;
    _input_right                = 2;
    _input_down                 = 3;

    _input_submit               = 4;
    _input_back                 = 5;
    _input_menu                 = 6;
    _input_mode                 = 7;

    _input_console              = 8;
    _input_console_up           = 9;
    _input_console_down         = 10;
    _input_console_page_up      = 11;
    _input_console_page_down    = 12;
    _input_console_clear        = 13;

    _input_shoot                = 14;
    _input_special              = 15;

    // Joystick button constants:
    _joy_left                   = 100;
    _joy_up                     = 101;
    _joy_right                  = 102;
    _joy_down                   = 103;
    _joy_button1                = 104;
    _joy_button2                = 105;
    _joy_button3                = 106;
    _joy_button4                = 107;

// Log console
// ----------------------------------------------------------------------------
    _log_buffer_size            = 512;              // The log buffer size.
    _log_to_file                = 1;                // The log can write to file in disk.
    _log_filename               = "console.log";    // Filename to write the log (if _log_to_file == 1).
    _console_width              = 640;              // This value varies for desired video mode.
    _console_text_offset_x      = 5;                // Offset X value in write() calls.
    _console_text_offset_y      = 5;                // Offset Y value in write() calls.
    _console_background_color   = 1;                // Black (DIV.PAL: 1).
    _console_border_color       = 223;              // White (DIV.PAL: 15).
    _console_text_lines         = 15;               // Number of visible lines in console.

// Advanced timers
// ----------------------------------------------------------------------------
    _max_xtimers                = 31;

// Mode7 starfield
// ----------------------------------------------------------------------------
    _starfield_max_stars        = 100;
    _starfield_width            = 30;
    _starfield_height           = 100;
    _starfield_max_distance     = 300;

// Mode7 background asteroids
// ----------------------------------------------------------------------------
    _max_background_asteroids   = 30;

// Game scene
// ----------------------------------------------------------------------------
    _scene_size_left            = -60;
    _scene_size_top             = 100;
    _scene_size_right           = 60;
    _scene_size_bottom          = -200;
    _scene_max_distance         = 2000;             // Default draw distance in mode7.

// Player
// ----------------------------------------------------------------------------
    _max_player_shoots          = 8;

global
// Log console
// ----------------------------------------------------------------------------
    int log_index = 0;                  // Index of the next log entry.
    struct log_buffer[_log_buffer_size] // String array to store log entries.
        string value;
    end;

    int console_id;                     // Console process instance.
    int console_index;                  // Index to start to read log in console.
    int console_visible = 1;            // Is the console visible?
    struct console_ui                   // UI console elements:
        struct text[_console_text_lines]    // Console text lines:
            int obj;                        // Instances for write() objects.
            string value;                   // String instances for write() objects.
        end
        int box;                            // Box instance for background.
        int line;                           // Line instance for bottom border.
        int height;                         // Height calculated by visible lines.
    end

// Input Manager
// ----------------------------------------------------------------------------
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

// Advanced timers
// ----------------------------------------------------------------------------
    struct xtimer_data[_max_xtimers] // Not access directly to this structure. Use xtimer* functions.
        int start_time;
        int pause_delta;
        int time;           // Time value (updated by update_xtimers()).
        int paused = 0;     // Is timer paused? (not changes this manually!)
    end

// Resources
// ----------------------------------------------------------------------------
    int black_pixel;
    int white_pixel;

    int fpg_background;
    int fpg_hyperspace;
    int fpg_player;
    int fpg_ui;
    int fpg_player_shoots;
    int fpg_asteroid[3];
    int fpg_enemy[2];

// Player
// ----------------------------------------------------------------------------
    int player_id;
    int player_shoots;

// Level definition
// ----------------------------------------------------------------------------
    struct level_data       // Level data. Stores events to spawn entities:
        int lenght;             // Scene lenght. When the player reach the end, finish the level.
        int events;             // Number of events to invoke entities (max 64 per level).
        struct event[63]        // Level events:
            int position;           // Position in the level lenght.
            int entities;           // Number of entities in this event (max 8 per event).
            struct entity[7]        // Entity definition:
                int e_type;             // Entity id type.
                int x, y;               // Position in scene (Y and Height in this mode7 setup).
            end
        end
    end

    int level_position = 0;     // Current level position.
    int level_speed = 1;        // Level speed, setup by player speed.

// Hyper-space
// ----------------------------------------------------------------------------
    int in_hyperspace;

local
// Input Manager
// ----------------------------------------------------------------------------
    struct pointer input_actions input_action_ptr;

// Mode7 entity locals
// ----------------------------------------------------------------------------
    int sprite;         // Uses to store the m7_graph() id.
    int height_offset;
    int life;           // Entity life.

// Main process
// ----------------------------------------------------------------------------
begin
    initialize();

    game_scene();

    repeat
        process_input();
        xtimer_update();

        frame;
    until (input_down(_input_menu));

    exit("(C) Visual Studio EX3 - 2020", 0);
end

// Initialization functions
// ----------------------------------------------------------------------------
function initialize()
begin
    log("Initializating systems...");

    create_log_console();
    setup_video_mode();
    load_resources();
    set_default_input();

    if (_show_fps_counter) write_int(0, 640, 0, 2, offset fps); end

    log("Systems initialized!");
end

function create_log_console()
begin
    if (_log_enable)
        log("> Initializating log console...");
        console_id = log_console();
    end
end

function setup_video_mode()
begin
    set_mode(m640x400);
    set_fps(_max_fps, _skip_fps);

    vsync           = _vsync;
    restore_type    = _restore_mode;

    log("> Initialized video mode at 640x400, " + itoa(_max_fps) + "fps, vsync: " + itoa(_vsync) + ", restore mode: " + itoa(_restore_mode));
end

function load_resources()
private
    int i;

begin
    // Resolve working path:
    if (chdir(_working_path_dev))
        log("> Working path: C:/DIV2/" + upper(_working_path_dev));
    else
        chdir(_working_path_prod); // DIV2 Game Jam required path.
        log("> Working path: C:/DIV2/" + upper(_working_path_prod));
    end

    load_pal("space.pal");

    black_pixel         = new_map(1, 1, 0, 0, 0);
    white_pixel         = new_map(1, 1, 0, 0, 223);

    fpg_background      = load_fpg("fpg/space.fpg");
    fpg_hyperspace      = load_fpg("fpg/hyperspc.fpg");
    fpg_player          = load_fpg("fpg/player.fpg");
    fpg_ui              = load_fpg("fpg/ui.fpg");
    fpg_player_shoots   = load_fpg("fpg/plyshoot.fpg");

    from i = 1 to 4;
        fpg_asteroid[i - 1] = load_fpg("fpg/asterod" + itoa(i) + ".fpg");
    end

    from i = 1 to 3;
        fpg_enemy[i - 1] = load_fpg("fpg/enemy" + itoa(i) + ".fpg");
        break; // Until complete the enemy fpg lefts, only load the first.
    end
end

function set_default_input()
begin
    log("> Set default input bindings...");

    set_input_action(_input_left,               "LEFT",             _left,      _a,     _joy_left);
    set_input_action(_input_up,                 "UP",               _up,        _w,     _joy_up);
    set_input_action(_input_right,              "RIGHT",            _right,     _d,     _joy_right);
    set_input_action(_input_down,               "DOWN",             _down,      _s,     _joy_down);

    set_input_action(_input_submit,             "SUBMIT",           _enter,     _space, _joy_button1);
    set_input_action(_input_back,               "BACK",             _esc,       0,      _joy_button2);
    set_input_action(_input_menu,               "MENU",             _esc,       0,      _joy_button4);
    set_input_action(_input_mode,               "MODE",             _tab,       0,      _joy_button3);

    set_input_action(_input_console,            "CONSOLE",          _wave,      0,      0);
    set_input_action(_input_console_up,         "CONSOLE_UP",       _up,        0,      0);
    set_input_action(_input_console_down,       "CONSOLE_DOWN",     _down,      0,      0);
    set_input_action(_input_console_page_up,    "CONSOLE_PGUP",     _pgup,      0,      0);
    set_input_action(_input_console_page_down,  "CONSOLE_PGDN",     _pgdn,      0,      0);
    set_input_action(_input_console_clear,      "CONSOLE_CLEAR",    _del,       0,      0);

    set_input_action(_input_shoot,              "SHOOT",            _control,   _space, _joy_button2);
    set_input_action(_input_special,            "SPECIAL",          _alt,       _e,     _joy_button1);
end

// Math functions
// ----------------------------------------------------------------------------
function clamp(v, min_v, max_v)
begin
    if (v < min_v) return (min_v); end
    if (v > max_v) return (max_v); end
    return (v);
end

function min(a, b)
begin
    if (a =< b) return (a); else return (b); end
end

function max(a, b)
begin
    if (a => b) return (a); else return (b); end
end

function between(v, min_v, max_v)
begin
    return (v >= min_v && v <= max_v);
end

// Process functions
// ----------------------------------------------------------------------------
function typeof(int process_id)
begin
    if (process_id)
        return (process_id.reserved.process_type);
    else
        return (0);
    end
end

// Input manager
// ----------------------------------------------------------------------------
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
function read_keyboard_input()
private
    int i;

begin
    from i = _esc to _f12;
        if (key(i)) return (i); end
    end

    return (0);
end

function read_joystick_input()
private
    int i;

begin
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

// Log console
// ----------------------------------------------------------------------------
// Create an instance of log console.
process log_console()
private
    int i;

begin
    console_ui.height = (_console_text_lines + 1) * 10;
    console_ui.box = new_map(640, console_ui.height + _console_text_offset_y, 0, 0, _console_background_color);
    console_ui.line = draw(1, _console_border_color, 15, 0,
                           0, console_ui.height + _console_text_offset_y,
                           _console_width, console_ui.height + _console_text_offset_y);

    from i = 0 to _console_text_lines;
        console_ui.text[i].obj = write(0,
                                       _console_text_offset_x, (i * 10) + _console_text_offset_y,
                                       0, console_ui.text[i].value);
    end

    if (!_show_console_at_start)
        hide_log_console();
    end

    z = -255;
    graph = console_ui.box;
    flags = 4;

    loop
        if (console_visible)
            size = 100;
        else
            size = 0;
        end

        if (input_down(_input_console))
            if (console_visible)
                hide_log_console();
            else
                show_log_console();
            end
        end if (console_visible)
            if (input_down(_input_console_clear))
                clear_log();
            end if (input_down(_input_console_up))
                console_nav_line_up();
            end if (input_down(_input_console_down))
                console_nav_line_down();
            end if (input_down(_input_console_page_up))
                console_nav_page_up();
            end if (input_down(_input_console_page_down))
                console_nav_page_down();
            end
        end

        frame;
    end
end

// Private: Updated console view.
function console_update()
private
    int i, read_line;

begin
    if (!_log_enable) return; end

    read_line = clamp(console_index - (_console_text_lines + 1), 0, _log_buffer_size);

    from i = 0 to _console_text_lines;
        console_ui.text[i].value = log_buffer[read_line].value;
        read_line++;
    end
end

// Allow to navigate one previous line in log historic.
function console_nav_line_up()
begin
    if (!_log_enable) return; end

    console_index = clamp(--console_index, _console_text_lines, _log_buffer_size);

    console_update();
end

// Allow to navigate one later line in log historic.
function console_nav_line_down()
begin
    if (!_log_enable) return; end

    console_index = clamp(++console_index, 0, log_index);

    console_update();
end

// Allow to navigate one previous page in log historic.
function console_nav_page_up()
begin
    if (!_log_enable) return; end

    console_index -= _console_text_lines;
    console_index = clamp(console_index, _console_text_lines, _log_buffer_size);

    console_update();
end

// Allow to navigate one later page in log historic.
function console_nav_page_down()
begin
    if (!_log_enable) return; end

    console_index += _console_text_lines;
    console_index = clamp(console_index, 0, log_index);

    console_update();
end

// Shows debug console.
function show_log_console()
private
    int i;

begin
    if (!_log_enable) return; end

    from i = 0 to _console_text_lines;
        move_text(console_ui.text[i].obj, _console_text_offset_x, (i * 10) + _console_text_offset_y);
    end

    move_draw(console_ui.line, _console_border_color, 15,
              0, console_ui.height + _console_text_offset_y,
              _console_width, console_ui.height + _console_text_offset_y);

    console_visible = true;
end

// Hides debug console.
function hide_log_console()
private
    int i;

begin
    if (!_log_enable) return; end

    from i = 0 to _console_text_lines;
        move_text(console_ui.text[i].obj, 0, -10);
    end

    move_draw(console_ui.line, _console_border_color, 0, 0, 0, 0, 0);

    console_visible = false;
end

// Add message to log buffer.
function log(string message)
private
    int i;

    string echo_output;
begin
    if (!_log_enable) return; end

    if (_log_to_file)
        echo_output = message;
        // Avoid "ECHO IS ON" when write empty strings:
        if (echo_output == "")
            echo_output = "~";
        end
        system("echo " + echo_output + " >> " + _log_filename);
    end

    log_buffer[log_index].value = message;

    if (log_index == _log_buffer_size)
        from i = 0 to _log_buffer_size - 1;
            log_buffer[i].value = log_buffer[i + 1].value;
        end
    else
        log_index++;
        console_index = log_index;
    end

    console_update();
end

// Clear all log buffer.
function clear_log()
private
    int i;

begin
    from i = 0 to _log_buffer_size;
        log_buffer[i].value = "";
        if (i =< _console_text_lines)
            console_ui.text[i].value = "";
        end
    end

    log_index = console_index = 0;
end

// Advanced timers
// ----------------------------------------------------------------------------
function xtimer_update()
private
    int i;
begin
    from i = 0 to _max_xtimers;
        if (!xtimer_data[i].paused)
            xtimer_data[i].time = timer[0] - xtimer_data[i].start_time;
        else
            xtimer_data[i].time = xtimer_data[i].pause_delta - xtimer_data[i].start_time;
        end
    end
end

function xtimer(int index)
begin
    return (xtimer_data[index].time);
end

function xtimer_pause(int index)
begin
    if (!xtimer_data[index].paused)
        xtimer_data[index].pause_delta = timer[0];
        xtimer_data[index].paused = true;
    end
end

function xtimer_resume(int index)
begin
    if (xtimer_data[index].paused)
        xtimer_data[index].start_time += timer[0] - xtimer_data[index].pause_delta;
        xtimer_data[index].paused = false;
    end
end

function xtimer_reset(int index, int paused)
begin
    xtimer_data[index].start_time = timer[0];
    xtimer_data[index].pause_delta = 0;
    xtimer_data[index].paused = paused;
end

// // // // // // // // // // // // // // // // // // // // // // // // // // //
// G A M E   C O D E   S T A R T S   H E R E
// // // // // // // // // // // // // // // // // // // // // // // // // // //

// Scrolled background
// ----------------------------------------------------------------------------
// Create the scrollable background process:
process space_background()
private
    struct plane[1]
        int width;
        int height;
    end = 160, 80,
          80,  40;

begin
    ctype = c_scroll;

    start_scroll(0, fpg_background, 2, 1, 0, 15);

    scroll.x0 = plane[0].width / 2; scroll.y0 = plane[0].height / 2;
    scroll.x1 = plane[1].width / 2; scroll.y1 = plane[1].height / 2;

    loop
        scroll.x0 = clamp(scroll.x0, 0, plane[0].width);
        scroll.y0 = clamp(scroll.y0, 0, plane[0].height);

        scroll.x1 = clamp(scroll.x1, 0, plane[1].width);
        scroll.y1 = clamp(scroll.y1, 0, plane[1].height);

        frame;
    end
end

// Move background in single steps:
function move_background(int x, int y)
begin
    if (x < 0) x = -1; end
    if (x > 0) x = 1; end

    if (y < 0) y = -1; end
    if (y > 0) y = 1; end

    scroll.x0 += x; // * 2;
    scroll.y0 += y; // * 2;

    //scroll.x1 -= x;
    //scroll.y1 -= y;

    //m7.camera.y -= x * 2;
    m7.camera.height -= y * 2;
end

// Mode7 Starfield
// ----------------------------------------------------------------------------
// Creates a particle system to simulate speed and movement in the void.
function starfield()
private
    int i;

begin
    from i = 0 to _starfield_max_stars;
        star();
    end
end

process star()
begin
    graph = white_pixel;
    flags = 4;
    ctype = c_m7;
    x = rand(0, 300) + m7.camera.x + m7.distance;
    y = rand(-_starfield_width, _starfield_width);
    height = m7.height + rand(_starfield_height, -_starfield_height);

    loop
        repeat
            x -= level_speed;
            if (in_hyperspace) size = 0; else size = 100; end
            frame();
        until(m7.camera.x > x + m7.distance);

        x = _starfield_max_distance + m7.camera.x + m7.distance;
    end
end

// Game scene
// ----------------------------------------------------------------------------
process game_scene()
private
    int i;

begin
    init_mode7(id);

    space_background();
    starfield();
    player();
    create_background_asteroids();

    hyperspace();

    loop
        frame;
    end

    signal(type space_background, s_kill);
    signal(type star, s_kill);
    signal(type player, s_kill_tree);

    stop_mode7(0);
end

function init_mode7(int camera_id)
begin
    start_mode7(0, 0, black_pixel, 0, 0, 175);

    m7.height = 0;
    m7.camera = camera_id;
end

// Get angle between mode7 camera and process_id:
function m7_get_angle(int process_id)
begin
    return (fget_angle(m7.camera.x, m7.camera.y, process_id.x, process_id.y));
end

// Custom xgraph logic to determine the sprite to show in each height and angle:
function m7_xgraph(int process_id)
private
    int ret;

begin
    // This tables defines how the different angle sprites must be stored in the FPG:
    // +--------------------------------------------------------------+
    // | Player                                                       |
    // |--------------------------------------------------------------|
    // | Angles |  30 | 22.5|  15 | 7.5 |  0  |-7.5 | -15 |-22.5| -30 |
    // |--------+-----+-----+-----+-----+-----+-----+-----+-----+-----|
    // | Graphs | 104 | 103 | 102 | 101 | 100 | 111 | 112 | 113 | 114 |
    // +--------------------------------------------------------------+

    // +--------------------------------------+
    // | Others                               |
    // |--------------------------------------|
    // | Angles |  30 |  20 |   0 | -20 | -30 |
    // |--------+-----+-----+-----+-----+-----|
    // | Graphs | 102 | 101 | 100 | 111 | 112 |
    // +--------------------------------------+

    // This lists defines how the different height sprites must be stored in the FPG:
    // +---------------------------------------+
    // |      Player       |      Others       |
    // |-------------------+-------------------|
    // | Height    | Range | Height    | Range |
    // |-----------+-------+-----------+-------|
    // | 100 :  60 |  100  | 100 :  50 |  200  |
    // |  60 : -10 |  200  |  50 : -80 |  300  |
    // | -10 : -60 |  300  | -80 :-170 |  400  |
    // | -60 :-100 |  400  |-170 :-200 |  500  |
    // |-100 :-150 |  500  |           |       |
    // |-150 :-200 |  600  |           |       |
    // +-------------------+-------------------+

    angle = m7_get_angle(process_id);

    switch (typeof(process_id))
        case type player,
             type player_shoot:

            // Select angle view:
            switch (abs(angle))
                case     0.. 7500: ret = 0; end
                case  7500..15000: ret = 1; end
                case 15000..22500: ret = 2; end
                case 22500..30000: ret = 3; end
                default:           ret = 4; end
            end

            // Select right views if angle is under -7.5 degrees:
            if (angle <= -7500) ret += 10; end

            // Select height view:
            switch (process_id.height)
                case   60.. 100: ret += 100; end
                case  -10..  60: ret += 200; end
                case  -60.. -10: ret += 300; end
                case -100.. -60: ret += 400; end
                case -150..-100: ret += 500; end
                case -200..-150: ret += 600; end
            end

        end
        default:

            // Select angle view:
            switch (abs(angle))
                case     0..20000: ret = 0; end
                case 20000..30000: ret = 1; end
                default:           ret = 2; end
            end

            // Select right views if angle is under -15 degrees:
            if (angle <= -20000) ret += 10; end

            // Select height view:
            switch (process_id.height)
                case   50.. 100: ret += 200; end
                case  -80..  50: ret += 300; end
                case -170.. -80: ret += 400; end
                case -200..-170: ret += 500; end
            end

        end
    end

    return (ret);
end

// Uses as child process of the entity to place in mode7 scene to fix the height position:
// Set graph = -1 enables internal m7_xgraph() call.
process m7_graph(int file, int graph, int height_offset)
private
    int use_m7_xgraph = false;

begin
    ctype = c_m7;

    if (graph < 0)
        use_m7_xgraph = true;
    end

    loop
        x = father.x;
        y = father.y;
        height = father.height + height_offset;

        if (use_m7_xgraph)
            graph = m7_xgraph(father);
        end

        frame (200);
    end
end

// Process A collides with process B in mode7?
// This functions uses distance in 3D between A and B, and the radius local var of B.
function m7_collision(int a, int b)
private
    int d;

begin
    d = sqrt((b.x - a.x) * 2 + (b.y - a.y) * 2 + (b.height - a.height) * 2);

    return (d <= b.radius);
end

// Hyper-space effect background
// ----------------------------------------------------------------------------
process hyperspace()
begin
    ctype = c_m7;
    x = 101;
    size = 130;
    height = -350;
    file = fpg_hyperspace;

    in_hyperspace = true;
    level_speed = 50;

    hyperspace_hud_animation();

    repeat
        if (++graph == 10) graph = 1; end
        frame;
    until (!in_hyperspace);
end

process hyperspace_hud_animation()
private
    int _max_messages = 18;
    int _max_text_lines = 5;
    int _xtimer_id = 15;

    int i, j;
    int msg_len;
    int msg_index;

    int write_id[4];        // Write instances.
    struct write_text[4]    // String array for Write instances.
        string text;
    end

    struct messages[17]     // Messages to show in HUD:
        int line;               // Line to write the message.
        int wait;               // Wait after show message to jump to the next.
        int exit;               // Flag to call hyperspace_exit_effect() process.
        int clear;              // Clear all text lines after write the current message and complete the wait.
        string content;         // Message content to show.
    end =   0, 50,  false, false, "Warning!",
            0, 30,  false, false, " Arriving to target coordinates (333.25, 0.507, 99.257)",
            1, 10,  false, false, "Start countdown to exit hyperspace:",
            1, 100, false, false, " 3",
            1, 100, false, false, ", 2",
            1, 100, false, true,  ", 1",
            0, 20,  true,  false, "Hyperdrive deactivated...",
            1, 100, false, false, "Target reached:",
            1, 250, false, true,  " ""Sector 24, Argos asteroid fields""",
            0, 50,  false, false, "Checking systems:",
            1, 10,  false, false, " Impulse engines:",
            1, 15,  false, false, " ................... OK",
            2, 10,  false, false, " Laser weapons:  ",
            2, 15,  false, false, " ................... OK",
            3, 10,  false, false, " Combat HUD:     ",
            3, 250, false, true,  " ................... OK",
            0, 50,  false, false, "Warning!",
            0, 250, false, false, " Enemy targets detected in 0.0050 parsecs...";

    struct pointer write_text t;
    struct pointer messages m;

begin
    // Creates all text instances:
    for (i = 0; i < _max_text_lines; i++);
        write_id[i] = write(0, 10, (i + 1) * 10, 0, write_text[i].text);
    end

    // Loop animated messages:
    for (i = 0; i < _max_messages; i++)
        m = offset messages[i];         // Message to print.
        t = offset write_text[m.line];  // Line to show the message.

        if (m.exit) // If exit is enabled, call hyperspace exit effect function:
            hyperspace_exit_effect();
        end

        // Typewriter effect:
        for (msg_index = 0; msg_index < strlen(m.content); msg_index++)
            if (msg_index > 1) t.text--; end
            t.text += m.content[msg_index];
            if (msg_index > 0) t.text += "Û"; end // Print cursor at the end of line.

            frame (100);
        end

        t.text--; // Removed the text cursor at the end.

        // Apply wait before jump to the next message:
        xtimer_reset(_xtimer_id, false);
        repeat
            frame;
        until (xtimer(_xtimer_id) > m.wait);

        // Clear all text lines:
        if (m.clear)
            for (j = 0; j < _max_text_lines; j++);
                write_text[j].text = "";
                frame;
            end
        end
    end

    // Destroy all text instances:
    for (i = 0; i < _max_text_lines; i++)
        delete_text(write_id[i]);
    end
end

function hyperspace_exit_effect()
private
    int i;

begin
    // Scaling the hyperspace vortex:
    from i = 130 to 200;
        father.father.size = i;
        father.father.height -= 3;
        frame (5);
    end

    // Start flash effect:
    fade(200, 200, 200, 24);

    // Altering camera focus:
    from i = 256 to 512;
        m7.focus = i;
        frame (5);
    end

    // Disable hyperspace vortex:
    in_hyperspace = false;

    // Restoring the flash colors to normal pallete:
    fade(100, 100, 100, 8);

    // Restoring the camera focus:
    from i = 0 to 256;
        m7.focus = i;
        frame (5);
    end

    // Restoring the normal scene speed:
    from i = 50 to 5 step -1;
        level_speed = i;
        frame;
    end

    // Waiting for end fading colors:
    while (fading)
        frame;
    end
end

// Player
// ----------------------------------------------------------------------------
process player()
private
    struct _move_speed
        int x = 4;
        int y = 8;
    end
    int _default_height = -175;

begin
    ctype = c_m7;
    x = 100;
    y = 0;
    height = _default_height;

    sprite = m7_graph(fpg_player, -1, -120);

    player_id = id;
    player_shoots = 0;

//    player_aim_sight();

    loop
        if (!in_hyperspace)
            if (input_pressed(_input_right))
                y = min(y + _move_speed.x, _scene_size_right);
                if (y < _scene_size_right) move_background(1, 0); end
            end
            if (input_pressed(_input_left))
                y = max(y - _move_speed.x, _scene_size_left);
                if (y > _scene_size_left) move_background(-1, 0); end
            end
            if (input_pressed(_input_up))
                height = min(height + _move_speed.y, _scene_size_top);
                if (height < _scene_size_top) move_background(0, -1); end
            end
            if (input_pressed(_input_down))
                height = max(height - _move_speed.y, _scene_size_bottom);
                if (height > _scene_size_bottom) move_background(0, 1); end
            end

            if (input_down(_input_special))
                create_enemies();
            end

            if (input_down(_input_shoot)) player_shoot_spawner(); end
        end

//        log("Player x" + itoa(x) + " y" + itoa(y) + " h" + itoa(height) +
//            " a" + itoa(m7_get_angle(id)));

        frame;
    end
end

process create_enemies()
private
    int i;

begin
    from i = 1 to 3;
        enemy1(rand(_scene_size_left, _scene_size_right),
               rand(_scene_size_top, _scene_size_bottom));
        frame(250);
    end
end

process player_aim_sight()
begin
    ctype = c_m7;
    x = father.x + 300;

    m7_graph(fpg_ui, 1, -80).size = 400;

    loop
        y = father.y;
        height = father.height;
        frame (200);
    end
end

function player_shoot_spawner()
private
    int _offset = 10;

begin
    player_shoot(father.x + 1, father.y + _offset, father.height);
    player_shoot(father.x + 1, father.y - _offset, father.height);
end

process player_shoot(int x, int y, int height)
private
    int _step = 20;
    int _max_distance = 1000;
    int _max_steps = 0;

    int i;

begin
    _max_steps = _max_distance / _step;

    ctype = c_m7;

    if (player_shoots == _max_player_shoots) return; end

    sprite = m7_graph(fpg_player_shoots, -1, -85);
    sprite.size = 150;

    player_shoots++;

    for (i = 0; i < _max_steps; i++)
        x += _step;
        frame;
    end

    player_shoots--;
    signal(sprite, s_kill);
end

function create_background_asteroids()
private
    int i;

begin
    from i = 0 to _max_background_asteroids;
        background_asteroid();
    end
end

function destroy_background_asteroids()
begin
    signal(type background_asteroid, s_kill);
end

process background_asteroid()
begin
    ctype = c_m7;

    set_background_asteroid_random_position(id);
    x = rand(m7.camera.x + m7.distance, _scene_max_distance);

    file = fpg_asteroid[rand(0, 2)];
    graph = get_random_view_index();
    size = rand(100, 300);

    loop
        repeat
            x -= level_speed;
            frame();
        until(m7.camera.x > x + m7.distance);

        set_background_asteroid_random_position(id);
    end
end

function get_random_view_index()
private
    int i = 0;

begin
    loop
        switch (i)
            case 100..104, 111..114,
                 200..204, 211..214,
                 300..304, 311..314,
                 400..404, 411..414,
                 500..504, 511..514,
                 600..604, 611..614:
                    return (i);
                end
            default:
                i = rand(100, 614);
            end
        end
    end
end

function set_background_asteroid_random_position(process_id)
private
    struct _max
        int x = 600;
        int y = 800;
    end

begin
    process_id.x        = m7.camera.x + m7.distance + _scene_max_distance + _max.x;
    process_id.height   = rand(_scene_size_bottom - _max.y, _scene_size_top + _max.y);

    if (rand(0, 10))
        process_id.y = rand(_scene_size_left - _max.x, _scene_size_left - (_max.x / 2));

    else
        process_id.y = rand(_scene_size_right + (_max.x / 2), _scene_size_right + _max.x);
    end
end

// Asteroid obstacle:
process asteroid(x, y)
begin
    ctype = c_m7;
    height = y;
    y = x;
    x = m7.camera.x + m7.distance + _scene_max_distance;

    sprite = m7_graph(fpg_asteroid[rand(0, 2)], -1, -64);

    repeat
        // TODO: Check collision if reach player position.
        x -= level_speed;
        frame;
    until(m7.camera.x > x + m7.distance);

    signal(sprite, s_kill);
end

// Enemy type 1;
process enemy1(x, y)
begin
    ctype = c_m7;
    height = y;
    y = x;
    x = m7.camera.x + m7.distance + _scene_max_distance;

    sprite = m7_graph(fpg_enemy[0], -1, -128);

    repeat
        // TODO: Check collision if reach player position.
        x -= level_speed;
        frame;
    until(m7.camera.x > x + m7.distance);

    signal(sprite, s_kill);
end