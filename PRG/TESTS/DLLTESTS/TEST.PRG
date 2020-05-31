program TEST_DLL_TEST;

import "test.dll";

global
    string s;
    int proc;
    int a;
    int b;
    int t;

begin
    s = "Test";

    /*load_fpg("help\help.fpg");

    x = 160;
    y = 100;
    graph = 100;

    proc = test_get_process_id();
    write_int(0, 0, 0, 0, offset proc);*/

    proc = ball();

    a = type ball;
    write_int(0, 0, 0, 0, offset a);

    b = proc.reserved.process_type;
    write_int(0, 0, 10, 0, offset b);

    t = typeof(proc);
    write_int(0, 0, 20, 0, offset t);

    write_int(0, 0, 30, 0, offset proc);

    if (typeof(proc) == type ball)
        write(0, 0, 40, 0, "Process match!");
    end

    loop
        test_text_out(s);
        frame;
    end
end

process ball()
begin
    load_fpg("help\help.fpg");

    x = 160;
    y = 100;
    graph = 100;

    loop
        frame;
    end
end
