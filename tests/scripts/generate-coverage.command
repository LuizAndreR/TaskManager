#!/bin/bash
set -e

# Clean up
rm -rf TestCoverage TestResults CoverageReport

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestCoverage/TestResults

# Find coverage file
coverage_file=$(find ./TestCoverage/TestResults -name "coverage.cobertura.xml" | head -n 1)

# Generate HTML report
reportgenerator -reports:"$coverage_file" -targetdir:"CoverageReport" -reporttypes:Html

# Open report
open TestCoverage/CoverageReport/index.html