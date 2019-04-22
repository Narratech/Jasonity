using Assets.Code.BDIAgent;
using Assets.Code.parser;
using Assets.Code.Runtime;
using Assets.Code.Util;
using System;
using System.IO;

namespace Assets.Code.AsSyntax.directives
{ 
    public partial class Include : DefaultDirective, IDirective {

        //HACER: Clase SourcePath de runtime
        private SourcePath aslSourcePath = new SourcePath();

        public Agent Process(Pred directive, Agent outerContent, Agent innerContent)
        {
            if (outerContent == null)
                return null;

            // handles file (arg[0])
            string file = ((IStringTerm)directive.GetTerm(0)).GetString().Replace("\\\\", "/");
            try
            {
                //CAMBIAR: similar c# de InputStream
                InputStream input = null;
                // test include from jar
                if (file.StartsWith("$"))
                { // the case of "$jasonJar/src/a.asl"
                    string jar = file.Substring(1, file.IndexOf("/"));
                    //HACER: Clase Config de util
                    if (Config.Get().Get(jar) == null)
                    {
                        return null;
                    }

                    string path = Config.Get().Get(jar).ToString();
                    file = "jar:file:" + path + "!" + file.Substring(file.IndexOf("/"));
                    //CAMBIAR: Similar URL en c#
                    input = new URL(file).openStream();
                }

                else
                {
                    string outerPrefix = outerContent.GetASLSrc(); // the source file that has the include directive
                    if (outerContent != null && outerPrefix != null)
                    {
                        // check if the outer is URL
                        if (outerPrefix.StartsWith("jar"))
                        {
                            outerPrefix = outerPrefix.Substring(0, outerPrefix.IndexOf("!") + 1) + "/";
                            file = aslSourcePath.FixPath(file, outerPrefix);
                            //HACER: Similar URL en c#
                            input = new URL(file).openStream();
                        }

                        //HACER: Crear clase SourcePath de runtime
                        else if (outerPrefix.StartsWith(SourcePath.CRPrefix))
                        {
                            // outer is loaded from a resource ("application".jar) file, used for java web start
                            int posSlash = outerPrefix.LastIndexOf("/");

                            //HACER: Crear clase SourcePath de runtime
                            SourcePath newpath = new SourcePath();
                            if (outerPrefix.IndexOf("/") != posSlash)
                            { // has only one slash
                                newpath.AddPath(outerPrefix.Substring(SourcePath.CRPrefix.Length + 1, posSlash));
                            }
                            newpath.AddAll(aslSourcePath);

                            file = newpath.FixPath(file, SourcePath.CRPrefix + "/");
                            input = Agent.GetResource(file.Substring(SourcePath.CRPrefix.Length)).openStream();

                        }

                        else if (outerPrefix.StartsWith("file:") || outerPrefix.StartsWith("http:") || outerPrefix.StartsWith("https:"))
                        {
                            //CAMBIAR: Similiar URL en c#
                            URL url = new URL(new URL(outerPrefix), file);
                            file = url.ToString();
                            input = url.openStream();

                        }

                        else if (file.StartsWith("jar:") || file.StartsWith("file:") || file.StartsWith("http:") || file.StartsWith("https:"))
                        {
                            URL url = new URL(file);
                            file = url.ToString();
                            input = url.openStream();
                        }

                        else
                        {
                        // get the directory of the source of the outer agent and
                        // try to find the included source in the same directory
                        // or in the source paths
                        //HACER: Crear clase SourcePath de runtime
                        SourcePath newpath = new SourcePath();
                        
                        //CAMBIAR: Similar File en c#
                        newpath.AddPath(new File(outerPrefix).GetAbsoluteFile().getParent());
                        newpath.AddAll(aslSourcePath);
                        file = newpath.FixPath(file, null);
                        input = new FileInputStream(file);
                        }   
                    }

                    else
                    {
                        input = new FileInputStream(aslSourcePath.FixPath(file, null));
                    }
                }

                // handles namespace (args[1])
                Atom ns = directive.GetNS();
                if (directive.GetArity() > 1) {
                    if (directive.GetTerm(1).IsVar()) {
                        ns = new Atom("ns"+NameSpace.GetUniqueID());
                        directive.SetTerm(1, ns);
                    }

                    else
                    {
                        if (directive.GetTerm(1).IsAtom())
                        {
                            ns = new Atom(((Atom)directive.GetTerm(1)).GetFunctor());
                        }
                    }
                }
                Agent ag = new Agent();
                ag.InitAg();

                //HACER: Terminar el parser
                as2j sparser = new as2j(input);
                sparser.SetASLSource(file);
                sparser.SetNS(ns);
                sparser.agent(ag);
            
                return ag;
            }

            catch (FileNotFoundException e) {
            
            }
            catch (Exception e) {
            
            }
        return null;
        }

        //HACER: Crear clase SourcePath de runtime
        public void SetSourcePath(SourcePath sp)
        {
            aslSourcePath = sp;
        }

        public SourcePath GetSourcePaths()
        {
            return aslSourcePath;
        }
    }
}