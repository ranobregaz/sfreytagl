/*
 * 原作者: blu10ph
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OSChina.Controls;
using WP7_ControlsLib.Controls;
using System.Windows.Input;
using System.Net;
using System.IO;
using System.Text;

namespace OSChina
{
    public static class HttpGetHelper
    {
        public static Dictionary<string, string> GetParamtersFromQueryString(string uri, char spliter = '?') {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string[] parametersString= uri.Split(spliter);
            foreach (string parameterString in parametersString)
            {
                string[] parameter = parameterString.Split('=');
                if (parameter.Length > 2)
                {
                    parameters.Add(parameter[0], parameter[1]);
                }
                else if (parameter.Length > 1)
                {
                    parameters.Add(parameter[0], "");
                }
            }
            return parameters;
        }
        public static string GetQueryStringByParameters(string uriPrefix, Dictionary<string, string> parameters)
        {
            return GetQueryStringByParameters(uriPrefix,parameters,'&');
        }
        public static string GetQueryStringByParameters(string uriPrefix, Dictionary<string, string> parameters, char spliter)
        {
            StringBuilder buffer = new StringBuilder(uriPrefix);
            int i = 0;
            foreach (string key in parameters.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("{0}{1}={2}", spliter, key, parameters[key]);
                }
                else
                {
                    buffer.AppendFormat("?{0}={1}", key, parameters[key]);
                }
                i++;
            }
            return buffer.ToString();
        }
    }
}
