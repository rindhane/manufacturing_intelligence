using DFQhandler;
using System.Collections.Generic; //to access List;
using DFQModelSet; // to access the DFQ models
using System.IO; // to acces the File class
using dataFromJson; // to access the JsonHelpers

namespace MainProgram {

    class MainProgram
    {
        void testDFQ(){
            var testModel = new DFQmodel();
                //creating test Model
                testModel.K0100 = "2";
                var partData = new DFQPartModel();
                    //partDetail
                    partData.K1001="1";
                    partData.K1002="part_name_1";
                    partData.K1102="line_detail_1";
                //char Detail_1;
                var char_detail_1 = new DFQCharacteristicModel();
                    //charDetail
                    char_detail_1.K2001="1";
                    char_detail_1.K2002="char_name_1";
                    //measurement_1
                    var test_measurement = new DFQMeasurementModel();
                        //measurement_detail 
                        test_measurement.K0001="12.36";
                        test_measurement.K0004="123";
                        test_measurement.K0007="z1";
                        test_measurement.K0014="124";
                    var test_measurement2 = new DFQMeasurementModel();
                        //measurement_detail 
                        test_measurement2.K0001="13.36";
                        test_measurement2.K0004="113";
                        test_measurement2.K0007="z11";
                        test_measurement2.K0014="122";
                    char_detail_1.measurementVals= new List<DFQMeasurementModel>();
                    char_detail_1.measurementVals!.Add(test_measurement);
                    char_detail_1.measurementVals!.Add(test_measurement2);
                //char Detail_2;
                var char_detail_2 = new DFQCharacteristicModel();
                    //charDetail
                    char_detail_2.K2001="2";
                    char_detail_2.K2002="char_name_2";
                    char_detail_2.K2900="char_description";
                    //measurement_1
                    var test_measurement3 = new DFQMeasurementModel();
                        //measurement_detail 
                        test_measurement3.K0001="10.36";
                        test_measurement3.K0004="323";
                        test_measurement3.K0007="y1";
                        test_measurement3.K0014="456";
                    var test_measurement4 = new DFQMeasurementModel();
                        //measurement_detail 
                        test_measurement4.K0001="19.36";
                        test_measurement4.K0004="016";
                        test_measurement4.K0007="932";
                        test_measurement4.K0014="124";
                    char_detail_2.measurementVals= new List<DFQMeasurementModel>();
                    char_detail_2.measurementVals!.Add(test_measurement3);
                    char_detail_2.measurementVals.Add(test_measurement4);
                partData.chars= new List<DFQCharacteristicModel>();
                partData.chars!.Add(char_detail_1);
                partData.chars.Add(char_detail_2);
                testModel.partData = new List<DFQPartModel>();
                testModel.partData.Add(partData);
            var writer = new DFQWriter_v1("fileTest.dfq");
            writer.DfqModeltoFileWrite(testModel);       
        }
        static void Main(){
            var dfqJsonString = File.ReadAllText("sample.json");
            var dfqResult = JsonHelpers.extractKfieldResultFromJson(dfqJsonString);
            var writer = new DFQWriter_v1("fileTestResult.dfq");
            writer.DfqModeltoFileWrite(dfqResult);    
    }
    }
}