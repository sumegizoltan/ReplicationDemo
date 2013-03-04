using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace DFSReplicationDemoClient
{
    /// <summary> 
    /// Common class with methods for the DFS Replication Demo 
    /// </summary>
    public class Common
    {
        #region methods
        /// <summary> 
        /// Initialization from the Options.xml parameter file 
        /// </summary>
        /// <return>Singleton instance</return>
        public static Options LoadSettings()
        {
            Options deserializedXml = default(Options);
            string filepath = String.Empty;
            string xml = String.Empty;

            try
            {
                filepath = String.Format("{0}\\{1}", 
                                         Directory.GetCurrentDirectory(), 
                                         "Options.xml");
                xml = File.ReadAllText(filepath);
                deserializedXml = DeserializeXml<Options>(xml);
            }
            catch (FileNotFoundException ex)
            {
                Common.RaiseError(null, 
                                  ex.Message, 
                                  "A konfigurációs fájl nem található.");
            }
            catch (Exception ex2)
            {
                Common.RaiseError(null,
                                  ex2.Message,
                                  "A konfigurációs fájl hibás.");
            }

            return deserializedXml;
        }

        /// <summary> 
        /// Deserialize xml 
        /// </summary>
        /// <param name="xml">string for deserialization</param>
        /// <return>Deserialized object</return>
        public static T DeserializeXml<T>(string xml)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(xml);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(bytes);

            return (T)xs.Deserialize(stream);
        }

        /// <summary>
        /// Show Info message
        /// </summary>
        /// <param name="infoMessage">info message</param>
        public static void ShowInfo(string infoMessage)
        {
            ErrorHandler errorHandler = ErrorHandler.GetErrorHandler();

            errorHandler.RaiseError(null, "", infoMessage);
        }

        /// <summary>
        /// Raise error via DFS error handling
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="errorMessage">error message</param>
        /// <param name="infoMessage">info message</param>
        public static void RaiseError(object sender, 
                                      string errorMessage, 
                                      string infoMessage = "")
        {
            ErrorHandler errorHandler = ErrorHandler.GetErrorHandler();

            errorHandler.RaiseError(sender, errorMessage, infoMessage);
        }
        #endregion
    }
}
