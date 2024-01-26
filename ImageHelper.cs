using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DImage
{
    public class ImageHelper
    {
        public static void DImage(string filepath, Encoding encoding, string output)
        {
            var header = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
            using (var stream = File.OpenRead(filepath))
            {
                var buffer = new byte[4];
                var goNext = false;
                var index = 0L;
                do
                {
                    stream.Seek(index, SeekOrigin.Begin);
                    if (stream.ReadByte() == header[0])
                    {
                        if (stream.ReadByte() == header[1])
                        {
                            if (stream.ReadByte() == header[2])
                            {
                                if (stream.ReadByte() == header[3])
                                {
                                    index = stream.Position - 4;
                                    goNext = true;
                                    break;
                                }
                            }
                        }
                    }
                    index++;
                } while (!goNext);
                stream.Seek(index, SeekOrigin.Begin);
                var path = Path.Combine(AppContext.BaseDirectory, output);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read, true, encoding))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            entry.ExtractToFile(Path.Combine(path, entry.Name), true);
                        }
                    }
                }
            }
        }

        public static void EImage(string image, string[] files, string output, Encoding encoding)
        {
            var path = Path.Combine(AppContext.BaseDirectory, output);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var outfile = Path.Combine(AppContext.BaseDirectory, output, Path.GetFileName(image));
            using (var outputStream = new FileStream(outfile, FileMode.Create, FileAccess.Write))
            {
                using (var imageStream = File.OpenRead(image))
                {
                    imageStream.CopyTo(outputStream);
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true, encoding))
                    {
                        foreach (var file in files)
                        {
                            var filename = Path.GetFileName(file);
                            zip.CreateEntryFromFile(file, filename);
                        }
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(outputStream);
                }
                outputStream.Flush();
                outputStream.Close();
            }
        }
    }
}
