using Newtonsoft.Json;
using DFQJSON.Models;
using DFQModelSet;
namespace DFQJSON.helpers {    
    class JsonHelpers{
        public static DFQmodel extractDFQResultFromStdJsonString(string dfqJsonString){
            var result = JsonConvert.DeserializeObject<DFQJSONModel>(dfqJsonString,
                        new JsonSerializerSettings
                                                {
                                                    NullValueHandling = NullValueHandling.Ignore,
                                                    
                                                });

            //System.Console.WriteLine(JsonConvert.SerializeObject(result,Formatting.Indented));
            return extractDFQResultFromStdJson(result!);
        }
        public static DFQmodel extractDFQResultFromStdJson(DFQJSONModel dat){
            return (DFQmodel)dat;
        }
    }
}