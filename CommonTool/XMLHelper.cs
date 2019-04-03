using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CommonTool
{
    /// <summary>
    /// Author : 谭振
    /// DateTime : 2017/12/8 10:52:32
    /// Mail : tanz01@haid.com.cn
    /// Description : 
    /// </summary>
    public class XMLHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(XMLHelper));

        public XMLHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region Fields and Properties

        public enum XmlType
        {
            File,
            String
        }

        //必须创建对象才能使用的类  

        private XmlDocument xmlDoc = new XmlDocument();

        private XmlNode xmlNode;
        private XmlElement xmlElem;

        #endregion



        #region 创建xml文档
        /**/
        /************************************************** 
     * 对象名称:XmlObject 
     * 功能说明:创建xml文档         
     * 使用示列: 
     *          using EC; //引用命名空间 
     *          string xmlPath = Server.MapPath("test.xml"); 
     *          XmlObject obj = new XmlObject(); 
     *          创建根节点 
     *          obj.CreateXmlRoot("root"); 
     *          // 创建空节点 
     *          //obj.CreatXmlNode("root", "Node"); 
     *          //创建一个带值的节点 
     *          //obj.CreatXmlNode("root", "Node", "带值的节点"); 
     *          //创建一个仅带属性的节点 
     *          //obj.CreatXmlNode("root", "Node", "Attribe", "属性值"); 
     *          //创建一个仅带两个属性值的节点 
     *          //obj.CreatXmlNode("root", "Node", "Attribe", "属性值", "Attribe2", "属性值2"); 
     *          //创建一个带属性值的节点值的节点 
     *          // obj.CreatXmlNode("root", "Node", "Attribe", "属性值","节点值"); 
     *          //在当前节点插入带两个属性值的节点 
     *          obj.CreatXmlNode("root", "Node", "Attribe", "属性值", "Attribe2", "属性值2","节点值"); 
     *          obj.XmlSave(xmlPath); 
     *          obj.Dispose();         
     ************************************************/


        #region 创建一个只有声明和根节点的XML文档
        /**/
        /// <summary>  
        /// 创建一个只有声明和根节点的XML文档  
        /// </summary>  
        /// <param name="root"></param>  
        public XmlDocument CreateXmlRoot(string root)
        {
            //加入一个根元素  
            xmlElem = xmlDoc.CreateElement("", root, "");
            xmlDoc.AppendChild(xmlElem);
            return xmlDoc;

        }
        #endregion


        #region 创建一个节点，并插入该节点的值
        public static XmlElement CreateXmlNode(XmlDocument xmlDocu,  string NodeName, string NodeValue)
        {
            XmlElement xmlEle = xmlDocu.CreateElement(NodeName);
            XmlText xmlTxt = xmlDocu.CreateTextNode(NodeValue);
            xmlEle.AppendChild(xmlTxt);
            return xmlEle;
        }
        #endregion

        #region 在当前节点下插入一个空节点节点
        /**/
        /// <summary>  
        /// 在当前节点下插入一个空节点节点  
        /// </summary>  
        /// <param name="mainNode">当前节点路径</param>  
        /// <param name="node">节点名称</param>  
        public void CreatXmlNode(string mainNode, string node)
        {
            XmlNode MainNode = xmlDoc.SelectSingleNode(mainNode);
            XmlElement objElem = xmlDoc.CreateElement(node);
            MainNode.AppendChild(objElem);
        }
        #endregion

        #region 在当前节点插入一个仅带值的节点
        /**/
        /// <summary>  
        ///  在当前节点插入一个仅带值的节点  
        /// </summary>  
        /// <param name="mainNode">当前节点</param>  
        /// <param name="node">新节点</param>  
        /// <param name="content">新节点值</param>  
        public void CreatXmlNode(string mainNode, string node, string content)
        {
            XmlNode MainNode = xmlDoc.SelectSingleNode(mainNode);
            XmlElement objElem = xmlDoc.CreateElement(node);
            objElem.InnerText = content;
            MainNode.AppendChild(objElem);
        }
        #endregion


        #region 保存Xml
        /**/
        /// <summary>  
        /// 保存Xml  
        /// </summary>  
        /// <param name="path">保存的当前路径</param>  
        public void XmlSave(string path)
        {
            xmlDoc.Save(path);
        }

        #endregion

        #endregion
        /// <summary>
        /// 获取xml的根节点
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static XmlNode GetRootNode(string source)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(source);
            XmlNode root = xmlDocument.DocumentElement;
            return root;
        }

        /// <summary>
        /// 获取xml节点的集合
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlNode root, string nodeName)
        {
            XmlNode node = root.SelectSingleNode(nodeName);
            return node;
        }


        /// <summary>
        /// 获取xml节点的集合
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static XmlNodeList GetNodeList(XmlNode root, string xPath)
        {
            XmlNodeList list = root.SelectNodes(xPath);
            return list;
        }

        /// <summary>
        ///     读取XML资源中的指定节点内容
        /// </summary>
        /// <param name="source">XML资源</param>
        /// <param name="xmlType">XML资源类型：文件，字符串</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns>节点内容</returns>
        public static string GetNodeValue(string source, XmlType xmlType, string nodeName)
        {
            var xmlDocument = new XmlDocument();
            if (xmlType == XmlType.File)
                xmlDocument.Load(source);
            else
                xmlDocument.LoadXml(source);
            var documentElement = xmlDocument.DocumentElement;
            var selectSingleNode = documentElement.SelectSingleNode(nodeName);
            return selectSingleNode.InnerText;
        }

        /// <summary>
        ///     读取XML资源中的指定节点内容
        /// </summary>
        /// <param name="root">xml根节点</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns>节点内容</returns>
        public static string GetNodeValue(XmlNode root, string nodeName)
        {
            var documentElement = root;
            var selectSingleNode = documentElement.SelectSingleNode(nodeName);
            if (selectSingleNode == null)
            {
                return "";
            }
            return selectSingleNode.InnerText;
        }

        /// <summary>
        /// 获取唯一XML路径的值
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static string GetXmlNodeText(System.Xml.XmlDocument xdoc, string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
                return "";
            try
            {
                System.Xml.XmlNode xnode = xdoc.SelectSingleNode(xpath);
                if (xnode == null || xnode == default(System.Xml.XmlNode))
                    return "";
                return xnode.InnerText;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 获取XmlNode的值
        /// </summary>
        /// <param name="xnode"></param>
        /// <returns></returns>
        public static string GetXmlNodeText(System.Xml.XmlNode xnode)
        {
            if (xnode == null || xnode == default(System.Xml.XmlNode))
                return "";
            return xnode.InnerText;
        }

        /// <summary>
        /// 获取XmlNode的属性值
        /// </summary>
        /// <param name="xnode"></param>
        /// <param name="attrname"></param>
        /// <returns></returns>
        public static string GetXmlNodeAttributeValue(System.Xml.XmlNode xnode, string attrname)
        {
            if (string.IsNullOrEmpty(attrname))
                return "";
            if (xnode == null || xnode == default(System.Xml.XmlNode))
                return "";
            if (xnode.Attributes.Count == 0)
                return "";
            System.Xml.XmlAttribute attr = xnode.Attributes[attrname];
            if (attr == null || attr == default(System.Xml.XmlAttribute))
                return "";
            return attr.Value;
        }


    }
}
