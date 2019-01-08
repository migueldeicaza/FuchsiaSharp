import re
import sys

file = open("syscalls.abigen", "r") 
lines = file.readlines ()

maps={
    "zx_status_t": ("ZxStatus","ZxStatus"),
    "uint32_t": ("uint", "uint"),
    "zx_time_t": ("ZxTime","ZxTime"),
    "zx_clock_t": ("ZxClock","ZxClock"),
    "zx_ticks_t": ("ZxTicks","ZxTicks"),
    "zx_handle_t": ("uint", "HANDLE"),
    "uint64_t":("ulong", "ulong"),
    "zx_signals_t":("ZxObjectSignal","ZxObjectSignal"),
    "zx_duration_t": ("ZxDuration", "ZxDuration"),
    "int64_t":("long","long"),
    "size_t":("IntPtr", "IntPtr"),
    "zx_vaddr_t": ("IntPtr", "IntPtr"),
    "uint8_t": ("byte", "byte"),
    "int32_t": ("int", "int"),
    "bool":("byte", "bool"),
    "zx_vm_option_t": ("ZxVmOption", "ZxVmOption"),
    "zx_rights_t" : ("ZxRights", "ZxRights"),
    "uint16_t": ("ushort", "ushort"),
    "zx_paddr_t":("IntPtr", "IntPtr"),
    "uintptr_t": ("UIntPtr", "UIntPtr"),
}

def clean(arg):
    #print ("Hitting: " + arg)
    arg = arg.strip()
    pname,typeAndAttr = arg.split(':')
    typeAndAttr = typeAndAttr.strip ()
    typename,*typeattrs = typeAndAttr.split (' ')
    #print ("   T: " + typename)
    typename = typename.strip()
    if not typename in maps:
        return (False, False,False)
    if pname == "out":
        pname = "result"
    return (pname, maps[typename])

syscalls={}
i = 0
while i < len(lines):
    if lines [i].startswith ("#"):
        i = i + 1
        continue
    if lines[i].startswith ("syscall"):
        if lines[i].find ("noreturn") != -1:
            i = i + 1
            continue

        if lines[i].find ("test") != -1:
            i = i + 1
            continue

        v, syscall, *rest = re.split ("\s", lines[i])
        i=i+1
        args=lines[i].strip ()
        args=args[1:-1]
        i=i+1
        ret=lines[i].strip()[9:-2]

        vargs=args.split(',')
        presult = ""
        pargs = ""
        cargs = ""
        cresult = ""
        call = ""
        for varg in vargs:
            if varg != "":
                cleanresult = clean(varg)
                if len(cleanresult) == 3:
                    print ("// Skipping " + syscall + " due to unhandled type: " + varg+ "\n\n")
                    continue
                p = (cleanresult[1][0]) + " " + (cleanresult[0])
                c = (cleanresult[1][1]) + " " + (cleanresult[0])
                if len(pargs) == 0:
                    pargs = p
                    cargs = c
                else:
                    pargs = pargs + ", " + p
                    cargs = cargs + ", " + c
                if len (call) == 0:
                    call = cleanresult [0]
                else:
                    call = call + ", " + cleanresult [0]

        fret,*aret=ret.split(',')
        if not fret in maps:
            i = i +1
            print ("// Skipping " + syscall + " due to unhandled return " + fret+ "\n\n")
            continue
        #if len(aret) != 0:
            #print ("AARG: " + ",".join (aret));
        for vr in aret:
            #print ("HANDLING AN ADDITIONAL RETURN IN " + syscall + "-> " + vr)
            cleanresult = clean(vr)
            if len(cleanresult) == 3:
                print ("// Skipping " + syscall + " due to additional return: " + vr + "\n\n")
                continue

            p = "out " + (cleanresult[1][0]) + " " + (cleanresult[0])
            c = "out " + (cleanresult[1][1]) + " " + (cleanresult[0])
            if len(pargs) == 0:
                pargs = p
                cargs = c
            else:
                pargs = pargs + ", " + p
                cargs = cargs + ", " + c
            if len (call) == 0:
                call = "out " + cleanresult [0]
            else:
                call = call + ", out " + cleanresult [0]


        presult = maps[fret][0]
        cresult = maps[fret][1]

        print ("[DllImport (Library)]")
        print ("extern static " + presult + " " + syscall + " (" + pargs + ");");
        print ("")  
        print ("public void " + cresult + " " + syscall + " (" + cargs + ")");
        print ("{");
        print ("\t"+ syscall + "(" + call + ")");
        print ("}");
        print ("")

    i = i + 1

    
