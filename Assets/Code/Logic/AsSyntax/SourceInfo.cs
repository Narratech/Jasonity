using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic.AsSyntax
{
    [Serializable]
    public class SourceInfo
    {
        private readonly string source;
        private readonly int beginSrcLine; // the line this literal appears in the source
        private readonly int endSrcLine;

        public SourceInfo(string file, int beginLine)
        {
            source = file;
            beginSrcLine = beginLine;
            endSrcLine = beginLine;
        }

        public SourceInfo(string file, int beginLine, int endLine)
        {
            source = file;
            beginSrcLine = beginLine;
            endSrcLine = endLine;
        }

        public SourceInfo(SourceInfo o)
        {
            if (o == null)
            {
                source = null;
                beginSrcLine = 0;
                endSrcLine = 0;
            }
            else
            {
                source = o.source;
                beginSrcLine = o.beginSrcLine;
                endSrcLine = o.endSrcLine;
            }
        }

        public SourceInfo Clone()
        {
            return this;
        }

        public string GetSrcFile()
        {
            return source;
        }

        public int GetSrcLine()
        {
            return beginSrcLine;
        }

        public int GetBeginSrcLine()
        {
            return beginSrcLine;
        }

        public int GetEndSrcLine()
        {
            return endSrcLine;
        }

        public override string ToString()
        {
            return (source == null ? "nofile" : source) + (beginSrcLine >= 0 ? ":"+beginSrcLine :"");
        }
    }
}
