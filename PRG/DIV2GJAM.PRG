/* ============================================================================
 * DIV2 GAME JAM Project
 *
 * A "Space Shoot 'Em Up" developed in Div Game Studio 2 'vanilla'.
 *
 * <project_name> (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
 * Div Game Studio 2 (C) Hammer Technologies - 1998 - 1999
 * ============================================================================
 */

program DIV2_GAME_JAM_PROJECT;

const
// System setup
// ============================================================================
    _working_path_dev       = "projects\div2gjam\";
    _working_path_prod      = "ex3\div2gjam\";
    _max_fps                = 60;
    _vsync                  = 1;
    _restore_mode           = no_restore;
    _show_console_at_start  = 1;
    _show_fps_counter       = 1;
// ============================================================================

// Input Manager
// ============================================================================
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
    _joy_left       = 100;
    _joy_up         = 101;
    _joy_right      = 102;
    _joy_down       = 103;
    _joy_button1    = 104;
    _joy_button2    = 105;
    _joy_button3    = 106;
    _joy_button4    = 107;
// ============================================================================

// Log console
// ============================================================================
    _log_buffer_size = 512;             // The log buffer size.
    _log_to_file = 1;                   // The log can write to file in disk.
    _log_filename = "console.log";      // Filename to write the log (if _log_to_file == 1).
    _console_width = 640;               // This value varies for desired video mode.
    _console_text_offset_x = 5;         // Offset X value in write() calls.
    _console_text_offset_y = 5;         // Offset Y value in write() calls.
    _console_background_color = 1;      // Black (DIV.PAL: 1).
    _console_border_color = 215;        // White (DIV.PAL: 15).
    _console_text_lines = 15;           // Number of visible lines in console.
// ============================================================================

// Advanced timers
// ============================================================================
    _max_xtimers = 31;
// ============================================================================

// Mode7 starfield
// ============================================================================
    _starfield_max_stars = 100;
    _starfield_width = 30;
    _starfield_height = 100;
// ============================================================================

global
// Log console
// ============================================================================
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
// ============================================================================

// Input Manager
// ============================================================================
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
// ============================================================================

// Advanced timers
// ============================================================================
    struct xtimer[_max_xtimers]
        int start_time;
        int pause_delta;
        int time;           // Time value (updated by update_xtimers()).
        int paused = 0;     // Is timer paused? (not changes this manually!)
    end
// ============================================================================

// Mode7 starfield
// ============================================================================
    int starfield_speed;
// ============================================================================

// Resources
// ============================================================================
    int fpg_background;
// ============================================================================

local
// Input Manager
// ============================================================================
    struct pointer input_actions input_action_ptr;
// ============================================================================

// Main process
// ============================================================================
begin
    initialize();

    game_scene();

    repeat
        process_input();
        frame;
    until(input_down(_input_menu));

    exit("(C) Visual Studio EX3 - 2020", 0);
end
// ============================================================================

// Initialization functions
// ============================================================================
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
    log("> Initializating log console...");
    console_id = log_console();
end

function setup_video_mode()
begin
    set_mode(m640x400);
    set_fps(_max_fps, 0);
    vsync = _vsync;
    restore_type = _restore_mode;

    log("> Initialized video mode at 640x400, " + itoa(_max_fps) + "fps, vsync: " + itoa(_vsync) + ", restore mode: " + itoa(_restore_mode));
end

function load_resources()
private
    string working_path;

begin
    // Resolve working path:
    if (chdir(_working_path_dev))
        log("> Working path: C:\DIV2\" + upper(_working_path_dev));
    else
        chdir(_working_path_prod); // DIV2 Game Jam required path.
        log("> Working path: C:\DIV2\" + upper(_working_path_prod));
    end

    load_pal("space.pal");
    fpg_background = load_fpg("fpg/space.fpg");
end

function set_default_input()
begin
    log("> Set default input bindings...");

    set_input_action(_input_left,       "LEFT",     _left,      _a,     _joy_left);
    set_input_action(_input_up,         "UP",       _up,        _w,     _joy_up);
    set_input_action(_input_right,      "RIGHT",    _right,     _d,     _joy_right);
    set_input_action(_input_down,       "DOWN",     _down,      _s,     _joy_down);

    set_input_action(_input_submit,     "SUBMIT",   _enter,     _space, _joy_button1);
    set_input_action(_input_back,       "BACK",     _esc,       0,      _joy_button2);
    set_input_action(_input_menu,       "MENU",     _esc,       0,      _joy_button4);
    set_input_action(_input_mode,       "MODE",     _tab,       0,      _joy_button3);

    set_input_action(_input_console,            "CONSOLE",          _wave,  0,  0);
    set_input_action(_input_console_up,         "CONSOLE_UP",       _up,    0,  0);
    set_input_action(_input_console_down,       "CONSOLE_DOWN",     _down,  0,  0);
    set_input_action(_input_console_page_up,    "CONSOLE_PGUP",     _pgup,  0,  0);
    set_input_action(_input_console_page_down,  "CONSOLE_PGDN",     _pgdn,  0,  0);
    set_input_action(_input_console_clear,      "CONSOLE_CLEAR",    _del,   0,  0);

    set_input_action(_input_shoot,      "SHOOT",    _control,   _space, _joy_button2);
    set_input_action(_input_special,    "SPECIAL",  _alt,       _e,     _joy_button1);
end
// ============================================================================

// Math functions
// ============================================================================
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
// ============================================================================

// Input manager
// ============================================================================
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
// ============================================================================

// Log console
// ============================================================================
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
    read_line = clamp(console_index - (_console_text_lines + 1), 0, _log_buffer_size);

    from i = 0 to _console_text_lines;
        console_ui.text[i].value = log_buffer[read_line].value;
        read_line++;
    end
end

// Allow to navigate one previous line in historic.
function console_nav_line_up()
begin
    console_index = clamp(--console_index, _console_text_lines, _log_buffer_size);

    console_update();
end

// Allow to navigate one later line in historic.
function console_nav_line_down()
begin
    console_index = clamp(++console_index, 0, log_index);

    console_update();
end

// Allow to navigate one previous page in historic.
function console_nav_page_up()
begin
    console_index -= _console_text_lines;
    console_index = clamp(console_index, _console_text_lines, _log_buffer_size);

    console_update();
end

// Allow to navigate one later page in historic.
function console_nav_page_down()
begin
    console_index += _console_text_lines;
    console_index = clamp(console_index, 0, log_index);

    console_update();
end

// Shows debug console.
function show_log_console()
private
    int i;

begin
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
// ============================================================================


// Advanced timers
// ============================================================================
function update_xtimers()
private
    int i;
begin
    from i = 0 to _max_xtimers;
        if (!xtimer[i].paused)
            xtimer[i].time = timer[0] - xtimer[i].start_time;
        else
            xtimer[i].time = xtimer[i].pause_delta - xtimer[i].start_time;
        end
    end
end

function pause_xtimer(int index)
begin
    if (!xtimer[index].paused)
        xtimer[index].pause_delta = timer[0];
        xtimer[index].paused = true;
    end
end

function resume_xtimer(int index)
begin
    if (xtimer[index].paused)
        xtimer[index].start_time += timer[0] - xtimer[index].pause_delta;
        xtimer[index].paused = false;
    end
end

function reset_xtimer(int index, int paused)
begin
    xtimer[index].start_time = timer[0];
    xtimer[index].pause_delta = 0;
    xtimer[index].paused = paused;
end
// ============================================================================

// // // // // // // // // // // // // // // // // // // // // // // // // // //

// Scrolled background
// ============================================================================
// Create the scrollable background process:
process space_background()
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
// ============================================================================

// Mode7 Starfield
// ============================================================================
// Creates a particle system to simulate speed and movement in the void.
process starfield()
private
    int i;
    int star_sprite;

begin
    star_sprite = new_map(1, 1, 0, 0, 215);

    from i = 0 to _starfield_max_stars;
        star(star_sprite);
    end

    loop
        // TODO: Replaces by condition.
        /*if (key(_esc))
            unload_map(star_sprite);
            signal(id, s_kill_tree);
        end*/
        frame;
    end
end

process star(graph)
private
    int distance = 200;

begin
    flags = 4;
    ctype = c_m7;
    x = rand(0, 300) + m7.camera.x + m7.distance;
    y = rand(-_starfield_width, _starfield_width);
    height = m7.height + rand(_starfield_height, -_starfield_height);

    loop
        repeat
            x -= starfield_speed;
            frame();
        until(m7.camera.x > x + m7.distance);

        x = distance + m7.camera.x + m7.distance;
    end
end
// ============================================================================

// Game scene process
// ============================================================================
process game_scene()
private
    int dummy_black_map;

    int background_id;
    int starfield_id;

begin
    dummy_black_map = new_map(1, 1, 0, 0, 0); // TODO: Externalize on global section.

    start_mode7(0, 0, dummy_black_map, 0, 0, 200);
    m7.distance = 32;
    m7.height = 0;
    m7.color = 0;
    m7.camera = id;

    background_id = space_background();
    starfield_id = starfield();

    starfield_speed = 3;

    loop
        if (input_pressed(_input_right))
            move_background(1, 0);
        end
        if (input_pressed(_input_left))
            move_background(-1, 0);
        end
        if (input_pressed(_input_down))
            move_background(0, 1);
        end
        if (input_pressed(_input_up))
            move_background(0, -1);
        end

        frame;
    end

    unload_map(dummy_black_map);
end
// ============================================================================