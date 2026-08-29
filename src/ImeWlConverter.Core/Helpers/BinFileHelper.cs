namespace ImeWlConverter.Core.Helpers;

internal static class BinFileHelper
{
    public static short ReadInt16(Stream fs)
    {
        var temp = new byte[2];
        fs.ReadExactly(temp, 0, 2);
        return BitConverter.ToInt16(temp, 0);
    }

    public static int ReadInt32(Stream fs)
    {
        var temp = new byte[4];
        fs.ReadExactly(temp, 0, 4);
        return BitConverter.ToInt32(temp, 0);
    }

    public static ushort ReadUInt16(Stream fs)
    {
        var temp = new byte[2];
        fs.ReadExactly(temp, 0, 2);
        return BitConverter.ToUInt16(temp, 0);
    }

    public static uint ReadUInt32(Stream fs)
    {
        var temp = new byte[4];
        fs.ReadExactly(temp, 0, 4);
        return BitConverter.ToUInt32(temp, 0);
    }

    public static long ReadInt64(Stream fs)
    {
        var temp = new byte[8];
        fs.ReadExactly(temp, 0, 8);
        return BitConverter.ToInt64(temp, 0);
    }

    public static byte[] ReadArray(Stream fs, int count)
    {
        var bytes = new byte[count];
        fs.ReadExactly(bytes, 0, count);
        return bytes;
    }

    public static byte[] ReadArray(byte[] fs, int position, int count)
    {
        var bytes = new byte[count];
        for (var i = 0; i < count; i++) bytes[i] = fs[position + i];

        return bytes;
    }
}
