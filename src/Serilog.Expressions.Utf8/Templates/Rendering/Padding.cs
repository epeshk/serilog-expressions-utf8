// Copyright 2013-2020 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Serilog.Parsing;
using Serilog.Utf8.Commons;

namespace Serilog.Templates.Rendering;

static class Padding
{
  /// <summary>
  /// Writes the provided value to the output, applying direction-based padding when <paramref name="alignment"/> is provided.
  /// </summary>
  public static void Apply(ref Utf8Writer output, int charsWritten, int bytesWritten, in Alignment alignment, Span<byte> workingSpan)
  {
    if (charsWritten >= alignment.Width)
      return;

    ApplyInternal(ref output, charsWritten, bytesWritten, alignment, workingSpan);
  }

  static void ApplyInternal(ref Utf8Writer output, int charsWritten, int bytesWritten, Alignment alignment, Span<byte> workingSpan)
  {
    var pad = alignment.Width - charsWritten;

    if (alignment.Direction == AlignmentDirection.Right)
    {
      workingSpan.Slice(0, bytesWritten).CopyTo(workingSpan.Slice(pad, bytesWritten));
      workingSpan.Slice(0, pad).Fill((byte)' ');
      output.Advance(pad);
    }
    else
    {
      output.Fill((byte)' ', pad);
    }
  }

  //
  /// <summary>
  /// Writes the provided value to the output, applying direction-based padding when <paramref name="alignment"/> is provided.
  /// </summary>
  public static void ApplyAscii(ref Utf8Writer output, ReadOnlySpan<byte> value, in Alignment alignment)
  {
    if (value.Length >= alignment.Width)
    {
      output.Write(value);
      return;
    }

    ApplyAsciiInternal(output, value, alignment);
  }

  private static void ApplyAsciiInternal(Utf8Writer output, ReadOnlySpan<byte> value, Alignment alignment)
  {
    var pad = alignment.Width - value.Length;

    if (alignment.Direction == AlignmentDirection.Left)
      output.Write(value);

    output.Fill((byte)' ', pad);

    if (alignment.Direction != AlignmentDirection.Right)
      return;

    output.Write(value);
  }
}