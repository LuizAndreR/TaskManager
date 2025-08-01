$ErrorActionPreference = "Stop"

# Limpar pastas antigas
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue ".\tests\TestCoverage", ".\TestResults", ".\CoverageReport"

New-Item -ItemType Directory -Path ".\tests\TestCoverage"

# Defina o caminho do projeto de testes (ajuste conforme sua estrutura)
$testProjectPath = ".\tests\TaskManager.Tests"

# Executa os testes dentro da pasta do projeto com resultados na pasta de cobertura
dotnet test $testProjectPath --collect:"XPlat Code Coverage" --results-directory ".\tests\TestCoverage\TestResults"

# Procurar arquivo de cobertura na pasta correta
$coverageFile = Get-ChildItem -Path ".\tests\TestCoverage\TestResults" -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1

# Gerar relatório HTML
reportgenerator -reports:$coverageFile.FullName -targetdir:".\tests\TestCoverage\CoverageReport" -reporttypes:Html

# Abrir relatório no navegador
Start-Process ".\tests\TestCoverage\CoverageReport\index.html"
