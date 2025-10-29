namespace TyzenR.Taskman.Entity
{
    public class MemberModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
    }
}
