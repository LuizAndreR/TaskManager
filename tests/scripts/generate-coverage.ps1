$ErrorActionPreference = "Stop"

# Clean old coverage results
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue ".\tests\ReservaDezoito.Tests\TestCoverage", ".\TestResults", ".\CoverageReport"

New-Item -ItemType Directory -Path ".\tests\ReservaDezoito.Tests\TestCoverage"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ".\tests\ReservaDezoito.Tests\TestCoverage\TestResults"

# Find coverage file
$coverageFile = Get-ChildItem -Path ".\tests\ReservaDezoito.Tests\TestCoverage\TestResults" -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1

# Generate HTML report
reportgenerator -reports:$coverageFile.FullName -targetdir:".\tests\ReservaDezoito.Tests\TestCoverage\CoverageReport" -reporttypes:Html

Start-Process ".\tests\ReservaDezoito.Tests\TestCoverage\CoverageReport\index.html"