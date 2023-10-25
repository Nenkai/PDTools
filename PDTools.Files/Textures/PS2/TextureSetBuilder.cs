using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp;

using PDTools.Utils;

namespace PDTools.Files.Textures.PS2
{
    public class TextureSetBuilder
    {
        public TextureSet1 TextureSet { get; set; } = new();

        private ushort _tbp_Textures = 0;
        private ushort TBP_Transfers = 0;

        public List<TextureTask> Textures { get; set; } = new();

        // Adds an image to the texture set.
        public void AddImage(string imagePath, SCE_GS_PSM format)
        {
            Image<Rgba32> img = Image.Load<Rgba32>(imagePath);
            var pgluTexture = new PGLUtexture();

            pgluTexture.tex0.PSM = format;

            // Calculate bounds of textures, which may be beyond actual render dimensions
            uint widthPow2 = BitOperations.RoundUpToPowerOf2((uint)img.Width);
            uint heightPow2 = BitOperations.RoundUpToPowerOf2((uint)img.Height);
            pgluTexture.tex0.TW_TextureWidth = (byte)Math.Log(widthPow2, 2);
            pgluTexture.tex0.TH_TextureHeight = (byte)Math.Log(heightPow2, 2);

            // Calculate TBW
            pgluTexture.tex0.TBW_TextureBufferWidth = (byte)((img.Width + 63) / 64);
            if ((pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT4 || pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8) && (pgluTexture.tex0.TBW_TextureBufferWidth & 1) != 0)
                pgluTexture.tex0.TBW_TextureBufferWidth++;

            // Set actual render dimensions
            pgluTexture.ClampSettings.WMS = SCE_GS_CLAMP_PARAMS.SCE_GS_REGION_CLAMP;
            pgluTexture.ClampSettings.WMT = SCE_GS_CLAMP_PARAMS.SCE_GS_REGION_CLAMP;
            pgluTexture.ClampSettings.MAXU = (ulong)img.Width - 1;
            pgluTexture.ClampSettings.MAXV = (ulong)img.Height - 1;

            // PSMT8 and PSMT4 are indexed formats, flag as clut use 
            if (pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 || pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT4)
                pgluTexture.tex0.CLD_ClutBufferLoadControl = 1;

            // Color enabled
            pgluTexture.tex0.TCC_ColorComponent = 1;

            var textureTask = new TextureTask();
            textureTask.Image = img;
            textureTask.PGLUTexture = pgluTexture;

            // Quantize image and create palette for formats that requires it
            if (pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 || pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT4)
            {
                pgluTexture.tex0.CBP_ClutBlockPointer = _tbp_Textures;

                // Quantize image
                var quantizer = new WuQuantizer(new QuantizerOptions { Dither = null, MaxColors = pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 ? 256 : 16 });
                ImageFrame<Rgba32> frame = img.Frames.RootFrame;

                using IQuantizer<Rgba32> frameQuantizer = quantizer.CreatePixelSpecificQuantizer<Rgba32>(Configuration.Default);
                IndexedImageFrame<Rgba32> result = frameQuantizer.BuildPaletteAndQuantizeFrame(frame, frame.Bounds());
                textureTask.IndexedImage = result;

                ReadOnlyMemory<Rgba32> palette = result.Palette;
                Rgba32[] fullPalette = new Rgba32[pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8 ? 256 : 16];
                palette.CopyTo(fullPalette);

                // Scale down alpha for PS2 (0-128)
                for (int i = 0; i < fullPalette.Length; i++)
                    fullPalette[i].A = (byte)Tex1Utils.Normalize(fullPalette[i].A, 0x00, 0xFF, 0x00, 0x80);

                byte[] imageData = new byte[(int)(double)(img.Width * img.Height * Tex1Utils.GetBitsPerPixel(format) / 8)];

                if (pgluTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8)
                {
                    var tiledPaletteData = MakeTiledPaletteFromLinearPalette(fullPalette);
                    textureTask.PaletteColors = tiledPaletteData.TiledPalette;
                }
                else
                {
                    textureTask.PaletteColors = fullPalette;
                }
            }

            TextureSet.pgluTextures.Add(pgluTexture);
            CreateImageData(textureTask, format);
            Textures.Add(textureTask);
        }

        private void CreateImageData(TextureTask texture, SCE_GS_PSM outputFormat)
        {
            int bpp = Tex1Utils.GetBitsPerPixel(outputFormat);

            byte[] imageData = new byte[Tex1Utils.GetDataSize(texture.Image.Width, texture.Image.Height, outputFormat)];
            BitStream bs = new BitStream(BitStreamMode.Write, imageData, BitStreamSignificantBitOrder.MSB);

            if (outputFormat == SCE_GS_PSM.SCE_GS_PSMT8 || outputFormat == SCE_GS_PSM.SCE_GS_PSMT4)
            {
                var img = texture.IndexedImage;
                for (var y = 0; y < img.Height; y++)
                {
                    var row = img.DangerousGetRowSpan(y);
                    for (var x = 0; x < img.Width; x++)
                    {
                        bs.WriteBits(row[x], (ulong)bpp);
                    }
                }
            }
            else
            {
                Image<Rgba32> image = texture.Image;
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        bs.WriteByte(image[x, y].R);
                        bs.WriteByte(image[x, y].G);
                        bs.WriteByte(image[x, y].B);
                        bs.WriteByte((byte)Tex1Utils.Normalize(image[x, y].A, 0x00, 0xFF, 0x00, 0x80));
                    }
                }
            }

            texture.PackedImageData = imageData;
        }

        public void Build()
        {
            GSMemory mem = new GSMemory();

            int totalTextureSize = 0;

            // Some sort of atlas is built for swizzled textures, textures sorted from size descending kinda
            // var texturesBySize = Textures.OrderByDescending(e => e.Image.Width * e.Image.Height);

            foreach (TextureTask texture in Textures)
            {
                texture.PGLUTexture.tex0.TBP0_TextureBaseAddress = _tbp_Textures;

                /*
                switch (texture.PGLUTexture.tex0.PSM)
                {
                    case SCE_GS_PSM.SCE_GS_PSMT8:
                        mem.WriteTexPSMT8(_tbp_Textures, texture.PGLUTexture.tex0.TBW_TextureBufferWidth, 
                            0, 0, 
                            texture.Image.Width, texture.Image.Height, 
                            texture.PackedImageData);
                        break;

                    case SCE_GS_PSM.SCE_GS_PSMT4:
                        mem.WriteTexPSMT4(_tbp_Textures, texture.PGLUTexture.tex0.TBW_TextureBufferWidth, 
                            0, 0, texture.Image.Width, texture.Image.Height, 
                            texture.PackedImageData);
                        break;
                }
                */

                totalTextureSize += Tex1Utils.GetDataSize(texture.Image.Width, texture.Image.Height, texture.PGLUTexture.tex0.PSM);

                uint nextTbp = (uint)Tex1Utils.FindBlockIndexAtPosition(texture.PGLUTexture.tex0.PSM, texture.Image.Width - 1, texture.Image.Height - 1) + 1;
                _tbp_Textures += (ushort)nextTbp;

                // TODO: Find an appropriate place to put the palette
                // i.e place it outside the rendered range (using 2^width & 2^height's zone)
                if (texture.PaletteColors is not null)
                {
                    texture.PGLUTexture.tex0.CBP_ClutBlockPointer = _tbp_Textures;

                    switch (texture.PGLUTexture.tex0.PSM)
                    {
                        case SCE_GS_PSM.SCE_GS_PSMT8:
                            /*
                            mem.WriteTexPSMCT32(_tbp_Textures, 1, 
                                0, 0, 
                                16, 16, 
                                MemoryMarshal.Cast<Rgba32, uint>(texture.PaletteColors));
                            */
                            totalTextureSize += Tex1Utils.GetDataSize(16, 16, SCE_GS_PSM.SCE_GS_PSMCT32);

                            _tbp_Textures += 4;
                            break;

                        case SCE_GS_PSM.SCE_GS_PSMT4:
                            /*
                            mem.WriteTexPSMCT32(_tbp_Textures, 1, 
                                0, 0, 
                                8, 2, 
                                MemoryMarshal.Cast<Rgba32, uint>(texture.PaletteColors));
                            */
                            totalTextureSize += Tex1Utils.GetDataSize(8, 2, SCE_GS_PSM.SCE_GS_PSMCT32);
                            _tbp_Textures += 1;
                            break;
                    }
                }
            }

            foreach (var texture in Textures)
            {
                AddTransfer(texture.PGLUTexture.tex0.PSM, texture.PGLUTexture.tex0.TBW_TextureBufferWidth, (ushort)texture.Image.Width, (ushort)texture.Image.Height, texture.PackedImageData);
                
                if (texture.PaletteColors != null)
                {
                    if (texture.PGLUTexture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8)
                    {
                        AddTransfer(SCE_GS_PSM.SCE_GS_PSMCT32, 4, // bw = 4, important
                            16, 16, MemoryMarshal.Cast<Rgba32, byte>(texture.PaletteColors).ToArray());
                    }
                    else
                    {
                        AddTransfer(SCE_GS_PSM.SCE_GS_PSMCT32, 1,
                            8, 2, MemoryMarshal.Cast<Rgba32, byte>(texture.PaletteColors).ToArray());
                    }

                }
            }

            // Computed swizzled transfers, PSMCT32 for speed - this is probably completely wrong
            /*
            ushort transferWidth = 64;
            int remTotalSize = totalTextureSize;
            /*
            while (remTotalSize > 0)
            {
                // Calculate width/height for swizzled transfer
                ushort transferHeight = (ushort)(remTotalSize /
                    transferWidth / // Width
                    4); // Size of one PSMCT32 
                
                int transferSize = transferWidth * transferHeight * sizeof(uint);
                byte[] transferData = new byte[transferSize];
                mem.ReadTexPSMCT32(TBP_Transfers, 1, 0, 0, transferWidth, transferHeight, MemoryMarshal.Cast<byte, uint>(transferData));

                AddTransfer(SCE_GS_PSM.SCE_GS_PSMCT32, transferWidth, transferHeight, transferData);
                remTotalSize -= transferSize;

                transferWidth /= 2;
            }*/

            foreach (var transfer in TextureSet.GSTransfers)
                TextureSet.TotalBlockSize += (ushort)Tex1Utils.FindBlockIndexAtPosition(transfer.Format, transfer.Width - 1, transfer.Height - 1);
        }

        public (Rgba32[] TiledPalette, int[] LinearToTiledPaletteIndices) MakeTiledPaletteFromLinearPalette(ReadOnlyMemory<Rgba32> palette)
        {
            int[] indices = new int[palette.Length];

            const int tileSizeW = 8;
            const int tileSizeH = 2;

            Rgba32[] outpal = new Rgba32[256];
            int ntx = 16 / tileSizeW,
                nty = 16 / tileSizeH;
            int i = 0;

            for (int ty = 0; ty < nty; ty++)
                for (int tx = 0; tx < ntx; tx++)
                    for (int y = 0; y < tileSizeH; y++)
                        for (int x = 0; x < tileSizeW; x++)
                        {
                            int idx = (ty * tileSizeH + y) * 16 + (tx * tileSizeW + x);
                            outpal[idx] = palette.Span[i];
                            indices[i] = idx;
                            i++;
                        }

            return (outpal, indices);
        }

        private void AddTransfer(SCE_GS_PSM format, byte bw, ushort width, ushort height, byte[] data)
        {
            var inf = new GSTransfer()
            {
                BP = TBP_Transfers,
                BW = bw,
                Format = format,
                Width = width,
                Height = height,
                Data = data
            };

            TextureSet.GSTransfers.Add(inf);

            TBP_Transfers += (ushort)(Tex1Utils.FindBlockIndexAtPosition(format, width - 1, height - 1) + 1);
        }
    }

    public class TextureTask
    {
        public PGLUtexture PGLUTexture;
        public Image<Rgba32> Image;
        public IndexedImageFrame<Rgba32> IndexedImage;
        public byte[] PackedImageData;
        public Rgba32[] PaletteColors;
    }
}
