set VSTest="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
set NUGETFolder=".\packages"

"%NUGETFolder%\OpenCover.4.7.1221\tools\OpenCover.Console.exe" -target:%VSTest% -targetargs:"%cd%\D365.Testing.FakeXrmEasy\bin\Debug\D365.Testing.dll" -output:"%cd%\CoverageResults.xml" -register:user -filter:"+[Dynamics365.Monitoring.Plugins*]* -[D365.Testing]*"
"%NUGETFolder%\ReportGenerator.5.1.25\tools\net47\ReportGenerator.exe" -reports:"%cd%\CoverageResults.xml" -targetdir:"%cd%\CoverageReport"