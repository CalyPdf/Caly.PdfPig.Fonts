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

using UglyToad.PdfPig;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Fonts.SystemFonts;

namespace Caly.PdfPig.Fonts.Tests
{
    public class StandardFontSubstitutesTests
    {
        public static TheoryData<Standard14Font> Covered => new(StandardFontSubstitutes.Fonts);

        [Theory]
        [MemberData(nameof(Covered))]
        public void SubstituteHasEveryStandard14Glyph(Standard14Font font)
        {
            StandardFontSubstitutes.Register();

            Assert.True(SystemFontFinder.TryGetSubstituteFont(Standard14.GetAdobeFontMetrics(font).FontName, out var substitute));

            var missing = Standard14.GetAdobeFontMetrics(font).CharacterMetrics.Keys
                .Where(name => !substitute!.TryGetNormalisedPath(name, out _))
                .ToArray();

            Assert.True(missing.Length == 0, $"{font} substitute has no {string.Join(" ", missing.Select(n => "/" + n))}.");
        }

        [Fact]
        public void EveryStandard14FontIsCovered()
        {
            foreach (Standard14Font font in Enum.GetValues(typeof(Standard14Font)))
            {
                Assert.Contains(font, StandardFontSubstitutes.Fonts);
                Assert.NotEmpty(StandardFontSubstitutes.GetFontPrograms(font));
            }
        }

        [Fact]
        public void RegisterIsIdempotent()
        {
            StandardFontSubstitutes.Register();
            Assert.True(SystemFontFinder.TryGetSubstituteFont("ZapfDingbats", out var first));

            StandardFontSubstitutes.Register();
            Assert.True(SystemFontFinder.TryGetSubstituteFont("ZapfDingbats", out var second));

            Assert.Same(first, second);
        }

        /// <summary>
        /// https://github.com/BobLd/PdfPig.Rendering.Skia/issues/163: a non-embedded ZapfDingbats check mark (/a20,
        /// code '4') must get its outline from the substitute, with text and metrics still from the AFM.
        /// </summary>
        [Fact]
        public void NonEmbeddedZapfDingbatsCheckMarkHasOutline()
        {
            StandardFontSubstitutes.Register();

            using var document = PdfDocument.Open(BuildPdf("<< /Type /Font /Subtype /Type1 /BaseFont /ZapfDingbats >>", "(4)"));
            var letter = document.GetPage(1).Letters.Single();

            Assert.Equal("✔", letter.Value);
            Assert.True(letter.GetFont()!.TryGetNormalisedPath(0x34, out var path));
            Assert.NotEmpty(path);

            // /a20 is 846 units wide in ZapfDingbats.afm.
            Assert.Equal(0.846, letter.GetFont()!.GetBoundingBox(0x34).Width, 6);
        }

        private static byte[] BuildPdf(string font, string text)
        {
            string content = $"BT /F1 12 Tf 10 10 Td {text} Tj ET";
            string[] objects =
            [
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 200 200] /Contents 5 0 R /Resources << /Font << /F1 4 0 R >> >> >>",
                font,
                $"<< /Length {content.Length} >>\nstream\n{content}\nendstream"
            ];

            var builder = new System.Text.StringBuilder("%PDF-1.7\n");
            var offsets = new List<int>();
            for (int i = 0; i < objects.Length; i++)
            {
                offsets.Add(builder.Length);
                builder.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
            }

            int xref = builder.Length;
            builder.Append($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
            foreach (int offset in offsets)
            {
                builder.Append($"{offset:D10} 00000 n \n");
            }

            builder.Append($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
            return System.Text.Encoding.ASCII.GetBytes(builder.ToString());
        }
    }
}
