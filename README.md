# Serilog.Expressions.Utf8

A fork of Serilog.Expressions for formatting Serilog events to `IBufferWriter<byte>`. Integrates with [Serilog.Sinks.RawFile](https://www.nuget.org/packages/Serilog.Sinks.RawFile/), and [Serilog.Sinks.RawConsole](https://www.nuget.org/packages/Serilog.Sinks.RawConsole/).

Serilog.Expressions is an embeddable mini-language for filtering, enriching, and formatting Serilog
events.

### Usage
```csharp
var logger = new LoggerConfiguration()
    .WriteTo.RawFile("file.log", new Utf8ExpressionTemplate("{@t:yyyy-MM-dd HH:mm:ss.ffff} {@l:u3} {@m}\n"))
    .CreateLogger();
```

## Acknowledgements

Includes the parser combinator implementation from [Superpower](https://github.com/datalust/superpower), copyright Datalust,
Superpower Contributors, and Sprache Contributors; licensed under the Apache License, 2.0.
