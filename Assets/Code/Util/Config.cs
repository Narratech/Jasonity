using Assets.Code.ReasoningCycle;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Util
{
    public class Config
    {
        /** path to jason.jar */
        public static readonly string JASON_JAR     = "jasonJar";

        /** path to ant home (jar directory) */
        public static readonly string ANT_LIB       = "antLib";

        /** path to jade.jar */
        public static readonly string JADE_JAR      = "jadeJar";

        /** runtime jade arguments (the same used in jade.Boot) */
        public static readonly string JADE_ARGS     = "jadeArgs";

        /** boolean, whether to start jade RMA or not */
        public static readonly string JADE_RMA      = "jadeRMA";

        /** boolean, whether to start jade Sniffer or not */
        public static readonly string JADE_SNIFFER  = "jadeSniffer";

        /** path to java home */
        public static readonly string JAVA_HOME = "javaHome";

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

        public static Config Get(bool tryToFixConfig)
        {
            if (singleton == null)
            {
                if (configFactory == null)
                {
                    configFactory = GetType().Name;
                }
                try
                {
                    singleton = (Config)ForName(configFactory).NewInstance();
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

        /** returns the file where the user preferences are stored */
        //public File getUserConfFile()
        //{
        //    return new File(System.getProperties().get("user.home") + File.separator + ".jason/user.properties");
        //}

        //public File getLocalConfFile()
        //{
        //    return new File("jason.properties");
        //}

        public string getFileConfComment()
        {
            return "Jason user configuration";
        }

        /** Returns true if the file is loaded correctly */
        public bool load()
        {
            try
            {
                File f = GetLocalConfFile();
                if (f.Exists())
                {
                    base.load(new FileInputStream(f));
                    return true;
                }
                else
                {
                    f = getUserConfFile();
                    if (f.exists())
                    {
                        //System.out.println("User config file not found, loading master: "+f.getAbsolutePath());
                        base.load(new FileInputStream(f));
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

        /** Returns the full path to the jade.jar file */
        public string GetJadeJar()
        {
            string r = GetProperty(JADE_JAR);
            if (r == null)
            {
                TryToFixJarFileConf(JADE_JAR, "jade", 2000000);
                r = GetProperty(JADE_JAR);
            }
            return r;
        }

        /** Return the jade args (those used in jade.Boot) */
        public string GetJadeArgs()
        {
            string a = GetProperty(JADE_ARGS);
            return a == null ? "" : a;
        }

        public string[] getJadeArrayArgs()
        {
            List<string> ls = new List<string>();
            string jadeargs = GetProperty(JADE_ARGS);
            if (jadeargs != null && jadeargs.Length > 0)
            {
                StringTokenizer t = new StringTokenizer(jadeargs);
                while (t.hasMoreTokens())
                {
                    ls.Add(t.nextToken());
                }
            }
            string[] @as = new string[ls.Count];
            for (int i = 0; i < ls.Count; i++)
            {
                @as[i] = ls[i];
            }
            return @as;
        }

        /** Returns the path to the java  home directory */
        public string getJavaHome()
        {
            string h = GetProperty(JAVA_HOME);
            if (!h.EndsWith(File.separator))
                h += File.separator;
            return h;
        }

        /** Returns the path to the ant home directory (where its jars are stored) */
        public string getAntLib()
        {
            return GetProperty(ANT_LIB);
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

        public void SetJavaHome(string jh)
        {
            if (jh != null)
            {
                jh = new File(jh).getAbsolutePath();
                if (!jh.EndsWith(File.separator))
                {
                    jh += File.separator;
                }
                Put(JAVA_HOME, jh);
            }
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
        public string getKqmlPlansFile()
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

            // fix java home
            if (Get(JAVA_HOME) == null || !CheckJavaHomePath(GetProperty(JAVA_HOME)))
            {
                string javaHome = System.getProperty("java.home");
                if (CheckJavaHomePath(javaHome))
                {
                    SetJavaHome(javaHome);
                }
                else
                {
                    string javaEnvHome = System.getenv("JAVA_HOME");
                    if (javaEnvHome != null && CheckJavaHomePath(javaEnvHome))
                    {
                        SetJavaHome(javaEnvHome);
                    }
                    else
                    {
                        string javaHomeUp = javaHome + File.separator + "..";
                        if (CheckJavaHomePath(javaHomeUp))
                        {
                            SetJavaHome(javaHomeUp);
                        }
                        else
                        {
                            // try JRE
                            if (CheckJREHomePath(javaHome))
                            {
                                SetJavaHome(javaHome);
                            }
                            else
                            {
                                SetJavaHome(File.separator);
                            }
                        }
                    }
                }
            }

            // fix ant lib
            if (Get(ANT_LIB) == null || !CheckAntLib(getAntLib()))
            {
                try
                {
                    string jjar = GetJasonJar();
                    if (jjar != null)
                    {
                        string antlib = new File(jjar).getParentFile().getParentFile().getAbsolutePath() + File.separator + "libs";
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
                if (System.getProperty("os.name").startsWith("Windows 9"))
                {
                    Put(SHELL_CMD, "command.com /e:1024 /c ");
                }
                else if (System.getProperty("os.name").indexOf("indows") > 0)
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

            // jade args
            if (GetProperty(JADE_RMA) == null)
            {
                Put(JADE_RMA, "true");
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

        private void setDefaultInfra()
        {
            Put("infrastructure.Centralised", CentralisedFactory./*class*/.getName());
            Put("infrastructure.Jade", JadeFactory./*class*/.getName());
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
                        String newinfra = sk.Substring(p + 1);
                        if (!infras.Contains(newinfra))
                        {
                            infras.Add(newinfra);
                        }
                    }
                }
                if (infras.Count > 0)
                {
                    // copy infras to a array
                    String[] r = new string[infras.Count];
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

        public string getInfrastructureFactoryClass(string infraId)
        {
            object oClass = Get("infrastructure." + infraId);
            if (oClass == null)
            {
                // try to fix using default configuration
                setDefaultInfra();
                oClass = Get("infrastructure." + infraId);
            }
            return oClass.ToString();
        }

        public void SetInfrastructureFactoryClass(string infraId, string factory)
        {
            Put("infrastructure." + infraId, factory);
        }
        public void removeInfrastructureFactoryClass(string infraId)
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

        public bool tryToFixJarFileConf(string jarEntry, string jarFilePrefix, int minSize)
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
                String jwsDir = System.getProperty("jnlpx.deployment.user.home");
                if (jwsDir == null)
                {
                    // try another property (windows)
                    try
                    {
                        jwsDir = System.getProperty("deployment.user.security.trusted.certs");
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
                        if (files[i].GetName().endsWith(file) && files[i].Length() > minSize)
                        {
                            return files[i].GetAbsolutePath();
                        }
                    }
                }
            }
            return null;
        }

        public static string findJarInDirectory(File dir, string prefix)
        {
            if (dir.IsDirectory())
            {
                foreach (File f in dir.ListFiles())
                {
                    if (f.GetName().startsWith(prefix) && f.GetName().endsWith(".jar") && 
                        !f.GetName().endsWith("-sources.jar") && !f.GetName().endsWith("-javadoc.jar"))
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

        public static bool checkJar(string jar, int minSize)
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

        public static bool checkJavaHomePath(string javaHome)
        {
            try
            {
                if (!javaHome.EndsWith(File.separator))
                {
                    javaHome += File.separator;
                }
                File javac1 = new File(javaHome + "bin" + File.separatorChar + "javac");
                File javac2 = new File(javaHome + "bin" + File.separatorChar + "javac.exe");
                if (javac1.Exists() || javac2.Exists())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public static bool checkJREHomePath(string javaHome)
        {
            try
            {
                if (!javaHome.EndsWith(File.separator))
                {
                    javaHome += File.separator;
                }
                File javac1 = new File(javaHome + "bin" + File.separatorChar + "java");
                File javac2 = new File(javaHome + "bin" + File.separatorChar + "java.exe");
                if (javac1.Exists() || javac2.Exists())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public static bool CheckAntLib(ctring al)
        {
            try
            {
                if (!al.EndsWith(File.separator))
                {
                    al = al + File.separator;
                }
                if (findJarInDirectory(new File(al), "ant") != null) // new File(al + "ant.jar");
                    return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public static bool IsWindows()
        {
            return System.GetProperty("os.name").startsWith("Windows");
        }

        static protected string GetJarFromClassPath(string file)
        {
            StringTokenizer st = new StringTokenizer(System.GetProperty("java.class.path"), File.pathSeparator);
            while (st.hasMoreTokens())
            {
                String token = st.nextToken();
                File f = new File(token);
                if (f.GetName().startsWith(file) && DigitAfterMinus(f.GetName()) && f.GetName().endsWith(".jar") 
                    && !f.GetName().endsWith("-sources.jar") && !f.GetName().endsWith("-javadoc.jar"))
                {
                    return f.GetAbsolutePath();
                }
            }
            return null;
        }

        private static bool digitAfterMinus(string s)
        {
            int pos = s.IndexOf("-");
            return pos > 0 && char.IsDigit(s.Substring(pos + 1, pos + 2).CharAt(0));
        }

        protected string getEclipseInstallationDirectory()
        {
            return "jason";
        }

        private string getJarFromEclipseInstallation(string file)
        {
            string eclipse = System.getProperty("eclipse.launcher");
            //eclipse = "/Applications/eclipse/eclipse";
            if (eclipse != null)
            {
                File f = (new File(eclipse)).getParentFile().getParentFile();
                if (eclipse.contains("Eclipse.app/Contents")) // MacOs case
                    f = f.GetParentFile().getParentFile();
                return findJarInDirectory(new File(f + "/" + GetEclipseInstallationDirectory() + "/libs"), file);
            }
            return null;
        }

        public string getTemplate(string templateName)
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

        public static void main(string[] args)
        {
            Get().Fix();
            Get().Store();
        }

        public string GetMindInspectorArchClassName()
        {
            return "jason.architecture.MindInspectorAgArch";
        }

        public string GetMindInspectorWebServerClassName()
        {
            return "jason.architecture.MindInspectorWebImpl";
        }

        public string getPresentation()
        {
            return "Jason " + GetJasonVersion() + "\n" +
                   "     built on " + GetJasonBuiltDate() + "\n" +
                   "     installed at " + getHome();
        }
    }
}
