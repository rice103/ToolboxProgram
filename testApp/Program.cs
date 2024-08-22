using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ToolboxProgram
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayMinimizerForm());
        }
    }

    public class Settings
    {
        public String parametro1 = "valore di default";
    }

    static class SettingsManager
    {
        public static String settingsRootPath = "./";
        public static Settings language;

        static void init()
        {
            String languageRead = "";
            language = new Settings();
            System.Xml.Serialization.XmlSerializer serializzatore = new System.Xml.Serialization.XmlSerializer(language.GetType());
            if (File.Exists(Path.Combine(settingsRootPath, "default.xml")))
            {
                object o = serializzatore.Deserialize(new StreamReader(settingsRootPath + "default.xml"));
                language = (Settings)o;
                languageRead = File.ReadAllText(settingsRootPath + "default.xml");
            }
            else
            {
                serializzatore.Serialize(new StreamWriter(settingsRootPath + "default.xml"), language);
            }
        }

        private static bool readingRecipe = false;
        public static String change(String nomeRicetta, ref String languageRead)
        {
            String ret = "";
            System.Xml.Serialization.XmlSerializer serializzatore = new System.Xml.Serialization.XmlSerializer(SettingsManager.language.GetType());
            readingRecipe = false;

            //serializzatore.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            //serializzatore.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            serializzatore.UnknownAttribute += Serializzatore_UnknownAttribute;
            serializzatore.UnknownElement += Serializzatore_UnknownElement;
            serializzatore.UnknownNode += Serializzatore_UnknownNode;
            serializzatore.UnreferencedObject += Serializzatore_UnreferencedObject;

            if (File.Exists(Path.Combine(settingsRootPath, "language.xml")))
            {
                language = (Settings)serializzatore.Deserialize(new StreamReader(Path.Combine(SettingsManager.settingsRootPath, "language.xml")));
                languageRead = File.ReadAllText(settingsRootPath + "language.xml");
            }

            readingRecipe = true;

            try
            {
                if (File.Exists(Path.Combine(SettingsManager.settingsRootPath, "Ricette\\", nomeRicetta + ".xml")))
                {
                    XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine(SettingsManager.settingsRootPath, "Ricette\\", nomeRicetta + ".xml")));
                    Settings tmplanguage = (Settings)serializzatore.Deserialize(reader);
                    ret = File.ReadAllText(Path.Combine(SettingsManager.settingsRootPath, "Ricette\\", nomeRicetta + ".xml"));
                    foreach (FieldInfo field in typeof(Settings).GetFields())
                    {
                        try
                        {
                            if (ret.Contains("<" + field.Name + ">"))
                            {
                                typeof(Settings).GetField(field.Name).SetValue(language, typeof(Settings).GetField(field.Name).GetValue(tmplanguage));
                            }
                        }
                        catch (Exception pe)
                        {
                            //MessageBox.Show(pe.Message + " " + pe.InnerException);
                        }
                    }
                }
            }
            catch (Exception pe)
            {
                //MessageBox.Show(pe.Message + " " + pe.InnerException);
                
            }
            return ret;
        }

        private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            if (readingRecipe)
            {
                //QEye.Main.logInfo("Unknown Node:" + e.Name + "\t" + e.Text);
            }
        }

        private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            if (readingRecipe)
            {
                //System.Xml.XmlAttribute attr = e.Attr;
                //QEye.Main.logInfo("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
            }
        }

        private static void Serializzatore_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            if (readingRecipe)
            {
            }
            //throw new NotImplementedException();
        }

        private static void Serializzatore_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            if (readingRecipe)
            {
            }
            //throw new NotImplementedException();
        }

        private static void Serializzatore_UnknownElement(object sender, XmlElementEventArgs e)
        {
            //if (readingRecipe)
            {
            }
            //throw new NotImplementedException();
        }

        private static void Serializzatore_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            //if (readingRecipe)
            {
            }
            //throw new NotImplementedException();
        }
    }
}