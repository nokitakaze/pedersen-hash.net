# Main benchmark
## Windows 10 & AMD Ryzen 9 3900X BOX 3.8 GHz (Matisse) 
```ini
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19044.1826/21H2/November2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=5.0.203
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT AVX2
  DefaultJob : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT AVX2
```

|                       Method |    N |         Mean |      Error |     StdDev |
|----------------------------- |----- |-------------:|-----------:|-----------:|
| GetCommitmentFromPrivatePair |    1 |     64.24 ms |   1.200 ms |   1.064 ms |
| GetCommitmentFromPrivatePair |   10 |    611.45 ms |   9.704 ms |   8.602 ms |
| GetCommitmentFromPrivatePair |  100 |  7,478.48 ms |  38.358 ms |  32.030 ms |
| GetCommitmentFromPrivatePair | 1000 | 60,940.80 ms | 233.081 ms | 218.024 ms |
|       GenerateCommitmentPair |    1 |     67.22 ms |   1.324 ms |   2.765 ms |
|       GenerateCommitmentPair |   10 |    693.40 ms |  13.386 ms |  17.870 ms |
|       GenerateCommitmentPair |  100 |  6,458.38 ms | 106.725 ms |  94.609 ms |
|       GenerateCommitmentPair | 1000 | 68,144.39 ms | 278.706 ms | 260.702 ms |

## Windows 10 & Intel Core i7-10875H CPU 2.30GHz
```ini
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19043.1826/21H1/May2021Update)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=5.0.408
  [Host]     : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX2
  DefaultJob : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX2
```

|                       Method |    N |         Mean |        Error |       StdDev |
|----------------------------- |----- |-------------:|-------------:|-------------:|
| GetCommitmentFromPrivatePair |    1 |    100.48 ms |     1.810 ms |     2.653 ms |
| GetCommitmentFromPrivatePair |   10 |    996.01 ms |     9.927 ms |     8.800 ms |
| GetCommitmentFromPrivatePair |  100 |  8,219.46 ms |   157.308 ms |   174.848 ms |
| GetCommitmentFromPrivatePair | 1000 | 98,831.24 ms | 1,563.567 ms | 1,462.562 ms |
|       GenerateCommitmentPair |    1 |     94.15 ms |     1.878 ms |     4.712 ms |
|       GenerateCommitmentPair |   10 |    892.86 ms |    16.607 ms |    15.534 ms |
|       GenerateCommitmentPair |  100 |  9,237.14 ms |   152.652 ms |   135.322 ms |
|       GenerateCommitmentPair | 1000 | 90,280.35 ms | 1,317.828 ms | 1,168.220 ms |

## Windows 10 & Intel Core i7-10875H CPU 2.30GHz
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

