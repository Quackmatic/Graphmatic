﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Graphmatic.Interaction.Annotations;
using Graphmatic.Interaction.Plotting;

namespace Graphmatic.Interaction
{
    public class Page : Resource
    {
        public override string Type
        {
            get
            {
                return "Page";
            }
        }

        public Color BackgroundColor
        {
            get;
            set;
        }

        public Graph Graph
        {
            get;
            protected set;
        }

        public List<Annotation> Annotations
        {
            get;
            protected set;
        }

        public Page()
        {
            BackgroundColor = Properties.Settings.Default.DefaultPageColor;
            Graph = new Graph();
            Annotations = new List<Annotation>();
        }

        /// <summary>
        /// Initialize a new empty instance of the <c>Graphmatic.Interaction.Page</c> class from serialized XML data.
        /// </summary>
        /// <param name="xml">The XML data to use for deserializing this Resource.</param>
        public Page(XElement xml)
            : base(xml)
        {
            BackgroundColor = ResourceSerializationExtensionMethods.XmlStringToColor(xml.Element("BackgroundColor").Value);
            Graph = new Graph(xml.Element("Graph"));
            Annotations = new List<Annotation>(
                xml
                .Element("Annotations")
                .Elements()
                .Select(x => x.Deserialize<Annotation>(SerializationExtensionMethods.AnnotationName)));
        }

        public override Image GetResourceIcon(bool large)
        {
            return large ?
                Properties.Resources.Page32 :
                Properties.Resources.Page16;
        }

        public override XElement ToXml()
        {
            XElement baseElement = base.ToXml();
            baseElement.Name = "Page";
            baseElement.Add(new XElement("BackgroundColor", BackgroundColor.ToXmlString()));
            baseElement.Add(Graph.ToXml());
            baseElement.Add(new XElement("Annotations",
                Annotations.Select(a =>
                a.ToXml())));
            return baseElement;
        }

        public override void UpdateReferences(Document document)
        {
            Graph.UpdateReferences(document);
        }

        public override void ResourceModified(Resource resource, ResourceModifyType type)
        {
            base.ResourceModified(resource, type);
            if (resource == this)
            {
                Graph.OnUpdate();
            }
            else
            {
                Graph.ResourceModified(resource, type);
            }
        }
    }
}
