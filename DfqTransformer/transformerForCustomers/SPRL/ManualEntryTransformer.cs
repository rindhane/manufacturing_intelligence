using System.Collections.Generic;
using DFQJSON.Models;
using Newtonsoft.Json;
//using DFQModelSet;

namespace SPRLTransformers
{
    class inputResponseJson{
        public string? serialNum {get;set;}
        public string? stElem     {get;set;}
        public string? modelElem {get;set;}
        public string? DrawingNumberElem  {get;set;}
        public List<inputCharacteristicComponent>? characteristics {get;set;}
    }

    class inputCharacteristicComponent{
            public string? characteristicsSerialNum  {get;set;} 
            public string? characteristicsName   {get;set;}
            public string? obsElem   {get;set;}
            public string? JudgementElem {get;set;}
            public string? OperatorcommentsElem  {get;set;}
    }
    
    
    class ManualEntryJsonTransformer{
        
        public static PartDetailModel GetPartDetailModelFromInputResponse(inputResponseJson dat){
            var result_partDetail = new PartDetailModel();
            result_partDetail.drawing_no= dat.DrawingNumberElem;
            result_partDetail.model=dat.modelElem;
            result_partDetail.drawing_no= dat.stElem;
            result_partDetail.OPCode = dat.stElem;
            return result_partDetail;
        }
        public static characteristics_measured_valueModel getChar_MeasurementFromCharData (inputCharacteristicComponent charData){
            var result = new characteristics_measured_valueModel();
            result.name = charData.characteristicsName;
            result.id = charData.characteristicsSerialNum;
            result.measured_values = new List<measured_valuesModel>();
            result.measured_values.Add(new measured_valuesModel());
            result.measured_values[0].sequence_no = 1.ToString();
            result.measured_values[0].observed_value = charData.obsElem;
            result.measured_values[0].operator_comment =charData.OperatorcommentsElem;
            result.measured_values[0].text = charData.JudgementElem;
            return result; 
        }
        public static DFQJSONModel validDFQJsonFromInputResponse( string inputJsonString){
            inputResponseJson dat = JsonConvert.DeserializeObject<inputResponseJson>(
                                inputJsonString, 
                                new JsonSerializerSettings {
                                                    NullValueHandling = NullValueHandling.Ignore,
                                                    })!;
            var result = new DFQJSONModel();
            result.part_detail = GetPartDetailModelFromInputResponse(dat); // putting part_detail in place           
            // retreiving characteristics and measured data and placing input in the format
                result.characteristics_measured_values= new List<characteristics_measured_valueModel>();
                if(dat.characteristics is not null ){
                    foreach (var charData in dat.characteristics)
                    {
                        var transformedResult = getChar_MeasurementFromCharData(charData);
                        transformedResult.measured_values![0].component_id = dat.serialNum;
                        result.characteristics_measured_values.Add(transformedResult);
                    }
                }
            //characteristic & measurement retreival done
            return result; 
        }
    }
                    
}