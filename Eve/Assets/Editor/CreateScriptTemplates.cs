//
//Copyright (c) Hassan_Ali. All rights reserved 
//

using UnityEditor;

public class CreateScriptTemplates
{
    [MenuItem("Asset/Create/Code/MonoBehaviour" , priority = 40)]
    public static void CreateMonoBehaviourMenuItem()
    {
        string templatePath = "Resource/Assets/Editro/Templates/Monobehaviour.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewScript.cs");
    }
    [MenuItem("Asset/Create/Code/Enum" , priority = 41)]
    public static void CreateEnumMenuItem()
    {
        string templatePath = "Resource/Assets/Editro/Templates/Enum.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewEnum.cs");
    }
    [MenuItem("Asset/Create/Code/ScrptableObjectM", priority = 42)]
    public static void CreateScrptableObjectMenuItem()
    {
        string templatePath = "Resource/Assets/Editro/Templates/ScrptableObject.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewScrptableObject.cs");
    }
    [MenuItem("Asset/Create/Code/Class", priority = 43)]
    public static void CreateClassMenuItem()
    {
        string templatePath = "Resource/Assets/Editro/Templates/Class.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewClass.cs");
    }
}
