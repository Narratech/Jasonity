using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Util
{
    //Esto intentar quitarlo fuerte
    //Esto extiende de settings o de appsettings o similar. Lo del dictionary revisarlo por si acaso
    public class Config : Dictionary<string, string>
    {
        /** path to jason.jar */
        public static readonly string JASON_JAR     = "jasonJar";

        /** path to ant home (jar directory) */
        public static readonly string ANT_LIB       = "antLib";


        public static readonly string RUN_AS_THREAD = "runCentralisedInsideJIDE";
        public static readonly string SHELL_CMD     = "shellCommand";
        public static readonly string CLOSEALL      = "closeAllBeforeOpenMAS2J";
        public static readonly string CHECK_VERSION = "checkLatestVersion";
        public static readonly string WARN_SING_VAR = "warnSingletonVars";

        public static readonly string SHOW_ANNOTS   = "showAnnots";


        public static readonly string SHORT_UNNAMED_VARS = "shortUnnamedVars";
        public static readonly string START_WEB_MI       = "startWebMindInspector";
        public static readonly string START_WEB_EI       = "startWebEnvInspector";
        public static readonly string START_WEB_OI       = "startWebOrgInspector";

        public static readonly string NB_TH_SCH      = "numberOfThreadsForScheduler";

        public static readonly string KQML_RECEIVED_FUNCTOR   = "kqmlReceivedFunctor";
        public static readonly string KQML_PLANS_FILE         = "kqmlPlansFile";

        protected static Config singleton = null;

        protected static string configFactory = null;

        protected static bool showFixMsgs = true;

        public static void SetClassFactory(string f)
        {
            singleton = null;
            configFactory = f;
        }

        public static Config Get()
        {
            return Get(false);
        }

        public static Config Get(bool tryToFixConfig) //Esto se hace para intentar arreglar el valor del diccionario si es vacio
        {
            if (singleton == null)
            {
                if (configFactory == null)
                {
                    configFactory = typeof(Config).Name;
                }
                try
                {
                    singleton = (Config)Activator.CreateInstance(configFactory.GetType());
                    //singleton = (Config)ForName(configFactory).NewInstance();
                    //TODO: A LA WIKI
                }
                catch (Exception e)
                {
                    Debug.Log("Error creating config from " + configFactory +"("+e+"), using default.");
                    singleton = new Config();
                }
                if (!singleton.Load())
                {
                    if (tryToFixConfig)
                    {
                        singleton.Fix();
                        singleton.Store();
                    }
                }
            }
            return singleton;
        }

        protected Config() { }

        public void SetShowFixMsgs(bool b)
        {
            showFixMsgs = b;
        }


        //THis may not be needed because of unity magic
        /** returns the file where the user preferences are stored */
        /*public FileStream GetUserConfFile()
        {
            retuSystem.getProperties().get("user.home") rn new FileStream(+ File.separator + ".jason/user.properties");
            //return new File(System.getProperties().get("user.home") + File.separator + ".jason/user.properties");
        }*/

            //Unity magic
        /*public File Getlocalconffile()
        {
            return new File("jason.properties");
        }*/

            //Unity magic
            /*
        public string getFileConfComment()
        {
            return "Jason user configuration";
        }*/

        /** Returns true if the file is loaded correctly */
        public bool Load()
        {
            try
            {
                //FlieStream o FileReader o FileInfo mejor
                FileInfo f = GetLocalConfFile();//Aqui a pincho la ruta pero lo mismo no hace falta porque unity magic
                if (f.Exists())
                {
                    base.Load(new FileInputStream(f));
                    return true;
                }
                else
                {
                    f = GetUserConfFile(); //Esto igual
                    if (f.Exists())
                    {
                        //System.out.println("User config file not found, loading master: "+f.getAbsolutePath());
                        base.Load(new FileInputStream(f));
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error reading preferences");
                e.ToString();
            }
            return false;
        }

        public bool GetBool(string key)
        {
            return "true".Equals(Get(key));
        }

        /** Returns the full path to the jason.jar file */
        public string GetJasonJar()
        {
            return GetProperty(JASON_JAR);
        }

        /** returns the jason home (based on jason.jar) */
        public string getJasonHome()
        {
            try
            {
                return new File(getJasonJar()).getParentFile().getParent();
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
            return "";
        }
        

        /** Returns the path to the ant home directory (where its jars are stored) */
        public string getAntLib()
        {
            string result = null;
            TryGetValue(ANT_LIB, out result); //Esto en vez de los [] WIKIIIII
            return result;
        }

        public string getAntJar()
        {
            string ant = getAntLib();
            if (ant != null)
            {
                ant = findJarInDirectory(new File(ant), "ant-launcher");
                if (ant != null)
                {
                    File fAnt = new File(ant);
                    if (fAnt.exists())
                        return fAnt.GetName();
                }
            }

            return null;
        }
        
        public void SetAntLib(string al)
        {
            if (al != null)
            {
                al = new File(al).getAbsolutePath();
                if (!al.EndsWith(File.separator))
                {
                    al += File.separator;
                }
                Put(ANT_LIB, al);
            }
        }

        public string GetShellCommand()
        {
            return GetProperty(SHELL_CMD);
        }

        public string GetKqmlFunctor()
        {
            return GetProperty(KQML_RECEIVED_FUNCTOR, Message.kqmlReceivedFunctor);
        }
        public string GetKqmlPlansFile()
        {
            return GetProperty(KQML_PLANS_FILE, Message.kqmlDefaultPlans);
        }

        public void ResetSomeProps()
        {
            //System.out.println("Reseting configuration of "+Config.JASON_JAR);
            Remove(JASON_JAR);
            //System.out.println("Reseting configuration of "+Config.JADE_JAR);
            Remove(JADE_JAR);
            //System.out.println("Reseting configuration of "+Config.ANT_LIB);
            Remove(ANT_LIB);
            Put(SHOW_ANNOTS, "false");
        }

        /** Set most important parameters with default values */
        public void Fix()
        {
            TryToFixJarFileConf(JASON_JAR, "jason", 700000);

            // fix ant lib
            if (Get(ANT_LIB) == null || !CheckAntLib(GetAntLib()))
            {
                try
                {
                    string jjar = GetJasonJar();
                    if (jjar != null)
                    {
                        string antlib = new File(jjar).getParentFile().GetParentFile().GetAbsolutePath() + File.separator + "libs";
                        if (CheckAntLib(antlib))
                        {
                            SetAntLib(antlib);
                        }
                        else
                        {
                            antlib = new File(".") + File.separator + "libs";
                            if (CheckAntLib(antlib))
                            {
                                SetAntLib(antlib);
                            }
                            else
                            {
                                antlib = new File("..") + File.separator + "libs";
                                if (CheckAntLib(antlib))
                                {
                                    SetAntLib(antlib);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Error setting ant lib!");
                    e.ToString();
                }
            }

            // font
            if (Get("font") == null)
            {
                Put("font", "Monospaced");
            }
            if (Get("fontSize") == null)
            {
                Put("fontSize", "14");
            }

            // shell command
            if (Get(SHELL_CMD) == null)
            {
                if (System.GetProperty("os.name").StartsWith("Windows 9"))
                {
                    Put(SHELL_CMD, "command.com /e:1024 /c ");
                }
                else if (System.GetProperty("os.name").IndexOf("indows") > 0)
                {
                    Put(SHELL_CMD, "cmd /c ");
                }
                else
                {
                    Put(SHELL_CMD, "/bin/sh ");
                }
            }

            // close all
            if (Get(CLOSEALL) == null)
            {
                Put(CLOSEALL, "true");
            }

            if (Get(CHECK_VERSION) == null)
            {
                Put(CHECK_VERSION, "true");
            }

            // show annots
            if (GetProperty(SHOW_ANNOTS) == null)
            {
                Put(SHOW_ANNOTS, "true");
            }

            if (GetProperty(START_WEB_MI) == null)
            {
                Put(START_WEB_MI, "true");
            }

            if (GetProperty(NB_TH_SCH) == null)
            {
                Put(NB_TH_SCH, "2");
            }

            if (GetProperty(SHORT_UNNAMED_VARS) == null)
            {
                Put(SHORT_UNNAMED_VARS, "true");
            }

            if (GetProperty(KQML_RECEIVED_FUNCTOR) == null)
            {
                Put(KQML_RECEIVED_FUNCTOR, Message.kqmlReceivedFunctor);
            }

            if (GetProperty(KQML_PLANS_FILE) == null)
            {
                Put(KQML_PLANS_FILE, Message.kqmlDefaultPlans);
            }

            // Default infrastructures
            SetDefaultInfra();
        }

        private void SetDefaultInfra()
        {
            Put("infrastructure.Centralised", typeof(CentralisedFactory).Name);
        }

        public void Store()
        {
            Store(GetUserConfFile());
        }

        public void Store(File f)
        {
            try
            {
                if (!f.GetParentFile().exists())
                {
                    f.GetParentFile().mkdirs();
                }
                Debug.Log("Storing configuration at " + f.GetAbsolutePath());
                base.Store(new FileOutputStream(f), getFileConfComment());
            }
            catch (Exception e)
            {
               Debug.Log("Error writting preferences");
               e.ToString();
            }
        }

        public string[] GetAvailableInfrastructures()
        {
            try
            {
                List<string> infras = new List<string>();
                infras.Add("Centralised"); // set Centralised as the first
                foreach (object k in keySet())
                {
                    string sk = k.ToString();
                    int p = sk.IndexOf(".");
                    if (p > 0 && sk.StartsWith("infrastructure") && p == sk.LastIndexOf("."))
                    { // only one "."
                        string newinfra = sk.Substring(p + 1);
                        if (!infras.Contains(newinfra))
                        {
                            infras.Add(newinfra);
                        }
                    }
                }
                if (infras.Count > 0)
                {
                    // copy infras to a array
                    string[] r = new string[infras.Count];
                    for (int i = 0; i < r.Length; i++)
                    {
                        r[i] = infras[i];
                    }
                    return r;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error getting user infrastructures.");
            }
            return new string[] { "Centralised", "Jade" }; //,"JaCaMo"};
        }

        public string GetInfrastructureFactoryClass(string infraId)
        {
            object oClass = Get("infrastructure." + infraId);
            if (oClass == null)
            {
                // try to fix using default configuration
                SetDefaultInfra();
                oClass = Get("infrastructure." + infraId);
            }
            return oClass.ToString();
        }

        public void SetInfrastructureFactoryClass(string infraId, string factory)
        {
            Put("infrastructure." + infraId, factory);
        }
        public void RemoveInfrastructureFactoryClass(string infraId)
        {
            Remove("infrastructure." + infraId);
        }

        public string GetJasonVersion()
        {
            Package j = Package.getPackage("jason.util");
            if (j != null && j.getSpecificationVersion() != null)
            {
                return j.getSpecificationVersion();
            }
            return "";
        }

        public string GetJasonBuiltDate()
        {
            Package j = Package.getPackage("jason.util");
            if (j != null)
            {
                return j.getImplementationVersion();
            }
            return "?";
        }

        public bool TryToFixJarFileConf(string jarEntry, string jarFilePrefix, int minSize)
        {
            string jarFile = GetProperty(jarEntry);
            if (jarFile == null || !CheckJar(jarFile, minSize))
            {
                if (showFixMsgs)
                    Debug.Log("Wrong configuration for " + jarFilePrefix + ", current is " + jarFile);

                // try to get from classpath (the most common case)
                jarFile = GetJarFromClassPath(jarFilePrefix);
                if (CheckJar(jarFile, minSize))
                {
                    Put(jarEntry, jarFile);
                    if (showFixMsgs)
                        Debug.Log("found at " + jarFile + " by classpath");
                    return true;
                }

                // try eclipse installation
                jarFile = GetJarFromEclipseInstallation(jarFilePrefix);
                if (CheckJar(jarFile, minSize))
                {
                    Put(jarEntry, jarFile);
                    if (showFixMsgs)
                        Debug.Log("found at " + jarFile + " in eclipse installation");
                    return true;
                }

                // try from java web start
                string jwsDir = System.GetProperty("jnlpx.deployment.user.home");
                if (jwsDir == null)
                {
                    // try another property (windows)
                    try
                    {
                        jwsDir = System.GetProperty("deployment.user.security.trusted.certs");
                        jwsDir = new File(jwsDir).getParentFile().getParent();
                    }
                    catch (Exception e)
                    {
                    }
                }
                if (jwsDir != null)
                {
                    jarFile = FindFile(new File(jwsDir), jarFilePrefix, minSize);
                    if (showFixMsgs)
                        Debug.Log("Searching " + jarFilePrefix + " in " + jwsDir + " ... ");
                    if (jarFile != null && CheckJar(jarFile))
                    {
                        if (showFixMsgs)
                           Debug.Log("found at " + jarFile);
                        Put(jarEntry, jarFile);
                        return true;
                    }
                    else
                    {
                        Put(jarEntry, File.separator);
                    }
                }
                if (showFixMsgs)
                    Debug.Log(jarFilePrefix + " not found");
                return false;
            }
            return true;
        }

        static string FindFile(File p, string file, int minSize)
        {
            if (p.IsDirectory())
            {
                File[] files = p.ListFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].IsDirectory())
                    {
                        string r = FindFile(files[i], file, minSize);
                        if (r != null)
                        {
                            return r;
                        }
                    }
                    else
                    {
                        if (files[i].GetName().endsWith(file) && files[i].Length > minSize)
                        {
                            return files[i].GetAbsolutePath();
                        }
                    }
                }
            }
            return null;
        }

        public static string FindJarInDirectory(File dir, string prefix)
        {
            if (dir.IsDirectory())
            {
                foreach (File f in dir.ListFiles())
                {
                    if (f.GetType().Name.StartsWith(prefix) && f.GetType().Name.EndsWith(".jar") && 
                        !f.GetType().Name.EndsWith("-sources.jar") && !f.GetType().Name.EndsWith("-javadoc.jar"))
                    {
                        return f.GetAbsolutePath();
                    }
                }
            }
            return null;
        }

        public static bool CheckJar(string jar)
        {
            try
            {
                return jar != null && new File(jar).exists() && jar.EndsWith(".jar");
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public static bool CheckJar(string jar, int minSize)
        {
            try
            {
                return CheckJar(jar) && new File(jar).length() > minSize;
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public static bool CheckAntLib(string al)
        {
            try
            {
                if (!al.EndsWith(File.separator))
                {
                    al = al + File.separator;
                }
                if (FindJarInDirectory(new File(al), "ant") != null) // new File(al + "ant.jar");
                    return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public static bool IsWindows()
        {
            return System.GetProperty("os.name").StartsWith("Windows");
        }

        static protected string GetJarFromClassPath(string file)
        {
            StringTokenizer st = new StringTokenizer(System.GetProperty("java.class.path"), File.pathSeparator);
            while (st.hasMoreTokens())
            {
                string token = st.nextToken();
                File f = new File(token);
                if (f.GetType().Name.StartsWith(file) && DigitAfterMinus(f.GetType().Name)
                    && f.GetType().Name.EndsWith(".jar") 
                    && !f.GetType().Name.EndsWith("-sources.jar") && !f.GetType().Name.EndsWith("-javadoc.jar"))
                {
                    return f.GetAbsolutePath();
                }
            }
            return null;
        }

        private static bool DigitAfterMinus(string s)
        {
            int pos = s.IndexOf("-");
            return pos > 0 && char.IsDigit(s.Substring(pos + 1, pos + 2).ElementAt(0));
        }

        public string GetTemplate(string templateName)
        {
            try
            {
                if (templateName.Equals("agent.asl"))
                    templateName = "agent";
                if (templateName.Equals("project.mas2j"))
                    templateName = "project";

                string nl = System.getProperty("line.separator");
                // get template
                TextReader @in;

                // if there is jason/src/xml/build-template.xml, use it; otherwise use the file in jason.jar
                File bt = new File("src/templates/" + templateName);
                if (bt.Exists())
                {
                    @in = new TextReader(new FileReader(bt));
                }
                else
                {
                    bt = new File("../src/templates/" + templateName);
                    if (bt.Exists())
                    {
                        @in = new BufferedReader(new FileReader(bt));
                    }
                    else
                    {
                        bt = new File(getHome() + "/src/templates/" + templateName);
                        if (bt.Exists())
                        {
                            @in = new TextReader(new FileReader(bt));
                        }
                        else
                        {
                            bt = new File(getHome() + "/src/main/resources/templates/" + templateName);
                            if (bt.Exists())
                            {
                                @in = new TextReader(new FileReader(bt));
                            }
                            else
                            {
                                @in = new TextReader(new InputStreamReader(getDetaultResource(templateName)));
                            }
                        }
                    }
                }

                StringBuilder scriptBuf = new StringBuilder();
                string line = @in.ReadLine();
                while (line != null)
                {
                    scriptBuf.Append(line + nl);
                    line = @in.ReadLine();
                }
                return scriptBuf.ToString();
            }
            catch (Exception e)
            {
                Debug.Log("Error reading template: " + templateName);
                e.ToString();
                return null;
            }
        }

        protected string getHome()
        {
            return getJasonHome();
        }

        public StreamReader getDetaultResource(string templateName)
        {
            return Reasoner.GetResource("/templates/"+templateName).openStream();
        }

        public static void Main(string[] args)
        {
            Get().Fix();
            Get().Store();
        }

        public string GetPresentation()
        {
            return "Jason " + GetJasonVersion() + "\n" +
                   "     built on " + GetJasonBuiltDate() + "\n" +
                   "     installed at " + getHome();
        }
    }
}
