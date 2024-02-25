```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.2861/22H2/2022Update/SunValley2)
AMD Ryzen 5 3500U with Radeon Vega Mobile Gfx, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.200
  [Host]        : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  .NET 8.0      : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  NativeAOT 8.0 : .NET 8.0.2, X64 NativeAOT AVX2


```
| Method         | Job           | Runtime       | Mean      | Error    | StdDev    | Median    | Gen0    | Allocated |
|--------------- |-------------- |-------------- |----------:|---------:|----------:|----------:|--------:|----------:|
| StrideParse    | .NET 8.0      | .NET 8.0      |  94.97 μs | 4.438 μs | 13.016 μs |  94.82 μs | 20.2637 |  41.66 KB |
| SoftTouchParse | .NET 8.0      | .NET 8.0      |  10.31 μs | 0.242 μs |  0.703 μs |  10.44 μs |  2.2736 |   4.65 KB |
| StrideParse    | NativeAOT 8.0 | NativeAOT 8.0 | 105.68 μs | 2.391 μs |  6.859 μs | 104.48 μs | 20.2637 |  41.59 KB |
| SoftTouchParse | NativeAOT 8.0 | NativeAOT 8.0 |  12.94 μs | 0.257 μs |  0.601 μs |  12.62 μs |  2.2736 |   4.65 KB |
