﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Graphmatic.Expressions.Parsing;
using Graphmatic.Interaction.Plotting;

namespace Graphmatic.Interaction
{
    public partial class Equation : Resource, IPlottable
    {
        public const float EquationPenWidth = 2f;
        public const int EquationResolution = 3;

        public void PlotOnto(Graph graph, Graphics graphics, Size graphSize, Color color, PlotParameters parameters)
        {
            BinaryParseTreeNode parseTreeRoot = ParseTree as BinaryParseTreeNode;
            if (parseTreeRoot.Left is VariableParseTreeNode)
            {
                PlotExplicit(
                    graph,
                    graphics,
                    graphSize,
                    color,
                    parameters,
                    (parseTreeRoot.Left as VariableParseTreeNode).Variable,
                    parseTreeRoot.Right);
            }
            else if (parseTreeRoot.Right is VariableParseTreeNode)
            {
                PlotExplicit(
                    graph,
                    graphics,
                    graphSize,
                    color,
                    parameters,
                    (parseTreeRoot.Right as VariableParseTreeNode).Variable,
                    parseTreeRoot.Left);
            }
            else
            {
                PlotImplicit(graph, graphics, graphSize, color, parameters);
            }
        }

        private int IntLerp(int scale, double a, double b)
        {
            return (int)(a / (a - b) * (double)scale);
        }

        private void PlotImplicit(Graph graph, Graphics graphics, Size graphSize, Color color, PlotParameters parameters)
        {
            Dictionary<char, double> vars = new Dictionary<char, double>();
            Pen graphPen = new Pen(color, EquationPenWidth);

            int gridWidth = graphSize.Width / EquationResolution,
                gridHeight = graphSize.Height / EquationResolution;
            double[,] values = new double[
                gridWidth + 1,
                gridHeight + 1];

            for (int i = 0; i < gridWidth + 1; i++)
            {
                for (int j = 0; j < gridHeight + 1; j++)
                {
                    double horizontal = parameters.HorizontalPixelScale * ((i * EquationResolution) - graphSize.Width / 2) + parameters.CenterHorizontal,
                           vertical = parameters.VerticalPixelScale * -((j * EquationResolution) - graphSize.Height / 2) + parameters.CenterVertical;
                    vars[HorizontalVariable] = horizontal;
                    vars[VerticalVariable] = vertical;
                    values[i, j] = ParseTree.Evaluate(vars);
                }
            }

            // marching squares
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    double av = values[i, j],
                            bv = values[i + 1, j],
                            cv = values[i, j + 1],
                            dv = values[i + 1, j + 1];
                    if (Double.IsInfinity(av) || Double.IsNaN(av) ||
                        Double.IsInfinity(bv) || Double.IsNaN(bv) ||
                        Double.IsInfinity(cv) || Double.IsNaN(cv) ||
                        Double.IsInfinity(dv) || Double.IsNaN(dv))
                    {
                        graphics.DrawLine(Pens.Red,
                            i * EquationResolution,
                            j * EquationResolution,
                            (i + 1) * EquationResolution,
                            (j + 1) * EquationResolution);
                        continue;
                    }
                    bool ab = ((av > 0) ^ (bv > 0)),
                            bd = ((bv > 0) ^ (dv > 0)),
                            ac = ((av > 0) ^ (cv > 0)),
                            cd = ((cv > 0) ^ (dv > 0));
                    int x = i * EquationResolution,
                        y = j * EquationResolution;
                    Point abp = new Point(x + IntLerp(EquationResolution, av, bv), y),
                            bdp = new Point(x + EquationResolution, y + IntLerp(EquationResolution, bv, dv)),
                            acp = new Point(x, y + IntLerp(EquationResolution, av, cv)),
                            cdp = new Point(x + IntLerp(EquationResolution, cv, dv), y + EquationResolution);
                    if (ab && bd && ac && cd) { }
                    else if (!ab && !bd && !ac && !cd) { }
                    else if (ac && cd)
                        graphics.DrawLine(graphPen, acp, cdp);
                    else if (bd && cd)
                        graphics.DrawLine(graphPen, bdp, cdp);
                    else if (bd && ac)
                        graphics.DrawLine(graphPen, bdp, acp);
                    else if (ab && bd)
                        graphics.DrawLine(graphPen, abp, bdp);
                    else if (ab && cd)
                        graphics.DrawLine(graphPen, abp, cdp);
                    else if (ab && ac)
                        graphics.DrawLine(graphPen, abp, acp);
                }
            }
        }

        private void PlotExplicit(Graph graph, Graphics graphics, Size graphSize, Color color, PlotParameters parameters, char explicitVariable, ParseTreeNode node)
        {
            if (explicitVariable == HorizontalVariable)
            {
                PlotExplicitVertical(graph, graphics, graphSize, color, parameters, VerticalVariable, node); // x=...
            }
            else if (explicitVariable == VerticalVariable)
            {
                PlotExplicitHorizontal(graph, graphics, graphSize, color, parameters, HorizontalVariable, node); // y=...
            }
            else
            {
                // actually plotting implicitly in relation to a constant (eg. xy=a)
                PlotImplicit(graph, graphics, graphSize, color, parameters);
            }
        }

        private void PlotExplicitHorizontal(Graph graph, Graphics graphics, Size graphSize, Color color, PlotParameters parameters, char explicitVariable, ParseTreeNode node)
        {
            Dictionary<char, double> vars = new Dictionary<char, double>();
            Pen graphPen = new Pen(color, EquationPenWidth);
            int previousX = -1, previousY = -1;
            for (float i = 0; i < graphSize.Width; i += 0.1f)
            {
                double horizontal = parameters.HorizontalPixelScale * (i - graphSize.Width / 2) + parameters.CenterHorizontal;
                vars[explicitVariable] = horizontal;
                double vertical = node.Evaluate(vars);

                int x, y;
                graph.ToImageSpace(graphSize, parameters, horizontal, vertical, out x, out y);

                if (previousX != -1)
                {
                    if (y < -100 || y >= graphSize.Height + 100)
                    {
                        x = -1;
                        y = -1;
                    }
                    else
                    {
                        try
                        {
                            graphics.DrawLine(graphPen, previousX, previousY, x, y);
                        }
                        catch (OverflowException) // can happen if the graph line shoots up tending to infinity
                        {
                            x = -1;
                            y = -1;
                        }
                    }
                }

                previousX = x;
                previousY = y;
            }
        }

        private void PlotExplicitVertical(Graph graph, Graphics graphics, Size graphSize, Color color, PlotParameters parameters, char explicitVariable, ParseTreeNode node)
        {
            Dictionary<char, double> vars = new Dictionary<char, double>();
            Pen graphPen = new Pen(color, EquationPenWidth);
            int previousX = -1, previousY = -1;
            for (float i = 0; i < graphSize.Width; i += 0.1f)
            {
                double vertical = parameters.VerticalPixelScale * -(i - graphSize.Height / 2) + parameters.CenterVertical;
                vars[explicitVariable] = vertical;
                  double horizontal = node.Evaluate(vars);

                int x, y;
                graph.ToImageSpace(graphSize, parameters, horizontal, vertical, out x, out y);

                if (previousX != -1)
                {
                    if (x < -100 || x >= graphSize.Width + 100)
                    {
                        x = -1;
                        y = -1;
                    }
                    else
                    {
                        try
                        {
                            graphics.DrawLine(graphPen, previousX, previousY, x, y);
                        }
                        catch (OverflowException) // can happen if the graph line shoots up tending to infinity
                        {
                            x = -1;
                            y = -1;
                        }
                    }
                }

                previousX = x;
                previousY = y;
            }
        }
    }
}