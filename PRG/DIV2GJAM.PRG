/* ============================================================================
 * DIV2 GAME JAM Project
 *
 * A "Space Shoot 'Em Up" developed in Div Game Studio 2 'vanilla'.
 *
 * <project_name> (C) VisualStudioEX3, JosÃ© Miguel SÃ¡nchez FernÃ¡ndez - 2020
 * Div Game Studio 2 (C) Hammer Technologies - 1998 - 1999
 * ============================================================================
 */

program DIV2_GAME_JAM_PROJECT;

const
// System setup
// ============================================================================
    _working_path_dev       = "projects/div2gjam/";
    _working_path_prod      = "ex3/div2gjam/";
    _max_fps                = 60;
    _vsync                  = 1;
    _restore_mode           = no_restore;
    _show_console_at_start  = 1;
    _show_fps_counter       = 1;

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

// Log console
// ============================================================================
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
// ============================================================================
    _max_xtimers = 31;

// Mode7 starfield
// ============================================================================
    _starfield_max_stars    = 100;
    _starfield_width        = 30;
    _starfield_height       = 100;

// Game scene
// ============================================================================
    _scene_size_left        = -60;
    _scene_size_top         = 100;
    _scene_size_right       = 60;
    _scene_size_bottom      = -200;

// Player
// ============================================================================
    _max_player_shoots      = 3;

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

// Advanced timers
// ============================================================================
    struct xtimer[_max_xtimers]
        int start_time;
        int pause_delta;
        int time;           // Time value (updated by update_xtimers()).
        int paused = 0;     // Is timer paused? (not changes this manually!)
    end

// Mode7 starfield
// ============================================================================
    int starfield_speed;

// Resources
// ============================================================================
    int black_pixel;
    int white_pixel;

    int fpg_background;
    int fpg_hyperspace;
    int fpg_player;
    int fpg_ui;
    int fpg_shoots;

// Player shoots
// ============================================================================
    int player_shoots;

local
// Input Manager
// ============================================================================
    struct pointer input_actions input_action_ptr;

// Height Offset for mode7 objects
// ============================================================================
    int sprite;         // Uses to store the m7_graph() id.
    int height_offset;

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
begin
    // Resolve working path:
    if (chdir(_working_path_dev))
        log("> Working path: C:/DIV2/" + upper(_working_path_dev));
    else
        chdir(_working_path_prod); // DIV2 Game Jam required path.
        log("> Working path: C:/DIV2/" + upper(_working_path_prod));
    end

    load_pal("space.pal");

    black_pixel     = new_map(1, 1, 0, 0, 0);
    white_pixel     = new_map(1, 1, 0, 0, 223);

    fpg_background  = load_fpg("fpg/space.fpg");
    fpg_hyperspace  = load_fpg("fpg/hyperspc.fpg");
    fpg_player      = load_fpg("fpg/player.fpg");
    fpg_ui          = load_fpg("fpg/ui.fpg");
    fpg_shoots      = load_fpg("fpg/shoots.fpg");

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

function between(v, min_v, max_v)
begin
    return (v >= min_v && v <= max_v);
end

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

// // // // // // // // // // // // // // // // // // // // // // // // // // //
// G A M E   C O D E   S T A R T S   H E R E
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

// Mode7 Starfield
// ============================================================================
// Creates a particle system to simulate speed and movement in the void.
process starfield()
private
    int i;

begin
    from i = 0 to _starfield_max_stars;
        star();
    end

    loop
        frame;
    end
end

process star()
private
    int distance = 200;

begin
    graph = white_pixel;
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

// Game scene
// ============================================================================
process game_scene()
begin
    init_mode7(id);

    space_background();
    starfield();

    //hyperspace();
    player();

    starfield_speed = 3;

    loop
        /*if (input_pressed(_input_right))
            angle+=22500;
            move_background(1, 0);
        end
        if (input_pressed(_input_left))
            angle-=22500;
            move_background(-1, 0);
        end
        if (input_pressed(_input_down))
            move_background(0, 1);
        end
        if (input_pressed(_input_up))
            move_background(0, -1);
        end*/

        //log("GameScene x" + itoa(x) + " y" + itoa(y) + ", angle: " + itoa(angle));

        frame;
    end

    signal(type space_background, s_kill);
    signal(type starfield, s_kill_tree);
    signal(type hyperspace, s_kill);
    signal(type player, s_kill_tree);

    stop_mode7(0);

end

function init_mode7(camera_id)
begin
    start_mode7(0, 0, black_pixel, 0, 0, 175);

    m7.height = 0;
    m7.camera = camera_id;
end

// Get angle between mode7 camera and process_id:
function m7_get_angle(process_id)
begin
    return (fget_angle(m7.camera.x, m7.camera.y, process_id.x, process_id.y));
end

// Custom xgraph logic to determine the sprite to show in each height and angle:
function m7_xgraph(process_id)
private
    int ret = 100; // By default, the centered sprite.

begin
    // This table defines how the different angle sprites must be store in the FPG:
    //              30§  20§  10§  0§   -10§ -22§ -30§
    // Upper level: 303, 302, 301, 300, 311, 312, 313
    // Midle level: 103, 102, 101, 100, 111, 112, 113
    // Lower level: 203, 202, 201, 200, 211, 212, 213

    // This list defines how the different heights sprites must be store in the FPG:
    // Upper level: 100 to 12
    // Midle level: 12  to -64
    // Lower level: -64 to -200

    angle = m7_get_angle(process_id);

    // Select angle view:
    if (between(abs(angle), 10000, 20000)) ret++; end
    if (between(abs(angle), 20000, 30000)) ret += 2; end
    if (abs(angle) > 30000) ret += 3; end

    // Select right views if angle is under -10§:
    if (angle < -10000) ret += 10; end

    // Select height view:
    if (between(process_id.height, -200, -64)) ret += 100; end
    if (between(process_id.height, 12, 100)) ret += 200; end

//    log("Angle: " + itoa(angle) + ", Graph: " + itoa(ret));

    return (ret);
end

// Uses as child process of the entity to place in mode7 scene to fix the height position:
// Set graph = -1 enables internal m7_xgraph() call.
process m7_graph(file, graph, height_offset)
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

        frame;
    end
end

// Hyper-space effect background (maybe need to be optimized in graphic sizes)
// ============================================================================
process hyperspace()
private
    int i = 1;

begin
    x = 100;
    size = 130;
    height = -350;
    ctype = c_m7;
    file = fpg_hyperspace;

    loop
        graph = i;
        if (++i == 10) i = 1; end
        frame();
    end
end

// Test player ship process (used to test heights and playground dimensions)
// ============================================================================
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

    player_shoots = 0;

    sprite = m7_graph(fpg_player, -1, -120);

    player_aim_sight();

    loop
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

        if (input_down(_input_shoot)) player_shoot(); end

//        log("Player x" + itoa(x) + " y" + itoa(y) + " h" + itoa(height) +
//            " a" + itoa(m7_get_angle(id)));

        frame;
    end
end

process player_aim_sight()
begin
    ctype = c_m7;
    x = father.x + 300;

    m7_graph(fpg_ui, 1, -80).size = 600;

    loop
        y = father.y;
        height = father.height;
        frame;
    end
end

process player_shoot()
private
    int i;

begin
    if (player_shoots == _max_player_shoots) return; end

    ctype = c_m7;
    x = father.x + 1;
    y = father.y;
    height = father.height;

    sprite = m7_graph(fpg_shoots, 1, -12);

    player_shoots++;

    from i = 0 to 600;
        x++;
        frame(5);
    end

    player_shoots--;
    signal(sprite, s_kill);
end
