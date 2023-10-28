// Copyright 2017 Serilog Contributors
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

using System.Reflection.Metadata.Ecma335;
using System.Text;
using Serilog.Events;

// ReSharper disable StringLiteralTypo

namespace Serilog.Templates.Rendering;

/// <summary>
/// Implements the {Level} element.
/// can now have a fixed width applied to it, as well as casing rules.
/// Width is set through formats like "u3" (uppercase three chars),
/// "w1" (one lowercase char), or "t4" (title case four chars).
/// </summary>
static class LevelRenderer
{
    static readonly string[][] TitleCaseLevelMap =
    {
        new[] { "V", "Vb", "Vrb", "Verb", "Verb " },
        new[] { "D", "De", "Dbg", "Dbug", "Debug" },
        new[] { "I", "In", "Inf", "Info", "Info " },
        new[] { "W", "Wn", "Wrn", "Warn", "Warn " },
        new[] { "E", "Er", "Err", "Eror", "Error" },
        new[] { "F", "Fa", "Ftl", "Fatl", "Fatal" },
    };

    private static readonly byte[][][] TitleCaseLevelMapA;

    static readonly string[][] LowercaseLevelMap =
    {
        new[] { "v", "vb", "vrb", "verb", "verb " },
        new[] { "d", "de", "dbg", "dbug", "debug" },
        new[] { "i", "in", "inf", "info", "info " },
        new[] { "w", "wn", "wrn", "warn", "warn " },
        new[] { "e", "er", "err", "eror", "error" },
        new[] { "f", "fa", "ftl", "fatl", "fatal" },
    };

    private static readonly byte[][][] LowercaseLevelMapA;

    static readonly string[][] UppercaseLevelMap =
    {
        new[] { "V", "VB", "VRB", "VERB", "VERB " },
        new[] { "D", "DE", "DBG", "DBUG", "DEBUG" },
        new[] { "I", "IN", "INF", "INFO", "INFO " },
        new[] { "W", "WN", "WRN", "WARN", "WARN " },
        new[] { "E", "ER", "ERR", "EROR", "ERROR" },
        new[] { "F", "FA", "FTL", "FATL", "FATAL" },
    };

    private static readonly byte[][][] UppercaseLevelMapA;

    static LevelRenderer()
    {
        TitleCaseLevelMapA = TitleCaseLevelMap
            .Select(static x => x.Select(static y => System.Text.Encoding.ASCII.GetBytes(y)).ToArray()).ToArray();
        LowercaseLevelMapA = LowercaseLevelMap
            .Select(static x => x.Select(static y => System.Text.Encoding.ASCII.GetBytes(y)).ToArray()).ToArray();
        UppercaseLevelMapA = UppercaseLevelMap
            .Select(static x => x.Select(static y => System.Text.Encoding.ASCII.GetBytes(y)).ToArray()).ToArray();
    }

    public static byte[] GetLevelMoniker(LogEventLevel value, string? format)
    {
        if (format == null)
            return System.Text.Encoding.UTF8.GetBytes(value.ToString());
        //
        // if (format == null || format.Length != 2 && format.Length != 3)
        //     return Casing.Format(GetLevelMoniker(TitleCaseLevelMap, index), format);
        // if (format.Length != 2 && format.Length != 3)
        //     return Casing.Format(value.ToString(), format);

        byte[][][] map = GetMap(format);
        var width = GetWidth(format);

        var index = (int)value;
        if (index >= 0 && index <= (int)LogEventLevel.Fatal && width is >= 0 and < 6)
        {
            return map[index][Math.Min(width, map.Length) - 1];
        }

        var str = Casing.Format(value.ToString(), format[0].ToString());
        return Encoding.UTF8.GetBytes(str, 0, Math.Min(str.Length, width));
    }
    
    static byte[][][] GetMap(string? format)
    {
        if (format is null)
            return TitleCaseLevelMapA;
        return format[0] switch
        {
            'w' => LowercaseLevelMapA,
            'u' => UppercaseLevelMapA,
            't' => TitleCaseLevelMapA,
            _ => TitleCaseLevelMapA
        };
    }
    
    static int GetWidth(string? format)
    {
        if (format is null || format.Length < 2)
            return int.MaxValue;

        return ParseWidth(format);
    }

    static int ParseWidth(string format)
    {
        // Using int.Parse() here requires allocating a string to exclude the first character prefix.
        // Junk like "wxy" will be accepted but produce benign results.
        var width = format[1] - '0';
        if (format.Length == 3)
        {
            width *= 10;
            width += format[2] - '0';
        }

        if (width < 1)
            return 0;

        return width;
    }
}