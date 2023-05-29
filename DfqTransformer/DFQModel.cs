using System.Collections.Generic;

namespace DFQModelSet{
    public class DFQmodel{
        public string? K0100 {get;set;} // no. of characteristics. it cannot be empty. it is essential

        public List<DFQPartModel>? partData {get;set;}
    }

    public class DFQPartModel{
        public string? K1001 {get;set;} // part_code // number for part type
        public string? K1002 {get;set;} // name // part description  
        public string? K1008 {get;set;} //model 
        public string? K1041 {get;set;} // drawing_no
        public string? K1102 {get;set;} //line // 
        public string? K1086 {get;set;} //OpCode
        
        public List<DFQCharacteristicModel>? chars{get;set;} // child characteristics
    }

    public class DFQCharacteristicModel {
        public string? K2001 {get;set;}//id // characteristic id
        public string? K2002 {get;set;}//name // characteristic name 
        public string? K2900 {get;set;}//charRemarks // characteristic remarks
        public List<DFQMeasurementModel>? measurementVals{get;set;} // child measurements done for the char 

    }

    public class DFQMeasurementModel{
        public string? K0001{get;set;}//observed_value
        public string? K0004{get;set;}//dateTime format : "08-02-2023 00:00:00"
        public string? K0007 {get;set;}//tool_no
        public string? K0009 {get;set;}//text
        public string? K0014 {get;set;}//component_id // UID for serial number of measured_component
        public string? K0053 {get;set;}// shift
        public string? K0056 {get;set;} //operator_comment
    }
}