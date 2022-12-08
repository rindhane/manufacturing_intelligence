---
title: "Installation Instructions"
date: 2022-12-07T11:53:46+05:30
draft: false
---


### Pre-requisities for QDAS-T deployment 
- Complete the following *pre-requisities* before initiating the web-application installation : 
    1. Complete the installation of QDAS application including `launcher applications`, `QDAS database setup`, `QDAS-web-client` and `Sql-Management-Studio`
    2.  [Optional] Configure the control plans required for the production lines under traceability monitoring. 
        - Check out the ***Control Plan Constraints*** section to understand the essential configuration expectations by traceability application

### Installation Procedure :    
1. Get hold of the  `QdasTraceabilityWebApp.zip` package and place it within the folder ***C:\Q-Das\traceabilitySetup*** of the server which has to run traceability application
2. Unzip the files on to the same folder (***C:\Q-Das\traceabilitySetup***) from the traceability package.
3. This folder, which contains all the files from the package(.zip), will become the `root folder` for the application and make sure that it should remain intact for all future purposes 
3. Your setup should look similar as indicated with the following image : 
![FolderView on extraction](/img/traceabilitySetup.png)
4. Now follow sub-step to create a table in ***QDAS' data holding database*** . This table will hold the pdf data uploaded from lab stations
    1. Make sure you are logged-in with the windows account which has administrative access-rights on the SQL server. *It is assumed that the SQL server is hosted on the same machine as traceability application*
    1. Go within the `installationSupportItems` folder within the **root folder**
        ![SQL file](/img/sqlfile.png)
    1. open the `create_pdf_table.sql` file in notepad : 
    {{< highlight sql >}}
    DECLARE @qdas_db VARCHAR(100);
    declare @query nvarchar(max); 

    SET @qdas_db='QDAS_VALUE_DATABASE'; --replace with the data database of qdas

    --help https://stackoverflow.com/questions/2838490/a-table-name-as-a-variable
    set @query= 'CREATE TABLE'+ QUOTENAME(@qdas_db)+'.dbo.LABREPORT(
    id              INT           NOT NULL    IDENTITY    PRIMARY KEY,
    serialNum       VARCHAR(100)  NOT NULL,
    stationName     VARCHAR(50),
    pdfData  VARCHAR(max),
    )';
    EXEC sp_executesql @query;
    GO
    {{< /highlight >}}

    2. Assign the value of the variable `qdas_db` with the database name created to hold the inspection data for Q-das. In the above example it is *QDAS_VALUE_DATABASE*. Put the name within the single quotes as shown in example.
    3. Close the sql file. Now double click on file `create_table.cmd` 
    4. Check within the SQL server through studio application whether a table  named `LABREPORT` has been created within the *QDAS' Data database*
        ![Lab table](/img/labupload.png)
4. Set the listening port by double-clicking on the file `change_server_config.cmd`. Change the default port of server (8080 shown below in the example) as per your requirement. Prefer to choose above 6000 
    {{< highlight json >}}
    {
    "Configs" : {
        "AppPort-http": "8080",
        "AppPort-https": "8081"
    },
    "TokenHandlerSecret":"ABCDEFGHIJKL"
}
    {{< /highlight >}}
5. Provide the SQL-server login-details for enabling traceability-web application to connect with QDAS database by following steps: 
    - Double click on the file  `setup-qdas-details.cmd` to enter the details : 
        {{< highlight toml>}}
        #this are the Qdas Database Connectivity details
        [qdas_value_db]
        dataSource = "127.0.0.1"  # ip of the sql server. 127.0.0.1 if on located on the local machine
        userID = "qdas"            
        password = "qdas1234"
        dbName="QDAS_VALUE_DATABASE" #database which contains the measurement data of q-das
        ReportTableName="LABREPORT"
        {{< /highlight >}}
    - Provide the details for each variable as indicated in the above example within the `qdas_value_db` table i.e *dataSource(ip of server), userID, password, dbName & ReportTableName*
    - After configuration are completed , close the file
6. Now make configurations to provide the production-setup details related to qdas by following steps:
    - Double click on the file `setup-qdas-details.cmd` to enter the details  
    - Once the file opens, go to the `[prodPlant]` table: 
    {{< highlight toml>}}
        #Production Plant related configs
        [prodPlant]
        lines = ["ABC","DEF","GHI"] #define all the typical lines configured in control plan 
        labs = ["CMM", "Profile Tester", "Surface Roughness", "Hardness Tester", "Visual Measurement Machine"] # define all the labs stations eligible to submit report
    {{< /highlight >}}
    - Enter the names of the production line whose end-of-line dashboard are required. In the above example , three lines:  *ABC, DEF, GHI* are configured 
    - Enter the names of the labs who want to upload the pdf reports. In the above example, labs like: *CMM, Profile Tester, Surface Roughness ...* etc have been configured
    - Now go to the `[DFQ_FROM_SCAN]` table :   
    {{< highlight toml>}}
        #DFQ creation parameters
        [DFQ_FROM_SCAN]
        folder_path = "C:\\Q-DAS\\test\\"
        characteristic_id = "s1" # characteristic Id (K2001)
        characteristic_desc = "Manual Scan Attribute" # Description of characteristic (K2002)
        characteristic_remark = "Scanner/Manual Data Captured for Traceability App" # (K2900)
        characteristic_UID = "{4B8302DA-21AD-401F-AF45-1DFD956B80B5}" # (K2997)
        characteristic_measurement_type = "vision" # (K2142)
        characteristic_type = 1 # (K2004) this is anyway optional, it is already configured to be a attribute type
    {{< /highlight >}}
    - Provide the details related to the configuration of DFQ-files generated from manual-scans from the stations which don't have O-QIS installed.
    - Critical one (i.e mandatory ) is the ***folder_path*** which dictates the folder to place the dfq files from the manual scan

### Booting-up / Initiating the  Traceability application :
1. Go back to the `root folder` and double click on `StartUpScript.cmd` file : 
        ![FolderView on extraction](/img/traceabilitySetup.png)
2. Once the application boots-up, it will look like as shown below : 
        ![server boot up](/img/serverBoot.png)
3. Go to the browser and put in the address & port as shown on the command prompt of server application. In the above example, it is shown as http://0.0.0.0:8080, which means you should reach to `http://localhost:8080` to checkout the traceability application
4. You should be able to see the home page as shown below : 
    ![webview](/img/webview.png)

### Control Plan Constraints :
Following are the expectations & constraints imposed by the Traceability web-application , and need to be adhered by the QDAS control plan schema, to maintain the integrity of the solution : 
1. Each control plan created should have mandatorily following fields : 
    - K1001 : Part Number 
    - K1086 : Operation Number 
    - K1102 : Line Name 
1. A unique value in K1001 should be used to describe a combination of unique part type for a unique model 
1. K1086 should contain digits (0-9) within them to describe the operation stage. These numbers are used to identify the sequence of each operation within the given line 
2. To use K0055 to store the unique serail number / identification number (UID) of any component. It should capture the whole UID and not any of the partial value 
3. For measurement values, use K0008 to store the operator responsible and K0010 to store the machine involved in manufacutring/ inspecting certain component 
4. Traceability Web-Application assumes following mannerism in data storage within Qdas database: 
    - All characteristic's measurements are written down in database at once/ simultaneously (i.e without any practical time gap) involved within a specific operation stage
    - All characteristic's measurements will be tightly coupled to have the same values for these operation parameters such as K0008, K0010 & timestamp 