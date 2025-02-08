using Microsoft.EntityFrameworkCore;
using TyzenR.Account.Entity;
using TyzenR.EntityLibrary;
using TyzenR.Taskman.Entity;

namespace TyzenR.Taskman.Managers
{
    public class TaskManager : BaseRepository<TaskEntity>, ITaskManager
    {
        private EntityContext context;

        public TaskManager(EntityContext context) : base(context)
        {
            this.context = context;
        }

        public Task<(bool, TaskEntity)> CreateTaskAsync(TaskEntity task)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TaskEntity>> GetTasksForUserAsync(UserEntity user)
        {
            var result1 = await this.context.Tasks
                .Where(t => t.Status == TaskStatusEnum.InProgress && (t.CreatedBy == user.Id || t.AssignedTo == user.Id))
                .ToListAsync();

            var result2 = await this.context.Tasks
                .Where(t => t.Status == TaskStatusEnum.Completed && (t.CreatedBy == user.Id || t.AssignedTo == user.Id))
                .ToListAsync();

            result1.AddRange(result2);

            return result1;
        }
    }
}
