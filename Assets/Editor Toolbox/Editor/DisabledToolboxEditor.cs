﻿using System;

using UnityEditor;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Base Editor class for all Toolbox-related features.
    /// </summary>
  
   
    [CanEditMultipleObjects]
    public class DisabledToolboxEditor : Editor
    {
        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public override sealed void OnInspectorGUI()
        {
            try
            {
                OnBeginToolboxEditor?.Invoke(this);
                DrawCustomInspector();
            }
            catch (Exception)
            {
                //make sure to catch all Exceptions (especially ExitGUIException),
                //it will allow us to safely dispose all layout-based controls, etc.
                OnBreakToolboxEditor?.Invoke(this);
                throw;
            }
            finally
            {
                OnCloseToolboxEditor?.Invoke(this);
            }
        }


        /// <summary>
        /// Handles property display process using custom <see cref="Drawers.ToolboxDrawer"/>.
        /// </summary>
        /// <param name="property">Property to display.</param>
        public virtual void DrawCustomProperty(SerializedProperty property)
        {
            ToolboxEditorGui.DrawToolboxProperty(property);
        }

        /// <summary>
        /// Draws each available property using internal <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        public virtual void DrawCustomInspector()
        {
            if (ToolboxDrawerModule.ToolboxDrawersAllowed)
            {
                serializedObject.Update();

                var isExpanded = true;
                var property = serializedObject.GetIterator();
                //enter to the 'Base' property
                if (property.NextVisible(isExpanded))
                {
                    isExpanded = false;
                    var script = PropertyUtility.IsDefaultScriptProperty(property);

                    //try to draw the first property (m_Script)
                    using (new EditorGUI.DisabledScope(script))
                    {
                        DrawCustomProperty(property.Copy());
                    }

                    //iterate over all other serialized properties
                    //NOTE: every child will be handled internally
                    try
                    {
                        while (property.NextVisible(isExpanded))
                        {
                            DrawCustomProperty(property.Copy());
                        }
                    }
                    catch (NullReferenceException exception)
                    {
                        if (exception.Message != "SerializedObject of SerializedProperty has been Disposed.")
                            throw;
                    }
                  
                }

                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                DrawDefaultInspector();
            }
        }


        public static event Action<Editor> OnBeginToolboxEditor;
        public static event Action<Editor> OnBreakToolboxEditor;
        public static event Action<Editor> OnCloseToolboxEditor;
    }
}