using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Models.Entity;

namespace ProjectPlaner.Data
{
    public class MarkDbStorage
    {
        private readonly ApplicationDbContext _context;

        public MarkDbStorage(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE - �������� ����� �����
        public async Task<Mark> CreateMarkAsync(Mark mark)
        {
            if (mark == null)
                throw new ArgumentNullException(nameof(mark));

            mark.markId = Guid.NewGuid();
            _context.marks.Add(mark);
            await _context.SaveChangesAsync();
            return mark;
        }

        // READ (Single) - ��������� ����� �� ID
        public async Task<Mark?> GetMarkByIdAsync(Guid markId)
        {
            return await _context.marks
                .Include(m => m.tasks)
                .FirstOrDefaultAsync(m => m.markId == markId);
        }

        // READ (All) - ��������� ���� �����
        public async Task<List<Mark>> GetAllmarksAsync()
        {
            return await _context.marks
                .Include(m => m.tasks)
                .ToListAsync();
        }

        // UPDATE - ���������� �����
        public async Task<Mark> UpdateMarkAsync(Mark mark)
        {
            if (mark == null)
                throw new ArgumentNullException(nameof(mark));

            //_context.Entry(mark).State = EntityState.Modified;
            _context.marks.Update(mark);
            await _context.SaveChangesAsync();
            return mark;
        }

        // DELETE - �������� �����
        public async Task<bool> DeleteMarkAsync(Guid markId)
        {
            var mark = await _context.marks
                .Include(m => m.tasks)
                .FirstOrDefaultAsync(m => m.markId == markId);

            if (mark == null)
                return false;

            // ������� ����� �� ������������� �������
            foreach (var task in mark.tasks.ToList())
            {
                mark.tasks.Remove(task);
            }

            _context.marks.Remove(mark);
            await _context.SaveChangesAsync();
            return true;
        }

        // �������������� ������ ��� ������ �� �������

        // ���������� ����� � ������
        public async System.Threading.Tasks.Task AddMarkToTaskAsync(Guid markId, Guid taskId)
        {
            var mark = await _context.marks.FindAsync(markId);
            var task = await _context.tasks.FindAsync(taskId);

            if (mark != null && task != null)
            {
                if (!task.marks.Any(m => m.markId == markId))
                {
                    task.marks.Add(mark);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // �������� ����� �� ������
        public async System.Threading.Tasks.Task RemoveMarkFromTaskAsync(Guid markId, Guid taskId)
        {
            var task = await _context.tasks
                .Include(t => t.marks)
                .FirstOrDefaultAsync(t => t.taskId == taskId);

            var mark = task?.marks.FirstOrDefault(m => m.markId == markId);

            if (mark != null)
            {
                task.marks.Remove(mark);
                await _context.SaveChangesAsync();
            }
        }

        // ��������� ���� ����� � ������ ������
        public async Task<List<Models.Entity.Task>> GetTasksWithMarkAsync(Guid markId)
        {
            return await _context.tasks
                .Where(t => t.marks.Any(m => m.markId == markId))
                .ToListAsync();
        }

        // ��������� ���������� ����� � ������
        public async Task<int> GetTaskCountForMarkAsync(Guid markId)
        {
            return await _context.tasks
                .CountAsync(t => t.marks.Any(m => m.markId == markId));
        }
    }
}