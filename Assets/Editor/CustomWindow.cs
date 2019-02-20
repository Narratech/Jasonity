 using UnityEngine;
 using System.Collections;
 using UnityEditor;

 
 public class CustomWindow : EditorWindow {
 
     Vector3 vec3 = new Vector3(0,0,0);
     string objname = "JASONIY_MASTER";
     string fieldOne = "";
	 string fieldTwo = "";
	 string fieldThree = "";
 
     [MenuItem ("Window/My Custom Window")]
     static void ShowWindow () {
		 Debug.Log(GameObject.Find("JASONIY_MASTER"));
		 if(GameObject.Find("JASONIY_MASTER")!=null){
			// EditorGUILayout.HelpBox("You already have that object on the current scene!", MessageType.Warning);
		 }else{
         	EditorWindow.GetWindow(typeof(CustomWindow));
		 }
     }
 
     void OnInspectorUpdate () {

         Repaint ();
     }
     void OnGUI()
     {
         fieldOne = EditorGUILayout.TextField("Name for new object: ", fieldOne);
         fieldTwo = EditorGUILayout.TextField("Name for new object: ", fieldTwo);
         fieldThree = EditorGUILayout.TextField("Name for new object: ", fieldThree);
 
         if (fieldOne!="" && fieldTwo!="" && fieldThree != "")    {
             if (GUILayout.Button( "Add to scene"))
             {
                 
                 Object n = Resources.Load("prefab");
				 GameObject tempobj = (GameObject)Instantiate(n, new Vector3(0,0,0), Quaternion.identity);
				 tempobj.name = "JASONIY_MASTER";
				 this.Close();
             }
         } else {
             EditorGUILayout.LabelField ( "You must select an object and give it a name first");
         }
     }
 }