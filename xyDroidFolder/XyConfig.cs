using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace xyDroidFolder
{
    public static class XyConfig
    {
        private static string rootName = "xConfig";
        public static string SignleParsNodeName = "SignlePars";

        private static string fullName = "xyApp.config";
        private static string appDataPath = "config";
        private static string fullConfigFileName = Path.Combine(appDataPath, fullName);

        private static XmlDocument getXyConfigDoc()
        {
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            XmlDocument myDoc = new XmlDocument();

            if (File.Exists(fullConfigFileName))
            {
                myDoc.Load(fullConfigFileName);
            }
            else
            {
                XmlElement newNode = myDoc.CreateElement(rootName);
                myDoc.AppendChild(newNode);
            }

            return myDoc;
        }
        private static XmlElement creatNewNode(XmlNode pNode, string nodeName)
        {
            XmlElement newNode = pNode.OwnerDocument.CreateElement(nodeName);
            pNode.AppendChild(newNode);
            return newNode;
        }
        private static XmlElement get1LevelNode(string nName)
        {
            XmlDocument doc = getXyConfigDoc();
            if (doc.GetElementsByTagName(nName).Count == 0)
            {
                creatNewNode(doc.DocumentElement, nName);
            }
            return doc.GetElementsByTagName(nName)[0] as XmlElement; ;
        }

        private static XmlElement getSignleParsNode()
        {
            return get1LevelNode(SignleParsNodeName);
        }

        public static string? getOnePar(string parName)
        {
            XmlElement xe = getSignleParsNode();

            string retValue = null;

            if (xe.Attributes[parName] != null)
            {
                retValue = xe.GetAttribute(parName);
            }

            return retValue;
        }

        public static void setOnePar(string parName, string parValue)
        {
            XmlElement xe = getSignleParsNode();
            xe.SetAttribute(parName, parValue);
            xe.OwnerDocument.Save(fullConfigFileName);
        }
    }
}
