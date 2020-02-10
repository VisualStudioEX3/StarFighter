program Scrolled_Background;

global
    int fpg_background;

private
    int background_id;
    string message;

begin
    set_mode(m640x400);
    set_fps(60, 0);
    vsync = 1;

    chdir("projects\div2gjam\");

    fpg_background = load_fpg("space.fpg");
    load_pal("space.pal");


    background_id = background();

    write(0, 0, 0, 0, message);

    loop
        message = "x0 = " + itoa(scroll.x0) +
                  ", y0 = " + itoa(scroll.y0) +
                  ", x1 = " + itoa(scroll.x1) +
                  ", y1 = " + itoa(scroll.y1);

        if (key(_right))
            move_background(1, 0);
        end
        if (key(_left))
            move_background(-1, 0);
        end
        if (key(_down))
            move_background(0, 1);
        end
        if (key(_up))
            move_background(0, -1);
        end

        frame;
    end
end

function clamp(int value, int min, int max)
begin
    if (value < min) return (min); end
    if (value > max) return (max); end
    return (value);
end

///////////////////////////////////////////////////////////////////////////////
// Scrolled background
///////////////////////////////////////////////////////////////////////////////

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

///////////////////////////////////////////////////////////////////////////////