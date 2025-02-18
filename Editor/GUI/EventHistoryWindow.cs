using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyEvents.Editor.GUI
{
    public class EventHistoryWindow : EditorWindow
    {
        [MenuItem("Tools/Fixer33/Event history")]
        private static void ShowWindow()
        {
            var window = GetWindow<EventHistoryWindow>();
            window.titleContent = new UnityEngine.GUIContent("Event history");
            window.Show();
        }

        private ScrollView _historyContainer;

        private void CreateGUI()
        {
            string loadPath = "Packages/com.fixer33.easy-events/Editor/GUI/Styles/EventHistoryWindow";
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
                name = "event-history__header",
                text = "Called event history"
            });
            rootVisualElement.Add(new VisualElement()
            {
                name = "event-history__header-divider"
            });
            rootVisualElement.Add(_historyContainer = new ScrollView()
            {
                name = "event-history__container"
            });

            EasyEventsEditorBridge.EditorHistoryUpdated -= RefreshList;
            EasyEventsEditorBridge.EditorHistoryUpdated += RefreshList;
            
            if (Application.isPlaying)
                RefreshList();
        }

        private void OnDestroy()
        {
            EasyEventsEditorBridge.EditorHistoryUpdated -= RefreshList;
        }

        private void RefreshList()
        {
            _historyContainer.contentContainer.Clear();
            foreach (var eventHistoryRecord in EasyEventsEditorBridge.EditorHistory.Reverse())
            {
                _historyContainer.contentContainer.Add(new EventPreviewElement(eventHistoryRecord));
            }
        }
    }
}