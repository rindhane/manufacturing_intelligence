using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace helpers{
    
  public static partial class XMLHelpers{
      
    public static string XMLTestStringPrepaper(object obj) {

      string xmlString="";
      XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
      {
          Encoding = Encoding.UTF8,
          Indent = true
      };

      var sWriter= new StringWriter();
      using (XmlWriter writer= XmlWriter.Create(sWriter,xmlWriterSettings)){
          XmlSerializer serializer = new XmlSerializer(obj.GetType());
          serializer.Serialize(writer,obj);
          xmlString = sWriter.ToString();
          sWriter.Close();
      }
      return xmlString;
}

    public static async Task<T> XMLDesrializeStream<T>(Stream str){
      //function to pack a object from the http stream
      XmlSerializer serializer = new XmlSerializer(typeof(T));
      serializer.UnknownElement+=new XmlElementEventHandler(ElementExceptionToConsole);
      serializer.UnknownAttribute+=new XmlAttributeEventHandler(AttributeExceptionToConsole);
      serializer.UnknownNode+= new XmlNodeEventHandler(NodeExceptionToConsole);
      serializer.UnreferencedObject+=new UnreferencedObjectEventHandler(UnreferencedObjectToConsole);
      var reader = new StreamReader(str);
      // since can't find async option in xmlSerializer
      string tempString = await reader.ReadToEndAsync();
      System.Console.WriteLine(tempString); //delete this line ;
      var StringStream = new StringReader(tempString);
      //conversting stringStream to xmlobject
      var result = (T) serializer.Deserialize(StringStream); 
      return result; 
    }
  public static void ElementExceptionToConsole(object sender, XmlElementEventArgs elem){
      System.Console.WriteLine("Element Exception Released");
      System.Console.WriteLine("\t" + elem.Element.Name + " " + elem.Element.InnerXml);
      System.Console.WriteLine("\t LineNumber: " + elem.LineNumber);
      System.Console.WriteLine("\t LinePosition:" + elem.LinePosition);
      System.Console.WriteLine("UnknownNode Name: {0}", elem);
  }
  public static void NodeExceptionToConsole(object sender, XmlNodeEventArgs e){
      System.Console.WriteLine("Node Exception Released");
      System.Console.WriteLine
      ("UnknownNode Name: {0}", e.Name);
      System.Console.WriteLine
      ("UnknownNode LocalName: {0}" ,e.LocalName);
      System.Console.WriteLine
      ("UnknownNode Namespace URI: {0}", e.NamespaceURI);
      System.Console.WriteLine
      ("UnknownNode Text: {0}", e.Text);
      XmlNodeType myNodeType = e.NodeType;
      System.Console.WriteLine("NodeType: {0}", myNodeType);
  }
  public static void UnreferencedObjectToConsole(object sender, UnreferencedObjectEventArgs e){
    System.Console.WriteLine("Unreference Object Exception Released");
    System.Console.WriteLine("UnreferencedObject:");
    System.Console.WriteLine("ID: " + e.UnreferencedId);
    System.Console.WriteLine("UnreferencedObject: " + e.UnreferencedObject);
  }
  public static void AttributeExceptionToConsole(object sender, XmlAttributeEventArgs e){
    System.Console.WriteLine("Attribute Exception Released");
    System.Console.WriteLine("Unknown Attribute");
    System.Console.WriteLine("\t" + e.Attr.Name + " " + e.Attr.InnerXml);
    System.Console.WriteLine("\t LineNumber: " + e.LineNumber);
    System.Console.WriteLine("\t LinePosition: " + e.LinePosition);
  }

  }
}