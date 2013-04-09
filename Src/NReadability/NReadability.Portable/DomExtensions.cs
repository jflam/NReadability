/*
 * NReadability
 * http://code.google.com/p/nreadability/
 * 
 * Copyright 2010 Marek Stój
 * http://immortal.pl/
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using HtmlParserSharp.Portable;

namespace NReadability
{
    // This is a custom XmlWriter that forces writing of full end tags for XML
    // elements. This is required because sometimes XmlWriters will emit elements
    // using the auto-closing syntax (e.g. <foo />) when what you really need for
    // conformant HTML 5 is the explicit end-tag (e.g. <foo></foo>).
    public class FullEndTagXmlWriter : XmlWriter
    {
        private readonly XmlWriter _inner;

        public FullEndTagXmlWriter(XmlWriter inner)
        {
            this._inner = inner;
        }

        public override void WriteStartDocument()
        {
            _inner.WriteStartDocument();
        }

        public override void WriteStartDocument(bool standalone)
        {
            _inner.WriteStartDocument(standalone);
        }

        public override void WriteEndDocument()
        {
            _inner.WriteEndDocument();
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            _inner.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            _inner.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteEndElement()
        {
            _inner.WriteFullEndElement();
        }

        public override void WriteFullEndElement()
        {
            _inner.WriteFullEndElement();
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            _inner.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteEndAttribute()
        {
            _inner.WriteEndAttribute();
        }

        public override void WriteCData(string text)
        {
            _inner.WriteCData(text);
        }

        public override void WriteComment(string text)
        {
            _inner.WriteComment(text);
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            _inner.WriteProcessingInstruction(name, text);
        }

        public override void WriteEntityRef(string name)
        {
            _inner.WriteEntityRef(name);
        }

        public override void WriteCharEntity(char ch)
        {
            _inner.WriteCharEntity(ch);
        }

        public override void WriteWhitespace(string ws)
        {
            _inner.WriteWhitespace(ws);
        }

        public override void WriteString(string text)
        {
            _inner.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            _inner.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            _inner.WriteChars(buffer, index, count);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            _inner.WriteRaw(buffer, index, count);
        }

        public override void WriteRaw(string data)
        {
            _inner.WriteRaw(data);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            _inner.WriteBase64(buffer, index, count);
        }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            return _inner.LookupPrefix(ns);
        }

        public override WriteState WriteState
        {
            get { return _inner.WriteState; }
        }
    }

  public static class DomExtensions
  {
    #region XDocument extensions

    public static XElement GetBody(this XDocument document)
    {
      if (document == null)
      {
        throw new ArgumentNullException("document");
      }

      var documentRoot = document.Root;

      if (documentRoot == null)
      {
        return null;
      }

      return documentRoot.GetElementsByTagName("body").FirstOrDefault();
    }

    public static string GetTitle(this XDocument document)
    {
      if (document == null)
      {
        throw new ArgumentNullException("document");
      }

      var documentRoot = document.Root;

      if (documentRoot == null)
      {
        return null;
      }

      var headElement = documentRoot.GetElementsByTagName("head").FirstOrDefault();

      if (headElement == null)
      {
        return "";
      }

      var titleElement = headElement.GetChildrenByTagName("title").FirstOrDefault();

      if (titleElement == null)
      {
        return "";
      }

      return (titleElement.Value ?? "").Trim();
    }

    public static XElement GetElementById(this XDocument document, string id)
    {
      if (document == null)
      {
        throw new ArgumentNullException("document");
      }

      if (string.IsNullOrEmpty(id))
      {
        throw new ArgumentNullException("id");
      }

      return
        (from element in document.Descendants()
         let idAttribute = element.Attribute("id")
         where idAttribute != null && idAttribute.Value == id
         select element).SingleOrDefault();
    }

    #endregion

    #region XElement extensions

    public static string GetId(this XElement element)
    {
      return element.GetAttributeValue("id", "");
    }

    public static void SetId(this XElement element, string id)
    {
      element.SetAttributeValue("id", id);
    }

    public static string GetClass(this XElement element)
    {
      return element.GetAttributeValue("class", "");
    }

    public static void SetClass(this XElement element, string @class)
    {
      element.SetAttributeValue("class", @class);
    }

    public static string GetStyle(this XElement element)
    {
      return element.GetAttributeValue("style", "");
    }

    public static void SetStyle(this XElement element, string style)
    {
      element.SetAttributeValue("style", style);
    }

    public static string GetAttributeValue(this XElement element, string attributeName, string defaultValue)
    {
      if (element == null)
      {
        throw new ArgumentNullException("element");
      }

      if (string.IsNullOrEmpty(attributeName))
      {
        throw new ArgumentNullException("attributeName");
      }

      var attribute = element.Attribute(attributeName);

      return attribute != null
               ? (attribute.Value ?? defaultValue)
               : defaultValue;
    }

    public static void SetAttributeValue(this XElement element, string attributeName, string value)
    {
      if (element == null)
      {
        throw new ArgumentNullException("element");
      }

      if (string.IsNullOrEmpty(attributeName))
      {
        throw new ArgumentNullException("attributeName");
      }

      if (value == null)
      {
        var attribute = element.Attribute(attributeName);

        if (attribute != null)
        {
          attribute.Remove();
        }
      }
      else
      {
        element.SetAttributeValue(attributeName, value);
      }
    }

    public static string GetAttributesString(this XElement element, string separator)
    {
      if (element == null)
      {
        throw new ArgumentNullException("element");
      }

      if (separator == null)
      {
        throw new ArgumentNullException("separator");
      }

      var resultSb = new StringBuilder();
      bool isFirst = true;

      element.Attributes().Aggregate(
        resultSb,
        (sb, attribute) =>
        {
          string attributeValue = attribute.Value;

          if (string.IsNullOrEmpty(attributeValue))
          {
            return sb;
          }

          if (!isFirst)
          {
            resultSb.Append(separator);
          }

          isFirst = false;

          sb.Append(attribute.Value);

          return sb;
        });

      return resultSb.ToString();
    }

    public static string GetInnerHtml(this XContainer container)
    {
        if (container == null)
        {
            throw new ArgumentNullException("container");
        }

        var sb = new StringBuilder();
        using (var writer = new FullEndTagXmlWriter(
            XmlWriter.Create(sb,
                             new XmlWriterSettings()
                             {
                                 ConformanceLevel = ConformanceLevel.Fragment
                             })))
        {
            foreach (var childNode in container.Nodes())
            {
                childNode.WriteTo(writer);
            }

            writer.Flush();
            return sb.ToString();
        }
    }

    public static void SetInnerHtml(this XElement element, string html)
    {
        if (element == null)
        {
            throw new ArgumentNullException("element");
        }

        if (html == null)
        {
            throw new ArgumentNullException("html");
        }

        element.RemoveAll();

        var parser = new SimpleHtmlParser();
        var nodes = parser.ParseFragment(new StringReader(html), String.Empty);
        foreach (var node in nodes)
        {
            element.Add(node);
        }
    }

    #endregion

    #region XContainer extensions

    public static IEnumerable<XElement> GetElementsByTagName(this XContainer container, string tagName)
    {
      if (container == null)
      {
        throw new ArgumentNullException("container");
      }

      if (string.IsNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("tagName");
      }

      return container.Descendants()
        .Where(e => tagName.Equals(e.Name.LocalName, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<XElement> GetChildrenByTagName(this XContainer container, string tagName)
    {
      if (container == null)
      {
        throw new ArgumentNullException("container");
      }

      if (string.IsNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("tagName");
      }

      return container.Elements()
        .Where(e => e.Name != null && tagName.Equals(e.Name.LocalName, StringComparison.OrdinalIgnoreCase));
    }

    #endregion
  }
}
