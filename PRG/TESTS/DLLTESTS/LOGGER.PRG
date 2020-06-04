program LOGGER_DLL_TEST;

import "logger.dll";

begin
    setup_logger(true, true, 80, 10);

    log("Test log...");
    log("Anther test log...");
    log("One more time testing log...");

    loop
        if (key(_s))
            show_log_console();
        end

        if (key(_h))
            hide_log_console();
        end

        frame;
    end
end