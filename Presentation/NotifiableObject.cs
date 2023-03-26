using System.ComponentModel;

namespace IntroSE.Kanban.Presentation
{
    ///<summary>Represents an abstract Notifiable Object</summary>
    public abstract class NotifiableObject : INotifyPropertyChanged
    {
        ///<summary>Represents the method that will handle the PropertyChanged event.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///<summary> Notify that a property value has changed.</summary>
        ///<param name="property">The property that was changed.</param>
        protected void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

}
