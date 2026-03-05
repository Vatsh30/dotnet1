using System.Text.Json;
using TodoApiJson.Models;

namespace TodoApiJson.Services
{
    public class TodoService
    {
        private readonly string _filePath;

        public TodoService(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "todos.json");
        }

        private List<TodoItem> ReadFile()
        {
            if (!File.Exists(_filePath))
                return new List<TodoItem>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
        }

        private void WriteFile(List<TodoItem> todos)
        {
            var json = JsonSerializer.Serialize(todos, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }

        public List<TodoItem> GetAll()
        {
            return ReadFile();
        }

        public TodoItem? GetById(int id)
        {
            return ReadFile().FirstOrDefault(x => x.Id == id);
        }

        public TodoItem Create(TodoItem item)
        {
            var todos = ReadFile();

            item.Id = todos.Count == 0 ? 1 : todos.Max(x => x.Id) + 1;

            todos.Add(item);

            WriteFile(todos);

            return item;
        }

        public bool Update(int id, TodoItem updated)
        {
            var todos = ReadFile();

            var existing = todos.FirstOrDefault(x => x.Id == id);

            if (existing == null)
                return false;

            existing.Title = updated.Title;
            existing.IsCompleted = updated.IsCompleted;

            WriteFile(todos);

            return true;
        }

        public bool Delete(int id)
        {
            var todos = ReadFile();

            var todo = todos.FirstOrDefault(x => x.Id == id);

            if (todo == null)
                return false;

            todos.Remove(todo);

            WriteFile(todos);

            return true;
        }
    }
}
