# Main benchmark
## AMD Ryzen 9 3900X BOX 3.8 GHz (Matisse) & Windows 10 
```ini
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19044.1826/21H2/November2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=5.0.203
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT AVX2
  DefaultJob : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT AVX2
```

|                       Method |    N |         Mean |      Error |     StdDev |
|----------------------------- |----- |-------------:|-----------:|-----------:|
| GetCommitmentFromPrivatePair |    1 |     77.49 ms |   0.529 ms |   0.469 ms |
| GetCommitmentFromPrivatePair |   10 |    764.99 ms |   3.432 ms |   3.210 ms |
| GetCommitmentFromPrivatePair |  100 |  6,460.13 ms |  72.036 ms |  67.382 ms |
| GetCommitmentFromPrivatePair | 1000 | 63,971.08 ms | 258.563 ms | 241.860 ms |
|       GenerateCommitmentPair |    1 |     67.95 ms |   1.306 ms |   1.554 ms |
|       GenerateCommitmentPair |   10 |    689.75 ms |  13.683 ms |  20.895 ms |
|       GenerateCommitmentPair |  100 |  6,788.44 ms | 116.647 ms | 103.404 ms |
|       GenerateCommitmentPair | 1000 | 68,562.88 ms | 319.980 ms | 299.310 ms |

## Intel Core i7-10875H CPU 2.30GHz & Windows 10
```ini
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19043.1826/21H1/May2021Update)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=5.0.408
  [Host]     : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX2
  DefaultJob : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX2
```

|                       Method |    N |         Mean |        Error |     StdDev |
|----------------------------- |----- |-------------:|-------------:|-----------:|
| GetCommitmentFromPrivatePair |    1 |     79.27 ms |     1.041 ms |   0.869 ms |
| GetCommitmentFromPrivatePair |   10 |    752.55 ms |     5.443 ms |   4.825 ms |
| GetCommitmentFromPrivatePair |  100 |  9,060.01 ms |    97.546 ms |  86.472 ms |
| GetCommitmentFromPrivatePair | 1000 | 76,733.19 ms |   886.448 ms | 829.184 ms |
|       GenerateCommitmentPair |    1 |     84.78 ms |     1.684 ms |   3.835 ms |
|       GenerateCommitmentPair |   10 |    858.05 ms |    17.147 ms |  32.207 ms |
|       GenerateCommitmentPair |  100 |  8,370.93 ms |   135.643 ms | 113.268 ms |
|       GenerateCommitmentPair | 1000 | 83,231.92 ms | 1,046.009 ms | 873.465 ms |

## Intel Xeon CPU E31275 3.40GHz & Debian (inside docker)
```ini
BenchmarkDotNet=v0.13.2, OS=debian 10 (container)
Intel Xeon CPU E31275 3.40GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.408
  [Host]     : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX
  DefaultJob : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX
```

|                       Method |    N |         Mean |       Error |      StdDev |       Median |
|----------------------------- |----- |-------------:|------------:|------------:|-------------:|
| GetCommitmentFromPrivatePair |    1 |     124.5 ms |     2.47 ms |     2.19 ms |     124.5 ms |
| GetCommitmentFromPrivatePair |   10 |   1,219.2 ms |    21.74 ms |    20.33 ms |   1,222.9 ms |
| GetCommitmentFromPrivatePair |  100 |  12,730.4 ms |   229.74 ms |   479.56 ms |  12,541.5 ms |
| GetCommitmentFromPrivatePair | 1000 | 111,710.1 ms | 2,187.37 ms | 2,246.27 ms | 111,274.2 ms |
|       GenerateCommitmentPair |    1 |     115.9 ms |     2.29 ms |     5.71 ms |     116.8 ms |
|       GenerateCommitmentPair |   10 |   1,159.8 ms |    23.17 ms |    49.87 ms |   1,158.6 ms |
|       GenerateCommitmentPair |  100 |  11,275.7 ms |   212.09 ms |   177.10 ms |  11,271.5 ms |
|       GenerateCommitmentPair | 1000 | 118,765.5 ms | 2,297.21 ms | 2,256.17 ms | 118,622.2 ms |
