using System.Runtime.InteropServices;

namespace Desktop.WebGPU;

public static unsafe class InteropHelper
{
    public static byte* ToPtr(this string str)
    {
        byte* ptr;

        fixed (char* p = str)
        {
            ptr = (byte*)p;
        }

        return ptr;
    }
}