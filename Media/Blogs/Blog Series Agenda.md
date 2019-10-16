Blog Series Agenda

1. ## **<u>Overview and Getting Started</u>**

   1. Summary
   2. Getting Started
      1. Dependencies
      2. Downloading using Nuget Package Manager
      3. Cloning from GitHub
      4. Cloning locally from Azure DevOps
   3. Reviewing Sample Unit Tests
      1. Looking into the Open Account Sample Unit Test
         1. Exploring Test Settings
            1. Application Configuration file
            2. Test Settings BrowserOptions object
   4. Next Steps
      1. Conclusion

2. ## **<u>Designing and Debugging Unit Tests</u>**

   1. Summary
   2. Getting Started
   3. Reviewing the Open Account Sample Unit Test in depth
      1. WebClient and XrmApp
      2. Commands
         1. Login
         2. Open UCI App
         3. Open Sub Area
         4. Search
         5. Open Record
         6. Think Time
   4. Designing Tests towards Customizations
      1. Adding custom UCI applications
      3. Searching and reviewing results in Grid
      3. Changing Forms and Referencing Fields
   5. Debugging Tests
      1. Running Unit Tests in non headless mode
      2. Running Unit Tests in Incongnito Mode
      3. Assertions and Validation
   6. Next Steps
      1. Exploring the Sample Unit Tests
   
3. ## **Extending the EasyRepro Framework**

   1. Summary
2. Getting Started
   3. Example: The ADFS Redirect Login Method
   1. FindElement
      2. WaitForPageToLoad
   3. SendKeys and Clear
   4. Understanding how to extend element references

      1. Building a reference for Entity Form controls
      1. Locating the Reference file
            1. The Element File
            2. The App Element Reference File
            3. Connecting Reference to Element
         2. Adding to the Reference and Element classes
   5. Locating elements in the DOM
      1. ID
   2. CssClass
      3. Tag
   4. XPath
   10. Next Steps
         1. Custom components and web resources
         2. Conclusion
   
   ## **Monitoring and Insights with EasyRepro**
   
   1. Summary
   2. Getting Started
   3. Command Timings
      1. Exploring the BrowserCommandResult class
      2. Exploring the BrowserCommand.cs file
      3. Execution Times
      4. Think and Transition Times
         1. How is all of this calculated?
   4. Logging and Monitoring
      1. Debug and Trace
   5. Application Insights
      1. Setup Application Insights Telemetry Client
      2. Push Timings to Application Insights
   6. Next Steps
      1. Expanding messages sent to Application Insights
   7. Conclusion
   
   ## **Running EasyRepro Tests in Azure DevOps**
   
   1. Summary
   2. Getting Started
   3. Run Settings File
      1. Class Initalizer
      2. Properties
   4. Setting up the Build Pipeline
      1. VSTest task
      2. Pointing to runsettings
   5. Running the Build Pipeline
   6. Exploring the Results File
   7. Next Steps
      1. Continuous Quality
   8. Conclusion