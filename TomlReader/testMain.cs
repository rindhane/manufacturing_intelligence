using Tomlyn.Model;

namespace TOMLreader{
    public class testMain{
        public static void Main(){
            var reader = new tomlConfigReader("testConfig.toml");
            var temp1=reader.getKeyValue("global"); //get bare key 
            var temp2=reader.getKeyValue("key","my_table"); //get key within the table
            //(TomlTable)model["my_table"])[key]
            System.Console.WriteLine($"result1:{temp1}"); 
            System.Console.WriteLine($"result1:{temp2}");
            var result = reader.getArrayType<System.Int64>("list","my_table");
            System.Console.WriteLine(result[0]);            
        } 
    } 
}
