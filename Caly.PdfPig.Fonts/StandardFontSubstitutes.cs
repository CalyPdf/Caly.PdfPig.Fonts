// Copyright BobLd
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Fonts.SystemFonts;

namespace Caly.PdfPig.Fonts
{
    /// <summary>
    /// Substitute programs for the 14 PDF Standard fonts.
    /// <para>
    /// Call <see cref="Register"/> once at startup so the glyphs of non-embedded Standard 14 fonts get the same
    /// outlines on every machine, instead of depending on the fonts installed.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Like pdf.js: Helvetica uses Liberation Sans; Times, Courier, Symbol and ZapfDingbats use the Foxit fonts PDFium
    /// ships. The Foxit Latin fonts lack the Central European accented letters, so Liberation Serif and Liberation Mono
    /// supply the glyphs Times and Courier are missing.
    /// </remarks>
    public static class StandardFontSubstitutes
    {
        // A zip of the files in Resources, built by the ZipFonts target in the project file.
        private const string FontsResource = "Caly.PdfPig.Fonts.fonts.zip";

        private static readonly object RegisterLock = new object();
        private static bool _registered;

        // Lookup order: a glyph comes from the first file that has it.
        private static readonly IReadOnlyDictionary<Standard14Font, string[]> Files = new Dictionary<Standard14Font, string[]>
        {
            [Standard14Font.Helvetica] = ["LiberationSans-Regular.ttf"],
            [Standard14Font.HelveticaBold] = ["LiberationSans-Bold.ttf"],
            [Standard14Font.HelveticaOblique] = ["LiberationSans-Italic.ttf"],
            [Standard14Font.HelveticaBoldOblique] = ["LiberationSans-BoldItalic.ttf"],

            [Standard14Font.TimesRoman] = ["FoxitSerif.pfb", "LiberationSerif-Regular.ttf"],
            [Standard14Font.TimesBold] = ["FoxitSerifBold.pfb", "LiberationSerif-Bold.ttf"],
            [Standard14Font.TimesItalic] = ["FoxitSerifItalic.pfb", "LiberationSerif-Italic.ttf"],
            [Standard14Font.TimesBoldItalic] = ["FoxitSerifBoldItalic.pfb", "LiberationSerif-BoldItalic.ttf"],

            [Standard14Font.Courier] = ["FoxitFixed.pfb", "LiberationMono-Regular.ttf"],
            [Standard14Font.CourierBold] = ["FoxitFixedBold.pfb", "LiberationMono-Bold.ttf"],
            [Standard14Font.CourierOblique] = ["FoxitFixedItalic.pfb", "LiberationMono-Italic.ttf"],
            [Standard14Font.CourierBoldOblique] = ["FoxitFixedBoldItalic.pfb", "LiberationMono-BoldItalic.ttf"],

            [Standard14Font.Symbol] = ["FoxitSymbol.pfb"],
            [Standard14Font.ZapfDingbats] = ["FoxitDingbats.pfb"]
        };

        /// <summary>
        /// The fonts this package has a substitute for: all 14.
        /// </summary>
        public static Standard14Font[] Fonts { get; } = Files.Keys.ToArray();

        /// <summary>
        /// Register the substitutes with <see cref="SystemFontFinder"/>. Process-wide; affects fonts read after the call.
        /// Nothing is read or parsed until a document needs a glyph from a given font. Safe to call more than once and
        /// from several threads: only the first call registers.
        /// </summary>
        public static void Register()
        {
            lock (RegisterLock)
            {
                if (_registered)
                {
                    return;
                }

                foreach (var pair in Files)
                {
                    Func<byte[]>[] loaders = pair.Value
                        .Select(file => (Func<byte[]>)(() => ReadResource(file)))
                        .ToArray();

                    SystemFontFinder.AddSubstituteFont(pair.Key, loaders);
                }

                _registered = true;
            }
        }

        /// <summary>
        /// Get the bytes of the substitute programs for <paramref name="font"/>, in lookup order (CFF or TrueType),
        /// or an empty array if this package has none.
        /// </summary>
        public static byte[][] GetFontPrograms(Standard14Font font)
        {
            return Files.TryGetValue(font, out var files)
                ? files.Select(ReadResource).ToArray()
                : [];
        }

        private static byte[] ReadResource(string file)
        {
            // Each call opens its own archive, so the lazy loaders can run on several threads at once.
            using Stream? stream = typeof(StandardFontSubstitutes).Assembly.GetManifestResourceStream(FontsResource);
            if (stream is null)
            {
                throw new InvalidOperationException($"Could not find the embedded font resource '{FontsResource}'.");
            }

            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            ZipArchiveEntry? entry = archive.GetEntry(file);
            if (entry is null)
            {
                throw new InvalidOperationException($"Could not find '{file}' in the embedded font resource '{FontsResource}'.");
            }

            using Stream entryStream = entry.Open();
            using var memory = new MemoryStream((int)entry.Length);
            entryStream.CopyTo(memory);
            return memory.ToArray();
        }
    }
}
