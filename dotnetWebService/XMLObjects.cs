using System.Xml.Serialization;
using XMLObjects;


namespace helpers {
public static partial class XMLHelpers {
    public static string XMLTestStringProvider()
    {
      UserDetails user = new UserDetails{Name="ABC", Surname="DEF"};
      Secret secret= new Secret{auth="GHI"};
      Payload payload = new Payload{userDetails=user,secret=secret};
      string result = XMLTestStringPrepaper(payload);
      return result;
    }
}
}



namespace XMLObjects{

    [XmlRoot("payload")]
    public class Payload {
        [XmlElement(ElementName ="UserDetails")]
        public UserDetails userDetails {get; set;} 
        [XmlElement(ElementName ="Secret")]
        public Secret secret {get;set;}

        public override string ToString()
            {
                return userDetails.Name+":"+userDetails.Surname+":"+secret.auth;
            }
 
    }

    [XmlRoot("UserDetails")]
    public class UserDetails {

        [XmlElement(ElementName = "Name")]
        public string Name{get; set;}

        [XmlElement(ElementName = "Surname")]
        public string Surname{get;set;}

    }

    [XmlRoot("Secret")]
    public class Secret {
        [XmlElement(ElementName ="Auth-Code")]
        public string auth{get;set;}
    }
}