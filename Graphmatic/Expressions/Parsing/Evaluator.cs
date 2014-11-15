﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphmatic.Expressions.Parsing
{
    public abstract class Evaluator
    {
        public string FormatString
        {
            get;
            protected set;
        }

        protected Evaluator()
        {

        }

        protected Evaluator(string formatString) {
            FormatString = formatString;
        }
    }
}
