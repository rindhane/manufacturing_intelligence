using Microsoft.AspNetCore.Http; //to Use Results,IResult Type
using System.Threading.Tasks; 
using System.Net.Http;
using System.Collections.Generic;
using helpers; //for randomGenerators
using XMLObjects;
using JWT;
using BackendStoreManager;//for authKeeper &TokenManager access
using FileHandler;// to access the result file of candidates
using Microsoft.AspNetCore.Mvc; 
using App.Configurations; // to access the configurations
using PartDataManager; // to access the jsonData Stored in BackendDatabase
using RequestResponseHandlers; //to read the body stream;
using System.IO;
//using pdfFileReader; // to access the test pdf file reader;
using Microsoft.AspNetCore.Hosting; // to access the IWebHostEnvironment

namespace App.RouteBindings
{
public static class RouteMethods{
  public static IResult IndexMethod(){
    return Results.LocalRedirect("~/index.html",false,true);
  }
  public static IResult pageRedirect(HttpRequest request){
    return Results.LocalRedirect($"~{request.Path}/index.html",false,true);
  }
  public static IResult pageRedirectWithLineParams(HttpRequest request){
    var param="Line";
    var val = request.Query[$"{param}"];
    return Results.LocalRedirect($"~{request.Path}/index.html?Line={val}",false,true);
  }
  public static IResult pageRedirectWithSerialParams(HttpRequest request){
    var param="serialNum";
    var val = request.Query[$"{param}"];
    return Results.LocalRedirect($"~{request.Path}/index.html?{param}={val}",false,true);
  }

  public static IResult MoveToHomeScreen(HttpRequest request){
    return Results.LocalRedirect("~/Home",false,true);
  }
  public static async Task testJsonData(HttpContext context)
  {
    HttpClient client = new HttpClient { BaseAddress = new System.Uri("https://jsonplaceholder.typicode.com/") };
    string posts = await client.GetStringAsync("posts");
    await context.Response.WriteAsync(posts);
    client.Dispose();
    //var posts= await client.GetStreamAsync("posts");
    //return Results.Stream(posts,"application/json");
    }
  public static async Task TestStartPage(HttpContext context, authKeeper keeper)
  {
    var tempAuth = RandomGenerators.RandomStringProvider(10);
    context.Response.Cookies.Append("Flag1",tempAuth);
    await context.Response.WriteAsync("cookie updated");
    keeper.initiateTempAuth(tempAuth,null);
    System.Console.WriteLine($"Cookie Released>> Flag1:{tempAuth}"); //delete this line
    return ;      
  }

  public static async Task getAuthorization(HttpContext context)
  {
    foreach(KeyValuePair<string, string> val in context.Request.Cookies ) {
      System.Console.WriteLine($"{val.Key};{val.Value}");
    }
    string test = XMLHelpers.XMLTestStringProvider();
    //System.Console.WriteLine(test);
    await context.Response.WriteAsync(test);      
  }

  public static async Task checkAuthorization(HttpContext context,HttpRequest request, 
                                                              TokenManager manager,
                                                              authKeeper keeper, 
                                                              runTimeConfiguration configs){
    string contentType= request.Headers["content-type"]!;
    if (contentType.ToLower().Equals("application/xml;charset=utf-8"))
    {
      string cooky = request.Cookies["Flag1"]!;
      System.Console.WriteLine($"Cookie Received:{cooky}"); //delete this line;
      try
      {
        Payload test = await XMLHelpers.XMLDesrializeStream<Payload>(request.Body);
        System.Console.WriteLine($" XML-Values Received >> {test.ToString()}"); //delete this line 
        if (keeper.checkAuthExist(cooky) && keeper.checkAuthExist(test.secret.auth)) {
          var handler = manager.handler;
          handler.payloadBuilder.name=test.userDetails.Name;
          handler.payloadBuilder.subject="auth1";
          handler.payloadBuilder.surname=test.userDetails.Surname;
          var token = handler.generateToken();
          var signedResult= handler.JWTSigner(token);
          context.Response.Headers.Append("JWT", signedResult);
          //for redirection
          context.Response.StatusCode = 307;
          //Location Header set
          context.Response.Headers.Append("Location", $"/{configs.jwt_url}"); 
          //end result 
          await context.Response.WriteAsync("authorized");
          //log the payload sent
          //System.Console.WriteLine("originalPayload:"+JsonConvert.SerializeObject(token.payload));
          System.Console.WriteLine("JWT successfully sent to client for next step");//Next Steps results passed. 
          return ;
        }
      }catch {}
    }
    await context.Response.WriteAsync("not authorized");
  }

  public static async Task getjWtoken (){
    TestHandler.HandlerTest();
    await Task.Delay(100);
  }

  [Consumes("text/plain", new[] { "text/html" })]
  public static async Task PostjWtoken (HttpContext context,HttpRequest request, 
                                        TokenManager manager, authKeeper keeper, 
                                        [FromServices]IFileHandler resultHandler){
    try{
      string result = request.Headers["JWT"]!;
      System.Console.WriteLine($"JWT received: {result}");
      var handler = manager.handler;
      var check = handler.validateStringToken(result);
      System.Console.WriteLine($"Validation of tokenSig:{check}");
      var pack = handler.UnpackTokenElementFromString(result);
      var pLoad = pack.Item2;
      var auth = pLoad.auth;
      System.Console.WriteLine($"Auth Received:{auth}");
      if (check && keeper.checkAuthExist(auth)) {
        await context.Response.WriteAsync("authorized");
        resultHandler.writeCandidates(pLoad.name,pLoad.surname);
        System.Console.WriteLine($"Register Name:{pLoad.name} Surname:{pLoad.surname}");
        return ;
      }
    }catch{}
    await context.Response.WriteAsync("Unauthorized");
    return ;
  }

  public static async Task ResultCandidates(HttpContext context, [FromServices] IFileHandler resultHandler){
    string mainTemplate="<!DOCTYPE html><html><head></head><body><h1>Candidate Results</h1><ul>$candidates$</ul></body></html>";
    var candidates = resultHandler.ReadCandidates();
    string htmlStringResult= "";
    string feed;
    foreach((string, string) candidate in candidates ) {
      feed= "<li>" + candidate.Item1+ " " + candidate.Item2 + "</li>";
      htmlStringResult +=feed;
    }
    mainTemplate=mainTemplate.Replace("$candidates$", htmlStringResult);
    await context.Response.WriteAsync(mainTemplate);
    return ;
  }

  public static async Task PartData(HttpContext context, IpartDataHandler dataManager, HttpRequest request){
    string bodyString = httpHandlers.getRequestBody(request.Body);
    string result = await dataManager.returnTraceJsonResult(bodyString);
    await context.Response.WriteAsync(result);
    return ;
  }

  public static async Task LabUpload(HttpContext context, HttpRequest request, IpartDataHandler qdasManager)
  {
    string serialNum=request.Headers["serialNum"]!;
    string stationName=request.Headers["LabStation"]!;
    var reader = new StreamReader(request.Body);
    string pdfData = await reader.ReadToEndAsync(); //it is the pdf which needs to be stored
    var stat= qdasManager.storePDF(serialNum,stationName,pdfData);
    if (stat) {
    await context.Response.WriteAsync("File was uploaded");
    return ;
    }
    await context.Response.WriteAsync("Something went wrong");
    return ;
  }
  
  public static async Task ManualScan(HttpContext context, HttpRequest request, IpartDataHandler qdasManager)
  {
    var reader = new StreamReader(request.Body);
    string tempString = await reader.ReadToEndAsync();//provides JSONstring for the Manual Scan inputs
    qdasManager.produceDfqForManualScan(tempString);
    await context.Response.WriteAsync("Scan was uploaded");
  }

  public static async Task ManualFormData(HttpContext context, HttpRequest request, IpartDataHandler qdasManager){
    var reader = new StreamReader(request.Body);
    string tempString = await reader.ReadToEndAsync();//provides JSONstring for the Manual Forms
    qdasManager.produceDfqForManualForms(tempString);
    await context.Response.WriteAsync("Scan was uploaded");
  }

  public static async Task PDFReportData(HttpContext context, HttpRequest request, IWebHostEnvironment host, IpartDataHandler qdasManager)
  {
    var reader = new StreamReader(request.Body);
    // since can't find async option in xmlSerializer
    string tempString = await reader.ReadToEndAsync();//it provides JSON with serialNum and pdf-ID to provide the string
    var result = await qdasManager.retrievePDF(tempString);
    //delete this line 
    string path = host.WebRootPath+@"\LabReportDisplay\test.pdf";
    //delete this line//var pdfReader= new PdfDataProvider(path);
    //delete this line //byte[] data = await pdfReader.ReadFileData();
    //delete this line await context.Response.Body.WriteAsync(data,0,data.Length);
    await context.Response.WriteAsync(result);
  }
  public static async Task PDFReportList(HttpContext context, HttpRequest request, IpartDataHandler qdasManager)
  {
    var reader = new StreamReader(request.Body);
    string tempString = await reader.ReadToEndAsync();//it provides JSON with serialNum 
    var temp = qdasManager.getPDFList(tempString);
    await context.Response.WriteAsync(temp);
    //await context.Response.WriteAsync("[{\"name\": \"Report-CMM\",\"id\": \"123456\"},{\"name\": \"Report-Portal\", \"id\": \"123478\"}]");
  }

  public static async Task DashboardLines(HttpContext context, IpartDataHandler qdasManager)
  {
    var result = qdasManager.provideConfiguredProdLines();
    await context.Response.WriteAsync(result);
  }

  public static async Task getLabEntries(HttpContext context, IpartDataHandler qdasManager)
  { //provide frontend the list of lab stations configured in qdasConfig file
    var result = qdasManager.getLabEntries();
    await context.Response.WriteAsync(result);
  }

  public static async Task getAllProductionLines(HttpContext context, IpartDataHandler qdasManager)
  { //provide frontend the list of productionLines configured in TEIL table of Qdas
    var result= qdasManager.getAllTheProducitonLines();
    await context.Response.WriteAsync(result);
  }

  public static async Task getAllOperationOfProductionLine(HttpContext context, IpartDataHandler qdasManager, HttpRequest request)
  { //provide frontend the list of operations(control plan) related to a production line configured in the teil table of Qdas
    var reader = new StreamReader(request.Body);
    string line = await reader.ReadToEndAsync();//get the string of line 
    var result= qdasManager.getAllTheOperationOfProducitonLine(line);
    await context.Response.WriteAsync(result);
  }

  public static async Task getAllPartCodesConfigured(HttpContext context, IpartDataHandler qdasManager){
    var result = qdasManager.getAllPartCategory();
    await context.Response.WriteAsync(result);
  }

  public static async Task syncUpAllPartCodesConfigured(HttpContext context, IpartDataHandler qdasManager){
    var result = qdasManager.updateAllPartOperationFlow();
    await context.Response.WriteAsync(result.ToString());
  }

} 
}