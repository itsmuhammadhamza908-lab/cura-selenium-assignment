@echo off
rem run this AFTER the tests have been run from Test Explorer
rem opens the allure report in the browser using the results created by the tests
allure serve Assignment\bin\Debug\allure-results
pause
