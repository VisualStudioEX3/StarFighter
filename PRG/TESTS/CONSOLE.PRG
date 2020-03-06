///////////////////////////////////////////////////////////////////////////////
// In-game basic log console
///////////////////////////////////////////////////////////////////////////////
program log_console_test;
const
    _log_buffer_size = 31;              // The log buffer can store 32 messages (for ease test buffer overflow logic).
    _log_to_file = 1;                   // The log can write to file in disk.
    _log_filename = "console.log";      // Filename to write the log (if _log_to_file == 1.
    _console_width = 320;               // This value varies for desired video mode.
    _console_text_offset_x = 5;         // Offset X value in write() calls.
    _console_text_offset_y = 5;         // Offset Y value in write() calls.
    _console_background_color = 1;      // Black (DIV.PAL).
    _console_background_op = 7;         // Mid opacity.
    _console_border_color = 15;         // White (DIV.PAL).
    _console_text_lines = 15;           // Number of visible lines in console.

global
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

begin
    put_screen(load_fpg("help\help.fpg"), 1); // Test background.

    console_id = log_console();

    log("DIV2 log console in-game");
    log("------------------------");
    log("");
    log("Press Space to log timer value...");
    log("Press ~ to hide or show the console...");
    log("Press Supr to clear the console...");
    log("Use up, down, pgUp pgDown to navigate previous msg...");
    log("");

    loop
        if (key(_space)) log("The timer 0 value is " + itoa(timer[0])); end
        frame;
    end
end

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

// Create an instance of log console.
process log_console()
private
    int i;

begin
    console_ui.height = (_console_text_lines + 1) * 10;

    console_ui.box = draw(3, _console_background_color, _console_background_op, 0,
                          0, 0,
                          _console_width, console_ui.height + _console_text_offset_y);

    console_ui.line = draw(1, _console_border_color, 15, 0,
                           0, console_ui.height + _console_text_offset_y,
                           _console_width, console_ui.height + _console_text_offset_y);

    from i = 0 to _console_text_lines;
        console_ui.text[i].obj = write(0,
                                       _console_text_offset_x, (i * 10) + _console_text_offset_y,
                                       0, console_ui.text[i].value);
    end

    loop
        // The input can be improved using the Input Manager (key down events!)
        if (key(_wave))
            if (!console_visible)
                show_log_console();
            else
                hide_log_console();
            end
        end if (console_visible)
            if (key(_del))  clear_log();                end
            if (key(_up))   console_nav_line_up();      end
            if (key(_down)) console_nav_line_down();    end
            if (key(_pgup)) console_nav_page_up();      end
            if (key(_pgdn)) console_nav_page_down();    end
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

    move_draw(console_ui.box, _console_background_color, _console_background_op,
              0, 0, _console_width, console_ui.height + _console_text_offset_y);

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

    move_draw(console_ui.box, _console_background_color, 0, 0, 0, 0, 0);
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