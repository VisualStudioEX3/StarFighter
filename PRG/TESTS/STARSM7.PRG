program start_field_mode7;

const
    _starfield_max_stars = 100;
    _starfield_width = 30;
    _starfield_height = 100;

global
    int starfield_speed;

private
    string fps_counter;

begin
    set_mode(m640x400);
    set_fps(60, 0);
    vsync = 1;
    restore_type = no_restore;

    // Ideal setup for 640x400 to hide the entire m7 floor plane:
    start_mode7(0, 0, new_map(1, 1, 0, 0, 0), 0, 0, 200);
    m7.distance = 32;
    m7.height = 0;
    m7.color = 0;
    m7.camera = id;

    // Test background to ensured the scroll is visible:
    start_scroll(0, 0, new_map(640, 400, 0, 0, 50), 0, 0, 15);

    star_field();
    starfield_speed = 3;

    write(0, 0, 0, 0, fps_counter);

    loop
        fps_counter = itoa(fps) + "fps";

        if (key(_right)) angle-=8000; end
        IF (key(_left)) angle+=8000; end

        if (key(_up)) starfield_speed = min(++starfield_speed, 10); end
        if (key(_down)) starfield_speed = max(--starfield_speed, 1); end

        frame;
    end
end

function max(a, b)
begin
    if (a >= b) return (a); else return (b); end
end

function min(a, b)
begin
    if (a <= b) return (a); else return (b); end
end

// Creates a particle system to simulate speed and movement in the void.
process star_field()
private
    int i;
    int star_sprite;

begin
    star_sprite = new_map(1, 1, 0, 0, 15);

    from i = 0 to _starfield_max_stars;
        star(star_sprite);
    end

    loop
        if (key(_esc)) // TODO: Replaces by condition.
            signal(id, s_kill_tree);
        end
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