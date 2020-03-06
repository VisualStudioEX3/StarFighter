PROGRAM ejemplo_start_scroll;

global
    fichero1;

BEGIN
    fichero1=load_fpg("help\help.fpg");

    start_scroll(0, fichero1, 103, 102, 0, 15);

    scroll.camera=id;

    ctype=c_scroll;
    graph=100;
    write(0, 160, 0, 1, "Utilice los cursores para moverse");

    a(100, 100, 0);
    a(0, 100, 1);

    LOOP
        IF (key(_right)) x+=2; END
        IF (key(_left)) x-=2; END
        IF (key(_down)) y+=2; END
        IF (key(_up)) y-=2; END
        FRAME;
    END
END

process a(x, y, s)
begin
    file = fichero1;
    graph = 101;
    z = 999; // Seems the scroll z is 512.

    loop
        x += s;
        // Collisions works behind the scroll system.
        if (collision(type a))
            debug;
        end

        frame;
    end
end