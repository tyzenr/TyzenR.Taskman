namespace TyzenR.Taskman.Entity.Helpers;

public class BusinessHelper
{
    public bool IsActive(DateTime date, DateTime startDate, DateTime endDate)
    {
        bool result = (date >= startDate) && (date <= endDate);

        return result;
    }
}
