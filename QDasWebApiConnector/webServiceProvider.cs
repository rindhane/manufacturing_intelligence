namespace qdasWebService{
    using ServiceReference;
    public class qdasServiceClient{
        
        private Qdas_Web_ServiceClient ws ; 
        private int connectionHandle;

        public qdasServiceClient(){
            ws = create_client();
            connectionHandle= initiateNewConnection().Handle;
        }

        Qdas_Web_ServiceClient create_client(){
            return new Qdas_Web_ServiceClient();
        }
        WebConnectResponse initiateNewConnection(){
            WebConnectRequest request= new WebConnectRequest(20, 44, "superuser", "superuser", "");
            return ws.WebConnectAsync(request).GetAwaiter().GetResult();
        }
        int closeConnection(){
            _ = ws.ClientDisconnectAsync(connectionHandle).GetAwaiter().GetResult();
            return 1;
        }

        public void getPPKResult()
        {
            var partID=1;
            var charID=1;
            //string partListStr = "<Part key = '" + partID + "'/>";
            string partListStr = "<Part key = " +'"'+ partID + '"'+">"+
                                    "<Char key= " +'"'+ charID+ '"' + "/>"+
                                 "</Part>";
            var req1 = new CreateQueryRequest(connectionHandle);
            System.Console.WriteLine("responseHandle:"+connectionHandle);
            string partListXML = "<PartCharList>" + partListStr + "</PartCharList>";
            System.Console.WriteLine(partListXML);
            var response1= ws.CreateQueryAsync(req1).GetAwaiter().GetResult();
            System.Console.WriteLine("queryCreation:"+response1.Result);
            System.Console.WriteLine("queryHandle"+response1.QueryHandle);
            var filterRequest = new CreateFilterRequest(connectionHandle,0,1,"125",129);
            var filterCreation = ws.CreateFilterAsync(filterRequest).GetAwaiter().GetResult();
            System.Console.WriteLine("filterHandle:"+filterCreation.FilterHandle);
            System.Console.WriteLine("filterCreation:"+filterCreation.Result);
            var filterAddResult = ws.AddFilterToQueryAsync(connectionHandle,response1.QueryHandle,
            filterCreation.FilterHandle,2,0,0).GetAwaiter().GetResult();
            System.Console.WriteLine("filterAddition"+filterAddResult.Result);
            var result = ws.ExecuteQuery_ExtAsync(connectionHandle,response1.QueryHandle,partListXML,true,true).GetAwaiter().GetResult();
            var evaluateResult = ws.EvaluateAllCharsAsync(connectionHandle).GetAwaiter().GetResult().Result.ToString();
            System.Console.WriteLine("eval"+evaluateResult);
            var datRequest = new GetGlobalInfoRequest(connectionHandle, 0, 0, 1);
            var check1 = ws.GetGlobalInfoAsync(datRequest).GetAwaiter().GetResult().Result.ToString();
            System.Console.WriteLine("check1:"+check1);
            System.Console.WriteLine("result:"+ result.Result.ToString());
            ws.FreeFilterAsync(connectionHandle,filterCreation.FilterHandle);
            ws.FreeQueryAsync(connectionHandle,response1.QueryHandle);           
            var closeResult = closeConnection();
            System.Threading.Thread.Sleep(3000);
        }

         public void getResult123()
        {
            //var input1 = new GetFirstModuleRequest(connectionHandle);
            //var result1= ws.GetFirstModuleAsync(input1).GetAwaiter().GetResult();
            var testString = "SELECT TETEIL, TETEILNR FROM TEIL;";
            //var testString= "<SQL sqlString=\"SELECT TETEIL, TETEILNR FROM TEIL\"><Column colNr=\"1\">TETEIL</Column><Column colNr=\"2\">TETEILNR</Column></SQL>";
            System.Console.WriteLine(testString);
            var request1=new CreateDirectSQLRequest(connectionHandle);
            var response1= ws.CreateDirectSQLAsync(request1).GetAwaiter().GetResult();
            System.Console.WriteLine(response1.Result+"|"+response1.SQLHandle);
            //var request2=new ExecuteQueryRequest(connectionHandle,response1.SQLHandle,)
            var request2=new ExecuteDirectSQLRequest(connectionHandle, response1.SQLHandle,testString);
            var result1 = ws.ExecuteDirectSQLAsync(request2).GetAwaiter().GetResult();
            System.Console.WriteLine(result1.FieldList);
            System.Console.WriteLine(result1.Result);
            var datRequest = new GetGlobalInfoRequest(connectionHandle, 1, 1, 1);
            var check1 = ws.GetGlobalInfoAsync(datRequest).GetAwaiter().GetResult().Result.ToString();
            System.Console.WriteLine("parts"+check1);
            
            ws.FreeDirectSQLAsync(connectionHandle,response1.SQLHandle);
            System.Console.WriteLine(closeConnection());
        }

        public void getResultPPKNew(){
            var req1 = new CreateQueryRequest(connectionHandle);
            var response1= ws.CreateQueryAsync(req1).GetAwaiter().GetResult();
            System.Console.WriteLine("queryCreation:"+response1.Result);
            System.Console.WriteLine("queryHandle"+response1.QueryHandle);
            var filterRequest = new CreateFilterRequest(connectionHandle,1,0,"6",0);
            var filterCreation = ws.CreateFilterAsync(filterRequest).GetAwaiter().GetResult();
            System.Console.WriteLine("filterHandle:"+filterCreation.FilterHandle);
            System.Console.WriteLine("filterCreation:"+filterCreation.Result);
            var filterAddResult = ws.AddFilterToQueryAsync(connectionHandle,response1.QueryHandle,filterCreation.FilterHandle,2,0,0).GetAwaiter().GetResult();
            System.Console.WriteLine("filterAddition" + filterAddResult.Result );
            var executeQueryResponse = ws.ExecuteQueryAsync(connectionHandle,response1.QueryHandle,"").GetAwaiter().GetResult();
            System.Console.WriteLine("queryExecute:" + executeQueryResponse.Result);
            var partRequestQuery = new GetGlobalInfoRequest(connectionHandle, 0, 0, 1);
            var globalInfoResponse =   ws.GetGlobalInfoAsync(partRequestQuery).GetAwaiter().GetResult();
            System.Console.WriteLine("partQueryRequest" + globalInfoResponse.ret, globalInfoResponse.Result);
            var evaluateResult = ws.EvaluateAllCharsAsync(connectionHandle).GetAwaiter().GetResult().Result.ToString();
            System.Console.WriteLine("eval"+evaluateResult);
            ws.FreeQueryAsync(connectionHandle,response1.QueryHandle);
            var evalInfoResponse =   ws.GetGlobalInfoAsync(
                    new GetGlobalInfoRequest(connectionHandle, 0, 0, 1))
                    .GetAwaiter().GetResult();
            System.Console.WriteLine("partQueryRequest" + evalInfoResponse.ret, evalInfoResponse.Result);
            var result1 = ws.GetStatResultAsync(new GetStatResultRequest(connectionHandle,5210,1,1,0))
            .GetAwaiter().GetResult();
            var result2 = ws.GetStatResultAsync(new GetStatResultRequest(connectionHandle,5220,1,1,0))
            .GetAwaiter().GetResult();
            var result3 = ws.GetStatResultAsync(new GetStatResultRequest(connectionHandle,5472,1,1,0))
            .GetAwaiter().GetResult();
            var result4 = ws.GetStatResultAsync(new GetStatResultRequest(connectionHandle,5450,1,1,0))
            .GetAwaiter().GetResult();
            var result5 = ws.GetStatResultAsync(new GetStatResultRequest(connectionHandle,5460,1,1,0))
            .GetAwaiter().GetResult();
            System.Console.WriteLine(result1.StatResult_str+"|"+result1.Result+"|"+result1.StatResult_dbl);
            System.Console.WriteLine(result2.StatResult_str+"|"+result2.Result+"|"+result2.StatResult_dbl);
            System.Console.WriteLine(result3.StatResult_str+"|"+result3.Result+"|"+result3.StatResult_dbl);
            System.Console.WriteLine(result4.StatResult_str+"|"+result4.Result+"|"+result4.StatResult_dbl);
            System.Console.WriteLine(result5.StatResult_str+"|"+result5.Result+"|"+result5.StatResult_dbl);
            closeConnection();
        }

    }
       
}