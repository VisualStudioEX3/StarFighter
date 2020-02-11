///////////////////////////////////////////////////////////////////////////////
// In-game basic log console
///////////////////////////////////////////////////////////////////////////////

program log_console_test;

const
    _log_buffer_size = 19;          // The log buffer can store 20 messages (for ease test buffer overflow logic).
    _console_width = 320;           // This value varies for desired video mode.
    _console_height = 100;          // This value varies for desired video mode.
    _console_text_offset_x = 5;     // Offset X value in write() calls.
    _console_text_offset_y = 5;     // Offset Y value in write() calls.
    _console_background_color = 1;  // Black (DIV.PAL).
    _console_background_op = 7;     // Mid opacity.
    _console_border_color = 15;     // White (DIV.PAL).
    _console_text_lines = 9;        // Number of visible lines in console.

global
    int log_index = 0;              // Index of the next log entry.
    struct log_buffer[_log_buffer_size]
        string value;
    end;
    int console_text[_console_text_lines];          // Instances for write() objects.
    struct console_messages[_console_text_lines]    // String instances for write() objects.
        string value;
    end
    int console_index;              // Index to start to read log in console.
    int console_box;                // Box instance for background.
    int console_line;               // Line instance for bottom border.
    int console_visible = 1;        // Is the console visible?

    int console_id;

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
        if (key(_space)) log("The timer value is " + itoa(timer[0])); end

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
    console_box = draw(3, _console_background_color, _console_background_op, 0,
                       0, 0,
                       _console_width, _console_height + _console_text_offset_y);

    console_line = draw(1, _console_border_color, 15, 0,
                        0, _console_height + _console_text_offset_y,
                        _console_width, _console_height + _console_text_offset_y);

    from i = 0 to _console_text_lines;
        console_text[i] = write(0, _console_text_offset_x, (i * 10) + _console_text_offset_y, 0, console_messages[i].value);
    end

    loop
        // The input can be improved using the Input Manager (key down events!)
        if (key(_wave))
            if (!console_visible)
                show_log_console();
            else
                hide_log_console();
            end
        end if (key(_del))
            clear_log();
        end if (key(_up))
            console_nav_line_up();
        end if (key(_down))
            console_nav_line_down();
        end if (key(_pgup))
            console_nav_page_up();
        end if (key(_pgdn))
            console_nav_page_down();
        end

        frame;
    end
end

function console_nav_line_up()
begin
    console_index = clamp(--console_index, _console_text_lines, _log_buffer_size);
    update_console_view();
end

function console_nav_line_down()
begin
    console_index = clamp(++console_index, 0, log_index);
    update_console_view();
end

function console_nav_page_up()
begin
    console_index -= _console_text_lines;
    console_index = clamp(console_index, _console_text_lines, _log_buffer_size);
    update_console_view();
end

function console_nav_page_down()
begin
    console_index += _console_text_lines;
    console_index = clamp(console_index, 0, log_index);
    update_console_view();
end


// Private: Update the view in console.
function update_console_view()
private
    int i, read_line;

begin
    read_line = clamp(console_index - 10, 0, _log_buffer_size);

    from i = 0 to _console_text_lines;
        console_messages[i].value = log_buffer[read_line].value;
        read_line++;
    end
end

// Shows debug console.
function show_log_console()
private
    int i;

begin
    from i = 0 to _console_text_lines;
        move_text(console_text[i], _console_text_offset_x, (i * 10) + _console_text_offset_y);
    end

    move_draw(console_box, _console_background_color, _console_background_op,
              0, 0, _console_width, _console_height + _console_text_offset_y);

    move_draw(console_line, _console_border_color, 15,
              0, _console_height + _console_text_offset_y,
              _console_width, _console_height + _console_text_offset_y);

    console_visible = 1;
end

// Hides debug console.
function hide_log_console()
private
    int i;

begin
    from i = 0 to _console_text_lines;
        move_text(console_text[i], 0, -9999);
    end

    move_draw(console_box, _console_background_color, 0, 0, 0, 0, 0);
    move_draw(console_line, _console_border_color, 0, 0, 0, 0, 0);

    console_visible = 0;
end

// Add message to log buffer.
function log(string message)
private
    int i;
begin
    log_buffer[log_index].value = message;

    if (log_index == _log_buffer_size)
        from i = 0 to _log_buffer_size - 1;
            log_buffer[i].value = log_buffer[i + 1].value;
        end
    else
        log_index++;
        console_index = log_index;
    end

    update_console_view();
end

// Clear all log buffer.
function clear_log()
private
    int i;

begin
    from i = 0 to _log_buffer_size;
        log_buffer[i].value = "";
    end
    log_index = console_index = 0;
    update_console_view();
end