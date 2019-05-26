using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class JasonityConsole : EditorWindow {

	[MenuItem("Window/Jasonity - Console")]
	public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(JasonityConsole));
    }
    
    void OnGUI()
    {
		
	}
}
