using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        string path = @"C:\Users\2380132\Downloads\HDFC_Passbook.pdf";

        Benchmark("FileStream default", () =>
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read,FileShare.ReadWrite,bufferSize:default, options: FileOptions.Asynchronous))
            {
                ReadAll(fs);
            }
        });

        Benchmark("FileStream 64KB buffer", () =>
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 65536, FileOptions.Asynchronous))
            {
                ReadAll(fs);
            }
        });

        Benchmark("FileStream + BufferedStream 64KB", () =>
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read,default, FileOptions.Asynchronous))
            using (var bs = new BufferedStream(fs, 65536))
            {
                ReadAll(bs);
            }
        });

        Benchmark("ReadAllBytes", () =>
        {
            File.ReadAllBytes(path);
        });

        Benchmark("ReadAllBytesAync", () =>
        {
            var text = File.ReadAllBytesAsync(path).Result;
        });
    }

    static void Benchmark(string label, Action action)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        Console.WriteLine($"{label}: {sw.ElapsedMilliseconds} ms");
    }

    static void ReadAll(Stream s)
    {
        byte[] buffer = new byte[4096];
        s.Read(buffer, 0, buffer.Length);
    }
}
