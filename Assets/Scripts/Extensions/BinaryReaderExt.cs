using System.IO;

public static class BinaryReaderExt
{
    public static byte[] ReadAllBytes(this BinaryReader reader)
    {
        const int bufferSize = 4096;
        using MemoryStream stream = new();
        byte[] buffer = new byte[bufferSize];
        int count;
        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
            stream.Write(buffer, 0, count);
        return stream.ToArray();
    }
}