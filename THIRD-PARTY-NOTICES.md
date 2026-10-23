# Third-party notices

The assembly embeds the font files below in a zip archive (Deflate). The archive holds the files unchanged and they are
read back byte for byte.

## Liberation fonts

`Caly.PdfPig.Fonts/Resources/Liberation*.ttf` are the unmodified TrueType files of Liberation Fonts 2.1.5
(https://github.com/liberationfonts/liberation-fonts, release asset `liberation-fonts-ttf-2.1.5.tar.gz`, SHA-256
`7191C669BF38899F73A2094ED00F7B800553364F90E2637010A69C0E268F25D0`). Liberation Fonts 2.x are licensed under the
SIL Open Font License 1.1, reproduced in `Caly.PdfPig.Fonts/Resources/LICENSE_LIBERATION` (the release's `LICENSE`
file):

```
Digitized data copyright (c) 2010 Google Corporation
	with Reserved Font Arimo, Tinos and Cousine.
Copyright (c) 2012 Red Hat, Inc.
	with Reserved Font Name Liberation.
```

## Foxit fonts (PDFium)

`Caly.PdfPig.Fonts/Resources/Foxit*.pfb` are the Foxit fonts compiled into PDFium
(`core/fxge/fontdata/chromefontdata/*.cpp`), byte-identical to those sources. The Dingbats, Symbol, Serif and Fixed files
are also byte-identical to the copies pdf.js ships in `external/standard_fonts`. Original code copyright 2014 Foxit
Software Inc. (http://www.foxitsoftware.com).

They are licensed under the BSD-3-Clause licence below (also in `Caly.PdfPig.Fonts/Resources/LICENSE_FOXIT`):

```
Copyright 2014 PDFium Authors. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

   * Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.
   * Redistributions in binary form must reproduce the above
copyright notice, this list of conditions and the following disclaimer
in the documentation and/or other materials provided with the
distribution.
   * Neither the name of Google Inc. nor the names of its
contributors may be used to endorse or promote products derived from
this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
```
