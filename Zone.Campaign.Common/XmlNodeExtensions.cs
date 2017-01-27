using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Zone.Campaign
{
    public static class XmlNodeExtensions
    {
        /// <summary>
        /// Append an attribute to the node.
        /// </summary>
        /// <param name="node">Node to append to</param>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns>The new attribute</returns>
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

        /// <summary>
        /// Append a child element to the node.
        /// </summary>
        /// <param name="node">Node to append to</param>
        /// <param name="name">Name of the new element</param>
        /// <returns>The new element</returns>
        public static XmlElement AppendChild(this XmlNode node, string name)
        {
            return node.AppendChildWithValue(name, null, null);
        }

        /// <summary>
        /// Append a child element to the node.
        /// </summary>
        /// <param name="node">Node to append to</param>
        /// <param name="name">Name of the new element</param>
        /// <param name="value">Value of the new element</param>
        /// <returns>The new element</returns>
        public static XmlElement AppendChildWithValue(this XmlNode node, string name, string value)
        {
            return node.AppendChildWithValue(name, null, value);
        }

        /// <summary>
        /// Append a child element to the node.
        /// </summary>
        /// <param name="node">Node to append to</param>
        /// <param name="qualifiedName">Name of the new element, qualified with a namespace prefix</param>
        /// <param name="namespaceUri">Namespace uri</param>
        /// <returns>The new element</returns>
        public static XmlElement AppendChild(this XmlNode node, string qualifiedName, string namespaceUri)
        {
            return node.AppendChildWithValue(qualifiedName, namespaceUri, null);
        }

        /// <summary>
        /// Append a child element to the node.
        /// </summary>
        /// <param name="node">Node to append to</param>
        /// <param name="qualifiedName">Name of the new element, qualified with a namespace prefix</param>
        /// <param name="namespaceUri">Namespace uri</param>
        /// <param name="value">Value of the new element</param>
        /// <returns>The new element</returns>
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

        /// <summary>
        /// Remove a child element from a parent element. Only the first child of matching name is removed.
        /// </summary>
        /// <param name="element">Element to remove child from</param>
        /// <param name="name">Name of child to remove</param>
        public static void RemoveChild(this XmlElement element, string name)
        {
            var child = element.ChildNodes.Cast<XmlNode>().FirstOrDefault(i => i.LocalName == name);
            if (child == null)
            {
                return;
            }

            element.RemoveChild(child);
        }

        /// <summary>
        /// Removes all attributes from an element except those specified.
        /// </summary>
        /// <param name="element">Element to remove attributes from</param>
        /// <param name="attributesToKeep">Names of the attributes which should be retained</param>
        public static void RemoveAllAttributesExcept(this XmlElement element, IEnumerable<string> attributesToKeep)
        {
            var attributesToDiscard = element.Attributes.Cast<XmlAttribute>().Where(i => !attributesToKeep.Contains(i.LocalName)).Select(i => i.LocalName).ToArray();
            foreach (var attribute in attributesToDiscard)
            {
                element.RemoveAttribute(attribute);
            }
        }
    }
}
