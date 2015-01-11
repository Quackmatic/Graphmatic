﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Graphmatic.Interaction.Plotting
{
    public class GraphAxis : IPlottable, IXmlConvertible
    {
        public double GridSize
        {
            get;
            set;
        }

        public double MajorInterval
        {
            get;
            set;
        }

        public GraphAxis(double gridSize, int majorInterval)
        {
            GridSize = gridSize;
            MajorInterval = majorInterval;
        }

        public GraphAxis(XElement xml)
        {
            GridSize = Double.Parse(xml.Element("GridSize").Value);
            MajorInterval = Double.Parse(xml.Element("MajorInterval").Value);
        }

        public void PlotOnto(Graph graph, Graphics g, Size graphSize, PlottableParameters plotParams, GraphParameters graphParams)
        {
            int axisX, axisY;
            graph.ToImageSpace(graphSize, graphParams, 0, 0, out axisX, out axisY);

            PlotGridLinesOnto(g, graphSize, plotParams, graphParams, axisX, axisY);
            PlotAxesOnto(g, graphSize, plotParams, axisX, axisY);
        }

        private void PlotAxesOnto(Graphics g, Size graphSize, PlottableParameters plotParams, int axisX, int axisY)
        {
            Pen axisPen = new Pen(plotParams.PlotColor);
            if (axisY >= 0 && axisY < graphSize.Height)
            {
                g.DrawLine(axisPen, 0, axisY, graphSize.Width, axisY);
            }
            if (axisX >= 0 && axisX < graphSize.Width)
            {
                g.DrawLine(axisPen, axisX, 0, axisX, graphSize.Height);
            }
        }

        private void PlotGridLinesOnto(Graphics g, Size graphSize, PlottableParameters plotParams, GraphParameters graphParams, int axisX, int axisY)
        {
            Pen majorPen = new Pen(plotParams.PlotColor.ColorAlpha(0.5));
            Pen minorPen = new Pen(plotParams.PlotColor.ColorAlpha(0.333));

            Font valueFont = SystemFonts.DefaultFont;
            Font axisFont = new Font(SystemFonts.MessageBoxFont.FontFamily, 15f, FontStyle.Italic);
            Brush valueBrush = majorPen.Brush;

            double incrementX = GridSize / graphParams.HorizontalPixelScale,
                   incrementY = GridSize / graphParams.VerticalPixelScale;

            g.DrawString(graphParams.VerticalAxis.ToString(), axisFont, valueBrush, (int)axisX, 2);
            g.DrawString(graphParams.HorizontalAxis.ToString(), axisFont, valueBrush, (int)graphSize.Width - 16, (int)axisY);

            double value = axisX;
            int index = 0;
            while (value < graphSize.Width)
            {
                bool major = index % MajorInterval == 0;
                g.DrawLine(major ? majorPen : minorPen,
                    (int)value, 0, (int)value, graphSize.Height);
                if (major)
                {
                    double horizontal = graphParams.HorizontalPixelScale * (value - graphSize.Width / 2) + graphParams.CenterHorizontal;
                    g.DrawString(horizontal.ToString(), valueFont, valueBrush, (int)value, (int)axisY);
                }
                value += incrementX; index++;
            }

            value = axisX; index = 0;
            while (value >= 0)
            {
                bool major = index % MajorInterval == 0;
                g.DrawLine(major ? majorPen : minorPen,
                    (int)value, 0, (int)value, graphSize.Height);
                if (major)
                {
                    double horizontal = graphParams.HorizontalPixelScale * (value - graphSize.Width / 2) + graphParams.CenterHorizontal;
                    g.DrawString(horizontal.ToString(), valueFont, valueBrush, (int)value, (int)axisY);
                }
                value -= incrementX; index++;
            }

            value = axisY; index = 0;
            while (value < graphSize.Height)
            {
                bool major = index % MajorInterval == 0;
                g.DrawLine(major ? majorPen : minorPen,
                    0, (int)value, graphSize.Width, (int)value);
                if (major)
                {
                    double vertical = graphParams.VerticalPixelScale * -(value - graphSize.Height / 2) + graphParams.CenterVertical;
                    g.DrawString(vertical.ToString(), valueFont, valueBrush, (int)axisX, (int)value);
                }
                value += incrementY; index++;
            }

            value = axisY; index = 0;
            while (value >= 0)
            {
                bool major = index % MajorInterval == 0;
                g.DrawLine(major ? majorPen : minorPen,
                    0, (int)value, graphSize.Width, (int)value);
                if (major)
                {
                    double vertical = graphParams.VerticalPixelScale * -(value - graphSize.Height / 2) + graphParams.CenterVertical;
                    g.DrawString(vertical.ToString(), valueFont, valueBrush, (int)axisX, (int)value);
                }
                value -= incrementY; index++;
            }
        }

        public XElement ToXml()
        {
            return new XElement("GraphAxis",
                new XElement("GridSize", GridSize),
                new XElement("MajorInterval", MajorInterval));
        }

        public void UpdateReferences(Document document)
        {
        }


        public bool CanPlot(char variable1, char variable2)
        {
            return true;
        }
    }
}