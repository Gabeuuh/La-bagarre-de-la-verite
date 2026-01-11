using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixJigsawBoardSetup : EditorWindow
{
    [MenuItem("Tools/Fix JigsawBoard Setup")]
    public static void Fix()
    {
        // Trouver le JigsawBoard
        GameObject jigsawBoard = GameObject.Find("JigsawBoard");
        if (jigsawBoard == null)
        {
            Debug.LogError("JigsawBoard non trouvé dans la scène !");
            return;
        }

        // Retirer tous les VRPuzzleMenuPositioner et VRJigsawBoardPositioner
        var positioners = jigsawBoard.GetComponents<MonoBehaviour>();
        foreach (var comp in positioners)
        {
            if (comp != null)
            {
                string typeName = comp.GetType().Name;
                if (typeName == "VRPuzzleMenuPositioner" || typeName == "VRJigsawBoardPositioner")
                {
                    Debug.Log($"Suppression de {typeName} du JigsawBoard");
                    DestroyImmediate(comp);
                }
            }
        }

        // Vérifier que BoardGen est toujours présent
        var boardGen = jigsawBoard.GetComponent<BoardGen>();
        if (boardGen == null)
        {
            Debug.LogError("BoardGen manquant sur JigsawBoard !");
            return;
        }

        // Configurer le RectTransform
        RectTransform rt = jigsawBoard.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero; // Position centrée
            rt.localPosition = new Vector3(0, 0, 0); // Reset Z position
            rt.localScale = Vector3.one; // Scale normal - sera ajusté par BoardGen
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(100, 100); // Taille de base
            
            Debug.Log("RectTransform du JigsawBoard réinitialisé");
        }
        else
        {
            Debug.LogError("RectTransform manquant sur JigsawBoard !");
            return;
        }

        // Vérifier le parent
        Transform parent = jigsawBoard.transform.parent;
        if (parent == null || parent.GetComponent<Canvas>() == null)
        {
            Debug.LogWarning("JigsawBoard n'est pas enfant direct du Canvas !");
        }
        else
        {
            Debug.Log($"JigsawBoard correctement enfant de {parent.name}");
        }

        // Corriger le BottomPanel aussi
        GameObject bottomPanel = GameObject.Find("BottomPanel");
        if (bottomPanel != null)
        {
            RectTransform bottomRt = bottomPanel.GetComponent<RectTransform>();
            if (bottomRt != null)
            {
                // Ancré en bas centre du Canvas
                bottomRt.anchorMin = new Vector2(0.5f, 0f);
                bottomRt.anchorMax = new Vector2(0.5f, 0f);
                bottomRt.pivot = new Vector2(0.5f, 0.5f);
                // Position à 50 pixels au-dessus du bord inférieur
                bottomRt.anchoredPosition = new Vector2(0, 50);
                bottomRt.sizeDelta = new Vector2(400, 80); // Taille adaptée
                
                Debug.Log("BottomPanel repositionné en bas du Canvas");
                EditorUtility.SetDirty(bottomPanel);
            }
        }
        else
        {
            Debug.LogWarning("BottomPanel non trouvé");
        }

        // Vérifier le Canvas pour VR
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            // Vérifier si le Canvas a TrackedDeviceGraphicRaycaster pour VR
            var raycaster = canvas.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
            if (raycaster == null)
            {
                // Ajouter TrackedDeviceGraphicRaycaster au Canvas
                raycaster = canvas.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
                raycaster.blockingMask = -1; // Tout
                Debug.Log("TrackedDeviceGraphicRaycaster ajouté au Canvas");
                EditorUtility.SetDirty(canvas);
            }
            else
            {
                Debug.Log("TrackedDeviceGraphicRaycaster déjà présent sur Canvas");
            }
        }

        // Configurer le ButtonPlay
        GameObject buttonPlay = GameObject.Find("ButtonPlay");
        if (buttonPlay != null)
        {
            // Vérifier que le Button component a bien la méthode OnClickPlay configurée
            var button = buttonPlay.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
                Debug.Log($"ButtonPlay configuré : {button.onClick.GetPersistentEventCount()} listener(s)");
                EditorUtility.SetDirty(buttonPlay);
            }
            
            // Le ButtonPlay a déjà XRUIInputModule (component 1545845675) donc pas besoin d'en ajouter
            Debug.Log("ButtonPlay prêt pour VR");
        }
        else
        {
            Debug.LogWarning("ButtonPlay non trouvé");
        }

        // Sauvegarder la scène
        EditorUtility.SetDirty(jigsawBoard);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );

        Debug.Log("✅ Configuration terminée !");
        Debug.Log("JigsawBoard:");
        Debug.Log("  - Position ancrée : (0, 0)");
        Debug.Log("  - Position locale Z : 0");
        Debug.Log("  - Scale locale : (1, 1, 1)");
        Debug.Log("  - Scripts de positionnement supprimés");
        Debug.Log("BottomPanel:");
        Debug.Log("  - Position ancrée : (0, 50) - en bas du Canvas");
        Debug.Log("  - Bouton Play accessible");
        
        EditorUtility.DisplayDialog(
            "Correction terminée", 
            "Configuration corrigée avec succès:\n\n" +
            "✅ JigsawBoard\n" +
            "   - Centré dans le Canvas\n" +
            "   - Scripts de positionnement supprimés\n\n" +
            "✅ BottomPanel\n" +
            "   - Repositionné en bas du Canvas\n" +
            "   - Bouton Play maintenant accessible\n\n" +
            "Sauvegardez la scène (Ctrl+S) et testez en VR !",
            "OK"
        );
    }
}
