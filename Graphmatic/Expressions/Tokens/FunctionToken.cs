﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Graphmatic.Expressions.Tokens
{
    class FunctionToken : IToken
    {

        public int Width
        {
            get;
            protected set;
        }

        public int Height
        {
            get;
            protected set;
        }

        public DisplaySize Size
        {
            get;
            protected set;
        }

        public int BaselineOffset
        {
            get
            {
                return Operand.BaselineOffset;
            }
        }

        public string Text
        {
            get;
            protected set;
        }

        public Expression Operand
        {
            get;
            protected set;
        }

        public Expression Parent
        {
            get;
            protected set;
        }

        public Expression[] Children
        {
            get;
            protected set;
        }

        public FunctionToken(Expression parent, string text, DisplaySize size)
        {
            Parent = parent;
            Size = size;
            Text = text;
            Operand = new Expression(this, size);
            Children = new Expression[] { Operand };
            RecalculateDimensions();
        }

        public XElement ToXml()
        {
            return new XElement("Function",
                new XAttribute("Name", Text),
                new XElement("Operand", Operand.ToXml()));
        }

        public void Paint(Graphics g, ExpressionCursor expressionCursor, int x, int y)
        {
            g.DrawPixelString(Text, Size == DisplaySize.Small, x, y + Operand.BaselineOffset);

            // draw bracket lines
            if (Size == DisplaySize.Large)
            {
                Operand.Paint(g, expressionCursor, x + 6 * Text.Length + 6, y);
                g.DrawLine(Expression.ExpressionPen,
                    x + 6 * Text.Length + 2,
                    y + 2,
                    x + 6 * Text.Length + 2,
                    y + Height - 2);
                g.DrawLine(Expression.ExpressionPen,
                    x + 6 * Text.Length + 2,
                    y + 2,
                    x + 6 * Text.Length + 4,
                    y);
                g.DrawLine(Expression.ExpressionPen,
                    x + 6 * Text.Length + 4,
                    y + Height,
                    x + 6 * Text.Length + 2,
                    y + Height - 2);

                g.DrawLine(Expression.ExpressionPen,
                    x + Operand.Width + 6 * Text.Length + 9,
                    y + 2,
                    x + Operand.Width + 6 * Text.Length + 9,
                    y + Height - 2);
                g.DrawLine(Expression.ExpressionPen,
                    x + Operand.Width + 6 * Text.Length + 9,
                    y + 2,
                    x + Operand.Width + 6 * Text.Length + 7,
                    y);
                g.DrawLine(Expression.ExpressionPen,
                    x + Operand.Width + 6 * Text.Length + 7,
                    y + Height,
                    x + Operand.Width + 6 * Text.Length + 9,
                    y + Height - 2);
            }
            else
            {
                Operand.Paint(g, expressionCursor, x + 6 * Text.Length + 4, y);
                g.DrawLine(Expression.ExpressionPen,
                    x + 6 * Text.Length + 1,
                    y + 1,
                    x + 6 * Text.Length + 1,
                    y + Height - 1);
                g.DrawLine(Expression.ExpressionPen,
                    x + 6 * Text.Length + 1,
                    y + 1,
                    x + 6 * Text.Length + 2,
                    y);
                g.DrawLine(Expression.ExpressionPen,
                    x + 6 * Text.Length + 2,
                    y + Height,
                    x + 6 * Text.Length + 1,
                    y + Height - 1);

                g.DrawLine(Expression.ExpressionPen,
                    x + Operand.Width + 6 * Text.Length + 6,
                    y + 1,
                    x + Operand.Width + 6 * Text.Length + 6,
                    y + Height - 1);
                g.DrawLine(Expression.ExpressionPen,
                    x + Operand.Width + 6 * Text.Length + 6,
                    y + 1,
                    x + Operand.Width + 6 * Text.Length + 5,
                    y);
                g.DrawLine(Expression.ExpressionPen,
                    x + Operand.Width + 6 * Text.Length + 5,
                    y + Height,
                    x + Operand.Width + 6 * Text.Length + 6,
                    y + Height - 1);
            }
        }

        public void RecalculateDimensions()
        {
            Operand.RecalculateDimensions();
            Width = Operand.Width + 6 * Text.Length + (Size == DisplaySize.Large ? 12 : 9);
            Height = Operand.Height;
        }
    }
}
