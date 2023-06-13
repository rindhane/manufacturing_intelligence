using Microsoft.AspNetCore.Builder; //for classType:WebApplication 
using App.RouteBindings;
using App.Configurations;

namespace App.Routings
{
public static class Routes{
    public static WebApplication AddRouting(this WebApplication app){
        var configs = new SystemConfigurations();
        //app.MapGet( "/" , RouteMethods.IndexMethod);
        //app.MapGet("/testPage", RouteMethods.testJsonData);
        //add key-link-reRerouting
        app.MapGet("/Home",RouteMethods.pageRedirect);        
        app.MapGet("/TraceSearch", RouteMethods.pageRedirect);
        app.MapGet("/LabReportDisplay", RouteMethods.pageRedirectWithSerialParams);
        app.MapGet("/DemoDashboard", RouteMethods.pageRedirect);
        app.MapGet("/MainDashboard", RouteMethods.pageRedirectWithLineParams);
        app.MapGet("/LabEntry",RouteMethods.pageRedirect);
        app.MapGet("/ManualEntry",RouteMethods.pageRedirect);
        app.MapGet("/ManualDataEntry", RouteMethods.pageRedirect);  
        app.MapGet("/LaserMarkingAuto",RouteMethods.pageRedirect);
        app.MapGet("/instructions",RouteMethods.pageRedirect);
        app.MapGet("/AutoScanEntry",RouteMethods.pageRedirect);
        app.MapGet("/AdminLogin", RouteMethods.pageRedirect);
            //
            app.MapGet("/", RouteMethods.MoveToHomeScreen);
        app.MapGet("/testStart", RouteMethods.TestStartPage);
        app.MapGet("/getAuthorization", RouteMethods.getAuthorization);
        app.MapPost("/checkAuthorization", RouteMethods.checkAuthorization);
        app.MapGet("/Jwt", RouteMethods.getjWtoken);
        app.MapPost($"/{configs.jwt_url}", RouteMethods.PostjWtoken);
        app.MapGet("/Results", RouteMethods.ResultCandidates);
        app.MapPost("/PartData",RouteMethods.PartData);
        app.MapPost("/LabData",RouteMethods.LabUpload);
        app.MapPost("/ManualScanData",RouteMethods.ManualScan);
        app.MapPost("/GetReportData",RouteMethods.PDFReportData);
        app.MapPost("/GetReportList",RouteMethods.PDFReportList);
            //add by pushparaj
           
        app.MapPost("/GetDashboardLines", RouteMethods.DashboardLines );
        app.MapPost("/GetLabStations", RouteMethods.getLabEntries );
        app.MapPost("/GetProductionLines",RouteMethods.getAllProductionLines);
        app.MapPost("/OperationInProdLine",RouteMethods.getAllOperationOfProductionLine);
        app.MapPost("/GetAllPartCodes",RouteMethods.getAllPartCodesConfigured);
        app.MapPost("/SyncControlPlans", RouteMethods.syncUpAllPartCodesConfigured);

        app.MapDelete("/DeleteReport", RouteMethods.DeletePDFReport);

            //for DFQ save from Manual Forms
            app.MapPost("/ManualFormData",RouteMethods.ManualFormData);
        return app;
    }
}  
}