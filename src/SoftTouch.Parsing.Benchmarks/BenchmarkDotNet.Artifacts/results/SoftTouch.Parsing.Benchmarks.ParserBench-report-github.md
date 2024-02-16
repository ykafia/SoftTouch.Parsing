```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.3930/22H2/2022Update)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


```
| Method         | Mean      | Error    | StdDev    | Gen0    | Allocated |
|--------------- |----------:|---------:|----------:|--------:|----------:|
| StrideParse    | 111.11 μs | 3.679 μs | 10.497 μs | 13.4277 |  41.66 KB |
| SoftTouchParse |  10.95 μs | 0.267 μs |  0.749 μs |  1.5106 |   4.65 KB |
