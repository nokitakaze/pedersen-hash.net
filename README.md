Pedersen-Hash.Net
===========
[![Build status](https://ci.appveyor.com/api/projects/status/8063wyx09n8gpvb3/branch/master?svg=true)](https://ci.appveyor.com/project/nokitakaze/pedersen-hash-net/branch/master)
[![Test status](https://img.shields.io/appveyor/tests/nokitakaze/pedersen-hash-net/master)](https://ci.appveyor.com/project/nokitakaze/pedersen-hash-net/branch/master)
[![codecov](https://codecov.io/gh/nokitakaze/pedersen-hash.net/branch/master/graph/badge.svg)](https://codecov.io/gh/nokitakaze/pedersen-hash.net)
[![Nuget version](https://badgen.net/nuget/v/PedersenHashNet)](https://www.nuget.org/packages/PedersenHashNet)
[![Total nuget downloads](https://badgen.net/nuget/dt/PedersenHashNet)](https://www.nuget.org/packages/PedersenHashNet)

This project is a C# implementation of [Pedersen Hash](https://docs.tornado.cash/general/how-does-tornado.cash-work) used in [Tornado Cash](https://docs.tornado.cash/general/how-does-tornado.cash-work) [encrypted notes](https://github.com/nokitakaze/tornado-cash-encrypted-note.net).

## Public Interface

* **`PedersenHashGenerator.GetHexCommitmentFromPrivatePair(string commitmentSecret): string`** — Get hexed public commitment for hex commitment secret
* **`PedersenHashGenerator.GetHexCommitmentFromPrivatePair(byte[] commitmentSecret): string`** — Get hexed public commitment for byte array commitment secret
* **`PedersenHashGenerator.GetCommitmentFromPrivatePair(string commitmentSecret): byte[]`** — Get 32-bytes array with public commitment for hex commitment secret
* **`PedersenHashGenerator.GetCommitmentFromPrivatePair(byte[] commitmentSecret): byte[]`** — Get 32-bytes array with public commitment for byte array commitment secret

### Example
Check commitment
```C#
var commitment = PedersenHashGenerator.GetHexCommitmentFromPrivatePair("0xc0b94c09303630d49984ee319ef44beb1cd29eb6d4f1f7fb8955731d76d79a5e94e1676adba37f0c7110db02b3cdc526f960d4c2685da441597c7533aa3d");
const string expectedCommitment = "0x01cd97f5fb94b8bd45979703af68790dff36dfb13c8227d6b68349fac72ff949";

Console.WriteLine("{0}\t{1}", commitment, commitment == expectedCommitment);
```

### Benchmark
- [Main benchmark](docs/main-benchmark.md)

## License
Licensed under the GPL-3.0.

This software is provided **"AS IS" WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied**.

Source code was transpiled from JavaScript code:
- [circomlib](https://github.com/iden3/circomlib) and [its tornado version](https://github.com/tornadocash/circomlib.git)
- [SnarkJS](https://github.com/iden3/snarkjs) and [its tornado version](https://github.com/tornadocash/snarkjs.git)
