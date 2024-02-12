```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.3930/22H2/2022Update)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


```
| Method         | Mean      | Error     | StdDev    | Median    | Gen0   | Allocated |
|--------------- |----------:|----------:|----------:|----------:|-------:|----------:|
| StrideParse    | 42.467 μs | 2.7278 μs | 8.0431 μs | 38.792 μs | 7.6904 |  23.73 KB |
| SoftTouchParse |  1.680 μs | 0.0864 μs | 0.2451 μs |  1.573 μs | 0.4139 |   1.27 KB |
