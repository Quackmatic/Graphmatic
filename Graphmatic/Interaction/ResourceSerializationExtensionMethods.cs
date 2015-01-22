﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Graphmatic.Interaction.Annotations;

namespace Graphmatic.Interaction
{
    /// <summary>
    /// Represents a method that deserializes a resource from its XML representation.
    /// </summary>
    /// <param name="xml">THe XML representation of the resource.</param>
    /// <returns>The deserialized form of the resource.</returns>
    public delegate Resource ResourceDeserializationFactory(XElement xml);

    /// <summary>
    /// Represents a method that deserializes an annotation from its XML representation.
    /// </summary>
    /// <param name="xml">The XML representation of the annotation.</param>
    /// <returns>The deserialized form of the annotation.</returns>
    public delegate Annotation AnnotationDeserializationFactory(XElement xml);

    /// <summary>
    /// Provides methods to assist in (de)serialization of serialized Graphmatic document resources.
    /// </summary>
    public static class ResourceSerializationExtensionMethods
    {
        /// <summary>
        /// A list of deserializing constructors for resource types with differing XML element names.
        /// </summary>
        private static Dictionary<string, ResourceDeserializationFactory> ResourceDeserializers = new Dictionary<string, ResourceDeserializationFactory>
        {
            { "Page", xml => new Page(xml) },
            { "Equation", xml => new Equation(xml) },
            { "DataSet", xml => new DataSet(xml) },
            { "Resource", xml => new Resource(xml) }
        };

        /// <summary>
        /// A list of deserializing constructors for annotation types with differing XML element names.
        /// </summary>
        private static Dictionary<string, AnnotationDeserializationFactory> AnnotationDeserializers = new Dictionary<string, AnnotationDeserializationFactory>
        {
            { "Annotation", xml => new Annotation(xml) },
            { "Drawing", xml => new Drawing(xml) }
        };

        /// <summary>
        /// Deserializes a resource from its XML representation.
        /// </summary>
        /// <param name="xml">The XML representation of a resource to deserialize.</param>
        /// <returns>The deserialized form of the given XML.</returns>
        public static Resource FromXml(XElement xml)
        {
            string elementName = xml.Name.LocalName;
            if (!ResourceDeserializers.ContainsKey(elementName))
                throw new IOException("Unknown resource type: " + elementName);
            return ResourceDeserializers[elementName](xml);
        }

        /// <summary>
        /// Deserializes an annotation from its XML representation.
        /// </summary>
        /// <param name="xml">The XML representation of an annotation to deserialize.</param>
        /// <returns>The deserialized form of the given XML.</returns>
        public static Annotation AnnotationFromXml(XElement xml)
        {
            string elementName = xml.Name.LocalName;
            if (!AnnotationDeserializers.ContainsKey(elementName))
                throw new IOException("Unknown annotation type: " + elementName);
            return AnnotationDeserializers[elementName](xml);
        }

        /// <summary>
        /// Converts an image to a byte array (representing the image in the PNG format) so it can be serialized.
        /// </summary>
        /// <param name="image">The image to convert to a byte array.</param>
        /// <returns>The specified image as a byte array.</returns>
        public static byte[] ToByteArray(this Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts a byte array, representing an image, to an Image object.
        /// </summary>
        /// <param name="data">The binary image data to convert to an image.</param>
        /// <returns>The Image representation of the given byte array.</returns>
        public static Image ImageToByteArray(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// Converts a <c>System.Drawing.Color</c> to a string that can be stored as XML.
        /// <para/>
        /// This is just a wrapper method around <c>ColorTranslator.ToHtml</c>.
        /// </summary>
        public static string ToXmlString(this Color color)
        {
            return ColorTranslator.ToHtml(color);
        }

        /// <summary>
        /// Converts a string, representing a color in the HTML-style representation, to a <c>System.Drawing.Color</c> object.
        /// <para/>
        /// This is just a wrapper method around <c>ColorTranslator.FromHtml</c>.
        /// </summary>
        public static Color XmlStringToColor(string data)
        {
            return ColorTranslator.FromHtml(data);
        }

        /// <summary>
        /// Converts a string to UTF-8 encoded Base64 representation.
        /// </summary>
        public static string ToBase64(this string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }

        /// <summary>
        /// Deconverts a string from the UTF-8 encoded Base64 representation to the original form.
        /// </summary>
        public static string FromBase64(this string s)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }

        /// <summary>
        /// Duplicates a resource in a document.
        /// <para/>
        /// The duplicated resource should be identical to the original in everything except Guid, AuthorName and CreationDate.
        /// </summary>
        /// <typeparam name="TResource">The type of resource to duplicate.</typeparam>
        /// <param name="resource">The resource to duplicate.</param>
        /// <param name="parentDocument">The Graphmatic.Interaction.Document object that this Resource resides in.</param>
        /// <returns>A duplicate of <paramref name="resource"/>.</returns>
        public static TResource Duplicate<TResource>(this TResource resource, Document parentDocument) where TResource : Resource
        {
            // This creates a copy of a resource by first serializing it to XML
            // and then deserializing it back. It's not the prettiest way of doing
            // it, but it means I don't have to go back through the codebase and
            // write yet another tree-recursive duplication function on just about
            // everything.
            // One side effect is that any ResourceReferences created (eg. used by
            // the Page resource) will still link back to the original object.

            XElement serializedForm = resource.ToXml();
            TResource duplicatedResource = FromXml(serializedForm) as TResource;
            duplicatedResource.UpdateReferences(parentDocument);
            duplicatedResource.InitializeIdentifyingAttributes();
            return duplicatedResource;
        }
    }
}
