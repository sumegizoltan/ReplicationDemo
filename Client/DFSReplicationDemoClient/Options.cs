using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DFSReplicationDemoClient
{
    /// <summary> class for the settings </summary>
    [SerializableAttribute()]
    [XmlRootAttribute("Options", Namespace = "", IsNullable = false)]
    public class Options : IOptions
    {
        #region declaration
        /// <summary> Field for the property Folders </summary>
        protected Folder[] folders = null;

        /// <summary> Field for the property IsIntervalEnabled </summary>
        protected bool isIntervalEnabled = true;

        /// <summary> Field for the property TimerInterval </summary>
        protected int timerInterval = 5000;
        #endregion

        #region properties
        /// <summary> 
        /// Property Folders 
        /// </summary>
        [XmlElementAttribute("Folders")]
        public Folder[] Folders
        {
            get { return this.folders; }
            set
            {
                if (this.folders != value)
                {
                    this.folders = value;
                }
            }
        }

        /// <summary> 
        /// Property IsIntervalEnabled 
        /// </summary>
        [XmlElementAttribute("IntervalEnabled")]
        public bool IsIntervalEnabled
        {
            get { return this.isIntervalEnabled; }
            set
            {
                if (this.isIntervalEnabled != value)
                {
                    this.isIntervalEnabled = value;
                }
            }
        }

        /// <summary> 
        /// Property TimerInterval 
        /// Interval for checking replication directories
        /// </summary>
        [XmlElementAttribute("TimerInterval")]
        public int TimerInterval
        {
            get { return this.timerInterval; }
            set
            {
                if (this.timerInterval != value)
                {
                    if (value > 0)
                        this.timerInterval = value;
                }
            }
        }
        #endregion

        #region methods
        /// <summary> 
        /// Save the current configuration 
        /// </summary>
        protected void SaveConfig()
        {
            try
            {
                string filepath = String.Format("{0}\\{1}", 
                                                Directory.GetCurrentDirectory(), 
                                                "Options.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(Options));
                using (XmlTextWriter writer = new XmlTextWriter(filepath, Encoding.UTF8))
                {
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                Common.RaiseError(this, ex.Message);
            }
        }
        #endregion
    }
}
