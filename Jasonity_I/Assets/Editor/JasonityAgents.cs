using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JasonityAgents : EditorWindow
{
    string agentsPath;
    bool groupEnabled, groupEnabled2;
    bool myBool = true, myBool2 = false;
    float myFloat = 1.23f, myFloat2 = 1.70f;
    int agentCounter = 0;
    
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Jasonity - Agents")]

    
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(JasonityAgents));
    }
    
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        for(int i = 0; i < 4; i++){
            string myString = "Agente 0" + i;
            GUILayout.Label(myString, EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Agent Name", myString);
            //GUILayout.Button("Rename");
            groupEnabled = EditorGUILayout.BeginToggleGroup ("Generate on Scene", groupEnabled);
                myBool = EditorGUILayout.Toggle ("  Fast Thinking", myBool);
                myFloat = EditorGUILayout.Slider ("  Heigth", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();
            if(GUILayout.Button("Edit")){
                string lPath = "frayAlejandro.asl";
                foreach (var lAssetPath in AssetDatabase.GetAllAssetPaths())
                {
                    if (lAssetPath.EndsWith(lPath)){
                        Debug.Log("found");
                        var lScript = (MonoScript)AssetDatabase.LoadAssetAtPath(lAssetPath, typeof(MonoScript));
                        if (lScript != null)
                        {
                            Debug.Log(lAssetPath);
                            AssetDatabase.OpenAsset(lScript);
                            break;
                        }
                    }
                }
                
                /*string path ="Assets/Jasonity/Agents/frayAlejandro.asl";
                var lScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                Debug.Log(lScript);
                AssetDatabase.OpenAsset(lScript);*/
            }   
            Rect rect = EditorGUILayout.GetControlRect(false, 1 );
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
            agentCounter++;
        }
        if(GUILayout.Button("Create new agent")){
            Debug.Log("Hey");
            string myString = "Agente 0" + agentCounter;
            GUILayout.Label(myString, EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("New Agent Name", myString);
            groupEnabled = EditorGUILayout.BeginToggleGroup ("Generate on Scene", groupEnabled);
                myBool = EditorGUILayout.Toggle ("  Fast Thinking", myBool);
                myFloat = EditorGUILayout.Slider ("  Heigth", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();

            Rect rect = EditorGUILayout.GetControlRect(false, 1 );
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
            agentCounter++;
        }
        EditorGUILayout.EndVertical();
    }
    void OnEnable(){
        agentsPath = Application.dataPath;
    }
}
