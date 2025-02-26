namespace TyzenR.Taskman.Entity
{
    public static class Constants
    {
        public static string ApplicationTitle = "Taskman";
        public static string ApplicationUrl = "https://taskman.futurecaps.com";
    }

    public enum TaskStatusEnum
    {
        Unknown,
        InProgress,
        Completed
    }

    public enum ActionTypeEnum
    {
        Create,
        Edit,
        Delete,
        View,
        Complete,
        InProgress
    }
}
