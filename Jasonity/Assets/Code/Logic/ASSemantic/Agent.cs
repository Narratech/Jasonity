using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.io.Reader;
import java.lang.reflect.Method;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Queue;
import java.util.Set;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.logging.Level;
import java.util.logging.Logger;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

import jason.JasonException;
import jason.RevisionFailedException;
import jason.architecture.AgArch;
import jason.architecture.MindInspectorWeb;
import jason.asSyntax.ASSyntax;
import jason.asSyntax.ArithFunctionTerm;
import jason.asSyntax.InternalActionLiteral;
import jason.asSyntax.Literal;
import jason.asSyntax.LogicalFormula;
import jason.asSyntax.Plan;
import jason.asSyntax.PlanLibrary;
import jason.asSyntax.Rule;
import jason.asSyntax.Term;
import jason.asSyntax.Trigger;
import jason.asSyntax.Trigger.TEOperator;
import jason.asSyntax.Trigger.TEType;
import jason.asSyntax.directives.FunctionRegister;
import jason.asSyntax.parser.ParseException;
import jason.asSyntax.parser.as2j;
import jason.bb.BeliefBase;
import jason.bb.DefaultBeliefBase;
import jason.bb.StructureWrapperForLiteral;
import jason.functions.Count;
import jason.functions.RuleToFunction;
import jason.mas2j.ClassParameters;
import jason.runtime.Settings;
import jason.runtime.SourcePath;
import jason.util.Config;

/**
 * The Agent class has the belief base and plan library of an
 * AgentSpeak agent. It also implements the default selection
 * functions of the AgentSpeak semantics.
 */
namespace Logica.ASSemantic
{
    public class Agent
    {
    }
}
