#!/bin/bash
set -e

# Clean up old coverage
rm -rf TestCoverage TestResults CoverageReport

mkdir TestCoverage

# Run tests with coverage and exclusion settings
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory ./tests/TestCoverage/TestResults

# Find the coverage file
coverage_file=$(find ./tests/TestCoverage/TestResults -name "coverage.cobertura.xml" | head -n 1)

# Generate HTML report
reportgenerator -reports:"$coverage_file" -targetdir:"./TestCoverage/CoverageReport" -reporttypes:Html

# Optionally open in browser
xdg-open TestCoverage/CoverageReport/index.html || echo "Open ./TestCoverage/CoverageReport/index.html manually"
