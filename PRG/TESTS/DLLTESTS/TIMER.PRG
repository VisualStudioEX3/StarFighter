program TIMER_DLL_TEST;

import "timer.dll";

global
    int time[1];

begin
    create_timer();
    create_timer();

    write(0, 0, 0, 0, "Press p to pause and r to resume second timer.");
    write(0, 0, 10, 0, "Press space to reset second timer.");

    write_int(0, 0, 30, 0, offset time[0]);
    write_int(0, 0, 40, 0, offset time[1]);

    loop
        if (key(_p)) pause_timer(1); end
        if (key(_r)) resume_timer(1); end
        if (key(_space)) reset_timer(1); end

        time[0] = get_time(0);
        time[1] = get_time(1);

        frame_timers();
        frame;
    end
end
