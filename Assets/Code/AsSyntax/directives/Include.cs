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

        public override Agent Process(Pred directive, Agent outerContent, Agent innerContent)
        {
            if (outerContent == null)
                return null;

            // handles file (arg[0])
            string file = ((IStringTerm)directive.GetTerm(0)).GetString().Replace("\\\\", "/");
            try
            {
                TextReader input = null; //Es posible que haya que cambiarlo a streamreader
                // test include from jar
                if (file.StartsWith("$"))
                { // the case of "$jasonJar/src/a.asl"
                    string jar = file.Substring(1, file.IndexOf("/"));
                    string result; 
                    if (Config.Get().TryGetValue(jar, out result))
                    {
                        return null;
                    }

                    Config.Get().TryGetValue(jar, out result);
                    string path =  result.ToString();
                    file = "jar:file:" + path + "!" + file.Substring(file.IndexOf("/"));
                    //este file tiene que ser una ruta al archivo asl
                    //input = new URL(file).openStream();
                    input = File.OpenText(file);
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
                            input = File.OpenText(file);
                        }

                        else if (outerPrefix.StartsWith(SourcePath.CRPrefix))
                        {
                            // outer is loaded from a resource ("application".jar) file, used for java web start
                            int posSlash = outerPrefix.LastIndexOf("/");
                            SourcePath newpath = new SourcePath();
                            if (outerPrefix.IndexOf("/") != posSlash)
                            { // has only one slash
                                newpath.AddPath(outerPrefix.Substring(SourcePath.CRPrefix.Length + 1, posSlash));
                            }
                            newpath.AddAll(aslSourcePath);

                            file = newpath.FixPath(file, SourcePath.CRPrefix + "/");
                            /*Esto esta buscando en el paquete concreto de bdi agent*/
                            //En input tenemos que acabar abriendo un textreader
                            //Tenemos que ver exactamente donde esta esta cosa
                            //Esto para hacerlo bien Unity hay que usar el sistema de getión de recursos de Unity: Resource.Load(string); 
                            //input = Agent.GetResource(file.Substring(SourcePath.CRPrefix.Length)).openStream();//Mirar los resources de unity por si acaso hay algo
                            input = new StringReader(file.Substring(SourcePath.CRPrefix.Length));
                            //Esto hay que hacerlo con unity porque esto es raruno
                        }

                        else if (outerPrefix.StartsWith("file:") || outerPrefix.StartsWith("http:") || outerPrefix.StartsWith("https:"))
                        {
                            //URL url = new URL(new URL(outerPrefix), file);
                            //file = url.ToString();
                            file = outerPrefix + file;
                            input = File.OpenText(file);

                        }

                        else if (file.StartsWith("jar:") || file.StartsWith("file:") || file.StartsWith("http:") || file.StartsWith("https:"))
                        {
                            //URL url = new URL(file);
                            //file = url.ToString(); //Mirar arriba
                            //input = url.openStream();
                            file = outerPrefix + file;
                            input = File.OpenText(file);
                        }

                        else
                        {
                        // get the directory of the source of the outer agent and
                        // try to find the included source in the same directory
                        // or in the source paths
                        SourcePath newpath = new SourcePath();
                        newpath.AddPath(Path.GetFullPath((new StreamReader(outerPrefix)).ToString())); //Hay que mirar que era el original
                                           // new StreamReader(outerPrefix).GetAbsoluteFile().getParent()
                        newpath.AddAll(aslSourcePath);
                        file = newpath.FixPath(file, null);
                        input = new StreamReader(file);
                        }   
                    }

                    else
                    {
                        input = new StreamReader(aslSourcePath.FixPath(file, null)); 
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