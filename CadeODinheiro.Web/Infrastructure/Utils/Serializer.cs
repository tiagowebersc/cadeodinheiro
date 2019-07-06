using CadeODinheiro.Core.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace CadeODinheiro.Web.Infrastructure.Utils
{
    public class Serializer
    {
        public static string SerializeAuthModel(AuthModel authModel)
        {
            var serializer = new XmlSerializer(typeof(AuthModel));
            var writer = new StringWriter();
            var xWriter = XmlWriter.Create(writer);
            serializer.Serialize(xWriter, authModel);
            var authModelSerialized = writer.ToString();
            return authModelSerialized;
        }

        public static AuthModel UnserializeAuthModel(string authModelSerialized)
        {
            var serializer = new XmlSerializer(typeof(AuthModel));
            var authModel = (AuthModel)serializer.Deserialize(new StringReader(authModelSerialized));
            return authModel;
        }
    }
}