using Assets.Code.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/** MAS Runtime Settings for an Agent (from mas2j file, agent declaration) */
namespace Assets.Code.Runtime
{
    public class Settings
    {
        public static readonly byte ODiscard = 1;
        public static readonly byte ORequeue = 2;
        public static readonly byte ORetrieve = 3;
        public static readonly bool OSameFocus = true;
        public static readonly bool ONewFocus = false;
        public static readonly int ODefaultNRC = 1;
        public static readonly int ODefaultVerbose = -1;
        public static readonly bool ODefaultSync = false;


        private byte events = ODiscard;
        private bool intBels = OSameFocus;
        private int nrcbp = ODefaultNRC;
        private int verbose = ODefaultVerbose;
        private bool sync = ODefaultSync;
        private bool qCache = false; // whether to use query cache
        private bool qProfiling = false; // whether has query profiling
        private bool troON = true; // tail recursion optimisation is on by default

        private Dictionary<string, object> userParameters = new Dictionary<string, object>();

        public static readonly string PROJECT_PARAMETER = "project-parameter";
        public static readonly string INIT_BELS  = "beliefs";
        public static readonly string INIT_GOALS = "goals";
        public static readonly string MIND_INSPECTOR = "mindinspector";
        
        public Settings()
        {
        }

        public Settings(string options)
        {
            SetOptions(options);
        }

        public void SetOptions(string options)
        {
            as2j parser = new as2j(new StringReader(options));
            try
            {
                SetOptions(parser.ASoptions());
                Debug.Log("Sttings are " + userParameters);
            }
            catch (Exception e)
            {
                Debug.Log("Error parseing options " + options);
            }
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            if (options == null)
                return;
            userParameters = options;

            foreach (string key in options.Keys)
            {
                if (key.Equals("events"))
                {
                    string events = (string)options["events"];
                    if (events.Equals("discard"))
                    {
                        SetEvents(ODiscard);
                    }
                    else if (events.Equals("requeue"))
                    {
                        SetEvents(ORequeue);
                    }
                    else if (events.Equals("retrieve"))
                    {
                        SetEvents(ORetrieve);
                    }
                }
                else if (key.Equals("intBels"))
                {
                    string intBels = (string)options["intBels"];
                    if (intBels.Equals("sameFocus"))
                    {
                        SetIntBels(OSameFocus);
                    }
                    else if (intBels.Equals("newFocus"))
                    {
                        SetIntBels(ONewFocus);
                    }
                }
                else if (key.Equals("nrcbp"))
                {
                    string nrc = (string)options["nrcbp"];
                    SetNRCBP(nrc);
                }
                else if (key.Equals("verbose"))
                {
                    string verbose = (string)options["verbose"];
                    SetVerbose(verbose);
                }
                else if (key.Equals("synchronised"))
                {
                    SetSync("true".Equals((string)options["synchronised"]));
                }
                else if (key.Equals("tro"))
                {
                    SetTRO("true".Equals((string)options["tro"]));
                }
                else if (key.Equals("qcache"))
                {
                    SetQueryCache("cycle".Equals((string)options["qcache"]));
                }
                else if (key.Equals("qprofiling"))
                {
                    SetQueryProfiling("yes".Equals((string)options["qprofiling"]));
                }
                else
                {
                    //userParameters.put(key, options.get(key));
                }
            }
        }

        /** add user defined option */
        public void AddOption(string key, object value)
        {
            //userParameters.Put(key, value);
            userParameters.Add(key, value);
        }

        public void SetEvents(byte opt)
        {
            events = opt;
        }

        public void SetIntBels(bool opt)
        {
            intBels = opt;
        }

        public void SetNRCBP(string opt)
        {
            try
            {
                SetNRCBP(int.Parse(opt)/*Integer.parseInt(opt)*/);
            }
            catch (Exception e)
            {
                Debug.Log("Error " + e);
            }
        }

        public void SetNRCBP(int opt)
        {
            nrcbp = opt;
        }

        public void SetVerbose(string opt)
        {
            try
            {
                SetVerbose(int.Parse(opt));
            }
            catch (Exception e)
            {
                Debug.Log("Error " + e);
            }
        }

        public void SetVerbose(int opt)
        {
            verbose = opt;
        }

        public bool Discard()
        {
            return events == ODiscard;
        }

        public bool Requeue()
        {
            return events == ORequeue;
        }

        public bool Retrieve()
        {
            return events == ORetrieve;
        }

        public bool SameFocus()
        {
            return (intBels);
        }

        public bool NewFocus()
        {
            return (!intBels);
        }

        public int Nrcbp()
        {
            return nrcbp;
        }

        public int Verbose()
        {
            return verbose;
        }

        /** returns true if the execution is synchronised */
        public bool IsSync()
        {
            return sync;
        }

        public void SetSync(bool pSync)
        {
            sync = pSync;
        }

        public bool IsTROon()
        {
            return troON;
        }
        public void SetTRO(bool tro)
        {
            troON = tro;
        }

        public bool HasQueryCache()
        {
            return qCache;
        }
        public void SetQueryCache(bool b)
        {
            qCache = b;
        }

        public bool HasQueryProfiling()
        {
            return qProfiling;
        }
        public void SetQueryProfiling(bool b)
        {
            qProfiling = b;
        }

        public Dictionary<string, object> GetUserParameters()
        {
            return userParameters;
        }

        public string GetUserParameter(string key)
        {
            string vl = (string)userParameters[key];
            if (vl != null && vl.StartsWith("\"") && vl.EndsWith("\""))
            {
                vl = vl.Substring(1, vl.Length - 1);
                vl = vl.Replace("\\\\\"", "\"");
            }
            return vl;
        }

        public object RemoveUserParameter(string key)
        {
            return userParameters.Remove(key);
        }
    }
}
