///////////////////////////////////////////////////////////////////////////////
// Advanced timers (pausables and resettables)
///////////////////////////////////////////////////////////////////////////////
program advanced_timer;

const
    _max_xtimers = 31;

global
    struct xtimer[_max_xtimers]
        int start_time;
        int pause_delta;
        int time;           // Time value (updated by update_xtimers()).
        int paused = 0;     // Is timer paused? (not changes this manually!)
    end

private
    string timer_values;

begin
    write(0, 0, 0, 0, "Press Space to pause/resume xtimer[0] or R to reset.");
    write(0, 0, 10, 0, timer_values);

    loop
        update_xtimers();

        timer_values = "xTimer[0]: " + itoa(xtimer[0].time) + ", xTimer[1]: " + itoa(xtimer[1].time);

        if (key(_space))
            if (!xtimer[0].paused)
                pause_xtimer(0);
            else
                resume_xtimer(0);
            end
        end

        if (key(_r))
            reset_xtimer(0, false);
        end

        frame;
    end
end

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