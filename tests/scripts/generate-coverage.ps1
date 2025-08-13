# Limpar pastas antigas
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue -Path '.\tests\TestCoverage','.\tests\TestCoverage\TestResults','.\tests\TestCoverage\CoverageReport'

# Criar pasta de resultados
New-Item -ItemType Directory -Path '.\tests\TestCoverage\TestResults' -Force | Out-Null

# Criar pasta de relatório
New-Item -ItemType Directory -Path '.\tests\TestCoverage\CoverageReport' -Force | Out-Null

# Executar testes e coletar cobertura
dotnet test $testProjectPath ` --settings coverlet.runsettings.xml ` --collect:"XPlat Code Coverage" ` --results-directory '.\tests\TestCoverage\TestResults'

# Localizar arquivo de cobertura
$coverageFile = Get-ChildItem -Path '.\tests\TestCoverage\TestResults' -Recurse -Include 'coverage.opencover.xml','coverage.cobertura.xml' | Select-Object -First 1

# Gerar relatório HTML
reportgenerator -reports:$coverageFile.FullName -targetdir:'.\tests\TestCoverage\CoverageReport' -reporttypes:Html

# Abrir o index.html gerado
Invoke-Item '.\tests\TestCoverage\CoverageReport\index.htm'
