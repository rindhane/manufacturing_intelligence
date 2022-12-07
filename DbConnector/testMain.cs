
namespace DbConnectors
{
    public class testMain {
        public static void Main(){
            //DbConnection test1 = new DbConnection();
            //test1.TrySQLquery("SELECT name, collation_name, database_id FROM sys.databases", 3);
            dbOptions opt = new dbOptions {
                dataSource = "127.0.0.1",  
                userID = "qdas",            
                password = "qdas1234",     
                dbName="QDAS_VALUE_DATABASE"
            };
            string reportTable= "LABREPORT";
            QDasDbConnection test1 = new QDasDbConnection(opt,reportTable);
            //test1.TrySQLquery("SELECT name, collation_name, database_id FROM sys.databases", 3);
            //test1.getOperationFlowforPartCategory(4567821.ToString());
            //test1.updatePartOperationFlow(4567821.ToString());
            //test1.getCharacteristicSpecs(4567821.ToString(), "OP50", "C1");
            test1.GetAllInspectionOperationParams("201020228");
        }
    }       
}