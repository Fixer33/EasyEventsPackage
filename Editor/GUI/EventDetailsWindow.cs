using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyEvents.Editor.GUI
{
    public class EventDetailsWindow : EditorWindow
    {
        private static EventHistoryRecord _record;
        
        internal static void ShowWindow(EventHistoryRecord record)
        {
            _record = record;
            var window = GetWindow<EventDetailsWindow>();
            window.titleContent = new GUIContent(record.EventData.GetType().Name + " details");
            window.ShowModal();
        }

        private ScrollView _dataContainer;

        private void CreateGUI()
        {
            string loadPath = "Packages/com.fixer33.easy-events/Editor/GUI/Styles/EventDetailsWindow";
#if PACKAGES_DEV
            loadPath = "Assets/" + loadPath;
#endif

            var style = AssetDatabase.LoadAssetAtPath<StyleSheet>(loadPath + ".uss");
            if (style != null)
            {
                rootVisualElement.styleSheets.Add(style);
            }
            
            rootVisualElement.Add(new Label()
            {
                name = "header-text",
                text = _record.GetType().Name
            });
            rootVisualElement.Add(new Label()
            {
                name = "header-time",
                text = _record.Time.ToString("HH:mm:ss")
            });
            
            rootVisualElement.Add(_dataContainer = new ScrollView()
            {
                name = "data-container"
            });

            var type = _record.EventData.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                _dataContainer.contentContainer.Add(new Label()
                {
                    text = $"{fieldInfo.Name}: {fieldInfo.GetValue(_record.EventData)}"
                });
            }
        }
    }
}