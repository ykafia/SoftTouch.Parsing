```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4529/22H2/2022Update)
13th Gen Intel Core i5-1345U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.301
  [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2


```
| Method         | Mean      | Error    | StdDev   | Median    | Gen0    | Gen1   | Allocated |
|--------------- |----------:|---------:|---------:|----------:|--------:|-------:|----------:|
| SoftTouchParse |  16.65 μs | 0.395 μs | 1.100 μs |  16.12 μs |  2.5024 | 0.0610 |  15.38 KB |
| StrideEtoParse | 181.66 μs | 3.590 μs | 4.274 μs | 180.25 μs | 30.0293 | 8.0566 | 184.12 KB |
| StrideParse    | 111.96 μs | 1.088 μs | 0.850 μs | 111.96 μs | 23.4375 | 5.8594 | 144.76 KB |
