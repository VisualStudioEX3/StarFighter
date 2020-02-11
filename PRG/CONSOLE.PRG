///////////////////////////////////////////////////////////////////////////////
// In-game basic log console
///////////////////////////////////////////////////////////////////////////////

program log_console_test;

const
    _log_buffer_size = 1023;        // The log buffer can store 1024 messages.
    _console_width = 320;           // This value varies for desired video mode.
    _console_height = 100;          // This value varies for desired video mode.
    _console_text_offset_x = 5;     // Offset X value in write() calls.
    _console_text_offset_y = 5;     // Offset Y value in write() calls.
    _console_background_color = 1;  // Black (DIV.PAL).
    _console_background_op = 7;     // Mid opacity.
    _console_border_color = 15;     // White (DIV.PAL).
    _console_text_lines = 9;        // Number of visible lines in console.

global
    int log_index = 0;                  // Index of the next log entry.
    struct log_buffer[_log_buffer_size] // In this sample, the console stores 1024 lines (of 256 characters each one).
        string value;
    end;
    int console_text[_console_text_lines];            // Instances for write() objects.
    struct console_messages[_console_text_lines]      // String instances for write() objects.
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
    log("");

    loop
        if (key(_wave))
            if (!console_visible)
                show_log_console();
            else
                hide_log_console();
            end
        end

        if (key(_del)) clear_log(); end

        if (key(_space)) log("The timer value is " + itoa(timer[0])); end

        frame;
    end
end

function clamp(int value, int min, int max)
begin
    if (value < min) return (min); end
    if (value > max) return (max); end
    return (value);
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
        frame;
    end
end

// Private: Update the view in console.
function update_console_view(int index)
private
    int i, read_line;

begin
    read_line = clamp(index - 10, 0, _log_buffer_size);

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
begin
    log_buffer[log_index].value = message;
    log_index++;
    update_console_view(log_index);
end

// Clear all log buffer.
function clear_log()
private
    int i;

begin
    from i = 0 to _log_buffer_size;
        log_buffer[i].value = "";
    end
    log_index = 0;
    update_console_view(log_index);
end