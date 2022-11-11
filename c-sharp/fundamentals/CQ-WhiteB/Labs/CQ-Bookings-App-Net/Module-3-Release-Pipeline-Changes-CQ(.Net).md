**[Home](/4-Continuous-Quality/Labs/CQ-Bookings-App-Net) | [Module 1-Code changes for CQ](/4-Continuous-Quality/Labs/CQ-Bookings-App-Net/Module-1-Code-Changes-CQ\(.Net\)) | [Module 2-Build Pipeline Changes for CQ](/4-Continuous-Quality/Labs/CQ-Bookings-App-Net/Module-2-Build-Pipeline-Changes-CQ\(.Net\))| [Module 3-Release Pipeline changes for CQ](/4-Continuous-Quality/Labs/CQ-Bookings-App-Net/Module-3-Release-Pipeline-Changes-CQ\(.Net\))  |**

[[_TOC_]]

## Module 3 : Enabling Shift left Testing in Booking Release Pipeline

### Exercise 1 : Add Functional BDD Tests in QA Stage/Environment 
In this exercise we will add and **configure Functional BDD Tests** as part of release pipeline. We already integrated this test as part of Source Code in Module 1: Exercise 2. 

1. Navigate to `Releases -> Bookings-CD-Base` and click on `edit`

    ![Booking Edit Release Pipeline](/3-Continuous-Delivery/.Images/Bookings/Booking-CI-AzureDevops-Release-EditPipeline.png)

2. Switch to `Release QA` stage under tasks view. Add an agent job by clicking on `...` and `Add an agent job`.

   ![Add Agent Job Option](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-AddAgentJob.png =40%x)

3. Update the name of agent as `Run BDD Automated Functional Tests`. Click on **+** icon next to the newly created Agent job, and add a powershell task to  **Run BDD Automated Functional Tests** agent job in Booking-CD-Base release pipeline as shown below. 
  
   ![Add Powershell Task to BDD Agent](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-AddPowershell.png =60%x)

4.  Update the name of the Task "PowerShell Script" to `Update App Settings for BDD automated functional tests` and task details in the highlighted section as shown below:

    |  **Name**     | **Value**    |
    | ---------------------|----------------|
    | **Script Path**         |  $(System.DefaultWorkingDirectory)/_Bookings-CI/drop/ModernizationWithAppServiceDotNET/SmartHotel360.Registration.ARM/SpecflowIntegrationTest/UpdateAppConfigAtRunTimeWithoutKeyVault.ps1 | 
    | **Arguments**            | $(System.DefaultWorkingDirectory)/_Bookings-CI/drop/ModernizationWithAppServiceDotNET/SmartHotel360.Registration.Web.Int.Tests/bin/Release/SmartHotel360.Registration.Web.Integration.Tests.dll.config $(authenticationMode) $(sqlserver) $(databasename) $(sqlserveradmin) $(sqlserveradministratorpassword) |

      ![Update Powershell Task Config](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-BDD-UpdatePowershell.png =70%x)

5.  Click on **+** icon to add a `Visual Studio Test` task to  **Run BDD Automated Functional Tests** agent job in Booking-CD-Base release pipeline as shown below. Add this task below the `Update App Settings for BDD automated functional tests` powershell task.

      ![Add Visual Studio Test Task](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-BDD-AddUnitTest.png =40%x)

6. Update the name of task Visual Studio Test to `BDD Automated Functional Tests using Specflow` and task details as highlighted in the below image.
   
   |  **Name**     | **Value**  |
   | ---------------------|----------------|
   |**Test Files**| `**\*integration.tests*.dll !**\*TestAdapter.dll !**\obj\**` |

   ![Visual Studio Test Task Configuration Part 1](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-BDD-VSTest-UpdateConfig1.png =70%x)

7. Tick the checkbox as highlighted in the image below for the visual studio test task `BDD Automated Functional Tests using Specflow` to collect advanced diagnostics in case of failure.

   ![Visual Studio Test Task Configuration Part 2](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-BDD-VSTest-UpdateConfig2.png =40%x)

8. Once the tasks are setup successfully, Click on **Save** as shown below.
   
    ![Booking-CD Save Pipeline](/3-Continuous-Delivery/.Images/Bookings/Booking-CD-AzureDevops-Release-SavePipeline.png)

### Exercise 2 : Add UI test in SIT Stage/Environment

1. Switch to **Release SIT** stage under tasks view. Add an agent job by clicking on `...` and `Add an agent job`.

2. Update the name of agent as `Run UI Tests`. Click on **+** icon to add a powershell task to  **Run UI Tests** agent job in Booking-CD-Base release pipeline as shown below. 
  
   ![Add Powershell Task](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-UI-AddPowershell.png =40%x)

3. Update powershell task display name to `Update app config for UI Tests`, and task configurations as per the below values shown in table

     |  **Name**     | **Value**    |
    | ---------------------|----------------|
    | **Script Path**          |  $(System.DefaultWorkingDirectory)/_Bookings-CI/drop/ModernizationWithAppServiceDotNET/SmartHotel360.Registration.ARM/UITest/UpdateAppConfigSettingsUITests.ps1 | 
    | **Arguments**            | $(System.DefaultWorkingDirectory)/_Bookings-CI/drop/ModernizationWithAppServiceDotNET/SmartHotel360.Registration.Web.Tests/bin/Release/SmartHotel360.Registration.Web.Tests.dll.config "https://$(webapp).azurewebsites.net" |

   ![Update Powershell Task Configurations](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-UI-UpdatePowershell.png =70%x)

4.  Click on **+** icon to add a `Visual Studio Test` task to  **Run UI Tests** agent job in Booking-CD-Base release pipeline as shown below. Add this task below the `Update app config for UI Tests` powershell task.

      ![Add Visual Studio Test Task](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-BDD-AddUnitTest.png =40%x)

5. Update the Visual Studio Test task to `UI Test` and task details as highlighted in the below image.

   |  **Name**     | **Value**  |
   | ---------------------|----------------|
   |**Test filter criteria**| `TestCategory=UITest`|

   ><span style="color: black; background: lemonchiffon">**NOTE** As Release pipelines in SIT stage should run UI related tests, hence Test Filter Criteria is updated to run only UI Test related category.
	
   ![UI Test Task Configuration](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-UI-VSTest.png =70%x)

6. Once the tasks are setup successfully, Click on **Save** as shown below.
   
    ![Booking-CD Save Pipeline](/3-Continuous-Delivery/.Images/Bookings/Booking-CD-AzureDevops-Release-SavePipeline.png)

### Exercise 3: Verify BDD Integration Tests & UI Tests in Bookings Base Release Pipeline

1. Create new Release by clicking on `Create release` button. **Deselect** the **Undeploy stages** in all environmennts and click on `Create` button as shown below.

   ![Create Release Option](/3-Continuous-Delivery/.Images/Bookings/Booking-CD-AzureDevops-Release-CreateRelease-Option.png)

   ![Create Release Configuration](/3-Continuous-Delivery/.Images/Bookings/Booking-CI-AzureDevops-Release-CreateRelease-Config.png =80%x)

2. To open the Release, click as shown below - 

   ![Open Release](/3-Continuous-Delivery/.Images/Bookings/Booking-CD-AzureDevops-OpenRelease-Option.png)

3. Once dev stage is complete, Hover on `Release to QA` and click on `Approve`. This opens another tab, click on `Approve` again. This step is required as we have added pre-deployment approvals. Repeat the same for `Release SIT` stage.

   ![Approve Release QA Stage](/3-Continuous-Delivery/.Images/Bookings/Booking-CD-AzureDevops-Release-AcceptQA.png =60%x)

4. Switch to Release to QA stage and go to Tests View. You can see the number of Tests run and their status (Passed/Failed/Others). You can also filter based on passed and failed tests from the Outcome filter as highlighted in below image.

   ![BDD Test Pipeline Success](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-BDD-TestPass.png)

5. Switch to `Release to SIT` stage and go to Tests View. You can see the number of Tests run and their status (Passed/Failed/Others). You can also filter based on passed and failed tests from the Outcome filter as highlighted in below image.

   ![UI Test Pipeline Success](../../.Images/Bookings/Booking-CQ-AzureDevops-Release-UI-TestSuccess.png)