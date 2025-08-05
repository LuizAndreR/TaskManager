#!/bin/bash
set -e

# Clean up
rm -rf TestCoverage TestResults CoverageReport

mkdir TestCoverage

# Run tests and collect coverage
dotnet test --collect:"XPlat Code Coverage" \
  --results-directory ./TestCoverage/TestResults \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura

# Find coverage file
coverage_file=$(find ./TestCoverage/TestResults -name "coverage.cobertura.xml" | head -n 1)

# Generate summary report
reportgenerator -reports:"$coverage_file" -targetdir:"TestCoverage/CoverageReport" -reporttypes:HtmlSummary

# Get coverage percentage
coverage_percent=$(grep -oP 'line-rate="\K[0-9.]+' -m 1 "$coverage_file")
coverage_percent=$(awk "BEGIN {printf \"%.0f\n\", $coverage_percent * 100}")

echo "🔍 Current coverage: $coverage_percent%"

MIN_COVERAGE=80
if [ "$coverage_percent" -lt "$MIN_COVERAGE" ]; then
  echo "❌ Code coverage ($coverage_percent%) is below required minimum ($MIN_COVERAGE%)"
  exit 1
fi

echo "✅ Coverage check passed: $coverage_percent% ≥ $MIN_COVERAGE%"