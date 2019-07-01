using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class Output : Command
    {
        public Output(Eval eval)
        {
            this.eval = eval;
        }

        private Eval eval { get; set; }

        public string Name
        {
            get
            {
                return this.eval.ToString();
            }
        }

        public Eval Value
        {
            get
            {
                return this.eval;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.OUTPUT, this.Name);
        }
    }
}