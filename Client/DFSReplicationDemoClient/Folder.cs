using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DFSReplicationDemoClient
{
    /// <summary> 
    /// class for the settings - Folder item
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute("Folders", Namespace = "", IsNullable = false)]
    public class Folder
    {
        #region declaration
        /// <summary> Field for the property Destination </summary>
        protected string destination = String.Empty;

        /// <summary> Field for the property FileMask </summary>
        protected string fileMask = String.Empty;

        /// <summary> Field for the property Source </summary>
        protected string source = String.Empty;
        #endregion

        #region properties
        /// <summary> 
        /// Property Destination 
        /// </summary>
        [XmlAttribute("Destination")]
        public string Destination
        {
            get { return this.destination; }
            set
            {
                if (this.destination != value)
                {
                    this.destination = value;
                }
            }
        }

        /// <summary> 
        /// Property FileMask 
        /// </summary>
        [XmlAttribute("FileMask")]
        public string FileMask
        {
            get { return this.fileMask; }
            set
            {
                if (this.fileMask != value)
                {
                    this.fileMask = value;
                }
            }
        }

        /// <summary> 
        /// Property Source 
        /// </summary>
        [XmlAttribute("Source")]
        public string Source
        {
            get { return this.source; }
            set
            {
                if (this.source != value)
                {
                    this.source = value;
                }
            }
        }
        #endregion
    }
}
