program compare_types;

global
    int ida, idb;

begin
    ida = a();
    idb = b();

    if (typeof(ida) == type a)
        debug;
    end

end

function typeof(int process_id)
begin
    return (process_id.reserved.process_type);
end

process a()
begin
    loop frame; end
end

process b()
begin
    loop frame; end
end