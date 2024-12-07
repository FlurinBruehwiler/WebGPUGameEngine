using System.Text;

namespace Desktop.WebGPU;

public static unsafe class InteropHelper
{
    public static byte* ToPtr(this byte[] str)
    {
        byte* ptr;

        fixed (byte* p = str)
        {
            ptr = (byte*)p;
        }

        return ptr;
    }

    public static string UnmarshalUtf8(byte* data)
    {
        if (data == null)
            return string.Empty;

        int i = 0;
        while (data[i] != 0)
        {
            i++;
        }

        return Encoding.UTF8.GetString(data, i);
    }

    public static byte[] MarshalUtf8(this string? text)
    {
        if (text == null)
        {
            return [];
        }

        if (utf8Cache.TryGetValue(text, out var buf))
        {
            return buf;
        }

        var length = Encoding.UTF8.GetByteCount(text) + 1;//need Length+1 so that we always can guarantee a null terminated ending char
        var toReturn = new byte[length];
        Encoding.UTF8.GetBytes(text.AsSpan(), toReturn.AsSpan());

        utf8Cache.Add(text, toReturn);
        return toReturn;
    }

    public static Dictionary<string, byte[]> utf8Cache = new();
}