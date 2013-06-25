using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.NodeFactory;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web.UI;

namespace FE.Extensions
{
    public class Extensions
    {
        public static XPathNodeIterator Translate(string nodeID, string langISO)
        {

            XmlDocument xd = new XmlDocument();
            xd.LoadXml("<language></language>");
            XPathNavigator xn = xd.CreateNavigator();

            //HttpContext.Current.Request.Path;

            try
            {
                if(langISO.Contains(',')){
                    Log.Add(LogTypes.Debug, 0, "Translator detected=>"+langISO);
                    langISO=langISO.Split(',')[0];
                }

                if (langISO == "en")
                {
                    return umbraco.library.GetXmlNodeById(nodeID);
                }
                else
                {
                    Node rootNode=new Node(Convert.ToInt32(nodeID));

                    foreach (Node child in rootNode.ChildrenAsList)
                    {
                        //Log.Add(LogTypes.Custom, 0, nodeID+" Translator=>Looking for 'Translations' folder, found: "+child.Name+" "+child.NodeTypeAlias);
                        if (child.NodeTypeAlias == rootNode.NodeTypeAlias+"_TranslationFolder")
                        {
                            //Log.Add(LogTypes.Custom, 0, nodeID+" Translator=>Found Translations folder");
                            foreach (Node translation in child.ChildrenAsList)
                            {
                                //Log.Add(LogTypes.Custom, 0, nodeID+" Translator=>Looking for lang: "+langISO+" in "+translation.Name);

                                if (translation.GetProperty("language").Value == langISO)
                                {
                                    //Log.Add(LogTypes.Custom, 0, nodeID+" Translator=>found: " + langISO);
                                    return umbraco.library.GetXmlNodeById(translation.Id.ToString());
                                }
                            }
                            //return english if no translation 
                            //return umbraco.library.GetXmlNodeById(nodeID);
                            return xn.Select("//noTranslation");
                        }
                    }
                    //return english if no translation 
                    return xn.Select("//noTranslation");
                }
            }
            catch (Exception e)
            {
                return umbraco.library.GetXmlNodeById(nodeID);
            }
        }

        public static string GetUrlParameter(string param)
        {
            try
            {
                return HttpContext.Current.Request.QueryString[param].Split(',')[0];
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static string ImplodeNodes(XPathNodeIterator xi, string delimiter)
        {
            string implodedString = "";
            int count = 0;

            foreach (XPathNavigator node in xi)
            {
                if (count == 0)
                {
                    implodedString += node.Value.ToString();
                }
                else
                {
                    implodedString += delimiter + node.Value.ToString();
                }
                count++;
            }
            return implodedString;
        }

        public static string ToLower(string input)
        {
            return input.ToLower();
        }

        public static string ToUpper(string input)
        {
            return input.ToUpper();
        }

        public static string GetPageStatusCode()
        {
            return HttpContext.Current.Response.StatusCode.ToString();
        }

        public static string GetCurrentUrl()
        {
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }

        public static XPathNodeIterator GetXmlFromUri(string uri, string nodeName)
        {

            WebRequest request = WebRequest.Create(uri);

            // Set the Method property of the request to GET.
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(responseStream);
            XmlDocument xd = new XmlDocument();
            string responseXml = reader.ReadToEnd();

            xd.LoadXml(responseXml);

            // Close the Stream object.
            responseStream.Close();

            XPathNavigator xn = xd.CreateNavigator();
            return xn.Select("//" + nodeName);
            
        }

        public static string Replace(string needle, string haystack, string replacement)
        {
            return haystack.Replace(needle, replacement);
        }

        public static string GetCurrentYear()
        {
            return DateTime.Today.Year.ToString();
        }

        public static string MachineName(string param)
        {
            return Regex.Replace(param, @"[^a-zA-z0-9_\-]", "");
        }

        //public static string RenderUserControl(string path)
        //{
        //    Page page = new Page();
            
        //    Control userControl=page.LoadControl(path);

        //    page.Controls.Add(userControl);

        //    TextWriter textWriter = new StringWriter();
        //    HtmlTextWriter htmlTextWriter=new HtmlTextWriter(textWriter);

        //    page.RenderControl(htmlTextWriter);

        //    userControl.RenderControl(htmlTextWriter);

            

        //    Log.Add(LogTypes.Custom, 0, "uc=>"+textWriter.ToString());

        //    return textWriter.ToString();
            
        //}
    }
}
