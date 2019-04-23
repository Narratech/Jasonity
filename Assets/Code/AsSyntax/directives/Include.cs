using Assets.Code.BDIAgent;
using Assets.Code.parser;
using Assets.Code.Runtime;
using Assets.Code.Util;
using System;
using System.IO;
using UnityEngine;

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
                TextReader input = null; //Es posible que haya que cambiarlo a streamreader
                // test include from jar
                if (file.StartsWith("$"))
                { // the case of "$jasonJar/src/a.asl"
                    string jar = file.Substring(1, file.IndexOf("/"));
                    string result; 
                    if (Config.Get().TryGetValue(jar, out result)) //esto mirarlo con la calma
                    {
                        return null;
                    }

                    Config.Get().TryGetValue(jar, out result);
                    string path =  result.ToString();
                    file = "jar:file:" + path + "!" + file.Substring(file.IndexOf("/")); //este file tiene que ser una ruta al archivo asl
                    //CAMBIAR: Similar URL en c# Existe una clase URI pero es mas facil con string
                    //input = new URL(file).openStream(); //WIKIIIIII
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
                            //HACER: Similar URL en c#
                            input = File.OpenText(file);
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
                            /*Esto esta buscando en el paquete concreto de bdi agent*/
                            //En input tenemos que acabar abriendo un textreader
                            //Tenemos que ver exactamente donde esta esta cosa
                            //input = Agent.GetResource(file.Substring(SourcePath.CRPrefix.Length)).openStream();//Mirar los resources de unity por si acaso hay algo
                            //Esto hay que hacerlo con unity porque esto es raruno
                        }

                        else if (outerPrefix.StartsWith("file:") || outerPrefix.StartsWith("http:") || outerPrefix.StartsWith("https:"))
                        {
                            //CAMBIAR: Similiar URL en c#
                            URL url = new URL(new URL(outerPrefix), file);
                            file = url.ToString();
                            input = File.OpenText(file);.openStream(); //Mirar arriba

                        }

                        else if (file.StartsWith("jar:") || file.StartsWith("file:") || file.StartsWith("http:") || file.StartsWith("https:"))
                        {
                            URL url = new URL(file);
                            file = url.ToString(); //Mirar arriba
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
                        input = new FileInputStream(aslSourcePath.FixPath(file, null)); //mirar arriba
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