using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Zone.Campaign.WebServices
{
    public static class XmlNodeExtensions
    {
        public static XmlAttribute AppendAttribute(this XmlNode node, string name, string value = null)
        {
            var ownerDocument = node.OwnerDocument;
            if (ownerDocument == null)
            {
                throw new ArgumentException("Node has no parent document.");
            }

            var attribute = ownerDocument.CreateAttribute(name);
            node.Attributes.Append(attribute);
            if (value != null)
            {
                attribute.InnerText = value;
            }

            return attribute;
        }

        public static XmlElement AppendChild(this XmlNode node, string name)
        {
            return node.AppendChildWithValue(name, null, null);
        }

        public static XmlElement AppendChildWithValue(this XmlNode node, string name, string value)
        {
            return node.AppendChildWithValue(name, null, value);
        }

        public static XmlElement AppendChild(this XmlNode node, string qualifiedName, string namespaceUri)
        {
            return node.AppendChildWithValue(qualifiedName, namespaceUri, null);            
        }
        
        public static XmlElement AppendChildWithValue(this XmlNode node, string qualifiedName, string namespaceUri, string value)
        {
            var ownerDocument = node.OwnerDocument;
            if (ownerDocument == null)
            {
                throw new ArgumentException("Node has no parent document.");
            }

            var child = ownerDocument.CreateElement(qualifiedName, namespaceUri);
            node.AppendChild(child);
            if (value != null)
            {
                child.InnerText = value;
            }

            return child;
        }
    }
}
