```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4529/22H2/2022Update)
13th Gen Intel Core i5-1345U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.301
  [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2


```
| Method         | Mean     | Error    | StdDev   | Median   | Gen0    | Gen1    | Allocated |
|--------------- |---------:|---------:|---------:|---------:|--------:|--------:|----------:|
| RecursiveParse | 411.2 μs |  9.83 μs | 28.82 μs | 401.2 μs | 39.0625 | 13.6719 | 243.01 KB |
| PrattParse     | 386.7 μs | 11.28 μs | 32.00 μs | 380.6 μs | 39.5508 |  9.7656 | 242.51 KB |
