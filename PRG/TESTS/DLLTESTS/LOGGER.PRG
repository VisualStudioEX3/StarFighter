program LOGGER_DLL_TEST;

import "logger.dll";

global
    int fpg;
    string message;

begin
    fpg = load_fpg("help\help.fpg");
    put_screen(fpg, 1);

    setup_logger(true, true, 80, 10);

    //log("Test log...");
    //log("Another test log...");
    //log("One more time testing log...");

    //for (x = 0; x < 15; x++)
    //    message = "Test iteration: " + itoa(x);
    //    log(message);
    //end

    loop
        if (key(_s))
            show_log_console();
        end

        if (key(_h))
            hide_log_console();
        end

        if (key(_space))
            log(itoa(timer[0]));
        end

        frame;
    end
end