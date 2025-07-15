using Microsoft.EntityFrameworkCore;

namespace ProjectPlaner.Data
{
    public class TaskDbStorage
    {
        private readonly ApplicationDbContext _context;

        public TaskDbStorage(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create - ���������� ����� ������
        public async Task<Models.Entity.Task> CreateTaskAsync(Models.Entity.Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            _context.tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        // Read (Single) - ��������� ������ �� ID
        public async Task<Models.Entity.Task?> GetTaskByIdAsync(Guid taskId)
        {
            return await _context.tasks
                .Include(t => t.project)
                .Include(t => t.marks)
                .Include(t => t.marker)
                .Include(t => t.status)
                .FirstOrDefaultAsync(t => t.taskId == taskId);
        }

        // Read (All) - ��������� ���� �����
        public async Task<List<Models.Entity.Task>> GetAllTasksAsync()
        {
            return await _context.tasks
                .Include(t => t.project)
                .Include(t => t.marks)
                .Include(t => t.marker)
                .Include(t => t.status)
                .ToListAsync();
        }

        // Update - ���������� ������
        public async Task<Models.Entity.Task> UpdateTaskAsync(Models.Entity.Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            _context.tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        // Delete - �������� ������
        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            var task = await _context.tasks.FindAsync(taskId);
            if (task == null)
                return false;

            _context.tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        // �������������� ������

        // ��������� ����� �� �������
        public async Task<List<Models.Entity.Task>> GetTasksByProjectAsync(Guid projectId)
        {
            return await _context.tasks
                .Where(t => t.projectId == projectId)
                .Include(t => t.marks)
                .Include(t => t.marker)
                .Include(t => t.status)
                .ToListAsync();
        }

        // ���������� ������� ������
        //public async Task<bool> UpdateTaskStatusAsync(Guid taskId, string status)
        //{
        //    var task = await _context.tasks.FindAsync(taskId);
        //    if (task == null)
        //        return false;

        //    task.status = status;
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        // ���������� ����� � ������
        public async Task AddMarkToTaskAsync(Guid taskId, Guid markId)
        {
            var task = await _context.tasks.Include(t => t.marks).FirstOrDefaultAsync(t => t.taskId == taskId);
            var mark = await _context.marks.FindAsync(markId);

            if (task != null && mark != null)
            {
                task.marks.Add(mark);
                await _context.SaveChangesAsync();
            }
        }

        // �������� ����� �� ������
        public async Task RemoveMarkFromTaskAsync(Guid taskId, Guid markId)
        {
            var task = await _context.tasks.Include(t => t.marks).FirstOrDefaultAsync(t => t.taskId == taskId);
            var mark = task?.marks.FirstOrDefault(m => m.markId == markId);

            if (mark != null)
            {
                task.marks.Remove(mark);
                await _context.SaveChangesAsync();
            }
        }
    }
}