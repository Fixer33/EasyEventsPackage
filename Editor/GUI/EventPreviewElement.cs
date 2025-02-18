using UnityEngine.UIElements;

namespace EasyEvents.Editor.GUI
{
    public class EventPreviewElement : VisualElement
    {
        private readonly Button _viewDetailsBtn;
        private readonly EventHistoryRecord _record;
        
        public EventPreviewElement(EventHistoryRecord record)
        {
            _record = record;
            
            this.Add(new Label()
            {
                name = "event-preview__header",
                text = $"[{record.Time:HH:mm:ss}] {record.EventData.GetType().Name}"
            });
            this.Add(_viewDetailsBtn = new Button()
            {
                name = "event-preview__view-details-btn",
                text = "Details"
            });
            _viewDetailsBtn.clicked += ViewDetailsClicked;
        }

        private void ViewDetailsClicked()
        {
            EventDetailsWindow.ShowWindow(_record);
        }
    }
}