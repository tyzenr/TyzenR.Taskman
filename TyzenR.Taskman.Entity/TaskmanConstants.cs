namespace TyzenR.Taskman.Entity
{
    public static class TaskmanConstants
    {
        public static string ApplicationTitle = "Taskman";
        public static string ApplicationUrl = "https://taskman.futurecaps.com";
        public const string CommonErrorMessage = "An error occurred while processing your request. Please try again later.";
    }

    public enum TaskTypeEnum
    {
        Task,
        Timesheet
    }

    public enum TaskStatusEnum
    {
        Unknown,
        InProgress,
        Completed
    }

    public enum ActionTypeEnum
    {
        Add,
        Edit,
        Delete,
        View,
        Complete,
        InProgress
    }
}
