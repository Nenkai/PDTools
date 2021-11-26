using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

using Pfim;
using Pfim.dds;

namespace PDTools.Files.Textures
{
    public class Texture
    {
        public uint ImageOffset { get; set; }
        public uint ImageSize { get; set; }

        public string Name { get; set; }
        public PGLUTextureInfo TextureInfo { get; set; }

        public int Mipmap { get; set; }
        public CELL_GCM_TEXTURE_FORMAT FormatBits { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public short Depth { get; set; } = 1;
        public int Pitch { get; set; }

        private Memory<byte> _ddsData;

        public Texture(CELL_GCM_TEXTURE_FORMAT format)
        {
            FormatBits = format;
        }

        public void SetDDSData(Memory<byte> data)
            => _ddsData = data;

        public Memory<byte> GetDDSData()
            => _ddsData;

        public void InitFromDDSImage(Pfim.IImage image)
        {
            Width = (ushort)image.Width;
            Height = (ushort)image.Height;
            Mipmap = image.MipMaps.Length;


            Pitch = Width * 4;
        }

        public void WriteDataInfo()
        {

        }

        public void SaveAsPng(string dir)
        {
            using var ms = new MemoryStream(_ddsData.ToArray()); // FIXME
            var dds = Pfim.Pfim.FromStream(ms);
            var encoder = new PngEncoder();

            string finalFileName = Path.Combine(dir, Name) + ".png";

            if (dds.Format == Pfim.ImageFormat.Rgb24)
                Save<Bgr24>(dds, finalFileName);
            else if (dds.Format == Pfim.ImageFormat.Rgba32)
                Save<Bgra32>(dds, finalFileName);
            else
            {
                Console.WriteLine($"Invalid format to save..? {dds.Format}");
                return;
            }
        }

        private void Save<T>(Pfim.IImage dds, string path) where T : unmanaged, IPixel<T>
        {
            using var i = Image.LoadPixelData<T>(dds.Data, dds.Width, dds.Height);
            /*
            if (this.Flags == 0)
            {
                i.Mutate(p =>
                {
                    p.Flip(FlipMode.Vertical);
                });
            }
            */
            i.Save(path);
        }

    }
}