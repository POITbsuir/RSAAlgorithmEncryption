using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ThirdTaskApplication.Models
{
    public class FileService
    {
        public void SaveToFile(long[] data, string path)
        {
            File.WriteAllText(path, string.Join(" ", data), Encoding.UTF8);
        }

        public long[] OpenFile(string path)
        {
            if (!File.Exists(path))
                return new long[0];

            string content = File.ReadAllText(path, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(content))
                return new long[0];

            return content
                .Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToArray();
        }

        public bool IsFileExists(string path)
        {
            return File.Exists(path);
        }

        public string ReadText(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public void SaveText(string path, string text)
        {
            File.WriteAllText(path, text, Encoding.UTF8);
        }

        public byte[] ReadBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public void SaveBytes(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
        }

        public void SaveUShortBlocks(string path, ushort[] blocks)
        {
            byte[] bytes = new byte[blocks.Length * 2];

            for (int i = 0; i < blocks.Length; i++)
            {
                byte[] blockBytes = BitConverter.GetBytes(blocks[i]);
                bytes[i * 2] = blockBytes[0];
                bytes[i * 2 + 1] = blockBytes[1];
            }

            File.WriteAllBytes(path, bytes);
        }

        public ushort[] ReadUShortBlocks(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);

            if (bytes.Length % 2 != 0)
                throw new Exception("Ошибка! Зашифрованный файл должен состоять из 16-битных блоков.");

            ushort[] blocks = new ushort[bytes.Length / 2];

            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = BitConverter.ToUInt16(bytes, i * 2);
            }

            return blocks;
        }
    }
}