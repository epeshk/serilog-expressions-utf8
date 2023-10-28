// Copyright © Serilog Contributors
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

namespace Serilog.Templates.Themes;

public static class Utf8TemplateThemes
{
    public static Utf8TemplateTheme Literate { get; } = new(
        new Dictionary<Utf8TemplateThemeStyle, byte[]>
        {
            [Utf8TemplateThemeStyle.Text] = "\x1b[38;5;0015m"u8.ToArray(),
            [Utf8TemplateThemeStyle.SecondaryText] = "\x1b[38;5;0007m"u8.ToArray(),
            [Utf8TemplateThemeStyle.TertiaryText] = "\x1b[38;5;0008m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.Invalid] = "\x1b[38;5;0011m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.Null] = "\x1b[38;5;0027m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.Name] = "\x1b[38;5;0007m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.String] = "\x1b[38;5;0045m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.Number] = "\x1b[38;5;0200m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.Boolean] = "\x1b[38;5;0027m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.Scalar] = "\x1b[38;5;0085m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.LevelVerbose] = "\x1b[38;5;0007m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.LevelDebug] = "\x1b[38;5;0007m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.LevelInformation] = "\x1b[38;5;0015m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.LevelWarning] = "\x1b[38;5;0011m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.LevelError] = "\x1b[38;5;0015m\x1b[48;5;0196m"u8.ToArray(),  
            [Utf8TemplateThemeStyle.LevelFatal] = "\x1b[38;5;0015m\x1b[48;5;0196m"u8.ToArray(),  
        });

    public static Utf8TemplateTheme Grayscale { get; } = new(
        new Dictionary<Utf8TemplateThemeStyle, byte[]>
        {
            [Utf8TemplateThemeStyle.Text] = "\x1b[37;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.SecondaryText] = "\x1b[37m"u8.ToArray(),
            [Utf8TemplateThemeStyle.TertiaryText] = "\x1b[30;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Invalid] = "\x1b[37;1m\x1b[47m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Null] = "\x1b[1m\x1b[37;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Name] = "\x1b[37m"u8.ToArray(),
            [Utf8TemplateThemeStyle.String] = "\x1b[1m\x1b[37;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Number] = "\x1b[1m\x1b[37;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Boolean] = "\x1b[1m\x1b[37;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Scalar] = "\x1b[1m\x1b[37;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelVerbose] = "\x1b[30;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelDebug] = "\x1b[30;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelInformation] = "\x1b[37;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelWarning] = "\x1b[37;1m\x1b[47m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelError] = "\x1b[30m\x1b[47m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelFatal] = "\x1b[30m\x1b[47m"u8.ToArray(),
        });

    public static Utf8TemplateTheme Code { get; } = new(
        new Dictionary<Utf8TemplateThemeStyle, byte[]>
        {
            [Utf8TemplateThemeStyle.Text] = "\x1b[38;5;0253m"u8.ToArray(),
            [Utf8TemplateThemeStyle.SecondaryText] = "\x1b[38;5;0246m"u8.ToArray(),
            [Utf8TemplateThemeStyle.TertiaryText] = "\x1b[38;5;0242m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Invalid] = "\x1b[33;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Null] = "\x1b[38;5;0038m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Name] = "\x1b[38;5;0081m"u8.ToArray(),
            [Utf8TemplateThemeStyle.String] = "\x1b[38;5;0216m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Number] = "\x1b[38;5;151m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Boolean] = "\x1b[38;5;0038m"u8.ToArray(),
            [Utf8TemplateThemeStyle.Scalar] = "\x1b[38;5;0079m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelVerbose] = "\x1b[37m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelDebug] = "\x1b[37m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelInformation] = "\x1b[37;1m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelWarning] = "\x1b[38;5;0229m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelError] = "\x1b[38;5;0197m\x1b[48;5;0238m"u8.ToArray(),
            [Utf8TemplateThemeStyle.LevelFatal] = "\x1b[38;5;0197m\x1b[48;5;0238m"u8.ToArray(),
        });
}