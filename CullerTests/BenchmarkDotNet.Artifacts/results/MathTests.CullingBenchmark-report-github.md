```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
13th Gen Intel Core i7-13700HX 2.10GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 10.0.109
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


```
| Method                    | Mean      | Error     | StdDev    | Ratio | RatioSD | Rank | Gen0      | Gen1     | Gen2     | Allocated | Alloc Ratio |
|-------------------------- |----------:|----------:|----------:|------:|--------:|-----:|----------:|---------:|---------:|----------:|------------:|
| StandardCpuCulling        |  1.788 ms | 0.0340 ms | 0.0318 ms |  1.00 |    0.02 |    1 |   27.3438 |  27.3438 |  27.3438 |  97.69 KB |        1.00 |
| GpuCulling_StandardFloat  | 63.406 ms | 1.2899 ms | 3.7628 ms | 35.46 |    2.18 |    2 | 1000.0000 | 875.0000 | 875.0000 | 7457.5 KB |       76.34 |
| GpuCulling_SmartPrecision |        NA |        NA |        NA |     ? |       ? |    ? |        NA |       NA |       NA |        NA |           ? |

Benchmarks with issues:
  CullingBenchmark.GpuCulling_SmartPrecision: DefaultJob
