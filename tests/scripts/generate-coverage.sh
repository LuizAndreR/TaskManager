#!/bin/bash
set -e

# Clean up old coverage
rm -rf ./tests/TestCoverage 

mkdir ./tests/TestCoverage

# Run tests with coverage and exclusion settings
dotnet test --settings coverlet.runsettings.xml --collect:"XPlat Code Coverage" --results-directory ./tests/TestCoverage/TestResults

# Find the coverage file
coverage_file=$(find ./tests/TestCoverage/TestResults -name "coverage.opencover.xml" | head -n 1)

# Generate HTML report
reportgenerator -reports:"$coverage_file" -targetdir:"./tests/TestCoverage/CoverageReport" -reporttypes:Html

# Optionally open in browser
xdg-open ./tests/TestCoverage/CoverageReport/index.html || echo "Open ./tests/TestCoverage/CoverageReport/index.html manually"
