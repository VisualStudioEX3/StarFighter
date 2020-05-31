program PROCESS_DLL_TEST;

import "process.dll";

global
    int proc[3];
    int stat[4];

begin
    load_fpg("help\help.fpg");

    proc[0] = ball(160, 100);
    proc[1] = triangle(60, 140);
    proc[2] = sphere(220, 180);
    proc[3] = ball(10, 200);

    // Check type process:
    if (typeof(proc[0]) == type ball)
        write(0, 0, 0, 0, "Process 0 is type ball.");
    end

    if (typeof(proc[0]) != type triangle)
        write(0, 0, 10, 0, "Process 0 is not type triangle.");
    end

    // Get status of processes:
    stat[0] = get_status(proc[0]);
    write_int(0, 0, 20, 0, offset stat[0]);

    signal(proc[1], s_sleep);
    stat[1] = get_status(proc[1]);
    write_int(0, 0, 30, 0, offset stat[1]);

    signal(proc[2], s_freeze);
    stat[2] = get_status(proc[2]);
    write_int(0, 0, 40, 0, offset stat[2]);

    signal(proc[3], s_kill);
    stat[3] = get_status(proc[3]);
    write_int(0, 0, 50, 0, offset stat[3]);

    stat[4] = get_status(1234);
    write_int(0, 0, 60, 0, offset stat[4]);

    loop
        frame;
    end
end

process ball(x, y)
begin
    graph = 100;

    loop
        frame;
    end
end

process triangle(x, y)
begin
    graph = 101;

    loop
        angle += 1000;
        frame;
    end
end

process sphere(x, y)
begin
    graph = 103;

    loop
        frame;
    end
end

