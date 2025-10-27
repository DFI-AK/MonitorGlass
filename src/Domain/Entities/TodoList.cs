namespace MonitorGlass.Domain.Entities;

public class TodoList : BaseEntity<int>
{
    public string? Title { get; set; }

    public Colour Colour { get; set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
