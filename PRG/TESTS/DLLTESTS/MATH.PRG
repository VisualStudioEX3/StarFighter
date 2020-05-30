program MATH_DLL_TEST;

import "math.dll";

global 
    int write_y;

local
    int ret;

begin
    test_max(15, 20); // 20
    test_max(25, 20); // 25

    test_min(15, 20); // 15
    test_min(25, 20); // 20

    test_clamp(5, 20, 25); // 20
    test_clamp(25, 20, 50); // 25

    test_is_clamped(15, 20, 50); // 0
    test_is_clamped(25, 20, 50); // 1

    loop
        frame;
    end
end

process print(text, var)
private
    string message;

begin
    message = text + itoa(var);
    write(0, 0, write_y, 0, message);
    write_y += 10;

    loop
        frame;
    end
end

function test_max(a, b)
begin
    ret = max(a, b);
    print("Max:", ret);
end

function test_min(a, b)
begin
    ret = min(a, b);
    print("Min:", ret);
end

function test_clamp(a, b, c)
begin
    ret = clamp(a, b, c);
    print("Clamp:", ret);
end

function test_is_clamped(a, b, c)
begin
    ret = is_clamped(a, b, c);
    print("is_clamped:", ret);
end
