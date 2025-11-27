using System.IO;
using System.Windows;
using System.Windows.Automation;
using System.Xml;

namespace CBReader.Services;

public class FilePersistanceService
{
    public string? LibraryFilePath { get; set; }
    public void InitialiseConfigFile()
    {
        if (!File.Exists("config.xml"))
        {
            CreateXMLConfig();      // creates the file
            XmlDocument doc = new XmlDocument();
            doc.Load(@"config.xml");    // loads it


            XmlNode path = doc.SelectSingleNode("configuration/path");
            path.InnerText = "";
            doc.Save(@"config.xml");
            LibraryFilePath = path.InnerText;       // "" at first
        }
        else
        {
            LibraryFilePath = ReadLibraryPathFromConfig();      // reads from the config once and sets the value of the property
        }
    }

    public void CreateXMLConfig()
    {
        XmlDocument config = new XmlDocument();
        XmlElement root = config.CreateElement("configuration");
        config.AppendChild(root);

        root.SelectSingleNode("configuration");
        XmlElement path = config.CreateElement("path");
        root.AppendChild(path);

        config.Save(@"config.xml");
    }

    public void SavePathToConfig(string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"config.xml");

        XmlNode? filePath = doc.SelectSingleNode("configuration/path");
        if (path != null)
        {
            filePath.InnerText = path;
            LibraryFilePath = path;     // property
        }
        doc.Save(@"config.xml");
    }

    public string GetLibraryPath()
    {
        return LibraryFilePath ?? "";
    }

    public string ReadLibraryPathFromConfig()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"config.xml");

        XmlNode? node = doc.SelectSingleNode("configuration/path");
        return node?.InnerText ?? "";
    }

}
