namespace Pose.DomainModel
{
    public abstract class Entity
    {
        public readonly long Id;

        protected Entity(long id)
        {
            Id = id;
        }
    }
}
