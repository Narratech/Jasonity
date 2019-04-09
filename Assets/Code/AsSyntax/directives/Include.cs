


using Assets.Code.AsSyntax;
using Assets.Code.Logic.parser;
using System;
using System.IO;

namespace Assets.Code.Logic.AsSyntax.directives
{ 
    public partial class Include : DefaultDirective, IDirective {

        //HACER: Clase SourcePath de runtime
        private SourcePath aslSourcePath = new SourcePath();

        public Agent.Agent Process(Pred directive, Agent.Agent outerContent, Agent.Agent innerContent)
        {
            if (outerContent == null)
                return null;

            // handles file (arg[0])
            string file = ((IStringTerm)directive.GetTerm(0)).GetString().replaceAll("\\\\", "/");
            try
            {
                //CAMBIAR: similar c# de InputStream
                InputStream in = null;
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
                    in = new URL(file).openStream();
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
                            file = aslSourcePath.fixPath(file, outerPrefix);
                            //HACER: Similar URL en c#
                            in = new URL(file).openStream();
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
                                newpath.addPath(outerPrefix.Substring(SourcePath.CRPrefix.length() + 1, posSlash));
                            }
                            newpath.addAll(aslSourcePath);

                            file = newpath.fixPath(file, SourcePath.CRPrefix + "/");
                            in = Agent.Agent.GetResource(file.Substring(SourcePath.CRPrefix.length())).openStream();

                        }

                        else if (outerPrefix.StartsWith("file:") || outerPrefix.StartsWith("http:") || outerPrefix.StartsWith("https:"))
                        {
                            //CAMBIAR: Similiar URL en c#
                            URL url = new URL(new URL(outerPrefix), file);
                            file = url.ToString();
                            in = url.openStream();

                        }

                        else if (file.StartsWith("jar:") || file.StartsWith("file:") || file.StartsWith("http:") || file.StartsWith("https:"))
                        {
                            URL url = new URL(file);
                            file = url.ToString();
                            in = url.openStream();
                        }

                        else
                        {
                        // get the directory of the source of the outer agent and
                        // try to find the included source in the same directory
                        // or in the source paths
                        //HACER: Crear clase SourcePath de runtime
                        SourcePath newpath = new SourcePath();
                        
                        //CAMBIAR: Similar File en c#
                        newpath.addPath(new File(outerPrefix).getAbsoluteFile().getParent());
                        newpath.addAll(aslSourcePath);
                        file = newpath.fixPath(file, null);
                        in = new FileInputStream(file);
                        }   
                    }

                    else
                    {
                        in = new FileInputStream(aslSourcePath.fixPath(file, null));
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
                Agent.Agent ag = new Agent.Agent();
                ag.InitAg();

                //HACER: Terminar el parser
                as2j sparser = new as2j(in);
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