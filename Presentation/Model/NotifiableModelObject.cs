namespace IntroSE.Kanban.Presentation.Model
{
    ///<summary>Represents a abstract Notifiable Model Object</summary>
    public abstract class NotifiableModelObject : NotifiableObject
    {
        ///<summary>Set and get of BackendController</summary>
        ///<param name="value">The Controller.</param>
        ///<returns>return BackendController.</returns>
        ///<exception cref = "Exception" > if password is not in correct format.</exception>
        public BackendController Controller { get; private set; }

        ///<summary>Constructor of Notifiable Model Object.</summary>
        ///<param name="controller">The Controller.</param>
        protected NotifiableModelObject(BackendController controller)
        {
            this.Controller = controller;
        }
    }
}
