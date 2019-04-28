using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Runtime
{

    /** manages source paths and fixes absolute path for .asl */
    public class SourcePath
    {
        public static readonly string CRPrefix = "ClassResource:";

        protected string root = ".";
        protected string urlPrefix = null;
        protected List<string> paths = new List<string>();

        public void SetRoot(string r)
        {
            root = r;
        }
        public string GetRoot()
        {
            return root;
        }

        public void SetUrlPrefix(string p)
        {
            urlPrefix = p;
        }
        public string GetUrlPrefix()
        {
            return urlPrefix;
        }

        public void AddPath(string cp)
        {
            if (cp.StartsWith("\""))
                cp = cp.Substring(1, cp.Length - 1);
            if (cp.EndsWith("/"))
                cp = cp.Substring(0, cp.Length - 1);
            cp = cp.Replace("\\\\", "/"); // use unix path separator
            paths.Add(cp);
        }

        public void AddAll(SourcePath sp)
        {
            if (sp == null)
                return;
            foreach (string p in sp.paths)
                paths.Add(p);
        }

        public void ClearPaths()
        {
            paths.Clear();
        }

        public List<string> GetPaths()
        {
            List<string> r = new List<string>();
            if (paths.Count == 0)
            {
                r.Add(root);
            }
            else
            {
                foreach (string p in paths)
                {
                    r.Add(p);
                    if (!p.StartsWith(".") && !p.StartsWith("/") && p[1] != ':' && !root.Equals("."))
                    {
                        // try both, with and without the current directory
                        r.Add(root + "/" + p);
                    }
                }
            }
            return r;
        }

        public bool IsEmpty()
        {
            return paths.Count == 0;
        }

        public string FixPath(string f)
        {
            return FixPath(f, urlPrefix);
        }

        /** fix path of the asl code based on aslSourcePath,
         * also considers code from a jar file (if urlPrefix is not null) */
        public string FixPath(string f, string urlPrefix)
        {
            if (f == null)
                return f;
            if (f.Length == 0)
                return f;
            if (urlPrefix == null || urlPrefix.Length == 0)
            {
                if (new File(f).exists())
                {
                    return f;
                }
                else
                {
                    foreach (string path in GetPaths())
                    {
                        try
                        {
                            File newname = new File(path + "/" + f.ToString());
                            if (newname.exists())
                            {
                                return newname.GetCanonicalFile().ToString();
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Error: " + e);
                        }
                    }
                }
            }
            else
            {
                if (TestURLSrc(urlPrefix + f))
                {
                    return urlPrefix + f;
                }
                else
                {
                    foreach (string path in GetPaths())
                    {
                        string newname = urlPrefix + path + "/" + f;
                        newname = newname.Replace("\\./", "");
                        if (TestURLSrc(newname))
                            return newname;
                    }
                }
            }
            return f;
        }

        private static bool TestURLSrc(string asSrc)
        {
            try
            {
                if (asSrc.StartsWith(CRPrefix))
                {
                    SourcePath./*class*/GetResource(asSrc.Substring(CRPrefix.Length).OpenStream());
                }
                else
                {
                    new URl(asSrc).openStream();
                }
            }
            catch (Exception e)
            {
                
            }
            return false;
        }

        public override string ToString()
        {
            return urlPrefix + ":::" + this.root + " " + this.paths;
        }
    }
}
