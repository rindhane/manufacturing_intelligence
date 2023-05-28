using Newtonsoft.Json;
using DFQJSONModels;
using DFQModelSet;
namespace dataFromJson {    
    class JsonHelpers{
        public static DFQmodel extractKfieldResultFromJson(string dfqJsonString){
            var result = JsonConvert.DeserializeObject<DFQJSON>(dfqJsonString,
                        new JsonSerializerSettings
                                                {
                                                    NullValueHandling = NullValueHandling.Ignore,
                                                    
                                                });

            //System.Console.WriteLine(JsonConvert.SerializeObject(result,Formatting.Indented));
            return (DFQmodel)result!;
        }
    }
}