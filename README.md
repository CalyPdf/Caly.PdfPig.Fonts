# Caly.PdfPig.Fonts

Substitute font programs for the 14 PDF Standard fonts, for [PdfPig](https://github.com/UglyToad/PdfPig).

This package bundles permissively licensed substitutes, the same choice as pdf.js, and registers them with PdfPig:

```csharp
using Caly.PdfPig.Fonts;

StandardFontSubstitutes.Register(); // once, at startup

using var document = PdfDocument.Open("file.pdf");
```

`PdfPig.Rendering.Skia` references this package and calls `Register()` for you. Nothing is read or parsed until a
document needs a glyph from a given font.

Only glyph outlines come from the substitute. Text, widths and bounding boxes keep coming from the document and the
Standard 14 metrics.

## Fonts

Each glyph comes from the first file in the list that has it.

| Standard 14 font | Substitutes, in lookup order |
|---|---|
| Helvetica (+ Bold, Oblique, BoldOblique) | Liberation Sans (Regular, Bold, Italic, BoldItalic) |
| Times-Roman (+ Bold, Italic, BoldItalic) | Foxit Serif, then Liberation Serif |
| Courier (+ Bold, Oblique, BoldOblique) | Foxit Fixed, then Liberation Mono |
| Symbol | Foxit Symbol |
| ZapfDingbats | Foxit Dingbats |

The Foxit Latin fonts lack the Central European accented letters (`ecaron`, `scommaaccent`, `aogonek`, ...), about 85 of
the 315 glyphs in the Standard 14 metrics. Liberation fills those gaps, so every Standard 14 glyph has an outline.
Despite the `.pfb` extension (PDFium's names), the Foxit files are bare CFF programs.

## Licences

- Code: Apache-2.0 (`LICENSE.txt`).
- Foxit fonts: BSD-3-Clause (`LICENSE_FOXIT`).
- Liberation fonts: SIL Open Font License 1.1 (`LICENSE_LIBERATION`).

See `THIRD-PARTY-NOTICES.md` for provenance.
