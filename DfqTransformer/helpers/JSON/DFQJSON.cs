using System.Collections.Generic;
using DFQModelSet;

namespace DFQJSON.Models{
    public class PartDetailModel {
        public string? part_code {get;set;}
        public string? name {get;set;}
        public string? drawing_no {get;set;}
        public string? model {get;set;}
        public string? line {get;set;}
        public string? OPCode {get;set;}
        public static implicit operator DFQPartModel( PartDetailModel partData) => new DFQPartModel {
            K1001=  partData. part_code,
            K1002 = partData.name,
            K1041 = partData.drawing_no,
            K1008 = partData.model,
            K1102 = partData.line,
            K1086 = partData.OPCode, 
        };

    }

    public class characteristics_measured_valueModel {
        public string? id {get;set;}
        public string? name {get;set;}
        public string? charRemarks {get;set;}
        public List<measured_valuesModel>? measured_values {get;set;}
        public static implicit operator DFQCharacteristicModel( characteristics_measured_valueModel charMeasured) => new DFQCharacteristicModel {
            K2001 = charMeasured.id,
            K2002 = charMeasured.name,
            K2900 = charMeasured.charRemarks,
            measurementVals = convertDFQfromJSONMeasurements(charMeasured.measured_values!),  
        };
        static List<DFQMeasurementModel> convertDFQfromJSONMeasurements(List<measured_valuesModel> measurementInputs){
            if(measurementInputs is not null){
                var result = new List<DFQMeasurementModel>();
                foreach(var item in measurementInputs){
                    result.Add((DFQMeasurementModel)item);
                }
                return result;
            }
            return new List<DFQMeasurementModel>();
        }
    }
    
    public class measured_valuesModel{
        public string? sequence_no {get;set;}
        public string? observed_value  {get;set;}
        public string? dateTime {get;set;}
        public string? component_id{get;set;}
        public string? cavity_no{get;set;}
        public string? shift_production{get;set;}

        public string? operator_comment {get;set;}

        public string? text {get;set;}
        public static implicit operator DFQMeasurementModel( measured_valuesModel measurement) => new DFQMeasurementModel {
            K0001= measurement.observed_value,
            K0004 = measurement.dateTime,
            K0009 = measurement.text,
            K0007 = measurement.cavity_no,
            K0014 = measurement.component_id,
            K0053 = measurement.shift_production,
            K0056 = measurement.operator_comment,
        };
        
    }
    
    public class DFQJSONModel {
        public PartDetailModel? part_detail {get;set;}       
        public List<characteristics_measured_valueModel> ? characteristics_measured_values {get;set;}

        public static implicit operator DFQmodel(DFQJSONModel jsonData) => new DFQmodel{
            partData = produceDFQPartListFromJSON(jsonData),
            K0100 = jsonData.characteristics_measured_values!.Count.ToString()
        };
        static List<DFQCharacteristicModel> DFQCharModelFromJSONModel (List<characteristics_measured_valueModel> charMeasuredVals){
            if(charMeasuredVals is not null){
                var result = new List<DFQCharacteristicModel>();
                foreach (var item in charMeasuredVals){
                    result.Add((DFQCharacteristicModel)item);
                }
                return result;
            }
            return new List<DFQCharacteristicModel>();
        }
        static List<DFQPartModel> produceDFQPartListFromJSON(DFQJSONModel JsonModel){
            var chars = DFQCharModelFromJSONModel(JsonModel.characteristics_measured_values!);
            var result = new List<DFQPartModel>{
                (DFQPartModel)JsonModel.part_detail!
            };
            result[0].chars=chars;
            return result;
        }
    }
}