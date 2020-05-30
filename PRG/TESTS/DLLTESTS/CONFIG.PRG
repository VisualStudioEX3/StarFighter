program CONFIG_DLL_TEST;

import "config.dll";

global
    int rrb;
    int rri;
    string rrs;

    int rsb;
    int rsi;
    string rss;

begin
    // Set configuration file to read/write:
    config_set_file("test.ini");

    // Read values from root:
    rrb = config_read_bool("bool_value", true);
    rri = config_read_int("integer_value", 32);
    config_read_string("string_value", "Test default in root", rrs);

    // Read values from section:
    config_set_section("Test_Section");
    rsb = config_read_bool("bool_value", true);
    rsi = config_read_int("integer_value", 32);
    config_read_string("string_value", "Test default in section", rss);

    // Write values to root:
    config_set_section("");
    config_write_bool("bool_value", false);
    config_write_int("integer_value", 8);
    config_write_string("string_value", "Test in root");

    // Write values to section:
    config_set_section("Test_Section");
    config_write_bool("bool_value", false);
    config_write_int("integer_value", 256);
    config_write_string("string_value", "Test in section");

    // Show readed values in screen:
    write_int(0, 0, 0, 0, offset rrb);
    write_int(0, 0, 10, 0, offset rri);
    write(0, 0, 20, 0, rrs);

    write_int(0, 0, 40, 0, offset rsb);
    write_int(0, 0, 50, 0, offset rsi);
    write(0, 0, 60, 0, rss);

    loop
        frame;
    end
end
