# 🎄 Shakey's AoC 2023 🌟

[![CI workflow](https://github.com/robshakespeare/aoc2023/actions/workflows/CI-workflow.yml/badge.svg)](https://github.com/robshakespeare/aoc2023/actions/workflows/CI-workflow.yml)
[![App releases](https://github.com/robshakespeare/aoc2023/actions/workflows/release-workflow.yml/badge.svg)](https://github.com/robshakespeare/aoc2023/actions/workflows/release-workflow.yml)

Rob Shakespeare's solutions to the Advent of Code 2023 challenges at https://adventofcode.com/2023.


### Shakey's AoC 2023 Releases
* [Linux CLI download](https://github.com/robshakespeare/aoc2023/releases/latest/download/AoC.CLI)
* [Windows CLI download](https://github.com/robshakespeare/aoc2023/releases/latest/download/AoC.CLI.exe)


### Prerequisites

* [.NET 8.0 SDK](https://aka.ms/get-dotnet-8)
* Optional: to be able to run the cake scripts, first: `dotnet tool restore`


### Run

To run the console application:

```
dotnet run --project AoC.CLI
```

To pull the puzzle input (requires access to the key vault containing valid AoC session token):

```
dotnet run --project AoC.CLI --pull
```

To decrypt the puzzle inputs in this repository (requires `AocPuzzleInputCryptoKey` config value):

```
dotnet run --project AoC.CLI --decrypt
```


### Test

```
dotnet test
```
