﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Graphmatic.Expressions.Tokens
{
    public class ConstantToken : SimpleToken
    {

        private ConstantType _Value;
        public ConstantType Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public override string Text
        {
            get
            {
                switch (_Value)
                {
                    case ConstantType.E:
                        return "£";
                    case ConstantType.Pi:
                        return "^";
                    default:
                        return "<unknown constant>";
                }
            }
            protected set
            {
                throw new NotImplementedException();
            }
        }

        public ConstantToken(Expression parent, ConstantType value)
            :base(parent)
        {
            Parent = parent;
            Children = new Expression[] { };
            Value = value;
        }

        public ConstantToken(Expression parent, XElement xml)
            :base(parent)
        {
            Parent = parent;
            string constantName = xml.Element("Value").Value;
            if(Enum.TryParse<ConstantType>(constantName, out _Value))
                throw new NotImplementedException("The operation " + constantName + " is not implemented.");
            Children = new Expression[] { };
        }

        public override XElement ToXml()
        {
            return new XElement("Constant",
                new XAttribute("Value", Value.ToString()));
        }

        public enum ConstantType
        {
            E,
            Pi
        }
    }
}